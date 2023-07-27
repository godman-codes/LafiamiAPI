using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class VisitSummaryResponse
    {
        public decimal UserBounceRate { get; set; }
        public decimal UniqueUserBounceRate { get; set; }
        public int UserCount { get; set; }
        public int UniqueUserCount { get; set; }
        public int OrderBy { get; set; }
        public DateTime Date { get; set; }
        public string Week { get; set; }
        public string Month { get; set; }
        public string QuaterName { get; set; }
        public string Year { get; set; }
    }

    public class PlanSummaryResponse
    {
        public int OrderBy { get; set; }
        public List<PlanCountResponse> PlanCounts { get; set; }
        public DateTime Date { get; set; }
        public string Week { get; set; }
        public string Month { get; set; }
        public string QuaterName { get; set; }
        public string Year { get; set; }
    }

    public class PlanCountResponse
    {
        public int VisitCount { get; set; }
        public string InsurancePlanName { get; set; }
    }
}
