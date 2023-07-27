using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class CompareLogModel : EntityBase<Guid>
    {
        public CompareLogModel() : base()
        {
        }

        public bool IsUseInComparison { get; set; }
        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
        public Guid VisitId { get; set; }
        public virtual VisitModel Visit { get; set; }
    }
}
