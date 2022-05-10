using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Account>().HasData(new Account[]
            // {
            //     new Account { Id = Guid.Parse("73c31f76-e39e-4248-9c18-66a08a5c62c9"), Email = "test@mail.com", Password = "123123123", Username = "test" }
            // });
        }

        public DbSet<Account> Accounts { get; set; }
    }
}