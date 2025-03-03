using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;


    namespace Utility
    {
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // إعدادات SMTP
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("assemkhashaba80@gmail.com", "usby pqet hoch znhl")
            };

            // إنشاء الرسالة
            var mailMessage = new MailMessage
            {
                From = new MailAddress("assemkhashaba0@gmail.com"),
                Subject = subject,
                Body = $"<html><body>{htmlMessage}</body></html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }

    }
}

