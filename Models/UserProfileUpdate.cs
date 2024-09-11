using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    [Index("Email", IsUnique=true)]
    public class UserProfileUpdate
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = "";
        [MaxLength(100)]
        public string LastName { get; set; } = "";
        [EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string? Phone { get; set; }
        [MaxLength(100)]
        public string Address { get; set; } = "";
    }
}
