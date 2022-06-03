using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CoronaCheckIn.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<Infection> Infections { get; set; } = null!;
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
            Console.WriteLine("[Seed] Start Seeding...");

            Console.WriteLine("[Seed] Adding Roles");
            List<IdentityRole> roles = new List<IdentityRole>();
            var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                roles.Add(
                    new() 
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    });
            }

            _context.Roles.AddRange(roles);
            _context.SaveChanges();
            
            Console.WriteLine("[Seed] Adding User");

            List<User> accounts = new List<User>();
            var testUser = _context.Users.FirstOrDefault(r => r.Email == "test@ethereal.com");
            if (testUser == null)
            {
                accounts.Add(
                    new() 
                    {
                        Email = "test@ethereal.com",
                        NormalizedEmail = "TEST@ETHEREAL.COM",
                        UserName = "test@ethereal.com",
                        NormalizedUserName = "TEST@ETHEREAL.COM",
                        Firstname = "John",
                        Lastname = "Doe",
                        EmailConfirmed = true,
                        // CoronaCheckIn1!
                        PasswordHash = "AQAAAAEAACcQAAAAEGxpSFInnWQ9g7rPuupxXhzp5oqVjvYb8mO4X6xoZ/1ZCwI53XSWsOvk2XTu8hig7w=="
                    });
            }
            
            var adminUser = _context.Users.FirstOrDefault(r => r.Email == "admin@ethereal.com");
            if (adminUser == null)
            {
                accounts.Add(
                    new() 
                    {
                        Email = "admin@ethereal.com",
                        NormalizedEmail = "ADMIN@ETHEREAL.COM",
                        UserName = "admin@ethereal.com",
                        NormalizedUserName = "ADMIN@ETHEREAL.COM",
                        Firstname = "Admin",
                        Lastname = "Admin",
                        EmailConfirmed = true,
                        // CoronaCheckIn1!
                        PasswordHash = "AQAAAAEAACcQAAAAEGxpSFInnWQ9g7rPuupxXhzp5oqVjvYb8mO4X6xoZ/1ZCwI53XSWsOvk2XTu8hig7w=="
                    });
            }

            _context.Users.AddRange(accounts);
            _context.SaveChanges();
            
            Console.WriteLine("[Seed] Adding User-Roles");

            adminUser = _context.Users.FirstOrDefault(r => r.Email == "admin@ethereal.com");
            adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();
            if (adminUser != null && adminRole != null)
            {
                var adminUserAdminRole = _context.UserRoles.FirstOrDefault(r => r.UserId == adminUser.Id && r.RoleId == adminRole.Id);
                if (adminUserAdminRole == null)
                {
                    userRoles.Add(new()
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id,
                    });
                }
            }
            
            _context.UserRoles.AddRange(userRoles);
            _context.SaveChanges();
            
            Console.WriteLine("[Seed] Adding Rooms");

            var aiRoom = _context.Rooms.FirstOrDefault(r => r.Name == "5.1.05" && r.Faculty == Faculties.AI);
            var getRoom = _context.Rooms.FirstOrDefault(r => r.Name == "7.2.01" && r.Faculty == Faculties.GET);
            
            List<Room> rooms = new List<Room>();
            
            if (aiRoom == null)
            {
                rooms.Add(new Room()
                {
                    Name = "5.1.05",
                    Faculty = Faculties.AI,
                    MaxDuration = 90,
                    MaxParticipants = 25
                });
            }
            
            if (getRoom == null)
            {
                rooms.Add(new Room()
                {
                    Name = "7.2.01",
                    Faculty = Faculties.GET,
                    MaxDuration = 90,
                    MaxParticipants = 25
                });
            }

            _context.Rooms.AddRange(rooms);
            _context.SaveChanges();
        }
    }
}