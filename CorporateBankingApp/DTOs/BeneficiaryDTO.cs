using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class BeneficiaryDTO
    {
        public virtual Guid Id { get; set; }

        public string BeneficiaryName { get; set; }

        public string AccountNumber { get; set; }

        public string BankIFSC { get; set; }
        
        public bool IsActive { get; set; }

        public CompanyStatus BeneficiaryStatus { get; set; }

        [Required]
        public HttpPostedFileBase BeneficiaryDocument1 { get; set; }

        [Required]
        public HttpPostedFileBase BeneficiaryDocument2 { get; set; }


    }
}