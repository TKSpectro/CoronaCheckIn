namespace CoronaCheckIn.Models
{
    public class Session
    {
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }

        public bool Infected { get; set; }
        
        public Room Room { get; set; }
        
        public User User { get; set; }
    }
}