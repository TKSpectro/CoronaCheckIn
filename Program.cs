using System.Globalization;
using System.Text;
using CoronaCheckIn;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using CoronaCheckIn.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Overwrite appsettings loading so we can have a Local one for f.e. db connection strings
builder.Host.ConfigureAppConfiguration(
    (hostingContext, config) =>
    {
        config.Sources.Clear();

        var env = hostingContext.HostingEnvironment;

        config.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) //load base settings
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true) //load environment settings
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true) //load local settings
                .AddEnvironmentVariables();
        
        config.AddCommandLine(args);
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("de"), new CultureInfo("en") };

    options.DefaultRequestCulture = new RequestCulture("de");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
    });

builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Get all jwt configuration from the appsettings or set defaults
var secret = builder.Configuration.GetValue<string>("Jwt:Secret");
if (secret == null || secret.Trim().Length == 0)
{
    throw new Exception("Please provide a valid Jwt:Secret in appsettings");
}
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
if (issuer.Trim().Length == 0) issuer = "ccn";
var audience = builder.Configuration.GetValue<string>("Jwt:Audience");;
if (audience.Trim().Length == 0) issuer = "ccn";

builder.Services.AddAuthentication(options =>
    {
        // custom scheme defined in .AddPolicyScheme() below
        options.DefaultScheme = "JWT_OR_COOKIE";
        options.DefaultChallengeScheme = "JWT_OR_COOKIE";
    })
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.LogoutPath = "/Identity/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };
    })
    // this is the key piece!
    .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
    {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return "Bearer";

            // otherwise always check for cookie auth
            return "Cookies";
        };
    });

builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<RoomManager>();
builder.Services.AddScoped<InfectionManager>();
builder.Services.AddScoped<SessionManager>();

// Add our own data seeder
builder.Services.AddTransient<DataSeeder>();

// Email sender for auth and other usages
builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Passing seed as a parameter when running dotnet run will seed the database
if (args.Length == 1 && args[0].ToLower() == "seed")
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory?.CreateScope();
    var service = scope?.ServiceProvider.GetService<DataSeeder>();
    service?.Seed();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var requestLocalizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
if (requestLocalizationOptions != null)
    app.UseRequestLocalization(requestLocalizationOptions);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
