using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceCompanyModel : EntityBase<int>
    {
        public InsuranceCompanyModel() : base()
        {
            CompanyExtras = new HashSet<CompanyExtraModel>();
        }

        public CompanyEnum Company { get; set; }
        public int OrderBy { get; set; }
        public string Logo { get; set; }

        public bool HasExtraInformation { get; set; }
        public bool RequiredIdenitication { get; set; }
        public bool RequiredNextOfKin { get; set; }
        public bool RequiredJobInformation { get; set; }
        public bool RequiredBankInformation { get; set; }
        public bool UseSystemHospitals { get; set; }

        public virtual ICollection<CompanyExtraModel> CompanyExtras { get; set; }

    }
}
