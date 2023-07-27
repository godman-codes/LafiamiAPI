using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class FindAPlanQuestionAnswerModel : EntityBase<int>
    {
        public FindAPlanQuestionAnswerModel() : base()
        {
            InsurancePlanAnswerAsTags = new HashSet<InsurancePlanAnswerAsTagModel>();
            InsuranceAuditQuestionAnswers = new HashSet<InsuranceAuditQuestionAnswerModel>();
        }

        public string Answer { get; set; }
        public string Explanation { get; set; }
        public int OrderBy { get; set; }
        public int? DependentQuestionId { get; set; }

        public int FIndAPlanQuestionId { get; set; }
        public virtual FIndAPlanQuestionModel FIndAPlanQuestion { get; set; }
        public virtual ICollection<InsurancePlanAnswerAsTagModel> InsurancePlanAnswerAsTags { get; set; }
        public virtual ICollection<InsuranceAuditQuestionAnswerModel> InsuranceAuditQuestionAnswers { get; set; }
    }
}
