using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class VisitModel : EntityBase<Guid>
    {
        public VisitModel() : base()
        {
            CompareLogs = new HashSet<CompareLogModel>();
        }

        public PageTypeEnum PageType { get; set; }
        public long? InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
        public string IPAddress { get; set; }

        public Guid UserSessionVisitId { get; set; }
        public virtual UserSessionVisitModel UserSessionVisit { get; set; }

        public Guid? PlanVisitId { get; set; }
        public virtual PlanVisitModel PlanVisit { get; set; }
        public virtual ICollection<CompareLogModel> CompareLogs { get; set; }
    }
}
