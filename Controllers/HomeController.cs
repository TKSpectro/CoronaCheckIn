using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CoronaCheckIn.Areas.api;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;

namespace CoronaCheckIn.Controllers
{
    public class HomeController : Controller
    
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionManager _sessionManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, SessionManager sessionManager, ApplicationDbContext context)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                return View();
            }

            return Redirect("/Identity/Account/Login");
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Imprint()
        {
            return View();
        }
        
        public IActionResult Frontoffice()
        {

            var userName = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userName);
            var result = new FrontofficeViewModel();
            var runningSessions = _sessionManager.GetSessions(userId: user.Id,
                endTimeNull: true, includeRoom: true, sortBy: "start", sortOrder: "desc").ToList();
            
            foreach (var session in runningSessions)
            {
                var maxEndTime = session.StartTime.AddMinutes(session.Room.MaxDuration);
                if (maxEndTime > DateTime.Now)
                {
                    session.EndTime = maxEndTime;
                    result.CurrentSession = session;
                    result.RemainingTime = (int)maxEndTime.Subtract(DateTime.Now).TotalMinutes;
                    
                    // Get current participants in this session
                    result.CurrentParticipants = _sessionManager.GetSessions(roomId: session.Room.Id, endTimeNull: true).Count();
                }
            }

            var sessions =
                _sessionManager.GetSessions(userId: user.Id, includeRoom: true, sortBy: "start", sortOrder: "desc");
            result.Sessions = sessions;

            return View(result);
        }

        public class ScanBodyPost
        {
            public string RoomId { get; set; }
            
            public DateTime Date { get; set; }
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Scan(ScanBodyPost body)
        {
            var userName = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userName);
            var roomId = new Guid(body.RoomId);
            var runningSessions = _sessionManager.GetSessions(userId: user?.Id, roomId: roomId,
                endTimeNull: true, includeRoom: true, sortBy: "start", sortOrder: "desc").ToList();

            var currentRunningSession = false;
            foreach (var session in runningSessions)
            {
                // TODO: Need to check this algorithm and proof with more test cases
                var maxEndTime = session.StartTime.AddMinutes(session.Room.MaxDuration);
                if (maxEndTime <= DateTime.Now)
                {
                    // Session is an old session which the user did not close
                    session.EndTime = maxEndTime;
                    _sessionManager.UpdateSession(session);
                }
                else
                {
                    if (roomId == session.RoomId)
                    {
                        // TODO: Check timestamp in qrcode against the one in the database
                        //body.Date == session.Room.QrCodeTimestamp;
                    }
                    // Session is the current session and the user scanned to leave
                    session.EndTime = DateTime.Now;
                    _sessionManager.UpdateSession(session);
                    currentRunningSession = true;
                }
            }

            if (currentRunningSession == false)
            {
                var newSession = new Session()
                {
                    Infected = false,
                    RoomId = roomId,
                    UserId = user.Id,
                    StartTime = DateTime.Now,
                };
                _sessionManager.AddSession(newSession);
                
                return Json("new");
            }
            
            return Json("closed");
        }

        public IActionResult SetLang(string lang, string url)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang))
            );
            return LocalRedirect(url);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult Setting()
        {
            return Redirect("/Identity/Account/Manage");
        }
    }

    public class FrontofficeViewModel
    {
        public Session? CurrentSession { get; set; } = null;

        public IEnumerable<Session> Sessions { get; set; } = new List<Session> { };
        
        public int RemainingTime { get; set; }
        
        public int CurrentParticipants { get; set; }
    }
}