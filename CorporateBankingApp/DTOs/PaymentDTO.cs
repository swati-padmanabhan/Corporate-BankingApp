using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class PaymentDTO
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; } 

        public string AccountNumber { get; set; }

        public string BeneficiaryType { get; set; }

        public Beneficiary Beneficiary { get; set; }

        public double Amount { get; set; }

        public virtual DateTime PaymentRequestDate { get; set; }

        public virtual string RazorpayPaymentId { get; set; }
    }
}