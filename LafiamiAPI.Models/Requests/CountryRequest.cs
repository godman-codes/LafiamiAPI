using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewCountryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Only Two Characters are allowed for Two Letter Iso-Code ")]
        public string TwoLetterIsoCode { get; set; }
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Only Three Characters are allowed for Three Letter Iso-Code ")]
        public string ThreeLetterIsoCode { get; set; }
        public bool Enable { get; set; }
    }

    public class ExistingCountryRequest : NewCountryRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Country Id is required")]
        public int Id { get; set; }
    }
}
