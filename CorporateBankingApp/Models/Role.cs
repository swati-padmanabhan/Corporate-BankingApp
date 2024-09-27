using System;

namespace CorporateBankingApp.Models
{
    public class Role
    {
        public virtual Guid Id { get; set; }

        public virtual string RoleName { get; set; }

        public virtual User User { get; set; }
    }
}
/* The Role class represents a user role in the Corporate Banking application. 
 * It includes properties for the role's unique identifier (Id), the name of the 
 * role (RoleName), and a reference to the associated User (User) assigned to that 
 * role. This model helps define and manage user roles within the system, 
 * allowing for role-based access control and permissions. */