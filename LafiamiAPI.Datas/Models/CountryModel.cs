using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class CountryModel : EntityBase<int>
    {
        public CountryModel() : base()
        {
            Users = new HashSet<UserViewModel>();
            Orders = new HashSet<OrderModel>();
            States = new HashSet<StateModel>();
        }

        public bool Enable { get; set; }
        public string Name { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string ThreeLetterIsoCode { get; set; }

        public virtual ICollection<UserViewModel> Users { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<StateModel> States { get; set; }
    }
}
