using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api/sessions")]
public class ApiSessionsController : ControllerBase
{
    private readonly ILogger<ApiSessionsController> _logger;
    private readonly RoomManager _roomManager;
    private readonly SessionManager _sessionManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;

    public ApiSessionsController(ILogger<ApiSessionsController> logger, SessionManager sessionManager,
        RoomManager roomManager, UserManager<User> userManager, IConfiguration config)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _roomManager = roomManager;
        _userManager = userManager;
        _config = config;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("")]
    public ActionResult<IEnumerable<Session>> GetAll([FromQuery] Guid? roomId, [FromQuery] string? userId,
        [FromQuery] bool? isInfected, [FromQuery] DateTime? after, [FromQuery] DateTime? before,
        [FromQuery] bool includeRoom = false, [FromQuery] int limit = 0)
    {
        var sessions = _sessionManager.GetSessions(roomId: roomId, userId: userId, isInfected: isInfected, after: after,
            before: before, includeRoom: includeRoom, limit: limit);

        return sessions.ToList();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}")]
    public ActionResult<Session> GetOne(Guid id)
    {
        var session = _sessionManager.GetSession(id);
        if(session == null)
        {
            throw new Exception("No session found with this id");
        }
        
        return session;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("current-session")]
    public ActionResult<Session> GetCurrentSession()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var secret = _config.GetValue<string>("Jwt:Secret");
        var key = Encoding.UTF8.GetBytes(secret);
        var issuer = _config.GetValue<string>("Jwt:Issuer");
        if (issuer.Trim().Length == 0) issuer = "ccn";
        var audience = _config.GetValue<string>("Jwt:Audience");;
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
        
        var runningSessions = _sessionManager.GetSessions(userId: userId,
            endTimeNull: true, includeRoom: true, sortBy: "start", sortOrder: "desc").ToList();
        
        foreach (var session in runningSessions)
        {
            // TODO: Need to check this algorithm and proof with more test cases
            var maxEndTime = session.StartTime.AddMinutes(session.Room.MaxDuration);
            if (maxEndTime > DateTime.Now)
            {
                // Session is an old session which the user did not close
                session.EndTime = maxEndTime;
                Console.WriteLine("session");
                Console.WriteLine(session);
                return session;
                
            }
        }

        Console.WriteLine("Error");
        throw new Exception("No current session was found!");
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("")]
    public ActionResult<Session> Create([FromBody] PostSession postSession)
    {
        var session = new Session
        {
            RoomId = postSession.RoomId,
            UserId = postSession.UserId,
            Infected = postSession.Infected,
            StartTime = postSession.StartTime,
            EndTime = null
        };

        var createdSession = _sessionManager.AddSession(session);
        if(createdSession == null)
        {
            throw new Exception("Session could not be created");
        }
        
        return createdSession;
    }

    public class PostSession
    {
        public Guid RoomId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool Infected { get; set; } = false;

        public DateTime StartTime { get; set; } = DateTime.Now;
    }
}