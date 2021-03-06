using System.Diagnostics;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Controllers
{
    public class HomeController : Controller
    
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
}