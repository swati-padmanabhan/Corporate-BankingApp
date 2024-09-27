namespace CorporateBankingApp.Models
{
    public class Admin : User
    {
        public virtual string BankName { get; set; }
    }
}

/*The Admin class inherits from the User class, representing an administrative user 
 * in the Corporate Banking application. In addition to the properties defined in 
 * the User class, it includes a BankName property, which specifies the name of 
 * the bank the admin is associated with. This structure allows for shared user 
 * functionality while adding specific attributes for admin users. */