using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class NextOfKinModel : EntityBase<Guid>
    {
        public NextOfKinModel() : base()
        {

        }

        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Address { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
