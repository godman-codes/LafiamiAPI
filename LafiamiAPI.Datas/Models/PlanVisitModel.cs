using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class  PlanVisitModel : EntityBase<Guid>
    {
        public PlanVisitModel() : base()
        {
        }

        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
        public Guid SessionId { get; set; }
        public int VisitCount { get; set; }
        public virtual ICollection<VisitModel> Visits { get; set; }
    }
}
