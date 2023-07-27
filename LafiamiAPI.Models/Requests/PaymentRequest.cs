using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{

    public class ConfirmManualPaymentRequest
    {
        [Required]
        public Guid Id { get; set; }
        public bool PaymentStatus { get; set; }
    }



    public class PaymentGatewayInfoRequest
    {
        public Guid PaymentId { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public PaymentGatewayEnum Gateway { get; set; }
    }
}
