using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class FinalizeAiicoPartnerPaymentRequest
    {
        public string partnerRef { get; set; }
        public string accountNumber { get; set; }
        public decimal amountPaid { get; set; }
        public string paymentRef { get; set; }
        public string transactionRef { get; set; }
    }
}
