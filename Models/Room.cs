namespace CoronaCheckIn.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxParticipants { get; set; }

        public int MaxDuration { get; set; }

        public Faculties Faculty { get; set; }

        public string? QrCode { get; set; }

        public List<Session> Sessions { get; set; }
    }

    public enum Faculties
    {
        AI, // Angewandte Informatik
        GET // Gebäude- und Energietechnik
    }
}