using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class IdentificationModel : EntityBase<Guid>
    {
        public IdentificationModel() : base()
        {

        }

        public string IdUrl { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
