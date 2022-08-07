using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api/cases")]
public class ApiCasesController : ControllerBase
{
    private readonly ILogger<ApiSessionsController> _logger;
    private readonly SessionManager _sessionManager;
    private readonly IConfiguration _config;

    public ApiCasesController(ILogger<ApiSessionsController> logger, SessionManager sessionManager,
        IConfiguration config)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _config = config;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("")]
    public ActionResult<InfectedResponse> SetSessionsAsInfected()
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

        _sessionManager.SetSessionsAsInfected(id: userId);
        return new InfectedResponse { Message = "your last 3 day session was reported as infected! " };
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("set-infected-session/{id}")]
    public ActionResult<InfectedResponse> SetSessionAsInfected(string id)
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
        var sessionId = Guid.Parse(id);

        _sessionManager.SetSessionAsInfected(sessionId, userId);
        return new InfectedResponse { Message = "your session is now marked as infected! " };
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("check-sessions")]
    public ActionResult<InfectedResponse> CheckSessions()
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
        var userStatus = _sessionManager.CheckSessions(id: userId);

        return new InfectedResponse { Message = userStatus };
    }
    public class InfectedResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}

