using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class BeneficiaryPaymentDTO
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "At least one beneficiary must be selected.")]
        public List<BeneficiaryDTO> Beneficiaries { get; set; }
    }
}