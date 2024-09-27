using System;

namespace CorporateBankingApp.Models
{
    public class Document
    {
        public virtual Guid Id { get; set; }

        public virtual string DocumentName { get; set; }

        //public virtual string DocumentType { get; set; } //beneficiary, payment, transaction

        //public virtual string FilePath { get; set; }

        public virtual string DocumentLink { get; set; }

        public virtual DateTime UploadDate { get; set; }

        public virtual Client Client { get; set; }
    }
}
/* The Document class represents a document associated with a client in the 
 * Corporate Banking application. It includes properties for the document's 
 * unique identifier (Id), type of document (DocumentType), file path where 
 * the document is stored (FilePath), and the date the document was uploaded 
 * (UploadDate). Additionally, it contains a reference to the associated 
 * Client (Client), allowing for the organization and management of 
 * client-related documents, which can include types such as beneficiary, 
 * payment, or transaction records. */


