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

        public AccountsController(ILogger<AccountsController> logger, AccountManager accountManager)
        {
            _logger = logger;
            _accountManager = accountManager;
        }

        // public IActionResult Index()
        // {
        //     return RedirectToAction("Index");
        // }
        
        [Authorize(Roles = "Admin")]
        public IActionResult List()
        {
            ViewBag.title = "Accounts";

            IEnumerable<User> accounts = _accountManager.GetAccounts();
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
    }
}
