using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoronaCheckIn.Controllers
{
    public class CasesController : Controller
    {
        private readonly ILogger<CasesController> _logger;
        private readonly SessionManager _sessionManager;

        public CasesController(ILogger<CasesController> logger, SessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index([FromQuery] string? roomName = null, [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null, [FromQuery] Faculty? faculty = null,
            [FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null)
        {
            var sessions = _sessionManager.GetSessions(isInfected: true, after: start, before: end,
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