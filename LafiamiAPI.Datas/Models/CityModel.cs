using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class CityModel : EntityBase<int>
    {
        public CityModel() : base()
        {
            Users = new HashSet<UserViewModel>();
            Orders = new HashSet<OrderModel>();
            Hospitals = new HashSet<HospitalModel>();
        }

        public string Name { get; set; }
        public int StateId { get; set; }
        public bool Enable { get; set; }

        public virtual StateModel State { get; set; }
        public virtual ICollection<UserViewModel> Users { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<HospitalModel> Hospitals { get; set; }
    }
}
