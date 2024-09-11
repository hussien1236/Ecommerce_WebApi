using System.Net;
using System.Net.Mail;

namespace WebApplication9.Services
{
    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmail(string subject, string toEmail, string username, string message)
        {
           
                var MailProvider = configuration["EmailSettings:MailProvider"];
                string pwd = configuration["EmailSettings:Pwd"]!;

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                //client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(MailProvider, pwd);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(MailProvider!);
                mailMessage.To.Add(new MailAddress(toEmail));
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                await client.SendMailAsync(mailMessage);
        }
    }
}

