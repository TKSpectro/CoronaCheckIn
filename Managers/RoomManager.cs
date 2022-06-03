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

        public IEnumerable<Room> GetRooms(Faculties? faculty)
        {
            var queryable = Context.Rooms.AsQueryable();

            if (faculty != null)
            {
                queryable = queryable.Where(room => room.Faculty.Equals(faculty));
            }

            return queryable.AsEnumerable();
        }

        public Room? GetRoom(Guid id)
        {
            return Context.Rooms.Find(id);
        }

        public Room? Add(Room room)
        {
            var addedRoom = Context.Rooms.Add(room);
            Context.SaveChanges();

            return addedRoom.Entity;
        }

        public Room? Update(Room room)
        {
            var editedRoom = Context.Rooms.Update(room);
            Context.SaveChanges();

            return editedRoom.Entity;
        }

        public void Remove(Room room)
        {
            Context.Rooms.Remove(room);
            Context.SaveChanges();
        }

        public void Remove(Guid id)
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