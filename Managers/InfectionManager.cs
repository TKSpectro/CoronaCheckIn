using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoronaCheckIn.Managers
{
    public class InfectionManager
    {
        private ApplicationDbContext Context { get; set; }

        public InfectionManager(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<Infection> GetInfections(string? userId, DateTime? after = null ,User? user = null)
        {
            var queryable = Context.Infections.AsQueryable();

            if (user != null)
            {
                queryable = queryable.Where(infection => infection.UserId == user.Id);
            }
            if (userId != null)
            {
                queryable = queryable.Where(infection => infection.UserId == userId.ToString());
            }
            if (after != null)
            {
                queryable = queryable.Where(infection => infection.Date >= after);
            }

            return queryable.AsEnumerable();
        }

        public Infection? GetInfection(Guid id)
        {
            return Context.Infections.Find(id);
        }

        public Infection? AddInfection(Infection infection)
        {
            var addedInfection = Context.Infections.Add(infection);
            Context.SaveChanges();

            return addedInfection.Entity;
        }
        
        public Infection? UpdateInfection(Infection infection)
        {
            var editedInfection = Context.Infections.Update(infection);
            Context.SaveChanges();

            return editedInfection.Entity;
        }

        public void RemoveInfection(Infection infection)
        {
            Context.Infections.Remove(infection);
            Context.SaveChanges();
        }

        public void RemoveInfection(Guid id)
        {
            var infection = Context.Infections.Find(id);
            if (infection == null)
            {
                throw new Exception($"Infection with {id} not found");
            }

            Context.Infections.Remove(infection);
            Context.SaveChanges();
        }

        public string CheckInfection(string id)
        {
            DateTime localDate = DateTime.Now.AddDays(-7);
            var infections = GetInfections(userId: id , after: localDate);
            Console.WriteLine("infection");
            Console.WriteLine(infections.ToArray().GetLength(0));
            if (infections.ToArray().GetLength(0) > 0)
            {
                return "infected!";
            }
            return "not infected";

        }
    }
}