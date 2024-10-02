using CorporateBankingApp.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Services
{
    public interface IEmailService
    {
        void SendOnboardingAcceptanceEmail(string clientEmail);
        void SendOnboardingRejectionEmail(string clientEmail, string reason);

        //void SendClientOnboardingStatusEmail(string toEmail, string subject, string body);

        void SendSalaryDisbursementApprovalEmail(string clientEmail, EmployeeDTO employee, double salaryAmount, string month);

        void SendBatchSalaryDisbursementApprovalEmail(string clientEmail, List<EmployeeDTO> employeeSalaries, string month);

        //void SendSalaryDisbursementRejectionEmail(string clientEmail, EmployeeDTO employee, double salaryAmount, string month);
    }
}
