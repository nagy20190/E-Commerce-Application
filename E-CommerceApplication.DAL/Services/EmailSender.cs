using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace E_CommerceApplication.DAL.Services
{
    public class EmailSender
    {
        private readonly string apiKey;
        private readonly string fromEmail;
        private readonly string senderName;

        public EmailSender(IConfiguration configuration)
        {
            apiKey = configuration["EmailSender:ApiKey"]!;
            fromEmail = configuration["EmailSender:FromEmail"]!;
            senderName = configuration["EmailSender:SenderName"]!;
        }

        public async Task SendEmail(string subject, string toEmail, string userName, string message)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mopp906k@gmail.com", "Mostafa Nagy E-Commerce");
            var to = new EmailAddress(toEmail, userName);
            var plainTextContent = message;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
