using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;

namespace CoronaCheckIn.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountsController(ILogger<AccountsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.title = "Accounts";

            IEnumerable<Account> accounts = _context.Accounts;
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
                _context.Add(model);

                // TODO: Encrypt Password

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Delete(Guid id)
        {
            Account? account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // TODO Add Edit and Delete Examples

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
