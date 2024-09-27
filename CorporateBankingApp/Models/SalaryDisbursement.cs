using System;
using System.IdentityModel.Protocols.WSTrust;

namespace CorporateBankingApp.Models
{
    public class SalaryDisbursement
    {
        public virtual Guid Id { get; set; }

        //public virtual Client Client { get; set; }

        public virtual double Salary { get; set; }

        public virtual DateTime DisbursementDate { get; set; }

        public virtual bool IsBatch { get; set; }

        public virtual Status SalaryStatus { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
/* The SalaryDisbursement class represents the process of disbursing salaries 
 * to employees within the Corporate Banking application. It includes properties 
 * for the unique identifier (Id), the associated Client (Client) whose employee 
 * is receiving the salary, and the specific Employee (Employee) being paid. 
 * Additional properties include the salary amount (Salary), the date of 
 * disbursement (DisbursementDate), a boolean indicating whether the disbursement 
 * was part of a batch process (IsBatch), and the status of the salary disbursement 
 * (SalaryStatus). This model facilitates the management and tracking of salary 
 * payments within the banking system. */