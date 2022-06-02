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

        public IEnumerable<User> GetAccounts()
        {
            return Container.Users.AsEnumerable();
        }
        
        public User? GetAccount(Guid id)
        {
            return Container.Users.Find(id.ToString());
        }

        public void Add(User user)
        {
            Container.Users.Add(user);
            Container.SaveChanges();
        }
        
        public void Remove(Guid id)
        {
            User? account = GetAccount(id);
            if (account == null)
            {
                throw new Exception("User not found");
            }
            
            Container.Users.Remove(account);
            Container.SaveChanges();
        }

        public void Remove(User user)
        {
            Container.Users.Remove(user);
            Container.SaveChanges();
        }

        private ApplicationDbContext Container { get; set; }
    }
}

