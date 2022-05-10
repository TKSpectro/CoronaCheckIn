using System.Diagnostics;
using CoronaCheckIn.Managers;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;

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

        public IActionResult Index()
        {
            ViewBag.title = "Accounts";

            IEnumerable<Account> accounts = _accountManager.GetAccounts();
            return View(accounts);
        }

        public IActionResult Create()
        {
            ViewBag.title = "Create Account";

            return View(new Account());
        }

        [HttpPost]
        public IActionResult Create(Account model)
        {
            if (ModelState.IsValid)
            {
                _accountManager.Add(model);

                // TODO: Encrypt Password
                
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Delete(Guid id)
        {
            _accountManager.Remove(id);

            return RedirectToAction("Index");
        }

        // TODO Add Edit

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
