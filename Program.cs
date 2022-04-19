var builder = WebApplication.CreateBuilder(args);

// Overwrite appsettings loading so we can have a Local one f.e. db connection strings
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

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
