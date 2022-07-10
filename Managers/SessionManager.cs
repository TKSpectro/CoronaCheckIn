using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace CoronaCheckIn.Managers
{
    public class SessionManager
    {
        private ApplicationDbContext Context { get; set; }

        public SessionManager(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<Session> GetSessions(Room? room = null, Guid? roomId = null, User? user = null,
            string? userId = null, bool? isInfected = null, DateTime? after = null, DateTime? before = null,
            string? sortBy = null, string? sortOrder = "asc", Faculty? faculty = null,
            string? roomName = null, bool includeRoom = false, bool includeUser = false, bool endTimeNull = false)
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
            if (roomName != null)
            {
                // Entity Framework doesnt support direct full text search so we do some lame string check
                queryable = queryable.Where(session => session.Room.Name.ToLower().Trim().Contains(roomName.ToLower().Trim()));
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
            if (endTimeNull)
            {
                queryable = queryable.Where(session => session.EndTime == null);
            }
            if (faculty != null)
            {
                queryable = queryable.Where(session => session.Room.Faculty.Equals(faculty));
            }
            if (includeRoom)
            {
                queryable = queryable.Include(s => s.Room);
            }
            if (includeUser)
            {
                queryable = queryable.Include(s => s.User);
            }
            
            // The sorting should always be done as the last queryable change
            if (sortBy != null)
            {
                queryable = sortBy switch
                {
                    "room" => sortOrder == "desc"
                        ? queryable.OrderByDescending(s => s.Room.Name)
                        : queryable.OrderBy(s => s.Room.Name),
                    "user" => sortOrder == "desc"
                        ? queryable.OrderByDescending(s => s.User.Email)
                        : queryable.OrderBy(s => s.User.Email),
                    "faculty" => sortOrder == "desc"
                        ? queryable.OrderByDescending(s => s.Room.Faculty)
                        : queryable.OrderBy(s => s.Room.Faculty),
                    "start" => sortOrder == "desc"
                        ? queryable.OrderByDescending(s => s.StartTime)
                        : queryable.OrderBy(s => s.StartTime),
                    "end" => sortOrder == "desc"
                        ? queryable.OrderByDescending(s => s.EndTime)
                        : queryable.OrderBy(s => s.EndTime),
                    _ => queryable
                };
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