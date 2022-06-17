using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CoronaCheckIn.Areas.api
{
    [ApiController]
    [Route("/api")]
    public class ApiIndexController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApiIndexController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public ApiIndexController(ILogger<ApiIndexController> logger, ApplicationDbContext context,
            SignInManager<User> signInManager, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet("")]
        public ActionResult<string> Index()
        {
            return "This is the rest api of CoronaCheckIn";
        }
    
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginBody body)
        {
            if (body.Email.Trim() == "" || body.Password.Trim() == "")
            {
                throw new Exception("No email or password given.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == body.Email);
            if (user == null)
            {
                throw new Exception("Invalid login");
            }

            var valid = _signInManager.CheckPasswordSignInAsync(user, body.Password, false);
            if (valid.Result != SignInResult.Success)
            {
                throw new Exception("Invalid login");
            }
            
            // Get all jwt configuration from the appsettings or set defaults
            var secret = _configuration.GetValue<string>("Jwt:Secret");
            if (secret == null || secret.Trim().Length == 0)
            {
                throw new Exception("Please provide a valid Jwt:Secret in appsettings");
            }
            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            if (issuer.Trim().Length == 0) issuer = "ccn";
            var audience = _configuration.GetValue<string>("Jwt:Audience");;
            if (audience.Trim().Length == 0) issuer = "ccn";
            
            var token = JwtHelper.GetJwtToken(
                user.Email,
                secret,
                issuer,
                audience,
                TimeSpan.FromMinutes(90),
                new[]
                {
                    new Claim("UserId", user.Id)
                }
            );
            
            var res = new LoginResponse();
            res.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return res;
        }
    }

    public class LoginBody
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}

