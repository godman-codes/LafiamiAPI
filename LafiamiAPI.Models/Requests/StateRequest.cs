using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewStateRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Country is required")]
        public int CountryId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Enable { get; set; }
    }

    public class ExistingStateRequest : NewStateRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "State Id is required")]
        public int Id { get; set; }
    }
}
