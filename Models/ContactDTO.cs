using System.ComponentModel.DataAnnotations;

namespace EntityFramework_practice.Models
{
    public class ContactDTO
    {
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        [Phone]
        public string Phone { get; set; } = "";
        [Required]
        public int SubjectId { get; set; }
        [Required] 
        public string message { get; set; } = "";
    }
}
