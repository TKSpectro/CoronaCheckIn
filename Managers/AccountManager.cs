using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn.Managers
{
    public class AccountManager
    {
        public AccountManager(ApplicationDbContext context)
        {
            Container = context;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return Container.Accounts.AsEnumerable();
        }
        
        public Account? GetAccount(Guid id)
        {
            return Container.Accounts.Find(id);
        }

        public void Add(Account account)
        {
            account.Id = Guid.NewGuid();
            Container.Accounts.Add(account);
            Container.SaveChanges();
        }
        
        public void Remove(Guid id)
        {
            Account? account = GetAccount(id);
            if (account == null)
            {
                throw new Exception("Account not found");
            }
            
            Container.Accounts.Remove(account);
            Container.SaveChanges();
        }

        public void Remove(Account account)
        {
            Container.Accounts.Remove(account);
            Container.SaveChanges();
        }

        private ApplicationDbContext Container { get; set; }
    }
}

