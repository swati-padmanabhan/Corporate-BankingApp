using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class BeneficiaryPaymentDTO
    {
        public double Amount { get; set; }

        public List<BeneficiaryDTO> Beneficiaries { get; set; }
    }
}