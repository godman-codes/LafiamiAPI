using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{
    public class FlutterwaveStatusResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public FlutterwaveStatusData data { get; set; }
    }

    public class FlutterwaveStatusData
    {
        //public int txid { get; set; }
        //public string txref { get; set; }
        //public string flwref { get; set; }
        //public string devicefingerprint { get; set; }
        // public string cycle { get; set; }
        public decimal amount { get; set; }
        // public string currency { get; set; }
        //public decimal chargedamount { get; set; }
        //public decimal appfee { get; set; }
        //public decimal merchantfee { get; set; }
        //public decimal merchantbearsfee { get; set; }
        public string chargecode { get; set; }
        //public string chargemessage { get; set; }
        //public string authmodel { get; set; }
        //public string ip { get; set; }
        //public string narration { get; set; }
        public string status { get; set; }
        //public string vbvcode { get; set; }
        //public string vbvmessage { get; set; }
        //public string authurl { get; set; }
        //public string acctcode { get; set; }
        //public string acctmessage { get; set; }
        //public string paymenttype { get; set; }
        //public string paymentid { get; set; }
        //public string fraudstatus { get; set; }
        //public string chargetype { get; set; }
        //public int createdday { get; set; }
        //public string createddayname { get; set; }
        //public int createdweek { get; set; }
        //public int createdmonth { get; set; }
        //public string createdmonthname { get; set; }
        //public int createdquarter { get; set; }
        //public int createdyear { get; set; }
        //public bool createdyearisleap { get; set; }
        //public int createddayispublicholiday { get; set; }
        //public int createdhour { get; set; }
        //public int createdminute { get; set; }
        //public string createdpmam { get; set; }
        //public string created { get; set; }
        //public string customerid { get; set; }
        //public string custphone { get; set; }
        //public string custnetworkprovider { get; set; }
        //public string custname { get; set; }
        //public string custemailprovider { get; set; }
        //public string custcreated { get; set; }
        //public int accountid { get; set; }
        //public string acctbusinessname { get; set; }
        //public string acctcontactperson { get; set; }
        //public string acctcountry { get; set; }
        //public int acctbearsfeeattransactiontime { get; set; }
        //public int acctparent { get; set; }
        //public string acctvpcmerchant { get; set; }
        //public string acctalias { get; set; }
        //public int acctisliveapproved { get; set; }
        //public string orderref { get; set; }
        //public string paymentplan { get; set; }
        //public string paymentpage { get; set; }
        //public string raveref { get; set; }
        //public int amountsettledforthistransaction { get; set; }
        //public FlutterwaveStatusDataCard card { get; set; }
        //public FlutterwaveStatusDataMeta meta { get; set; }        
    }

    public class FlutterwaveStatusDataCard
    {
        public string expirymonth { get; set; }
        public string expiryyear { get; set; }
        public string cardBIN { get; set; }
        public string last4digits { get; set; }
        public string brand { get; set; }
        public List<FlutterwaveStatusDataCardToken> card_tokens { get; set; }
        public string life_time_token { get; set; }

    }

    public class FlutterwaveStatusDataCardToken
    {
        public string embedtoken { get; set; }
        public string shortcode { get; set; }
        public string expiry { get; set; }
    }

    public class FlutterwaveStatusDataMeta
    {
        public int id { get; set; }
        public string metaname { get; set; }
        public string metavalue { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string deletedAt { get; set; }
        public int getpaidTransactionId { get; set; }
    }
}
