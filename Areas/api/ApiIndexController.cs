using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoronaCheckIn.Areas.Identity.Pages.Account;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly UserManager<User> _userManager;

        public ApiIndexController(ILogger<ApiIndexController> logger, ApplicationDbContext context,
            SignInManager<User> signInManager, IConfiguration configuration, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpGet("")]
        public ActionResult<string> Index()
        {
            return "This is the rest api of CoronaCheckIn";
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Login([FromBody] LoginBody body)
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
            var audience = _configuration.GetValue<string>("Jwt:Audience");
            
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

            var res = new AuthResponse();
            res.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return res;
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterBody registerBody)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(registerBody.Email);
                if(existingUser == null)
                {
                    User user = new User();
                    user.UserName = registerBody.Email;
                    user.Email = registerBody.Email;
                    user.Firstname = registerBody.Firstname;
                    user.Lastname = registerBody.Lastname;
 
                    IdentityResult result = _userManager.CreateAsync(user, registerBody.Password).Result;
 
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");

                        // Get all jwt configuration from the appsettings or set defaults
                        var secret = _configuration.GetValue<string>("Jwt:Secret");
                        if (secret == null || secret.Trim().Length == 0)
                        {
                            throw new Exception("Please provide a valid Jwt:Secret in appsettings");
                        }
                        
                        var issuer = _configuration.GetValue<string>("Jwt:Issuer");
                        if (issuer.Trim().Length == 0) issuer = "ccn";
                        var audience = _configuration.GetValue<string>("Jwt:Audience");
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

                        var res = new AuthResponse();
                        res.Token = new JwtSecurityTokenHandler().WriteToken(token);
                        return Ok(res);
                    }
                    
                    return BadRequest("Passwords must have at least one non alphanumeric character. \n" +
                                      "Passwords must have at least one uppercase ('A'-'Z').\n" +
                                      "Passwort must be at least 6 and at max 100 characters long.");
                    
                }
                return BadRequest("This account is already exists.");
 
            }
 
            return BadRequest();
        }
    }

    public class LoginBody
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterBody
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}