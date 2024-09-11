using System.ComponentModel.DataAnnotations;
using WebApplication9.Models;

namespace EntityFramework_practice.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = "";
        [Required, MaxLength(50)]
        public string Brand { get; set; } = "";
        [MaxLength(100)]
        public required Category category { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required, MaxLength(400)]
        public string Description { get; set; } = "";
        [Required]
        public string ImageFileName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
