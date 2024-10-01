using System;
using System.Collections.Generic;

namespace CorporateBankingApp.Models
{
    public class Employee
    {
        public virtual Guid Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual long Phone { get; set; }

        public virtual string Designation { get; set; }

        public virtual double Salary { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual Client Client { get; set; }

        public virtual IList<SalaryDisbursement> SalaryDisbursements { get; set; } = new List<SalaryDisbursement>();

    }
}
/* The Employee class represents an employee of a client company in the Corporate 
 * Banking application. It includes properties for the employee's unique identifier 
 * (Id), first name (FirstName), last name (LastName), email address (Email), 
 * phone number (Phone), and salary (Salary). Additionally, it contains a reference 
 * to the associated Client (Client), which identifies the company the employee 
 * belongs to. This model helps manage information specific to company employees 
 * within the banking system. */