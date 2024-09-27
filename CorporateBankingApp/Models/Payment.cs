using System;
using CorporateBankingApp.Enums;

namespace CorporateBankingApp.Models
{
    public class Payment
    {
        public virtual Guid Id { get; set; }

        //public virtual Client Client { get; set; }

        public virtual Beneficiary Beneficiary { get; set; }

        public virtual double Amount { get; set; }

        public virtual Status PaymentStatus { get; set; }

        public virtual DateTime PaymentRequestDate { get; set; }

        public virtual DateTime PaymentApprovalDate { get; set; }
    }
}
/* The Payment class represents a payment transaction in the Corporate Banking 
 * application. It includes properties for the unique identifier (Id), the 
 * associated Client (Client) making the payment, and the Beneficiary (Beneficiary) 
 * receiving the payment. Additional properties include the payment amount 
 * (Amount), the status of the payment (PaymentStatus), the date the payment 
 * request was made (PaymentRequestDate), and the date the payment was approved 
 * (PaymentApprovalDate). This model facilitates the tracking and management 
 * of payment transactions within the banking system. */