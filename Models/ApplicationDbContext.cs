using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
    }

    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            Console.WriteLine("Seeding...");

            List<Account> accounts = new List<Account>{
                new Account {
                    Username = "user1",
                    Email = "user.1@ethereal.email",
                    Password = "123123123",
                },
                new Account {
                    Username = "user2",
                    Email = "user.2@ethereal.email",
                    Password = "123123123",
                },
                new Account {
                    Username = "user3",
                    Email = "user.3@ethereal.email",
                    Password = "123123123",
                },
                new Account {
                    Username = "user4",
                    Email = "user.4@ethereal.email",
                    Password = "123123123",
                },
            };

            _context.Accounts.AddRange(accounts);
            _context.SaveChanges();
        }
    }
}