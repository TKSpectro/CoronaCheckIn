using System.Diagnostics;
using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionManager _sessionManager;

        public HomeController(ILogger<HomeController> logger, SessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        public IActionResult Index()
        {
            var result = new BackofficeViewModel();
            if (User.Identity is { IsAuthenticated: true })
            {
                var sessions = _sessionManager.GetSessions(includeRoom: true, sortBy: "start", sortOrder: "desc",
                    includeUser: true, limit: 5);
                var infections = _sessionManager.GetSessions(includeRoom: true, sortBy: "start", sortOrder: "desc",
                    includeUser: true, limit: 5, isInfected: true);
                result.Sessions = sessions;
                result.Infections = infections;
                return View(result);
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

    public class BackofficeViewModel
    {
        public IEnumerable<Session> Sessions { get; set; } = new List<Session> { }; 
        public IEnumerable<Session> Infections { get; set; } = new List<Session> { };
    }
}