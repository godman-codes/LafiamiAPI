using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewCartRequest
    {
        public long InsurancePlanId { get; set; }
        public string ReferralCode { get; set; }
        [Range(1, int.MaxValue, ErrorMessage ="Quantity is out of range")]
        public int QuatityOrder { get; set; } = 1;

    }

    public class UpdateCartRequest
    {
        public Guid Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity is out of range")]
        public int QuatityOrder { get; set; }
    }
}
