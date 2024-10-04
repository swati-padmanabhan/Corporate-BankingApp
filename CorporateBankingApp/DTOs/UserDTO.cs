using CorporateBankingApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class UserDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Username")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Please Provide Password")]
        public string Password { get; set; }
    }
}