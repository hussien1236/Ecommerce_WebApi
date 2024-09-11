namespace WebApplication9.Models
{
    public class CartDTO
    {
        public List<CartItemDTO> Items { get; set; } = new();
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
