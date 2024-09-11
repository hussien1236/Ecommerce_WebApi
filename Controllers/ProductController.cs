using EntityFramework_practice.Data;
using EntityFramework_practice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication9.Models;

namespace WebApplication9.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment env;

        public ProductController(ApplicationDbContext _context, IWebHostEnvironment _env)
        {
            context = _context;
            env = _env;
        }
        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            return Ok(context.Categories.ToList());
        }
        [HttpGet]
        public IActionResult GetProducts(string? search, string? category,int? min, int? max, string? sort, string? order, int? page)
        {
            var products = context.Products.Include(p => p.category).ToList();
            if(!string.IsNullOrEmpty(search)) {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)|| p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.category.Name.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if(min != null)
            {
                products = products.Where(p => p.price >= min).ToList();
            }
            if (max != null)
            {
                products = products.Where(p => p.price <= max).ToList();
            }
            if (string.IsNullOrEmpty(sort))
                sort = "id";
            if (sort.ToLower() == "name")
            {
                if (order == "desc")
                    products =products.OrderByDescending(p => p.Name).ToList();
                else
                    products = products.OrderBy(p => p.Name).ToList();
            }
            if (sort.ToLower() == "category")
            {
                if (order == "desc")
                    products = products.OrderByDescending(p => p.category.Name).ToList();
                else
                    products = products.OrderBy(p => p.category.Name).ToList();
            }
            if (sort.ToLower() == "Brand")
            {
                if (order == "desc")
                    products = products.OrderByDescending(p => p.Brand).ToList();
                else
                    products = products.OrderBy(p => p.Brand).ToList();
            }
            if (sort.ToLower() == "date")
            {
                if (order == "desc")
                    products = products.OrderByDescending(p => p.CreatedAt).ToList();
                else
                    products = products.OrderBy(p => p.CreatedAt).ToList();
            }
            if (sort.ToLower() == "price")
            {
                if (order == "desc")
                    products = products.OrderByDescending(p => p.price).ToList();
                else
                    products = products.OrderBy(p => p.price).ToList();
            }
            else
            {
                if (order == "desc")
                    products = products.OrderByDescending(p => p.Id).ToList();
                else
                    products = products.OrderBy(p => p.Id).ToList();
            }
            if (page == null || page<1)
                page = 1;
            int pageSize = 5;
            int count = products.Count;
            int totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            products = products.Skip((int)((page-1) * pageSize)).Take(pageSize).ToList();
            var result = new
            {
                Product = products,
                Page = page,
                TotalPages = totalPages,
                PageSize = pageSize,
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = context.Products.Include(p => p.category).FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddProduct([FromForm] ProductDTO productdto)
        {
            var categories = context.Categories;
            if (!categories.Any(c => c.Name == productdto.category)) return BadRequest("invalid category");
            if (productdto.ImageFile == null) return BadRequest("you should upload an image ");
            //Add the Product image to the server
            var ProductName = DateTime.Now.ToString("yyyyMMddhhmmss");
            ProductName += Path.GetExtension(productdto.ImageFile.FileName);
            var path = env.WebRootPath + "/Images/Products/";
            using (var stream = System.IO.File.Create(path + ProductName))
            {
                productdto.ImageFile.CopyTo(stream);
            }
            //Add the product to the database 
            Product product = new Product
            {
                Name = productdto.Name,
                Brand = productdto.Brand,
                category= categories.FirstOrDefault(c => c.Name == productdto.category)! ,
                price = productdto.price,
                Description = productdto.Description,
                ImageFileName = ProductName
            };
            context.Products.Add(product);
            context.SaveChanges();
            return Ok(product);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id,[FromForm] ProductDTO productdto)
        {
            var categories = context.Categories;
            if (!categories.Any(c => c.Name == productdto.category)) return BadRequest("invalid category");
            var product = context.Products.Find(id);
            if (product == null) return NotFound();
            if (productdto.ImageFile != null)
            {
                //save image on server
                string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                fileName += Path.GetExtension(productdto.ImageFile.FileName);
                var path = env.WebRootPath+ "/Images/Products/";
                using (var stream = System.IO.File.Create(path + fileName))
                {
                    productdto.ImageFile.CopyTo(stream);
                }
                //delete image from the server
                System.IO.File.Delete(path + product.ImageFileName);
                product.ImageFileName = fileName;
            }
            product.Name = productdto.Name;
            product.Brand = productdto.Brand;
            product.category = categories.FirstOrDefault(c => c.Name == productdto.category)!;
            product.price = productdto.price;
            product.Description = productdto.Description;
            //save in the database 
           
            context.SaveChanges();
            return Ok(product);

        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.Products.Find(id);
            if (product == null) return NotFound();
            System.IO.File.Delete(env.WebRootPath + "/Images/Products/" + product.ImageFileName);
            context.Products.Remove(product);
            context.SaveChanges();
            return Ok();
        }
      
    }
}
