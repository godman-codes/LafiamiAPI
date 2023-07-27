using System;

namespace LafiamiAPI.Models.Responses
{
    public class MyWalletResponse
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public decimal BookBalance { get; set; }
    }

    public class MyWalletTransactionResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public bool IsPending { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime? TransactionCompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class WalletTransactionResponse : MyWalletTransactionResponse
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
