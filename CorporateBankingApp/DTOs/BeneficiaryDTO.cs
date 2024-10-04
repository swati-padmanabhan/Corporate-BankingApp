using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class BeneficiaryDTO
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "Beneficiary Name is required.")]
        [StringLength(100, ErrorMessage = "Beneficiary Name cannot exceed 100 characters.")]
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Account Number must be exactly 12 digits.")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Bank IFSC is required.")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid Bank IFSC format.")]
        [Display(Name = "Bank IFSC")]
        public string BankIFSC { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Type")]
        public string BeneficiaryType { get; set; }

        [Display(Name = "Beneficiary Status")]
        public string BeneficiaryStatus { get; set; }

        [Required(ErrorMessage = "Beneficiary Address Proof is required.")]
        [Display(Name = "Beneficiary Address Proof")]
        public HttpPostedFileBase BeneficiaryAddressProof { get; set; }

        [Required(ErrorMessage = "Beneficiary ID Proof is required.")]
        [Display(Name = "Beneficiary Id Proof")]
        public HttpPostedFileBase BeneficiaryIdProof { get; set; }

        public List<string> DocumentPaths { get; set; }
    }
}
