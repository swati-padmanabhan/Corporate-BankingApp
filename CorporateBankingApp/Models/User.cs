using System;

namespace CorporateBankingApp.Models
{
    public class User
    {
        public virtual Guid Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual Role Role { get; set; } = new Role();
    }
}
/* The User class represents a user in the Corporate Banking application. 
 * It includes properties for the user's unique identifier (Id), username (UserName), 
 * password (Password), email address (Email), and role (Role) within the application. 
 * Each property is defined as virtual to support potential lazy loading */