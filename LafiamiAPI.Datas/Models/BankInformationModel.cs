using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class BankInformationModel : EntityBase<Guid>
    {
        public BankInformationModel() : base()
        {

        }

        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel  User { get; set; }
        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
