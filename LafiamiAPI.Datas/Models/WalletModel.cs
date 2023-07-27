using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class WalletModel : EntityBase<Guid>
    {
        public WalletModel() : base()
        {
            WalletTransactions = new HashSet<WalletTransactionModel>();
        }

        public string UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BookBalance { get; set; }

        public virtual UserViewModel User { get; set; }
        public virtual ICollection<WalletTransactionModel> WalletTransactions { get; set; }
    }
}
