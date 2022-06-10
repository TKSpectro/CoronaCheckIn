using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api")]
public class ApiIndexController : ControllerBase
{
    private readonly ILogger<ApiIndexController> _logger;

    public ApiIndexController(ILogger<ApiIndexController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public ActionResult<string> Index()
    {
        return "This is the rest api of CoronaCheckIn";
    }
}