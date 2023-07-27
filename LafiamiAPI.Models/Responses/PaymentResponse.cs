using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{

    public class MyLitePaymentResponse
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public bool HasPaid { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentCompletedDate { get; set; }
        public PaymentGatewayEnum? PaymentGateway { get; set; }
        public string ItemNames { get; set; }
        public string OrderId { get; set; }
    }

    public class LitePaymentResponse : MyLitePaymentResponse
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool CanManuallyConfirm { get; set; }
    }

    public class LiteAiicoPaymentResponse : LitePaymentResponse
    {
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationStatusName { get; set; }
        public string IntegrationErrorMessage { get; set; }
        public string FinalizePartnerJsonResponse { get; set; }

        public bool IsIntegrationPending { get; set; }
        public bool IsIntegrationCompleted { get; set; }
        public bool IsIntegrationFailed { get; set; }
    }


    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public string GatewayTransactionId { get; set; }
        public bool HasPaid { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsAwaitingVerification { get; set; }
        public bool IsOnlinePayment { get; set; }
        public string PaymentStatus { get; set; }
        public string Code { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentGateway { get; set; }
        public DateTime? PaymentCompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<OrderItemsResponse> PaymentItems { get; set; }
    }


    public class PaymentTypeResponse
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class PaymentStatusResponse
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public class PaymentGatewayResponse
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }
}
