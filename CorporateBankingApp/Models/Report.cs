using System;

namespace CorporateBankingApp.Models
{
    public class Report
    {
        public virtual Guid Id { get; set; }

        public virtual string ReportType { get; set; } // PaymentReport, SalaryReport, TransactionReport

        public virtual DateTime GeneratedDate { get; set; }

        public virtual string GeneratedBy { get; set; }

        public virtual Client Client { get; set; }
    }
}