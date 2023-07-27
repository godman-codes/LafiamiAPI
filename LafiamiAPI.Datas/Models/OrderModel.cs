using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class OrderModel : EntityBase<Guid>
    {
        public OrderModel() : base()
        {
            OrderNotes = new HashSet<OrderNoteModel>();
            Payments = new HashSet<PaymentModel>();
            UserCompanyExtraResults = new HashSet<UserCompanyExtraResultModel>();
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }

        [MaxLength(100)]
        public string OrderId { get; set; }
               
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Vat { get; set; }

        public OrderStatusEnum OrderStatus { get; set; }

        public DateTime DueDate { get; set; }

        public virtual EmailModel Email { get; set; }

        public string ReasonForRejecting { get; set; }
        public string ReasonForCanceling { get; set; }


        public bool ForSomeoneElse { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public SexEnums? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }




        public CompanyEnum? Company { get; set; }
        public bool RunBackgroundService { get; set; }
        public IntegrationStatusEnum IntegrationStatus { get; set; }
        public string IntegrationErrorMessage { get; set; }


        public string IntegrationBackgroundJsonResponse { get; set; }
        public decimal AiicoPremiumAmount { get; set; }
        public string AiicoTransactionRef { get; set; }
        public string HygeiaMemberId { get; set; }
        public string HygeiaLegacyCode { get; set; }
        public string HygeiaDependantId { get; set; }

        public int? HospitalId { get; set; }
        public virtual HospitalModel Hospital { get; set; }
        public virtual CartModel Cart { get; set; }


        public virtual IdentificationModel Identification { get; set; }
        public virtual NextOfKinModel NextOfKin { get; set; }
        public virtual JobModel Job { get; set; }
        public virtual BankInformationModel BankInformation { get; set; }


        public virtual UserViewModel User { get; set; }
        public virtual ICollection<OrderNoteModel> OrderNotes { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
        public virtual ICollection<UserCompanyExtraResultModel> UserCompanyExtraResults { get; set; }
    }
}
