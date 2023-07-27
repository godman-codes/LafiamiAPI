using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsurancePriceModel : EntityBase<Guid>
    {
        public InsurancePriceModel() : base()
        {

        }
        public long InsurancePlanId { get; set; }
        public bool EnableDiscount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        public int CartBulkCount { get; set; }
        public int BulkCount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CartBulkAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BulkAmount { get; set; }
        public bool EnableBulkAmount { get; set; }
        public bool EnableCartBulkAmount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }

        public virtual InsurancePlanModel InsurancePlan { get; set; }
    }
}
