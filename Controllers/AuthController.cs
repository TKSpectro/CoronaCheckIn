using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AccountManager _accountManager;

        public AuthController(ILogger<AuthController> logger, AccountManager accountManager)
        {
            _logger = logger;
            _accountManager = accountManager;
        }

        public IActionResult Login()
        {
            // TODO: Needs actual implementation
            _logger.LogDebug("[auth/login] called");
            IEnumerable<Account> account = _accountManager.GetAccounts();
            return View();
        }
        
        public IActionResult Register()
        {
            // TODO: Needs actual implementation
            return View();
        }
    }
}
