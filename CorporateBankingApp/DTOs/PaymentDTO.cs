using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class PaymentDTO
    {
        public Guid PaymentId { get; set; }

        public string ClientName { get; set; }

        public string Username { get; set; }

        public string CompanyName { get; set; } 

        public string AccountNumber { get; set; }

        public Beneficiary Beneficiary { get; set; }

        public BeneficiaryType BeneficiaryType { get; set; }

        public string BeneficiaryName { get; set; }

        public double Amount { get; set; }

        public CompanyStatus PaymentStatus { get; set; }

        public virtual DateTime PaymentRequestDate { get; set; }

        public virtual string RazorpayPaymentId { get; set; }
    }
}