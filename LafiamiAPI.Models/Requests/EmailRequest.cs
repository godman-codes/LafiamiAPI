using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class ContactUsRequest
    {
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Message { get; set; }
        public string CompanyName { get; set; }
    }
}
