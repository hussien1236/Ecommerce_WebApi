using EntityFramework_practice.Models;

namespace WebApplication9.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice {  get; set; }

        //Navigation Properties
        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;

    }
}
