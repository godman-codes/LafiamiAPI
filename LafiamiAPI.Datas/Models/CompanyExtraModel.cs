using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class CompanyExtraModel : EntityBase<int>
    {
        public CompanyExtraModel() : base()
        {
            UserCompanyExtraResults = new HashSet<UserCompanyExtraResultModel>();
            PlanCompanyExtraResults = new HashSet<PlanCompanyExtraResultModel>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DropdownListGetUrl { get; set; }
        public bool HasDependent { get; set; }
        public bool LoadingToBeTrigger { get; set; }
        public bool ForProductSetup { get; set; }
        public string DependentName { get; set; }
        public AnswerTypeEnum AnswerType { get; set; }
        public int OrderBy { get; set; }
        public int InsuranceCompanyId { get; set; }
        public virtual InsuranceCompanyModel InsuranceCompany { get; set; }
        public virtual ICollection<UserCompanyExtraResultModel> UserCompanyExtraResults { get; set; }
        public virtual ICollection<PlanCompanyExtraResultModel> PlanCompanyExtraResults { get; set; }
    }
}
