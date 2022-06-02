using System.Globalization;
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

        if (args != null)
        {
            config.AddCommandLine(args);
        }
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

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<AccountManager>();

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

    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetService<DataSeeder>();
    service.Seed();
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
