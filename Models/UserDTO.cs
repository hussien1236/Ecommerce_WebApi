using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    public class UserDTO
    {
        public string FirstName { get; set; } = "";
        [MaxLength(100)]
        public string LastName { get; set; } = "";
        [EmailAddress,MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string Phone { get; set; } = "";
        [MaxLength(100)]
        public string Address { get; set; } = "";
        [MaxLength(100)]
        public string Password { get; set; } = "";
       
    }
}
