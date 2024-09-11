using EntityFramework_practice.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication9.Models;

namespace WebApplication9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers(int? page)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }
            var users = context.Users;
            int pageSize = 1;
            int count = users.Count();
            int TotalPages = count / pageSize;
            var filteredUsers = users.OrderByDescending(u=> u.Id).Skip((int)(page-1)*pageSize).Take(pageSize);
            List<UserProfileDTO> profiles = new List<UserProfileDTO>();
            foreach (var user in filteredUsers)
            {
                var u = new UserProfileDTO();

                u.Id = user.Id;
                u.FirstName = user.FirstName;
                u.LastName = user.LastName;
                u.Email = user.Email;
                u.Phone = user.Phone;
                u.Address = user.Address;
                u.Role = user.Role;
                u.CreatedAt = user.CreatedAt;
                
                profiles.Add(u);
            }
            var response = new
            {
                Users = profiles,
                TotalPages = TotalPages,
                PageSize = pageSize,
                Page = page

            };
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = context.Users.Find("id");
            if (user == null)
                return NotFound();
            var UserProfile = new UserProfileDTO();

            UserProfile.Id = user.Id;
            UserProfile.FirstName = user.FirstName;
            UserProfile.LastName = user.LastName;
            UserProfile.Email = user.Email;
            UserProfile.Phone = user.Phone;
            UserProfile.Address = user.Address;
            UserProfile.Role = user.Role;
            UserProfile.CreatedAt = user.CreatedAt;
            return Ok(UserProfile);
        }
    }
}
