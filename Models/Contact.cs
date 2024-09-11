namespace EntityFramework_practice.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public required Subject subject { get; set; } 
        public string message { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
