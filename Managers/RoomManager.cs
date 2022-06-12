using CoronaCheckIn.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoronaCheckIn.Managers
{
    public class RoomManager
    {
        private ApplicationDbContext Context { get; set; }

        public RoomManager(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<Room> GetRooms(string? name = null, string? sortBy = null, string? sortOrder = "asc", Faculty? faculty = null)
        {
            var queryable = Context.Rooms.AsQueryable();

            if (name != null)
            {
                // Entity Framework doesnt support direct full text search so we do some lame string check
                queryable = queryable.Where(r => r.Name.ToLower().Trim().Contains(name.ToLower().Trim()));
            }
            
            if (faculty != null)
            {
                queryable = queryable.Where(room => room.Faculty.Equals(faculty));
            }
            
            // The sorting should always be done as the last queryable change
            if (sortBy != null)
            {
                queryable = sortBy switch
                {
                    "name" => sortOrder == "desc"
                        ? queryable.OrderByDescending(r => r.Name)
                        : queryable.OrderBy(r => r.Name),
                    "faculty" => sortOrder == "desc"
                        ? queryable.OrderByDescending(r => r.Faculty)
                        : queryable.OrderBy(r => r.Faculty),
                    "maxDuration" => sortOrder == "desc"
                        ? queryable.OrderByDescending(r => r.MaxDuration)
                        : queryable.OrderBy(r => r.MaxDuration),
                    "maxParticipants" => sortOrder == "desc"
                        ? queryable.OrderByDescending(r => r.MaxParticipants)
                        : queryable.OrderBy(r => r.MaxParticipants),
                    _ => queryable
                };
            }

            return queryable.AsEnumerable();
        }

        public Room? GetRoom(Guid id)
        {
            return Context.Rooms.Find(id);
        }
        public Room? GetRoomByName(string name)
        {
            return Context.Rooms.FirstOrDefault(r => r.Name == name);
        }

        public Room? AddRoom(Room room)
        {
            var addedRoom = Context.Rooms.Add(room);
            Context.SaveChanges();

            return addedRoom.Entity;
        }

        public Room? UpdateRoom(Room room)
        {
            var editedRoom = Context.Rooms.Update(room);
            Context.SaveChanges();

            return editedRoom.Entity;
        }

        public void RemoveRoom(Room room)
        {
            Context.Rooms.Remove(room);
            Context.SaveChanges();
        }

        public void RemoveRoom(Guid id)
        {
            var room = Context.Rooms.Find(id);
            if (room == null)
            {
                throw new Exception($"Room with {id} not found");
            }

            Context.Rooms.Remove(room);
            Context.SaveChanges();
        }
    }
}