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

        public string BeneficiaryType { get; set; }

        public string BeneficiaryStatus { get; set; }

        [Required]
        [Display(Name = "Beneficiary Id Proof")]
        public HttpPostedFileBase BeneficiaryAddressProof { get; set; }

        [Required]
        [Display(Name = "Beneficiary Address Proof")]
        public HttpPostedFileBase BeneficiaryIdProof { get; set; }

        public List<string> DocumentPaths { get; set; }


    }
}