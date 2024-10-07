using CorporateBankingApp.Enums;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class ClientDTO
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        //[RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email Id")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Contact Information")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact Information must be exactly 10 digits.")]
        public string ContactInformation { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Account number must be exactly 12 digits.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "IFSC Code is required.")]
        [RegularExpression(@"^[A-Z]{4}[0][A-Z0-9]{6}$", ErrorMessage = "IFSC must be in the format 'ABCD0123456'.")]
        [StringLength(11, ErrorMessage = "IFSC must be 11 characters long.")]
        public string ClientIFSC { get; set; }

        [Required]
        public CompanyStatus OnboardingStatus { get; set; }

        public string BeneficiaryStatus { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive value.")]
        public double Balance { get; set; }

        public HttpPostedFileBase Document1 { get; set; }

        public HttpPostedFileBase Document2 { get; set; }

        [Required]
        public List<String> DocumentLocation { get; set; } = new List<String>();

        public List<DocumentDTO> Documents { get; set; }
    }

}
