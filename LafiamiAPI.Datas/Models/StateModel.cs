using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class StateModel : EntityBase<int>
    {
        public StateModel() : base()
        {
            Users = new HashSet<UserViewModel>();
            Cities = new HashSet<CityModel>();
            Orders = new HashSet<OrderModel>();
            Hospitals = new HashSet<HospitalModel>();
        }

        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool Enable { get; set; }

        public virtual CountryModel Country { get; set; }
        public virtual ICollection<UserViewModel> Users { get; set; }
        public virtual ICollection<CityModel> Cities { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<HospitalModel> Hospitals { get; set; }
    }
}
