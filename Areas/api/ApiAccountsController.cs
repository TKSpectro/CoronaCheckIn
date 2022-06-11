using CoronaCheckIn.Managers;
using CoronaCheckIn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoronaCheckIn.Areas.api
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly AccountManager _accountManager;

        public AccountsController(ILogger<AccountsController> logger, AccountManager accountManager)
        {
            _logger = logger;
            _accountManager = accountManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAccounts()
        {
            var accounts = _accountManager.GetAccounts();
            return accounts.ToArray();
        }
    
        [HttpGet("{id}")]
        public ActionResult<User> GetAccount(Guid id)
        {
            var account = _accountManager.GetAccount(id);
            if (account == null)
            {
                return NotFound();
            }
        
            return account;
        }
    }
}

