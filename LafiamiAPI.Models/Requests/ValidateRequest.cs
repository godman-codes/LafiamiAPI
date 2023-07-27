using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class ValidateRequest : EmailRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
