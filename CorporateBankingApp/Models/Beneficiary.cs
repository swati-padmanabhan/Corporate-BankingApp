using System;
using System.Collections.Generic;
using CorporateBankingApp.Enums;

namespace CorporateBankingApp.Models
{
    public class Beneficiary
    {
        public virtual Guid Id { get; set; }

        public virtual string BeneficiaryName { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string BankIFSC { get; set; }

        //public virtual Payment Payment { get; set; }
        public virtual IList<Payment> Payments { get; set; } = new List<Payment>();


        public virtual BeneficiaryType BeneficiaryType { get; set; }

        public virtual Client Client { get; set; }
    }
}

/* The Beneficiary class represents a beneficiary associated with a client 
 * the Corporate Banking application. It includes properties for the beneficiary'
 * unique identifier (Id), name (BeneficiaryName), account number (AccountNumber
 * and the bank's IFSC code (BankIFSC). The class also contains references to 
 * the Payment (Payment) details and the type of beneficiary (BeneficiaryType) i.e Inbound or Outbond. 
 * Additionally, it links to the associated Client (Client), allowing for the 
 * management of beneficiary information related to specific clients in the banking system. */