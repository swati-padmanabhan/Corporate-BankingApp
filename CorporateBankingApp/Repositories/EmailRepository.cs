using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace CorporateBankingApp.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly string _fromEmail = "apropayments@gmail.com";
        private readonly string _password = "hxby ycrh efvw sclw";

        public void SendEmailNotification(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromEmail, _password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true, // if you want HTML email
            };
            mailMessage.To.Add(to);

            smtpClient.Send(mailMessage);
        }
    }
}