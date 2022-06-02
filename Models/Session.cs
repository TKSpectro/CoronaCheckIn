namespace CoronaCheckIn.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool Infected { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}