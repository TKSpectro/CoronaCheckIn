using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoronaCheckIn.Areas.api
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly AccountManager _accountManager;

        public AccountsController(ILogger<AccountsController> logger, AccountManager accountManager)
        {
            _logger = logger;
            _accountManager = accountManager;
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
        
        // [HttpPost("{name}")]
        // public ActionResult<string> GenerateJwtToken(string name)
        // {
        //     var jwtTokenHandler = new JwtSecurityTokenHandler();
        //     if (string.IsNullOrEmpty(name))
        //     {
        //         throw new InvalidOperationException("Name is not specified.");
        //     }
        //     SymmetricSecurityKey securityKey = new SymmetricSecurityKey(new Guid("00000000-0000-0000-0000-000000000000").ToByteArray());
        //     
        //     var claims = new[] { new Claim(ClaimTypes.Name, name) };
        //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //     var token = new JwtSecurityToken("ExampleServer", "ExampleClients", claims, expires: DateTime.Now.AddSeconds(60), signingCredentials: credentials);
        //     return jwtTokenHandler.WriteToken(token);
        // }
    }
}

