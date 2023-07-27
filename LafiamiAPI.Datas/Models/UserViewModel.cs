using LafiamiAPI.Utilities.Attributes;
using LafiamiAPI.Utilities.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class UserViewModel : IdentityUser
    {
        public UserViewModel() : base()
        {
            Orders = new HashSet<OrderModel>();
            Carts = new HashSet<CartModel>();
            UserRoles = new HashSet<ApplicationUserRoleModel>();
            InsurancePlans = new HashSet<InsurancePlanModel>();
            Ratings = new HashSet<RatingModel>();
            RatedUserRatings = new HashSet<RatingModel>();
            TotalRatings = new HashSet<TotalRatingModel>();
            InsuranceAudits = new HashSet<InsuranceAuditModel>();
            Banks = new HashSet<BankInformationModel>();
            Jobs = new HashSet<JobModel>();
            NextOfKins = new HashSet<NextOfKinModel>();
            Identifications = new HashSet<IdentificationModel>();
            UserCompanyExtraResults = new HashSet<UserCompanyExtraResultModel>();
            UserSessionVisits = new HashSet<UserSessionVisitModel>();

            IsActive = true;
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
        }

        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public SexEnums? Sex { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public string DeviceId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string TempPassword { get; set; }



        [UniqueKey(groupId: "1", order: 0)]


        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public virtual WalletModel Wallet { get; set; }

        public virtual EmailModel EmailModel { get; set; }
        public virtual CountryModel Country { get; set; }
        public virtual CityModel City { get; set; }
        public virtual StateModel State { get; set; }
        public virtual ICollection<ApplicationUserRoleModel> UserRoles { get; set; }


        public virtual ICollection<IdentificationModel> Identifications { get; set; }
        public virtual ICollection<NextOfKinModel> NextOfKins { get; set; }
        public virtual ICollection<JobModel> Jobs { get; set; }
        public virtual ICollection<BankInformationModel> Banks { get; set; }
        public virtual ICollection<InsurancePlanModel> InsurancePlans { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<CartModel> Carts { get; set; }
        public virtual ICollection<RatingModel> Ratings { get; set; }
        public virtual ICollection<RatingModel> RatedUserRatings { get; set; }
        public virtual ICollection<TotalRatingModel> TotalRatings { get; set; }
        public virtual ICollection<InsuranceAuditModel> InsuranceAudits { get; set; }
        public virtual ICollection<UserCompanyExtraResultModel> UserCompanyExtraResults { get; set; }
        public virtual ICollection<UserSessionVisitModel> UserSessionVisits { get; set; }
    }
}
