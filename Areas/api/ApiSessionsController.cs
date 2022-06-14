using System.Text.Json;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Areas.api;

[ApiController]
[Route("/api/sessions")]
public class ApiSessionsController : ControllerBase
{
    private readonly ILogger<ApiSessionsController> _logger;
    private readonly RoomManager _roomManager;
    private readonly SessionManager _sessionManager;
    private readonly UserManager<User> _userManager;

    public ApiSessionsController(ILogger<ApiSessionsController> logger, SessionManager sessionManager,
        RoomManager roomManager, UserManager<User> userManager)
    {
        _logger = logger;
        _sessionManager = sessionManager;
        _roomManager = roomManager;
        _userManager = userManager;
    }

    [HttpGet("")]
    public ActionResult<IEnumerable<Session>> GetAll([FromQuery] Guid? roomId, [FromQuery] string? userId,
        [FromQuery] bool? isInfected, [FromQuery] DateTime? after, [FromQuery] DateTime? before,
        [FromQuery] bool includeRoom = false)
    {
        var sessions = _sessionManager.GetSessions(roomId: roomId, userId: userId, isInfected: isInfected, after: after,
            before: before, includeRoom: includeRoom);

        return sessions.ToList();
    }

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

        public string UserId { get; set; }

        public bool Infected { get; set; } = false;

        public DateTime StartTime { get; set; } = DateTime.Now;
    }
}