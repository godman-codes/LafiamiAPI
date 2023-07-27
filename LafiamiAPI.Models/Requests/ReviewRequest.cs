using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class NewReviewRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Insurance Plan Id is required")]
        public long InsurancePlanId { get; set; }
    }

}
