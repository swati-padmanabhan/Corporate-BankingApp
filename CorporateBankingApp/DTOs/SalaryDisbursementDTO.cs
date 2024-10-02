using CorporateBankingApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class SalaryDisbursementDTO
    {
        public Guid SalaryDisbursementId { get; set; }

        public string CompanyName { get; set; }

        public EmployeeDTO Employee { get; set; }

        public string EmployeeFirstName { get; set; }

        public string EmployeeLastName { get; set; }

        public double Salary {  get; set; }

        public DateTime DisbursementDate { get; set; }

        public CompanyStatus SalaryStatus { get; set; }
    }
}