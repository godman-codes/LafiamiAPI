using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IInsuranceService : IRepositoryBase<InsurancePlanModel, long>
    {
        IQueryable<CompanyExtraModel> GetCompanyExtraQueryable();
        Task<string> GetCompanyExtraName(int companyExtraId);
        Task<string> GetCompanyExtraDisplayName(int companyExtraId);
        Task<bool> IsHygeiaPrincipalMemberIdCompanyExtra(int companyExtraId);
        void DeleteCompanyExtra(CompanyExtraModel companyExtra);
        void UpdateCompanyExtra(CompanyExtraModel companyExtra);
        void AddCompanyExtra(CompanyExtraModel companyExtra);
        Task<bool> DoesInsuranceCompaniesExist(List<long> ids);
        Task<bool> DoesInsuranceCompanyExist(long id);
        IQueryable<InsuranceCompanyModel> GetInsuranceCompaniesQueryable();
        void AddInsuranceCompany(InsuranceCompanyModel insuranceCompany);
        void UpdateInsuranceCompany(InsuranceCompanyModel insuranceCompany);
        Task<int> GetMinMonthsRequired(long insurancePlanId);
        IQueryable<InsuranceAuditModel> GetInsuranceAuditsQueryable();
        Task<bool> DoesInsurancePlanExist(long insurancePlanId, string userId = null);
        Task<AmountObjectModel> GetAmount(long insurancePlanId, DateTime? orderDate = null, int insuranceCount = 0, int cartCount = 0);
        Task<AmountObjectModel> GetActualAmount(long insurancePlanId, DateTime? orderDate = null);
        Task<bool> GetEnableDiscountStatus(long insurancePlanId, DateTime? orderDate = null);
        AmountObjectModel GetAmount(InsurancePriceModel insurancePrice, int insuranceCount, int cartCount = 0);
        Task<string> GetInsurancePlanName(long insurancePlanId);
        Task<InsurancePriceModel> GetInsurancePlanPriceObject(long insurancePlanId, DateTime? orderDate = null);
        Task<List<InsuranceCategoryResponse>> GetInsurancePlanCategories(long insurancePlanId);
        Task<InsurancePlanModel> GetInsurancePlan(long insurancePlanId, string userId);
        void DeleteInsuranceCategory(InsuranceCategory insuranceCategory);
        void DeletePlanCompanyExtraResult(PlanCompanyExtraResultModel planCompanyExtraResult);
        Task<bool> IsInsurancePlanInUse(long insurancePlanId, string userId);
        Task<bool> IsInsurancePlanNameInUse(string name, long ignoreinsurancePlanId = 0);

        Task<bool> DoesLastInsurancePlanPriceTallies(long insurancePlanId, InsurancePlanPriceRequest insurancePlanPriceRequest);
        void AddInsurancePlanPrice(InsurancePriceModel insurancePrice);
        Task<string> GenerateSearchName(string name, string prefixName = Constants.Empty, long ignorePlanId = 0);
        Task<InsurancePlanResponse> GetInsurancePlan(long id, string searchName, InsurancePlanResponse result);
        void AddInsuranceAudit(InsuranceAuditModel insuranceAudit);
    }
}
