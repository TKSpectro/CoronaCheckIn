using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public ApiIndexController(ILogger<ApiIndexController> logger, ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
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
            
            
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(new Guid("00000000-0000-0000-0000-000000000000").ToByteArray());
            var issuer = "ccn";
            var audience = "ccn";
            var signingKey = securityKey.ToString();
            
            var token = JwtHelper.GetJwtToken(
                user.Email,
                signingKey,
                issuer,
                audience,
                TimeSpan.FromMinutes(90),
                new[]
                {
                    new Claim("UserId", user.Id)
                });
            var res = new LoginResponse();
            res.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return res;
        }
    
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}

