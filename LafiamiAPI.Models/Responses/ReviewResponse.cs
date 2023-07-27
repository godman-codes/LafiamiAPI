using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime PostedDate { get; set; }
    }

    public class FullReviewResponse: ReviewResponse
    {
        public string InsurancePlanName { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusValue { get; set; }
    }
}
