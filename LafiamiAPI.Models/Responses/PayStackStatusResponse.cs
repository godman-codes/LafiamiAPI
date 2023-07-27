namespace LafiamiAPI.Models.Responses
{
    public class PayStackStatusResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PayStackStatusDataResponse data { get; set; }
    }

    public class PayStackStatusDataResponse
    {
        public long amount { get; set; }
        public string currency { get; set; }
        public string transaction_date { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public string domain { get; set; }
        //public string metadata { get; set; }
        public string gateway_response { get; set; }
        public string message { get; set; }
        public string channel { get; set; }
        public string ip_address { get; set; }
        public PayStackStatusDataAuthResponse authorization { get; set; }
    }

    public class PayStackStatusDataAuthResponse
    {
        public string authorization_code { get; set; }
        public string card_type { get; set; }
        public string last4 { get; set; }
        public string bank { get; set; }
    }
}
