using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class PaymentModel : EntityBase<Guid>
    {
        public PaymentModel() : base()
        {
        }

        [MaxLength(100)]
        public string TransactionId { get; set; }
        public string GatewayTransactionId { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string Code { get; set; }
        public string Reason { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public PaymentTypeEnum? PaymentType { get; set; }
        public PaymentGatewayEnum? PaymentGateway { get; set; }
        public string PaymentInitizationJson { get; set; }
        public string PaymentJson { get; set; }
        public DateTime? PaymentCompletedDate { get; set; }

        public Guid? OrderId { get; set; }

        public virtual OrderModel Order { get; set; }


        public bool RunBackgroundService { get; set; }
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationErrorMessage { get; set; }
        public string PartnerPaymentJsonResponse { get; set; }


        public virtual EmailModel Email { get; set; }
    }
}
