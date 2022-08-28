using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoronaCheckIn.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly AccountManager _accountManager;
        private readonly SessionManager _sessionManager;
        private readonly InfectionManager _infectionManager;

        public AccountsController(ILogger<AccountsController> logger, AccountManager accountManager,
            SessionManager sessionManager, InfectionManager infectionManager)
        {
            _logger = logger;
            _accountManager = accountManager;
            _sessionManager = sessionManager;
            _infectionManager = infectionManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult List([FromQuery] string? search = null, bool? infectionSuccess = null)
        {
            ViewBag.title = "Accounts";
            ViewBag.infection = infectionSuccess;

            IEnumerable<User> accounts = _accountManager.GetAccounts(search: search);
            return View(accounts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Remove(Guid id)
        {
            _accountManager.Remove(id);
            return RedirectToAction("List");
        }

        public IActionResult SetAsInfected(Guid id)
        {
            bool infectionSuccess = false;
            if ( _infectionManager.CheckInfection(id.ToString()) != "infected!")
            {
                _sessionManager.SetSessionsAsInfected(id.ToString());
                infectionSuccess = true;
            }
            return RedirectToAction("List", new { infectionSuccess = infectionSuccess });
        }
    }
}