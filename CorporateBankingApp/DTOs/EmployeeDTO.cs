using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(20, ErrorMessage = "First Name cannot exceed 20 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(20, ErrorMessage = "Last Name cannot exceed 20 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "email is required")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(20, ErrorMessage = "Designation cannot exceed 20 characters")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Please enter 10 digit Phone No.")]
        public long Phone { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a non-negative number")]
        public double Salary { get; set; }


        public bool IsActive { get; set; }
    }

}