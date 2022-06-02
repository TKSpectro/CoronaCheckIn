using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn.Models
{
    public class User: IdentityUser
    {
        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;
        
        public List<Session> Sessions { get; set; }
        
        public List<Infection> Infections { get; set; }
    }
}