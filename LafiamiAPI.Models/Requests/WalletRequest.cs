namespace LafiamiAPI.Models.Requests
{
    public class TopUpRequest : EmailRequest
    {
        public decimal Amount { get; set; }
    }

    public class MyTopUpRequest
    {
        public decimal Amount { get; set; }
    }
}
