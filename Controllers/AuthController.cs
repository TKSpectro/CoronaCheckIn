using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthController(ILogger<AuthController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Login()
        {
            // TODO: Needs actual implementation
            _logger.LogDebug("[auth/login] called");
            Account? account = _context.Accounts.FirstOrDefault();
            return View(account);
        }
        
        public IActionResult Register()
        {
            // TODO: Needs actual implementation
            return View();
        }
    }
}
