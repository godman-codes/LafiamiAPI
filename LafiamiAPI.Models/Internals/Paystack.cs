namespace LafiamiAPI.Models.Internals
{
    public class Paystack
    {
        public static string PayStackPaymentUrlContent { get { return "https://api.paystack.co/transaction/initialize"; } }
        public static string PayStackStatusUrlContent { get { return "https://api.paystack.co/transaction/verify/"; } }
        public static decimal Localtransactionchargepercent { get { return 1.5M; } }
        public static decimal InternationaltransactionChargePercent { get { return 3.9M; } }
        public static int Chargesforabove2500 { get { return 100; } }

        public static string GetSecretKey(PaystackSettings paystackSettings)
        {
            return (paystackSettings.GoLive ? paystackSettings.PayStackSecretKey : paystackSettings.TestPayStackSecretKey);
        }

        public static string GetPublicKey(PaystackSettings paystackSettings)
        {
            return (paystackSettings.GoLive ? paystackSettings.PayStackPublicKey : paystackSettings.TestPayStackPublicKey);
        }

        public decimal CalculateTransactionCharges(decimal totalamount, bool isLocal)
        {
            decimal result = ((isLocal) ? (decimal.Divide(decimal.Multiply(Localtransactionchargepercent, totalamount), 100)) : (decimal.Divide(decimal.Multiply(InternationaltransactionChargePercent, totalamount), 100)));

            if (totalamount > Chargesforabove2500)
            {
                result += Chargesforabove2500;
            }

            return result;
        }
    }
}
