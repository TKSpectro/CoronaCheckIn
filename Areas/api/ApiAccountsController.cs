using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CoronaCheckIn.Areas.api
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly AccountManager _accountManager;
        private readonly IConfiguration _config;

        public AccountsController(ILogger<AccountsController> logger, AccountManager accountManager, IConfiguration config)
        {
            _logger = logger;
            _accountManager = accountManager;
            _config = config;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAccounts()
        {
            var accounts = _accountManager.GetAccounts();
            return accounts.ToArray();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public ActionResult<User> GetAccount(Guid id)
        {
            var account = _accountManager.GetAccount(id);
            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("remove-profile")]
        public ActionResult<string> RemoveAccount()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var secret = _config.GetValue<string>("Jwt:Secret");
            var key = Encoding.UTF8.GetBytes(secret);
            var issuer = _config.GetValue<string>("Jwt:Issuer");
            if (issuer.Trim().Length == 0) issuer = "ccn";
            var audience = _config.GetValue<string>("Jwt:Audience");
            if (audience.Trim().Length == 0) issuer = "ccn"; 
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var userId = (jwtToken.Claims.First(x => x.Type == "UserId").Value);
            var id = Guid.Parse(userId);

            _accountManager.Remove(id);
            return "Account successfully deleted";
        }
    }
}

