using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = "";
        [MaxLength(100)]
        public string LastName { get; set; } = "";
        [MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string? Phone { get; set; } 
        [MaxLength(100)]
        public string Address { get; set; } = "";
        [MaxLength(100)]
        public string Role { get; set; } = "";
        public DateTime CreatedAt = DateTime.Now;

    }
}
