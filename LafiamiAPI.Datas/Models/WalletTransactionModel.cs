using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class WalletTransactionModel : EntityBase<Guid>
    {
        public WalletTransactionModel() : base()
        {
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        public TransactionStatusEnum TransactionStatus { get; set; }

        public DateTime? TransactionCompletedDate { get; set; }

        public string Reason { get; set; }

        public Guid WalletId { get; set; }

        public virtual WalletModel Wallet { get; set; }

        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }


        public virtual EmailModel Email { get; set; }
    }
}
