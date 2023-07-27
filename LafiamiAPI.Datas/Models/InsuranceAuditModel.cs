using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceAuditModel : EntityBase<Guid>
    {
        public InsuranceAuditModel() : base()
        {
            InsuranceAuditCategories = new HashSet<InsuranceAuditCategoryModel>();
            InsuranceAuditQuestionAnswers = new HashSet<InsuranceAuditQuestionAnswerModel>();
        }

        public string Keyword { get; set; }
        public bool HasResult { get; set; }
        public int ResultCount { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public virtual ICollection<InsuranceAuditCategoryModel> InsuranceAuditCategories { get; set; }
        public virtual ICollection<InsuranceAuditQuestionAnswerModel> InsuranceAuditQuestionAnswers { get; set; }

    }
}
