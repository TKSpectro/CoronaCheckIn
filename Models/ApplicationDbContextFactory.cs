using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoronaCheckIn.Models
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=coronacheckin;User=sa;Password=Coronacheckin1;");
            optionsBuilder.EnableSensitiveDataLogging();
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

