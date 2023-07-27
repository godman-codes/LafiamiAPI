using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class NewVisitRequest
    {
        [Required]
        public PageTypeEnum PageType { get; set; }
        public long? InsurancePlanId { get; set; }
        public string IPAddress { get; set; }
        [Required]
        public Guid SystemUserId { get; set; }
        [Required]
        public Guid SessionId { get; set; }
    }

    public class NewPlanForComparisonRequest
    {
        [Required]
        public long InsurancePlanId { get; set; }
        public bool IsUseInComparison { get; set; }
        [Required]
        public Guid VisitId { get; set; }
    }
}
