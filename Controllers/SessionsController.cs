using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoronaCheckIn.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ILogger<SessionsController> _logger;
        private readonly SessionManager _sessionManager;

        public SessionsController(ILogger<SessionsController> logger, SessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index([FromQuery] string? roomName = null, [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null,
            [FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null, [FromQuery] bool? infected = null)
        {
            var sessions = _sessionManager.GetSessions(isInfected: infected, after: start, before: end,
                sortBy: sortBy, sortOrder: sortOrder, faculty: faculty, roomName: roomName, includeRoom: true, includeUser: true);

            return View(sessions);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}