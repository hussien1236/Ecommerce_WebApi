using Microsoft.AspNetCore.SignalR;

namespace WebApplication9.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId {get;set;}
        public DateTime CreatedAt = DateTime.Now;
        public decimal ShippingFee { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string OrderStatus { get; set; } = "";

        //Navigation Properties
        public User User { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
    }
}
