# Covid Check-in

## Local development

```bash
# -d runs it in detached mode
docker-compose up -d

# can use "dotnet watch" for development, but its sometimes buggy
dotnet run
```

Before accessing any data you should run the Scripts/seed.sql script while being connected to the database
// TODO: Maybe find an easier way for seeding the database

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
