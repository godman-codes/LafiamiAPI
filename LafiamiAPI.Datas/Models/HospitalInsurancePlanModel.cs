using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class HospitalInsurancePlanModel
    {
        public int HospitalId { get; set; }
        public long InsurancePlanId { get; set; }

        public virtual HospitalModel Hospital { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
    }
}
