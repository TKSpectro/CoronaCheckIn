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

        public IEnumerable<Infection> GetInfections(User? user, Guid? userId)
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

            return queryable.AsEnumerable();
        }

        public Infection? GetInfection(Guid id)
        {
            return Context.Infections.Find(id);
        }

        public Infection? Add(Infection infection)
        {
            var addedInfection = Context.Infections.Add(infection);
            Context.SaveChanges();

            return addedInfection.Entity;
        }
        
        public Infection? Update(Infection infection)
        {
            var editedInfection = Context.Infections.Update(infection);
            Context.SaveChanges();

            return editedInfection.Entity;
        }

        public void Remove(Infection infection)
        {
            Context.Infections.Remove(infection);
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var infection = Context.Infections.Find(id);
            if (infection == null)
            {
                throw new Exception($"Infection with {id} not found");
            }

            Context.Infections.Remove(infection);
            Context.SaveChanges();
        }
    }
}