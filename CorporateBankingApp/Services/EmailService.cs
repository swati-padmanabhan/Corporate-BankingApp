using CorporateBankingApp.DTOs;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

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
                        background-color: #4e4187; /* Primary color */
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
                    .employee-list {{
                        margin-top: 15px;
                        border-collapse: collapse;
                        width: 100%;
                    }}
                    .employee-list th, .employee-list td {{
                        border: 1px solid #ddd;
                        padding: 8px;
                    }}
                    .employee-list th {{
                        background-color: #4e4187; /* Primary color */
                        color: white;
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
                        <p>Thank you for trusting us with your banking needs.</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        public void SendOnboardingAcceptanceEmail(string clientEmail)
        {
            string title = "Welcome to Our Banking Family!";
            string messageContent = "Dear Client,<br/>We are excited to let you know that your account has been successfully activated. Welcome aboard!";
            string body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendEmailNotification(clientEmail, title, body);
        }

        public void SendOnboardingRejectionEmail(string clientEmail, string reason)
        {
            string title = "Account Application Update";
            string messageContent = $"Dear Client,<br/>We regret to inform you that your account application has been declined for the following reason: {reason}. Please reach out for further assistance.";
            string body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendEmailNotification(clientEmail, title, body);
        }


        public void SendClientOnboardingStatusEmail(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("apropayments@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("apropayments", "hxby ycrh efvw sclw"),
                EnableSsl = true,
            };
            smtpClient.Send(mailMessage);
        }


        public void SendSalaryDisbursementApprovalEmail(string clientEmail, EmployeeDTO employee, double salaryAmount, string month)
        {
            var title = "Salary Disbursement Request Approved";
            var body = $@"
                Dear Client,<br/><br/>
                We are pleased to inform you that the salary disbursement request for your employee <strong>{employee.FirstName} {employee.LastName}</strong> 
                for the month of <strong>{month}</strong> amounting to <strong>{salaryAmount:C}</strong> has been approved successfully.<br/><br/>
                Thank you,<br/>
                Corporate Banking Application Team";

            _emailRepository.SendEmailNotification(clientEmail, title, body);
        }

        public void SendBatchSalaryDisbursementApprovalEmail(string clientEmail, List<EmployeeDTO> employeeSalaries, string month)
        {
            var title = "Salary Disbursement Approval Notification";

            var messageContent = "Dear Client,<br/><br/>" +
                                 $"We are delighted to announce that the salary disbursement requests for the following employees for the month of <strong>{month}</strong> have been approved:<br/><br/>" +
                                 "<table class='employee-list'>" +
                                 "<tr><th>Employee Name</th><th>Salary Amount</th></tr>";

            foreach (var employee in employeeSalaries)
            {
                messageContent += $"<tr><td><strong>{employee.FirstName} {employee.LastName}</strong></td><td><strong>{employee.Salary:C}</strong></td></tr>";
            }
            messageContent += "</table><br/>" +
                              "Thank you for your continued partnership,<br/>Corporate Banking Application Team";

            var body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendEmailNotification(clientEmail, title, body);
        }

        public void SendPayslipEmail(string employeeEmail, PayslipDTO payslip)
        {
            var title = "Your Payslip for " + payslip.Month;
            var messageContent = $@"
        Dear {payslip.EmployeeName},<br/><br/>
        We are pleased to provide you with your payslip for the month of <strong>{payslip.Month}</strong>.<br/><br/>
        <strong>Salary Amount:</strong> {payslip.Salary:C}<br/>
        <strong>Company:</strong> {payslip.CompanyName}<br/><br/>
        Thank you,<br/>
        Corporate Banking Application Team";

            var body = GenerateEmailTemplate(title, messageContent);
            _emailRepository.SendEmailNotification(employeeEmail, title, body);
        }



    }
}
