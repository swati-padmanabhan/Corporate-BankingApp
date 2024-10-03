using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class ClientReportDTO
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public IEnumerable<EmployeeReportDTO> Employees { get; set; }
        public IEnumerable<BeneficiaryReportDTO> Beneficiaries { get; set; }
    }
}