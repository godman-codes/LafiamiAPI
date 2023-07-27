using LafiamiAPI.Utilities.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsurancePlanModel : EntityBase<long>
    {
        public InsurancePlanModel() : base()
        {
            InsuranceCategories = new HashSet<InsuranceCategory>();
            InsurancePlans = new HashSet<InsurancePlanModel>();
            Carts = new HashSet<CartModel>();
            RejectionMessages = new HashSet<InsuranceRejectionMessageModel>();
            Ratings = new HashSet<RatingModel>();
            TotalRatings = new HashSet<TotalRatingModel>();
            InsurancePrices = new HashSet<InsurancePriceModel>();
            InsurancePlanAnswerAsTags = new HashSet<InsurancePlanAnswerAsTagModel>();
            Reviews = new HashSet<ReviewModel>();
            PlanCompanyExtraResults = new HashSet<PlanCompanyExtraResultModel>();
            HospitalInsurancePlans = new HashSet<HospitalInsurancePlanModel>();
            PlanVisits = new HashSet<PlanVisitModel>();
            Visits = new HashSet<VisitModel>();
            CompareLogs = new HashSet<CompareLogModel>();
        }

        public string Name { get; set; }
        [MaxLength(200)]
        public string SearchName { get; set; }
        public string Summary { get; set; }
        [Column(TypeName = "ntext")]
        public string PicturesJson { get; set; }
        public bool Featured { get; set; }
        public PlanTypeEnum PlanType { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        public string SEO { get; set; }
        public string Thumbnail { get; set; }
        public int ViewCounter { get; set; }
        public int MinMonthsRequired { get; set; }
        public int FrequencyPerMonthsAllowed { get; set; }
        public string HospitalPDFs { get; set; }
        public int OrderBy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SortingAmount { get; set; }
        public PublishStatusEnum PublishStatus { get; set; }
        public string PartnerPlanId { get; set; }
        public CompanyEnum Company { get; set; }


        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }

        public virtual ICollection<ReviewModel> Reviews { get; set; }
        public virtual ICollection<InsurancePlanAnswerAsTagModel> InsurancePlanAnswerAsTags { get; set; }
        public virtual ICollection<InsuranceCategory> InsuranceCategories { get; set; }
        public virtual ICollection<InsurancePlanModel> InsurancePlans { get; set; }
        public virtual ICollection<InsurancePriceModel> InsurancePrices { get; set; }
        public virtual ICollection<CartModel> Carts { get; set; }
        public virtual ICollection<InsuranceRejectionMessageModel> RejectionMessages { get; set; }
        public virtual ICollection<RatingModel> Ratings { get; set; }
        public virtual ICollection<TotalRatingModel> TotalRatings { get; set; }
        public virtual ICollection<PlanCompanyExtraResultModel> PlanCompanyExtraResults { get; set; }
        public virtual ICollection<HospitalInsurancePlanModel> HospitalInsurancePlans { get; set; }
        public virtual ICollection<PlanVisitModel> PlanVisits { get; set; }
        public virtual ICollection<VisitModel> Visits { get; set; }
        public virtual ICollection<CompareLogModel> CompareLogs { get; set; }
    }
}
