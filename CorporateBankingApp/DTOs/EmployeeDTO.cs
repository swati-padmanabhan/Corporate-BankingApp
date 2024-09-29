using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public long Phone { get; set; }

        public string Designation { get; set; }

        public bool IsActive { get; set; }
    }

    public class CSVDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public long Phone { get; set; }

        public string Designation { get; set; }
    }
}