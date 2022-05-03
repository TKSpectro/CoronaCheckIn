using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly ApplicationDbContext _context;

    public ApiController(ILogger<ApiController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IEnumerable<Account> Index()
    {
        IEnumerable<Account> accounts = _context.Accounts.ToList();
        return accounts;
    }
}