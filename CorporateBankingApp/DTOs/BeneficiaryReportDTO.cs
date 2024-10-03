using CorporateBankingApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class BeneficiaryReportDTO
    {
        public Guid BeneficiaryId { get; set; }


        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string BankIFSC { get; set; }
        public CompanyStatus BeneficiaryStatus { get; set; }
        public BeneficiaryType BeneficiaryType { get; set; }


        // Payment Details
        public double? Amount { get; set; }
        public DateTime? PaymentRequestDate { get; set; }
        public DateTime? PaymentApprovalDate { get; set; }
        public CompanyStatus? PaymentStatus { get; set; }
    }
}