using System.Collections.Immutable;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CoronaCheckIn.Controllers.api
{
    [ApiController]
    [Route("/api/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly ILogger<SessionsController> _logger;
        private readonly SessionManager _sessionManager;
        private readonly RoomManager _roomManager;
        private readonly UserManager<User> _userManager;

        public SessionsController(ILogger<SessionsController> logger, SessionManager sessionManager, RoomManager roomManager, UserManager<User> userManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _roomManager = roomManager;
            _userManager = userManager;
        }

        [HttpGet("")]
        public ActionResult<String> Index([FromQuery] Guid? roomId, [FromQuery] string? userId, [FromQuery] bool? isInfected, [FromQuery] DateTime? after, [FromQuery] DateTime? before)
        {
            var sessions = _sessionManager.GetSessions(roomId: roomId, userId: userId, isInfected: isInfected, after: after, before: before);

            return JsonSerializer.Serialize(sessions);
        }

        [HttpGet("{id}")]
        public ActionResult<String> GetOne(Guid id)
        {
            var session = _sessionManager.GetSession(id);
            return JsonSerializer.Serialize(session);
        }

        public class PostSession
        {
            public Guid RoomId { get; set; }
            
            public string UserId { get; set; }
            
            public bool Infected { get; set; } = false;

            public DateTime StartTime { get; set; } = DateTime.Now;
        }

        [HttpPost("")]
        public ActionResult<String> Create([FromBody] PostSession postSession)
        {
            var session = new Session()
            {
                RoomId = postSession.RoomId,
                UserId = postSession.UserId,
                Infected = postSession.Infected,
                StartTime = postSession.StartTime,
                EndTime = null
            };

            var createdSession = _sessionManager.Add(session);
            return JsonSerializer.Serialize(createdSession);
        }
    }
}