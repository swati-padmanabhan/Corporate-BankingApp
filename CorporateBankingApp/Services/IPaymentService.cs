using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Services
{
    public interface IPaymentService
    {
        void CreatePayment(Guid clientId, Beneficiary beneficiary, double amount, string razorpayPaymentId);

        void UpdatePaymentStatus(string razorpayOrderId, CompanyStatus status);

        void ApprovePayment(Payment payment);
    }
}
