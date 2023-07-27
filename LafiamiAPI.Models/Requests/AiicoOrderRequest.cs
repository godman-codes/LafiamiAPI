using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class AiicoOrderRequest
    {
        public string TitleId { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string ProductId { get; set; }
        public string CoverTypeId { get; set; }
        public decimal SumAssured { get; set; }
        public string DurationCovered { get; set; }
        public string Hospital { get; set; }
        public string LGAId { get; set; }
    }
}
