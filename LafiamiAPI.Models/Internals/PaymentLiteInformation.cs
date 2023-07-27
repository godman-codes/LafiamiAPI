using LafiamiAPI.Utilities.Enums;
using System;

namespace LafiamiAPI.Models.Internals
{
    public class PaymentLiteInformation
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public PaymentGatewayEnum? PaymentGateway { get; set; }
        public string GatewayTransactionId { get; set; }
    }
}
