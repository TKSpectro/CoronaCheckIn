namespace CoronaCheckIn.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxParticipants { get; set; }

        public int MaxDuration { get; set; }

        public Faculty Faculty { get; set; }

        public byte[]? QrCode { get; set; }

        public static Faculty? ParseFacultyFromString(string? faculty)
        {
            Faculty? parsedFaculty = null;
            if (faculty != null)
            {
                if (Enum.TryParse(faculty, out Faculty tryParsing))
                {
                    parsedFaculty = tryParsing;
                }
                else
                {
                    Console.WriteLine("[API/ROOMS] Faculty could not be parsed");
                }    
            }

            return parsedFaculty;
        }
    }
}