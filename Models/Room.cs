using System.ComponentModel.DataAnnotations;

namespace CoronaCheckIn.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Required")]
        [Range(1, 25, ErrorMessage = "Please enter a value bigger than {1} and smaller then {2}")]
        public int MaxParticipants { get; set; }
        
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int MaxDuration { get; set; }
        
        [Required(ErrorMessage = "Required")]
        public Faculty Faculty { get; set; }

        public byte[]? QrCode { get; set; }

        public DateTime? QrCodeCreatedAt { get; set; }

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