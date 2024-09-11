using EntityFramework_practice.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication9.Models;
using WebApplication9.Services;

namespace WebApplication9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CartController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet("PaymentMethods")]
        public IActionResult GetPaymentMethods()
        {
            return Ok(OrderHelper.PaymentMethods);
        }
        [HttpGet]
        public IActionResult GetCart(string ProductsIds)
        {
         Dictionary<int, int> productsInfo= OrderHelper.getdictionary(ProductsIds);
         CartDTO cartdto = new();
            cartdto.ShippingFee = OrderHelper.ShippingFee;
            cartdto.SubTotal = 0;
            var products = context.Products.Include(p => p.category).ToList();
         foreach (var productId in productsInfo)
            {
                var product = products.FirstOrDefault(p=> p.Id ==productId.Key);
                if(product == null) { continue; }
                var item = new CartItemDTO();
                item.Product = product;
                item.Quantity = productId.Value;
                cartdto.Items.Add(item);
                cartdto.SubTotal += product.price * productId.Value;
                cartdto.Total = cartdto.SubTotal + cartdto.ShippingFee;
            }
            return Ok(cartdto);
        }
    }
}
