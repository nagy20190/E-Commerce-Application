using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace E_CommerceApplication.DAL.Services
{
    public class EmailSender
    {
        public async Task SendEmail(string subject, string toEmail, string userName, string message)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            // var apiKey = "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";
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
