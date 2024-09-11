using EntityFramework_practice.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WebApplication9.Models;
using WebApplication9.Services;

namespace WebApplication9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly EmailService emailService;

        public AccountsController(IConfiguration configuration, ApplicationDbContext context, EmailService emailService)
        {
            this.configuration = configuration;
            this.context = context;
            this.emailService = emailService;
        }
      //  [NonAction]
    
        [Authorize]
        [HttpGet("GetProfile")]
        public IActionResult GetProfile()
        {
            int id = JwtReader.GetUserId(User);
            if (id == 0)
                return Unauthorized();
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Unauthorized();
            var UserProfile = new UserProfileDTO()
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
            return Ok(UserProfile);
        }
        [Authorize]
        [HttpPut("UpdateProfile")]
        public IActionResult UpdateProfile(UserProfileUpdate userProfileUpdate)
        {
            var id = JwtReader.GetUserId(User);
            if (id == 0)
                return Unauthorized();
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Unauthorized();
            user.FirstName = userProfileUpdate.FirstName;
            user.LastName = userProfileUpdate.LastName;
            user.Email = userProfileUpdate.Email;
            user.Phone = userProfileUpdate.Phone ?? "";
            user.Address = userProfileUpdate.Address;
            context.SaveChanges();
            var UserProfile = new UserProfileDTO()
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone ?? "",
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
            return Ok(UserProfile);
        }
        [Authorize]
        [HttpPut("UpdatePassword")]
        public IActionResult UpdatePassword([Required, MinLength(8), MaxLength(50)] string password)
        {
            var id = JwtReader.GetUserId(User); 
            if (id == 0)
                return Unauthorized();
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Unauthorized();
            var passwordHahser = new PasswordHasher<User>();
            var encryptedPassword = passwordHahser.HashPassword(new User(),password);
            user.Password = encryptedPassword;
            context.SaveChanges();
            return Ok("password was updated");
        }
        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return NotFound();
            var forgetRequest = context.ResetRequests.FirstOrDefault(r => r.Email == email);
            if(forgetRequest != null)
            {
                context.ResetRequests.Remove(forgetRequest);
            }
            var resetRequest = new ResetRequests();
            resetRequest.Email = email;
            resetRequest.Token = Guid.NewGuid().ToString()+"-"+Guid.NewGuid().ToString();
            resetRequest.CreatedAt = DateTime.Now;
            context.ResetRequests.Add(resetRequest);
            context.SaveChanges();
            //send token
            string username = user.FirstName + " " + user.LastName;
            string message = "Dear " + username + "\n" +
                "We recieved your password reset request.\n" +
                "Please copy the following token and paste it in the Password Reset Form:\n" +
                resetRequest.Token +"\n\n"+
                "Best Regards\n";
            emailService.SendEmail("Password Reset",email,username,message).Wait();
            return Ok();

        }
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var resetRequest = context.ResetRequests.FirstOrDefault(r => r.Token == token);
            if (resetRequest == null)
            {
                ModelState.AddModelError("Token", "invalid or expired token");
                return BadRequest(ModelState);
            }
            var user = context.Users.FirstOrDefault(u => u.Email == resetRequest.Email);
            if (user == null)
            {
                ModelState.AddModelError("Token", "invalid or expired token");
                return BadRequest(ModelState);
            }
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(new User(),newPassword);
            context.ResetRequests.Remove(resetRequest);
            context.SaveChanges();
            return Ok("password has been updated");
        }

        [HttpPost("Register")]
        public IActionResult Register(UserDTO userdto)
        {
            int count = context.Users.Count(u => u.Email == userdto.Email);
            if(count > 0)
            {
                ModelState.AddModelError("Email", "The email is already used");
                return BadRequest(ModelState);
            }  
            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new User(), userdto.Password);
            User user = new User()
            {
                FirstName = userdto.FirstName,
                LastName = userdto.LastName,
                Email = userdto.Email,
                Address = userdto.Address,
                Phone = userdto.Phone,
                Password = encryptedPassword,
                Role = "client",
                CreatedAt = DateTime.Now 
            };
            var jwt = CreateJwt(user);
            var userProfile = new UserProfileDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
            var response = new
            {
                Jwt = jwt, 
                UserProfile = userProfile
            };
            //save to database
            context.Users.Add(user);
            context.SaveChanges();
            return Ok(response);
        }
        [HttpPost("Login")]
        public IActionResult Login(string email, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if(user == null)
            {
                ModelState.AddModelError("Email", "invalid email address or password");
                return BadRequest(ModelState);
            }
            var passwordHasher = new PasswordHasher<User>();
            var verified = passwordHasher.VerifyHashedPassword(new User(), user.Password, password);
            if(verified == PasswordVerificationResult.Failed) {
                ModelState.AddModelError("Password", "incorrect password");
                return BadRequest(ModelState);
            }
            var jwt = CreateJwt(user);
            var UserProfile = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName, 
                LastName = user.LastName,
                Email = email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
            var response = new
            {
                Token = jwt,
                User = UserProfile
            };
             return Ok(response);
        }
        [Authorize]
        [HttpGet("UserClaims")]
        public IActionResult getUserClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
                return BadRequest();
            Dictionary<string, string> UserClaims = new Dictionary<string, string>();
            foreach(Claim c in identity.Claims)
            {
                UserClaims.Add(c.Type, c.Value);
            }
            return Ok(UserClaims);
        }

        private string CreateJwt(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim( "id", ""+user.Id),
                new Claim("role", user.Role)
            };
            string strKey = configuration["JwtSettings:Key"]!; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(

                issuer: configuration["JwtSettings:Issuer"],
                
                audience: configuration["JwtSettings:Audience"],
                
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var strToken = new JwtSecurityTokenHandler().WriteToken(token);

            return strToken;
        }
    }
}
