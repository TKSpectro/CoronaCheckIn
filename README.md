# Covid Check-in

## Local development

```bash
# -d runs it in detached mode
docker-compose up -d

# you need to pull our frontend libraries with libman
libman restore

# update the database with all migrations
dotnet ef database update

# If you want to use some seed data then call this run this
dotnet run seed

# can use "dotnet watch" for development, but its sometimes buggy
dotnet run
```

## Database Migrations
You will have to install the ef tool for the dotnet cli

```bash
# change "init" (title of the migration)
dotnet ef migrations add init
```

Bring your database up to the newest state/migration
```bash
dotnet ef database update
```

Seed your database with good testing data
```bash
dotnet run seed
```

## Enabling actual email sending

Create a appsettings.Local.json and set your own smtp settings
We recommend [Ethereal Mail](https://ethereal.email/) for testing purposes.

The appsettings.Local.json can be copied from the Development one or created by hand and should contain:

```json
{
    "smtp": {
        "enabled": true,
        "host": "smtp.ethereal.email",
        "enableSsl": true,
        "port": 587,
        "auth": {
            "user": "your user name (email)",
            "pass": "your password"
        }
    }
}
```

## Enabling the Frontoffice Test Page

This can be done by adding a flag in the appsettings.json
Add this to the root of your appsettings.json file or change it in the appsettings.Development.json
```json
{
    "EnableFrontoffice": true
}
```

## Examples

### Read from appsettings.json

```csharp
public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        bool parsedBool = bool.Parse(_configuration["is:this:enabled"]);
        int parsedInt = int.Parse(_configuration["what:number:is:this"]);
        string someString = _configuration["get:this:value"];
        return View();
    }
}
```

### Get data from the database

```csharp
public class AccountsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        IEnumerable<Account> accounts = _context.Accounts;
        return View(accounts);
    }
}
```
