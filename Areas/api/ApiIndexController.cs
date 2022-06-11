using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Areas.api
{
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
    
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginBody body)
        {
            if (body.Email.Trim() == "" || body.Password.Trim() == "")
            {
                throw new Exception("No email or password given.");
            }
            return "This is the rest api of CoronaCheckIn";
        }
    
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> Authed()
        {
        
            return "You have access to this route";
        }
    }

    public class LoginBody
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

