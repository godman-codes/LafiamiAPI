using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class HospitalModel : EntityBase<int>
    {
        public HospitalModel() : base()
        {
            HospitalInsurancePlans = new HashSet<HospitalInsurancePlanModel>();
            Orders = new HashSet<OrderModel>();
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public virtual CityModel City { get; set; }
        public int StateId { get; set; }
        public virtual StateModel State { get; set; }

        public virtual ICollection<HospitalInsurancePlanModel> HospitalInsurancePlans { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
    }
}
