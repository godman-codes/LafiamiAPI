using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{
    public class InsuranceSearchAuditResponse
    {
        public string SearchValues { get; set; }
        public bool HasResult { get; set; }
        public int ResultCount { get; set; }
        public string Action { get; set; }
        public string FullName { get; set; }
    }


    public class LiteInsurancePlanNamesResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class LiteInsurancePlanResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
    }


    public class InsurancePlanResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string Summary { get; set; }
        public string PicturesJson { get; set; }
        public bool Featured { get; set; }
        public PlanTypeEnum PlanType { get; set; }
        public string PlanTypeName { get; set; }
        public string Description { get; set; }
        public string SEO { get; set; }
        public string Thumbnail { get; set; }
        public int ViewCounter { get; set; }
        public int MinMonthsRequired { get; set; }
        public int FrequencyPerMonthsAllowed { get; set; }
        public string HospitalPDFs { get; set; }
        public string PartnerPlanId { get; set; }
        public CompanyEnum Company { get; set; }
        public string CompanyName { get; set; }
        public bool HasFixedQuantity { get; set; }
        public int OrderBy { get; set; }
        public PublishStatusEnum PublishStatus { get; set; }

        public bool EnableDiscount { get; set; }
        public decimal Amount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }
        public string MoneyUnitName { get; set; }
        public decimal DiscountAmount { get; set; }
        public int CartBulkCount { get; set; }
        public int BulkCount { get; set; }
        public decimal CartBulkAmount { get; set; }
        public decimal BulkAmount { get; set; }
        public bool EnableBulkAmount { get; set; }
        public bool EnableCartBulkAmount { get; set; }


        public bool IsPublished { get; set; }
        public List<InsuranceCategoryResponse> Categories { get; set; }
        public List<AnswerAsTagResponse> AnswerAsTags { get; set; }
        public List<CompanyPlanExtraResultResponse> CompanyPlanExtraResults { get; set; } = new List<CompanyPlanExtraResultResponse>();
    }


    public class CompanyPlanExtraResultResponse
    {
        public int CompanyExtraId { get; set; }
        public AnswerTypeEnum AnswerType { get; set; }
        public string ResultInString { get; set; }
        public string ResultInHTML { get; set; }
        public int? ResultInNumber { get; set; }
        public DateTime? ResultInDateTime { get; set; }
        public decimal? ResultInDecimal { get; set; }
    }

    public class AnswerAsTagResponse
    {
        public int AnswerId { get; set; }
    }

    public class InsuranceCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCovered { get; set; }
        public string CoveredNote { get; set; }
        public int ParentCategoryId { get; set; }
        public int OrderBy { get; set; }
    }

    public class ViewInsurancePlanResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string Thumbnail { get; set; }
        public string Summary { get; set; }
        public string PicturesJson { get; set; }
        public decimal Amount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }
        public string MoneyUnitName { get; set; }
        public int OrderBy { get; set; }
        public decimal ActualAmount { get; set; }
        public bool EnableDiscount { get; set; }
        public bool HasFixedQuantity { get; set; }
    }

    public class InsurancePlanForAddtoCartResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int CartBulkCount { get; set; }
        public int BulkCount { get; set; }
        public decimal CartBulkAmount { get; set; }
        public decimal BulkAmount { get; set; }
        public bool EnableBulkAmount { get; set; }
        public bool EnableCartBulkAmount { get; set; }
    }

    public class LiteInsuranceCompanyResponse
    {
        public CompanyEnum CompanyId { get; set; }
        public string Logo { get; set; }
        public string CompanyName { get; set; }
        public int OrderBy { get; set; }
        public bool HasExtra { get; set; }
    }

    public class InsuranceCompanyResponse : LiteInsuranceCompanyResponse
    {
        public bool RequiredExtraInformation { get; set; }
        public bool RequiredIdenitication { get; set; }
        public bool RequiredNextOfKin { get; set; }
        public bool RequiredJobInformation { get; set; }
        public bool RequiredBankInformation { get; set; }
        public bool HasFixedQuantity { get; set; }
        public List<CompanyExtraResponse> CompanyOrderExtras { get; set; }
        public List<CompanyExtraResponse> CompanyProductExtras { get; set; }
    }

    public class CompanyExtraResponse {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public AnswerTypeEnum AnswerType { get; set; }
        public int OrderBy { get; set; }
        public int Id { get; set; }

        public string DropdownListGetUrl { get; set; }
        public bool HasDependent { get; set; }
        public bool LoadingToBeTrigger { get; set; }
        public string DependentName { get; set; }
    }

}
