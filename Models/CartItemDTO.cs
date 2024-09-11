using EntityFramework_practice.Models;

namespace WebApplication9.Models
{
    public class CartItemDTO
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
