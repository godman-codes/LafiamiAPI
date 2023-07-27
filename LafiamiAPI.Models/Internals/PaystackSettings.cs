namespace LafiamiAPI.Models.Internals
{
    public class PaystackSettings
    {
        public bool GoLive { get; set; }
        public string PayStackSecretKey { get; set; }
        public string PayStackPublicKey { get; set; }
        public string TestPayStackSecretKey { get; set; }
        public string TestPayStackPublicKey { get; set; }
    }
}
