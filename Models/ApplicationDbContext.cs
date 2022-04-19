using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
    }
}