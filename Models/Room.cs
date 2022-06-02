namespace CoronaCheckIn.Models
{
    public class Room
    {
        public string Name { get; set; }  = string.Empty;

        public enum Faculty
        {
            AI, // Angewandte Informatik
            GET // Gebäude- und Energietechnik
        }

        public int MaxParticipants { get; set; }

        public int MaxDuration { get; set; }

        public string? QrCode { get; set; }
        
        public List<Session> Sessions { get; set; }
    }
}