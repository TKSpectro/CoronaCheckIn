using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api/infection")]
public class ApiInfectionController : ControllerBase
{
    private readonly ILogger<ApiSessionsController> _logger;
    private readonly InfectionManager _infectionManager;
    private readonly IConfiguration _config;

    public ApiInfectionController(ILogger<ApiSessionsController> logger, InfectionManager infectionManager,
        IConfiguration config)
    {
        _logger = logger;
        _infectionManager = infectionManager;
        _config = config;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("")]
    public ActionResult<InfectedResponse> getInfection()
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
        var infected = _infectionManager.CheckInfection( id: userId);
        
        return new InfectedResponse { Message = infected };
    }
    public class InfectedResponse
    {
        public string Message { get; set; } = string.Empty;
    }

}

