using EntityFramework_practice.Data;
using EntityFramework_practice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication9.Services;

namespace EntityFramework_practice.Controllers
{
    [Route("api/Contacts")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly EmailService emailService;

        public ContactController(ApplicationDbContext context, EmailService emailService) {
            this.context = context;
            this.emailService = emailService;
        }
        [HttpGet("Subjects")]
        public IActionResult GetSubjects() { 
        var subjects = context.Subjects.ToList();
        if(subjects.Any())
            {
                return Ok(subjects);
            }
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult getContacts(int? page)
        {   if(page == null || page < 1)
                page = 1;
        //NbOfContacts-(PageSize*page)
        //26-(
            int PageSize = 5;
            int NbOfContacts = context.Contacts.Count();
            int NbOfPages = (int) Math.Ceiling((decimal)NbOfContacts / PageSize);
            var contacts = context.Contacts.Include(c => c.subject)
                .OrderByDescending(c=>c.Id)
                .Skip((int)(page -1) * PageSize)
                .Take(PageSize)
                .ToList();
            var results = new
            {
                Contacts =contacts,
                Page = page,
                TotalPages = NbOfPages
            };
            if(contacts.Any())
            return Ok(results);
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public IActionResult getContact(int id)
        {
            var contact = context.Contacts.Include(c=> c.subject).FirstOrDefault(c=> c.Id == id);
            if (contact != null)
                return Ok(contact);
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddContact(ContactDTO contactDTO)
        {
            var sub = context.Subjects.Find(contactDTO.SubjectId);
            if (sub == null) {
                ModelState.AddModelError("subject", "the subject is invalid");
                return BadRequest(ModelState);
            }
            Contact contact = new Contact
            {
                FirstName = contactDTO.FirstName,
                LastName = contactDTO.LastName,
                Email = contactDTO.Email,
                message = contactDTO.message,
                subject = sub,
                //subject.Id = contactDTO.SubjectId,
                Phone = contactDTO.Phone
            };
            context.Contacts.Add(contact);
            context.SaveChanges();
            //send email 
            string emailSubject = "Contact Confimation";
            string username = contact.FirstName + " " + contact.LastName;
            string message = "Dear " + username + "\n" +
                "we recieved your message. Thank you for contacting us.\n" +
                "Our team will contact you very soon. \n " +
                "Best Regards\n\n" +
                "Your Message " + contactDTO.message;
            emailService.SendEmail(emailSubject, contact.Email, username, message).Wait();
            return Ok(contact);
        }
        [HttpPut("id")]
        /* public IActionResult UpdateContact(int id,ContactDTO contactDTO) { 
         var contact = context.Contacts.Find(id);
             if (contact == null)
                 return NotFound();
             var subject = context.Subjects.Find(contactDTO.SubjectId);
             if(subject == null)
             {
                 ModelState.AddModelError("subject", "the subject is invalid");
                 return BadRequest(ModelState);
             }
             contact.FirstName = contactDTO.FirstName;
             contact.LastName = contactDTO.LastName;
             contact.Phone = contactDTO.Phone;
             contact.Email = contactDTO.Email;
             contact.message = contactDTO.message;
             contact.subject = subject;
             context.Update(contact);
             context.SaveChanges();
             return Ok(contact);
         }*/
        [Authorize(Roles = "admin")]
        [HttpDelete("id")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                Contact contact = new Contact()
                {
                    Id = id,
                    subject = new Subject()
                };
                context.Remove(contact);
                context.SaveChanges();
                return Ok("the contact is deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

