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

        public IEnumerable<User> GetAccounts(string? search = null)
        {
            var queryable = Container.Users.AsQueryable();
            
            if (search != null)
            {
                var searchString = search.ToLower().Trim();
                // Entity Framework doesnt support direct full text search so we do some lame string check
                queryable = queryable.Where(user => user.Email.ToLower().Trim().Contains(searchString) || 
                    user.UserName.ToLower().Trim().Contains(searchString) || 
                    user.Firstname.ToLower().Trim().Contains(searchString) || 
                    user.Lastname.ToLower().Trim().Contains(searchString));
            }
            
            return queryable.OrderBy(u => u.Email).AsEnumerable();
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

