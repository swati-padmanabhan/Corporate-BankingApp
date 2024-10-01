using CorporateBankingApp.Repositories;
using System;

namespace CorporateBankingApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        private string GenerateEmailTemplate(string title, string messageContent)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        width: 100%;
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        background-color: #4CAF50;
                        color: white;
                        padding: 10px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        padding: 20px;
                        line-height: 1.6;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 10px;
                        font-size: 12px;
                        color: #777;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='content'>
                        <p>{messageContent}</p>
                    </div>
                    <div class='footer'>
                        <p>Thank you for choosing our banking service.</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        public void SendOnboardingAcceptanceEmail(string clientEmail)
        {
            string title = "Account Accepted";
            string messageContent = "Dear Client,<br/>Your account has been accepted. Welcome to our banking service.";
            string body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendOnboardingEmailNotification(clientEmail, title, body);
        }

        public void SendOnboardingRejectionEmail(string clientEmail, string reason)
        {
            string title = "Account Rejected";
            string messageContent = $"Dear Client,<br/>We regret to inform you that your account has been rejected for the following reason: {reason}.";
            string body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendOnboardingEmailNotification(clientEmail, title, body);
        }
    }
}
