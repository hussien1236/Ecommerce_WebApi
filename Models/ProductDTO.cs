using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    public class ProductDTO
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = "";
        [Required, MaxLength(50)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(100)]
        public string category { get; set; } = "";
        [Required]
        public decimal price { get; set; }
        [Required, MaxLength(400)]
        public string Description { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
    }
}
