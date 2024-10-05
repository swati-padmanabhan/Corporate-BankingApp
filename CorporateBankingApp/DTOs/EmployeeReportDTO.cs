using CorporateBankingApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class EmployeeReportDTO
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public double Salary { get; set; }

        public DateTime? DisbursementDate { get; set; }
        public CompanyStatus? SalaryStatus { get; set; }

        public string SalaryStatusString => SalaryStatus.HasValue ? SalaryStatus.Value.ToString() : "No status";

    }
}