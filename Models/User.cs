using Microsoft.AspNetCore.Identity;

namespace CoronaCheckIn.Models
{
    public class User : IdentityUser
    {
        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        public List<Session> Sessions { get; set; }

        public List<Infection> Infections { get; set; }
    }
}