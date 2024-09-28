using System.Collections.Generic;
using CorporateBankingApp.Enums;

namespace CorporateBankingApp.Models
{
    public class Client : User
    {
        public virtual string CompanyName { get; set; }

        public virtual string Location { get; set; }

        public virtual string ContactInformation { get; set; }

        public virtual Status OnBoardingStatus { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string ClientIFSC {  get; set; }

        public virtual double Balance { get; set; }

        public virtual IList<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

        public virtual IList<Document> Documents { get; set; } = new List<Document>();

        public virtual IList<Employee> Employees { get; set; } = new List<Employee>();

        public virtual IList<Report> Reports { get; set; }


    }
}

/* The Client class inherits from the User class, representing a client in the 
 * Corporate Banking application. In addition to the properties inherited from 
 * User, it includes several specific attributes: 
 * - CompanyName: the name of the client's company
 * - Location: the physical location of the company
 * - ContactInformation: details for reaching the company
 * - Balance: the client's account balance
 * - Beneficiaries: a list of beneficiaries associated with the client
 * - Documents: a list of documents related to the client
 * - Employees: a list of employees linked to the client
 * - OnBoardingStatus: an enumeration representing the client's onboarding 
 * status. This class provides a comprehensive structure for managing 
 * client-specific information within the banking system. */