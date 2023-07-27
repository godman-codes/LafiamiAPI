using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
//    public class PartnerPaymentResultResponse
//    {
//        public string fullName { get; set; }
//        public string agentName { get; set; }
//        public string clientAddress { get; set; }
//        public string clientEmail { get; set; }
//        public string clientEmail { get; set; }
//        public string clientEmail { get; set; }
//        public string clientEmail { get; set; }


//        "fullName": "Williams Ojo",
//        "agentName": null,
//        "policies": [
//            "271000122001"
//        ],
//        "clientAddress": "VICTORIA ISLAND",
//        "clientEmail": "wojo@aiicoplc.com",
//        "clientPhoneNumber": "08168483478",
//        "wef": "15-Nov-2020",
//        "wet": "14-Nov-2021",
//        "totalAmount": "₦1,075.00",
//        "printPolicyUrl": "https://portal.aiicoplc.com/api/services/app/HospitalCashProductService/GetGadgetCertificate?tr=HCA-781D89",
//        "printReceiptUrl": null,
//        "hash": "a0f37139388f4109a8e3bf301f5c2cf82557c2798008f07efb1b3fe1be24a64a246675962d98e4920df2c2ce995d07bbf4b8eef4e5b1a6a2270210c41111f5e1",
//        "responseMessage": null,
//        "responseCode": null,
//        "polledToTQ": false,
//        "isLoan": false
//    }

    public class FinalizeAiicoPartnerPaymentResponse
    {
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
        //public PartnerPaymentResultResponse result { get; set; }       
    }
}
