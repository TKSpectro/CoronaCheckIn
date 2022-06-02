namespace CoronaCheckIn.Models
{
    public class Infection
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}