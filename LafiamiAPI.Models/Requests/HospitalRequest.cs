using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class NewHospitalRequest
    {
        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address Is Required")]
        public string Address { get; set; }
        public int? CityId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="State Is Required")]
        public int StateId { get; set; }
    }

    public class ExistingHospitalRequest: NewHospitalRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Hospital Id Is Required")]
        public int Id { get; set; }
    }

    public class HospitalInsurancePlansRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Hospital Id Is Required")]
        public int HospitalId { get; set; }
        [Required(ErrorMessage = "Insurance Plan Id Is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Insurance Plan Id Is Required")]
        public long InsurancePlanId { get; set; }
    }
}
