using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoronaCheckIn.Models;

namespace CoronaCheckIn.Controllers;

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
