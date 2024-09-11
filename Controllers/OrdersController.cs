using EntityFramework_practice.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication9.Models;
using WebApplication9.Services;

namespace WebApplication9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public OrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetOrder(int? page)
        {
            var id = JwtReader.GetUserId(User);
            var role = JwtReader.GetUserRole(User);
            IQueryable<Order> query = context.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(item => item.Product);
            //Adding pagination
            if (page == null || page < 1)
                page = 1;
            int count = query.Count();
            int PageSize = 5;
            query = query.OrderByDescending(q => q.Id).Skip(((int)page - 1) * PageSize).Take(PageSize);
            int TotalPages = (int)Math.Ceiling((decimal)count / PageSize);
            if (role != "admin")
            {
                query = query.Where(q => q.Id == id);
            }

            //avoiding object cycle
            var orders = query.ToList();
            foreach (var o in orders)
            {
                foreach (var item in o.Items)
                    item.Order = null;
                o.User.Password = "";
            }
            var result = new
            {
                Orders = query,
                Page = page,
                totalPages = TotalPages,
                pageSize = query.Count()
            };
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = context.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                ModelState.AddModelError("id", "the order is not found");
                return NotFound(ModelState);
            }
            //avoid object cycle
            foreach (var item in order.Items)
                item.Order = null;
            order.User.Password = "";
            return Ok(order);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, string? PaymentStatus, string? OrderStatus)
        {
            var order = context.Orders.Find(id);

            if (order == null)
            {
                ModelState.AddModelError("Id", "invalid order id");
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(PaymentStatus) && string.IsNullOrEmpty(OrderStatus))
            {
                ModelState.AddModelError("Update order", "there is no thing to update");
                return BadRequest(ModelState);
            }
            if (!string.IsNullOrEmpty(PaymentStatus) && !OrderHelper.PaymentStatuses.Contains(PaymentStatus))
            {
                ModelState.AddModelError("PaymentStatus", "invalid payment status");
                return BadRequest(ModelState);
            }
            if (!string.IsNullOrEmpty(OrderStatus) && !OrderHelper.OrderStatuses.Contains(OrderStatus))
            {
                ModelState.AddModelError("OrderStatus", "invalid order status");
                return BadRequest(ModelState);
            }
            if (PaymentStatus != null)
                order.PaymentStatus = PaymentStatus;
            if (OrderStatus != null)
                order.OrderStatus = OrderStatus;
            context.SaveChanges();
            return Ok(order);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            try
            {
                var order = new Order()
                {
                    Id = id
                };
                context.Remove(order);
                context.SaveChanges();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDTO orderdto)
        {
            if (!OrderHelper.PaymentMethods.ContainsKey(orderdto.PaymentMethod))
            {
                ModelState.AddModelError("Payment Method", "invalid payment method");
                return BadRequest(ModelState);
            }
            var uId = JwtReader.GetUserId(User);
            var user = context.Users.Find(uId);
            if (user == null)
            {
                ModelState.AddModelError("Credentials", "something went wrong");
                return BadRequest(ModelState);
            }
            Dictionary<int, int> ProductsDic = OrderHelper.getdictionary(orderdto.ProductIdentifiers);
            Order order = new()
            {
                ShippingFee = OrderHelper.ShippingFee,
                PaymentMethod = orderdto.PaymentMethod,
                PaymentStatus = OrderHelper.PaymentStatuses[0],//Pending 
                OrderStatus = OrderHelper.OrderStatuses[0], //Created
                UserId = uId,
                DeliveryAddress = orderdto.DeliveryAddress
            };

            var products = context.Products.ToList();
            foreach(var pair in  ProductsDic)
            {
                var p = products.Find(p => p.Id == pair.Key);
                if(p == null)
                {
                    ModelState.AddModelError("Order", "Cannot create this order");
                    return BadRequest(ModelState);
                }

                OrderItem item = new()
                {
                    ProductId = pair.Key,
                    Quantity = pair.Value,
                    UnitPrice = p.price
                };
                //Add item to the order list
                order.Items.Add(item);
            }
            if (order.Items.Count < 1)
                return NoContent();
            context.Orders.Add(order);
            context.SaveChanges();
            var orderWithItems = context.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == order.Id);

            foreach (var item in order.Items)
            {
                item.Order = null;
            }
            order.User.Password = "";
            return Ok(orderWithItems);
        }
    }
}
