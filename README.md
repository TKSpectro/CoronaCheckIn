# Covid Check-in

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
