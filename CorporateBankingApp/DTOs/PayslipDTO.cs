using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class PayslipDTO
    {
        public string EmployeeName { get; set; }
        public double Salary { get; set; }
        public string Month { get; set; }
        public string CompanyName { get; set; }
    }
}