using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class HospitalScheduleResponse
    {
        public HospitalScheduleResultResponse result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }

    public class HospitalScheduleResultResponse
    {
        public string premium { get; set; }
        public string transactionRef { get; set; }
    }


}



//{
//    "result": {
//        "titleId": null,
//        "surName": "Adekoya",
//        "otherNames": "Samuel",
//        "phoneNumber": "08034451727",
//        "email": "samuel@igunleinnovations.com",
//        "dob": "1981-04-30T23:00:00",
//        "accountNumber": 7389202943,
//        "bankName": "Gtbank",
//        "occupation": "Software Dev",
//        "genderId": null,
//        "address": null,
//        "countryId": null,
//        "paymentFrequency": "Annually",
//        "durationOfCover": 1,
//        "unit": 0,
//        "effectiveDate": "2021-04-30T23:00:00Z",
//        "sumAssured": "0",
//        "premium": "430.0000",
//        "premiumRate": 0.0,
//        "clientShortDescription": null,
//        "agentShortDescription": null,
//        "marketer": null,
//        "hospital": "d25e80d8-3c58-3f0c-55a2-8425e545a233",
//        "maritalStatusId": "",
//        "lgaId": "578fa972-98e4-e711-83bb-44850015c0e4",
//        "identificationName": "Adekoya Samuel ",
//        "identificationUrl": "https://lafiamistore.blob.core.windows.net/lafiamicontainer/images/1614695367521_w1200.jpg",
//        "nextOfKinName": "Adekoya Samuel",
//        "nextOfKinPhone": "08034451727",
//        "nextOfKinRelationship": "Friend",
//        "nextOfKinAddress": "22 Surulere street, Cele Bus-stop, Alagbole, Ojodu berger, Lagos",
//        "transactionRef": "HCA-33E3D2",
//        "subclassSectCovtypeId": "7136facc-3ec0-ea11-a336-005056a00da2",
//        "referralCode": null,
//        "branchId": "15439e44-98cf-e711-83b7-44850015c0e7",
//        "productId": "9f304355-43c0-ea11-a336-005056a00da2",
//        "intermediaryId": null,
//        "userId": null,
//        "ravePublicKey": "FLWPUBK-c674c68d40a0cb428926869498f14171-X",
//        "interswitchMacKey": "E187B1191265B18338B5DEBAF9F38FEC37B170FF582D4666DAB1F098304D5EE7F3BE15540461FE92F1D40332FDBBA34579034EE2AC78B1A1B8D9A321974025C4",
//        "interswitchPaymentUrl": "https://sandbox.interswitchng.com/webpay/pay",
//        "interswitchProductId": "6204",
//        "interswitchProductItemId": "103",
//        "wemaBankMacKey": "7E410E2C6FAEB61C7A128026CB0160E4581F90194B72E609EDB46F8D4FFB1637",
//        "wemaBankPaymentUrl": "https://apps.wemabank.com/GateWay",
//        "clientId": "2034bf77-1aa8-eb11-a36e-005056a00da2",
//        "wef": "2021-04-30T23:00:00Z",
//        "wet": "2022-04-29T23:00:00Z",
//        "isSuccessful": true,
//        "isQuote": false
//    },
//    "targetUrl": null,
//    "success": true,
//    "error": null,
//    "unAuthorizedRequest": false,
//    "__abp": true
//}