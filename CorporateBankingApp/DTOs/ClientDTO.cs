using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CorporateBankingApp.DTOs
{
    public class ClientDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string ContactInformation { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public HttpPostedFileBase Document1 { get; set; }
        [Required]
        public HttpPostedFileBase Document2 { get; set; }

    }

}
