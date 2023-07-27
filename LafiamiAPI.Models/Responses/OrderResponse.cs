using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{
    public class OrderCountPerDayResponnse
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }

    public class OrderItemsResponse
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class MyLiteOrderResponse
    {
        public Guid Id { get; set; }
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatusValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
        public string ItemNames { get; set; }
    }

    public class LiteOrderResponse : MyLiteOrderResponse
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class LiteAiicoOrderResponse : LiteOrderResponse
    {
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationStatusName { get; set; }
        public string IntegrationErrorMessage { get; set; }
        public string HospitalCashScheduleJsonResponse { get; set; }
        public bool IsIntegrationPending { get; set; }
        public bool IsIntegrationCompleted { get; set; }
        public bool IsIntegrationFailed { get; set; }
    }

    public class LiteHygeiaOrderResponse : LiteOrderResponse
    {
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationStatusName { get; set; }
        public string IntegrationErrorMessage { get; set; }
        public bool IsIntegrationPending { get; set; }
        public bool IsIntegrationCompleted { get; set; }
        public bool IsIntegrationFailed { get; set; }
        public string HygeiaMemberId { get; set; }
        public string HygeiaLegacyCode { get; set; }
        public string HygeiaDependantId { get; set; }
        public string EnrolleeNumber { get; set; }
    }

    public class LiteRelianceHMOOrderResponse : LiteOrderResponse
    {
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationStatusName { get; set; }
        public string IntegrationErrorMessage { get; set; }

        public bool IsIntegrationPending { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public SexEnums? Sex { get; set; }
        public string HouseAddress { get; set; }
        public string StateOfResidence { get; set; }
        public string Hospital { get; set; }
        public string ProfilePicture { get; set; }
    }

    public class OrderResponse : LiteOrderResponse
    {
        public string Address { get; set; }
        public bool IsPending { get; set; }
        public bool IsFailed { get; set; }
        public bool IsAwaitingVerification { get; set; }
        public List<OrderItemsResponse> OrderItems { get; set; }

        public Guid PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public PaymentGatewayEnum? PaymentGatewayId { get; set; }
        public bool HasPaid { get; set; }

        public decimal Vat { get; set; }
        public DateTime DueDate { get; set; }

        public string AmountInWords { get; set; }

    }

    public class AiicoOrderResponse : OrderResponse
    {
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationStatusName { get; set; }
        public string IntegrationErrorMessage { get; set; }
        public string HospitalCashScheduleJsonResponse { get; set; }


    }
}
