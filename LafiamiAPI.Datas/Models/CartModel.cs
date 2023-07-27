using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class CartModel : EntityBase<Guid>
    {
        public CartModel() : base()
        {
        }

        public long? InsurancePlanId { get; set; }
        public string ItemName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string UserId { get; set; }
        public string UnknownUserCode { get; set; }
        public int QuatityOrder { get; set; }
        public Guid? OrderId { get; set; }


        public virtual OrderModel Order { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
        public virtual UserViewModel User { get; set; }


    }
}
