using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
