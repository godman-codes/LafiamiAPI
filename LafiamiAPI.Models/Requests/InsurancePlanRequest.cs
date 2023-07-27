using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class InsurancePlanPriceRequest
    {
        public bool EnableDiscount { get; set; }
        public decimal Amount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; } = 0;
        public decimal DiscountAmount { get; set; }
        public int CartBulkCount { get; set; }
        public int BulkCount { get; set; }
        public decimal CartBulkAmount { get; set; }
        public decimal BulkAmount { get; set; }
        public bool EnableBulkAmount { get; set; }
        public bool EnableCartBulkAmount { get; set; }
    }

    public class InsurancePlanSeachRequest
    {
        public string Keyword { get; set; }
        public SortingTypeEnum SortingType { get; set; } = SortingTypeEnum.Latest;
        public List<int> Categories { get; set; } = new List<int>();
        public long ReferenceInsurancePlanId { get; set; } = 0;
        public bool ForSimilarPlans { get; set; }
        public List<int> AnswerAsTags { get; set; } = new List<int>();
        public List<CompanyEnum> Companies { get; set; } = new List<CompanyEnum>();

        public PlanTypeEnum? PlanType { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class CommonInsurancePlanRequest : InsurancePlanPriceRequest
    {
        [Required]
        public string Name { get; set; }
        public string Summary { get; set; }
        public string PicturesJson { get; set; }
        public bool Featured { get; set; }
        [Range(0, 1, ErrorMessage ="Plan Type out of range")]
        public PlanTypeEnum PlanType { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string SEO { get; set; }
        public string Thumbnail { get; set; }
        public int OrderBy { get; set; }
        public int MinMonthsRequired { get; set; } = 1;
        public int FrequencyPerMonthsAllowed { get; set; } = 1;
        public string HospitalPDFs { get; set; }

        //public string PartnerPlanId { get; set; }
        [Required]
        public CompanyEnum Company { get; set; }

        public List<InsuranceCategoryRequest> Categories { get; set; }
        public List<AnswerAsTagRequest> AnswerAsTags { get; set; }
        public List<CompanyPlanExtraResultRequest> CompanyPlanExtraResults { get; set; } = new List<CompanyPlanExtraResultRequest>();
    }
    public class AnswerAsTagRequest
    {
        public int AnswerId { get; set; }
    }

    public class InsuranceCategoryRequest
    {
        public int Id { get; set; }
        public bool IsCovered { get; set; }
        public string CoveredNote { get; set; }
    }

    public class NewInsurancePlanRequest : CommonInsurancePlanRequest
    {
        public PublishStatusEnum PublishStatus { get; set; }
    }

    public class CompanyPlanExtraResultRequest
    {
        [Required]
        public int CompanyExtraId { get; set; }
        [Required]
        public AnswerTypeEnum AnswerType { get; set; }
        public string ResultInString { get; set; }
        public string ResultInHTML { get; set; }
        public int? ResultInNumber { get; set; }
        public DateTime? ResultInDateTime { get; set; }
        public decimal? ResultInDecimal { get; set; }
        public bool? ResultInBool { get; set; }
    }


    public class ExistingInsurancePlanRequest : NewInsurancePlanRequest
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Plan Id is required")]
        public long Id { get; set; }
    }

    public class FindAPlanSearchRequest
    {
        public List<int> AnswerAsTags { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class CompanyExtraRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public AnswerTypeEnum AnswerType { get; set; }
        public int OrderBy { get; set; }
    }
    public class NewInsuranceCompanyRequest
    {
        [Required]
        public string Name { get; set; }
        public int OrderBy { get; set; }
        public bool RequiredIdenitication { get; set; }
        public bool RequiredNextOfKin { get; set; }
        public bool RequiredJobInformation { get; set; }
        public bool RequiredBankInformation { get; set; }
        public List<CompanyExtraRequest> CompanyExtras { get; set; }
    }

    public class ExistingInsuranceCompanyRequest : NewInsuranceCompanyRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Company Id is required")]
        public int Id { get; set; }
    }


    public class InsuranceCompanyUpdateRequest 
    {
        [Required]
        public CompanyEnum CompanyId { get; set; }
        [Required]
        public string Logo { get; set; }
    }
}
