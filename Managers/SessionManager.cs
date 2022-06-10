using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoronaCheckIn.Managers
{
    public class SessionManager
    {
        private ApplicationDbContext Context { get; set; }

        public SessionManager(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<Session> GetSessions(Room? room = null, Guid? roomId = null, User? user = null, string? userId = null, bool? isInfected = null, DateTime? after = null, DateTime? before = null)
        {
            var queryable = Context.Sessions.AsQueryable();

            if (room != null)
            {
                queryable = queryable.Where(session => session.RoomId == room.Id);
            }
            if (roomId != null)
            {
                queryable = queryable.Where(session => session.RoomId == roomId);
            }
            if (user != null)
            {
                queryable = queryable.Where(session => session.UserId == user.Id);
            }
            if (userId != null)
            {
                queryable = queryable.Where(session => session.UserId == userId);
            }
            if (isInfected != null)
            {
                queryable = queryable.Where(session => session.Infected == isInfected);
            }
            if (after != null)
            {
                queryable = queryable.Where(session => session.StartTime >= after);
            }
            if (before != null)
            {
                queryable = queryable.Where(session => session.EndTime <= before || (session.EndTime == null && session.StartTime <= before));
            }

            return queryable.AsEnumerable();
        }

        public Session? GetSession(Guid id)
        {
            return Context.Sessions.Find(id);
        }

        public Session? AddSession(Session session)
        {
            var addedSession = Context.Sessions.Add(session);
            Context.SaveChanges();

            return addedSession.Entity;
        }

        public Session? UpdateSession(Session session)
        {
            var editedSession = Context.Sessions.Update(session);
            Context.SaveChanges();

            return editedSession.Entity;
        }

        public void RemoveSession(Session session)
        {
            Context.Sessions.Remove(session);
            Context.SaveChanges();
        }

        public void RemoveSession(Guid id)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                throw new Exception($"Session with {id} not found");
            }

            Context.Sessions.Remove(session);
            Context.SaveChanges();
        }
    }
}