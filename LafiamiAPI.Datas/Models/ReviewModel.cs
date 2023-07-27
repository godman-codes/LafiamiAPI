using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class ReviewModel : EntityBase<Guid>
    {
        public ReviewModel() : base()
        {
        }

        public string Name { get; set; }
        public string Comment { get; set; }
        public StatusEnum Status { get; set; }

        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
    }
}
