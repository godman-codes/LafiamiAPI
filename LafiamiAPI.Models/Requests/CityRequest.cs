using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewCityRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "State is required")]
        public int StateId { get; set; }
        public bool Enable { get; set; }
    }

    public class ExistingCityRequest : NewCityRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "State Id is required")]
        public int Id { get; set; }
    }
}
