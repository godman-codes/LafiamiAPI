using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class EmailModel : EntityBase<Guid>
    {
        public EmailModel() : base()
        {

        }

        [Column(TypeName = "ntext")]
        public string Message { get; set; }
        public EmailTypeEnums? EmailType { get; set; }
        public MessageStatusEnums Status { get; set; }
        public DateTime? Sentdate { get; set; }
        public DateTime? FailedDate { get; set; }
        [Column(TypeName = "ntext")]
        public string Emailaddresses { get; set; }

        public string Subject { get; set; }
        [Column(TypeName = "ntext")]
        public string ResponseMessage { get; set; }


        public Guid? IntegrationOrderId { get; set; }
        public Guid? HygeiaCompletionOrderId { get; set; }

        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
        public Guid? PaymentId { get; set; }
        public virtual PaymentModel Payment { get; set; }


        public Guid? WalletTransactionId { get; set; }
        public virtual WalletTransactionModel WalletTransaction { get; set; }


        public string ChangeOrResetUserId { get; set; }

        public string NewUserId { get; set; }
        public virtual UserViewModel NewUser { get; set; }

    }
}
