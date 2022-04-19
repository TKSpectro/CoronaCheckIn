using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoronaCheckIn.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}