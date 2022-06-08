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
                        Id = "00000000-0000-0000-0001-000000000001",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    });
            }

            _context.Roles.AddRange(roles);
            _context.SaveChanges();
            
            adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");

            Console.WriteLine("[Seed] Adding User");

            List<User> accounts = new List<User>();
            var adminUser = _context.Users.FirstOrDefault(r => r.Email == "admin@ethereal.com");
            if (adminUser == null)
            {
                accounts.Add(
                    new() 
                    {
                        Id = "00000000-0000-0000-0002-000000000001",
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
            
            var testUser = _context.Users.FirstOrDefault(r => r.Email == "test@ethereal.com");
            if (testUser == null)
            {
                accounts.Add(
                    new() 
                    {
                        Id = "00000000-0000-0000-0002-000000000002",
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

            _context.Users.AddRange(accounts);
            _context.SaveChanges();
            
            testUser = _context.Users.FirstOrDefault(r => r.Email == "test@ethereal.com");
            adminUser = _context.Users.FirstOrDefault(r => r.Email == "admin@ethereal.com");
            
            Console.WriteLine("[Seed] Adding User-Roles");

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
                    Id = new Guid("00000000-0000-0000-0003-000000000001"),
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
                    Id = new Guid("00000000-0000-0000-0003-000000000002"),
                    Name = "7.2.01",
                    Faculty = Faculties.GET,
                    MaxDuration = 90,
                    MaxParticipants = 25
                });
            }

            _context.Rooms.AddRange(rooms);
            _context.SaveChanges();

            aiRoom = _context.Rooms.FirstOrDefault(r => r.Name == "5.1.05" && r.Faculty == Faculties.AI);
            getRoom = _context.Rooms.FirstOrDefault(r => r.Name == "7.2.01" && r.Faculty == Faculties.GET);

            Console.WriteLine("[Seed] Adding Sessions");

            List<Session> sessions = new List<Session>();
            if (testUser?.Id != null && adminUser?.Id != null && aiRoom?.Id != null && getRoom?.Id != null)
            {
                {
                    if (_context.Sessions.Find(new Guid("00000000-0000-0000-0004-000000000001")) == null)
                    {
                        sessions.Add(new Session()
                        {
                            Id = new Guid("00000000-0000-0000-0004-000000000001"),
                            UserId = adminUser.Id,
                            RoomId = aiRoom.Id,
                            StartTime = DateTime.Parse("2022-05-28T07:58:00.0000000Z"),
                            EndTime = DateTime.Parse("2022-05-28T09:33:00.0000000Z"),
                            Infected = false,
                        });
                    }

                    if (_context.Sessions.Find(new Guid("00000000-0000-0000-0004-000000000002")) == null)
                    {
                        sessions.Add(new Session()
                        {
                            Id = new Guid("00000000-0000-0000-0004-000000000002"),
                            UserId = testUser.Id,
                            RoomId = aiRoom.Id,
                            StartTime = DateTime.Parse("2022-05-28T08:00:00.0000000Z"),
                            EndTime = DateTime.Parse("2022-05-28T09:25:00.0000000Z"),
                            Infected = false,
                        });
                    }

                    if (_context.Sessions.Find(new Guid("00000000-0000-0000-0004-000000000003")) == null)
                    { 
                        sessions.Add(new Session()
                        {
                            Id = new Guid("00000000-0000-0000-0004-000000000003"),
                            UserId = testUser.Id,
                            RoomId = getRoom.Id,
                            StartTime = DateTime.Parse("2022-05-29T08:14:00.0000000Z"),
                            EndTime = DateTime.Parse("2022-05-29T09:44:00.0000000Z"),
                            Infected = false,
                        });
                    }

                    if (_context.Sessions.Find(new Guid("00000000-0000-0000-0004-000000000004")) == null)
                    {
                        sessions.Add(new Session()
                        {
                            Id = new Guid("00000000-0000-0000-0004-000000000004"),
                            UserId = testUser.Id,
                            RoomId = getRoom.Id,
                            StartTime = DateTime.Parse("2022-05-30T08:14:00.0000000Z"),
                            EndTime = DateTime.Parse("2022-05-30T09:44:00.0000000Z"),
                            Infected = true,
                        });
                    }
                };
            }
            
            _context.Sessions.AddRange(sessions);
            _context.SaveChanges();
        }
    }
}