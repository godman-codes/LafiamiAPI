using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class InsuranceService : RepositoryBase<InsurancePlanModel, long>, IInsuranceService
    {
        public InsuranceService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<InsuranceCompanyModel> GetInsuranceCompaniesQueryable()
        {
            return DBContext.InsuranceCompanies.Where(r => !r.IsDeleted).AsQueryable();
        }
        public IQueryable<InsuranceAuditModel> GetInsuranceAuditsQueryable()
        {
            return DBContext.InsuranceAudits.Where(r => !r.IsDeleted).AsQueryable();
        }

        public IQueryable<CompanyExtraModel> GetCompanyExtraQueryable()
        {
            return DBContext.CompanyExtras.Where(r => !r.IsDeleted).AsQueryable();
        }


        public async Task<bool> DoesInsuranceCompanyExist(long id)
        {
            return await GetInsuranceCompaniesQueryable().Where((r => r.Id == id)).AnyAsync();
        }

        public async Task<bool> DoesInsuranceCompaniesExist(List<long> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesInsuranceCompanyExist(id))
                {
                    return false;
                }
            }

            return true;
        }


        public async Task<string> GetCompanyExtraName(int companyExtraId)
        {
            return await GetCompanyExtraQueryable().Where(r => r.Id == companyExtraId)
                .Select(r => r.Name)
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetCompanyExtraDisplayName(int companyExtraId)
        {
            return await GetCompanyExtraQueryable().Where(r => r.Id == companyExtraId)
                .Select(r => r.DisplayName)
                .SingleOrDefaultAsync();
        }


        public async Task<bool> IsHygeiaPrincipalMemberIdCompanyExtra(int companyExtraId)
        {
            return await GetCompanyExtraQueryable().Where(r => (r.Id == companyExtraId) && (r.InsuranceCompany.Company == CompanyEnum.Hygeia) && (r.Name == "PrincipalMemberId")).AnyAsync();
        }

        public async Task<bool> DoesInsurancePlanExist(long insurancePlanId, string userId = null)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable((r => r.Id == insurancePlanId));
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }

            return await queryable.AnyAsync();
        }

        public async Task<AmountObjectModel> GetAmount(long insurancePlanId, DateTime? orderDate = null, int insuranceCount = 0, int cartCount = 0)
        {
            IQueryable<InsurancePriceModel> priceQueryable = DBContext.InsurancePrices.Where(r => (r.InsurancePlanId == insurancePlanId));
            if (orderDate.HasValue)
            {
                priceQueryable = priceQueryable.Where(r => r.CreatedDate < orderDate.Value);
            }

            InsurancePriceModel productPrice = await priceQueryable.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
            return GetAmount(productPrice, insuranceCount, cartCount);
        }

        public async Task<AmountObjectModel> GetActualAmount(long insurancePlanId, DateTime? orderDate = null)
        {
            IQueryable<InsurancePriceModel> priceQueryable = DBContext.InsurancePrices.Where(r => (r.InsurancePlanId == insurancePlanId));
            if (orderDate.HasValue)
            {
                priceQueryable = priceQueryable.Where(r => r.CreatedDate < orderDate.Value);
            }

            return await priceQueryable.OrderByDescending(r => r.CreatedDate)
                    .Select(r => new AmountObjectModel()
                    {
                        MoneyUnit = r.MoneyUnit,
                        Amount = r.Amount
                    })
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> GetEnableDiscountStatus(long insurancePlanId, DateTime? orderDate = null)
        {
            IQueryable<InsurancePriceModel> priceQueryable = DBContext.InsurancePrices.Where(r => (r.InsurancePlanId == insurancePlanId));
            if (orderDate.HasValue)
            {
                priceQueryable = priceQueryable.Where(r => r.CreatedDate < orderDate.Value);
            }

            return await priceQueryable.OrderByDescending(r => r.CreatedDate).Select(r => r.EnableDiscount).FirstOrDefaultAsync();
        }


        public async Task<int> GetMinMonthsRequired(long insurancePlanId)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable(r => (r.Id == insurancePlanId));
            int minRequired = await queryable.Select(r => r.MinMonthsRequired).SingleOrDefaultAsync();
            return (minRequired < 1) ? (1) : (minRequired);
        }

        public AmountObjectModel GetAmount(InsurancePriceModel insurancePrice, int insuranceCount, int cartCount = 0)
        {
            if (insurancePrice == null)
            {
                return new AmountObjectModel()
                {
                    MoneyUnit = MoneyUnitEnum.Monthly,
                    Amount = 0M
                };
            }

            decimal amount = (insurancePrice.EnableDiscount) ? (insurancePrice.DiscountAmount) : (insurancePrice.Amount);
            if ((insuranceCount > 0) && insurancePrice.EnableBulkAmount && (insuranceCount == insurancePrice.BulkCount))
            {
                amount = insurancePrice.BulkAmount;
            }
            if ((cartCount > 0) && insurancePrice.EnableCartBulkAmount && (cartCount == insurancePrice.CartBulkCount))
            {
                amount = insurancePrice.CartBulkAmount;
            }

            return new AmountObjectModel()
            {
                MoneyUnit = insurancePrice.MoneyUnit,
                Amount = amount
            };
        }

        public async Task<string> GetInsurancePlanName(long insurancePlanId)
        {
            return await GetQueryable(r => r.Id == insurancePlanId)
                .Select(r => r.Name)
                .SingleOrDefaultAsync();
        }

        public async Task<InsurancePriceModel> GetInsurancePlanPriceObject(long insurancePlanId, DateTime? orderDate = null)
        {
            IQueryable<InsurancePriceModel> productPriceQueryable = DBContext.InsurancePrices.Where(r => (r.InsurancePlanId == insurancePlanId));
            if (orderDate.HasValue)
            {
                productPriceQueryable = productPriceQueryable.Where(r => r.CreatedDate < orderDate.Value);
            }

            InsurancePriceModel productPrice = await productPriceQueryable.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync();
            return productPrice;
        }

        public async Task<List<InsuranceCategoryResponse>> GetInsurancePlanCategories(long insurancePlanId)
        {
            CategoryService categoryService = new CategoryService(DBContext);
            var parentCategories = await categoryService.GetQueryable(r => !r.ParentId.HasValue)
                .Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    OrderBy = r.OrderBy
                })
                .ToListAsync();

            List<InsuranceCategoryResponse> finalCategories = await DBContext.InsuranceCategories
                .Where(r => (r.InsurancePlanId == insurancePlanId))
                .Select(r => new InsuranceCategoryResponse()
                {
                    CoveredNote = r.CoveredNote,
                    Id = r.CategoryId,
                    IsCovered = r.IsCovered,
                    Name = r.Category.Name,
                    ParentCategoryId = r.Category.ParentId ?? 0,
                    OrderBy = r.Category.OrderBy
                })
                .ToListAsync();

            foreach (var category in parentCategories)
            {
                if (finalCategories.Any(r => r.ParentCategoryId == category.Id) && !finalCategories.Any(r => r.Id == category.Id))
                {
                    finalCategories.Add(new InsuranceCategoryResponse()
                    {
                        CoveredNote = string.Empty,
                        Id = category.Id,
                        IsCovered = true,
                        Name = category.Name,
                        ParentCategoryId = 0,
                        OrderBy = category.OrderBy
                    });
                }
            }

            return finalCategories;
        }

        public async Task<InsurancePlanModel> GetInsurancePlan(long insurancePlanId, string userId)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable(r => r.Id == insurancePlanId);
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }

            return await queryable
                        .Include(r => r.InsuranceCategories)
                        .Include(r => r.InsurancePlanAnswerAsTags)
                        .Include(r => r.PlanCompanyExtraResults)
                        .SingleOrDefaultAsync();
        }

        public void DeleteInsuranceCategory(InsuranceCategory insuranceCategory)
        {
            DBContext.InsuranceCategories.Remove(insuranceCategory);
        }

        public void DeletePlanCompanyExtraResult(PlanCompanyExtraResultModel planCompanyExtraResult)
        {
            DBContext.PlanCompanyExtraResults.Remove(planCompanyExtraResult);
        }



        public async Task<bool> IsInsurancePlanInUse(long insurancePlanId, string userId)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable(r => (r.Id == insurancePlanId) && (r.Carts.Any(t => !t.IsDeleted)));
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }

            return await queryable.AnyAsync();
        }

        public async Task<bool> IsInsurancePlanNameInUse(string name, long ignoreinsurancePlanId = 0)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable(r => EF.Functions.Like(r.Name, name));
            if (ignoreinsurancePlanId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignoreinsurancePlanId);
            }

            return await queryable.AnyAsync();
        }


        public async Task<bool> DoesLastInsurancePlanPriceTallies(long insurancePlanId, InsurancePlanPriceRequest insurancePlanPriceRequest)
        {
            InsurancePriceModel insurancePrice = await GetInsurancePlanPriceObject(insurancePlanId);

            if (insurancePrice.Amount != insurancePlanPriceRequest.Amount)
            {
                return false;
            }
            if (insurancePrice.DiscountAmount != insurancePlanPriceRequest.DiscountAmount)
            {
                return false;
            }
            if (insurancePrice.EnableDiscount != insurancePlanPriceRequest.EnableDiscount)
            {
                return false;
            }

            if (insurancePrice.EnableBulkAmount != insurancePlanPriceRequest.EnableBulkAmount)
            {
                return false;
            }
            if (insurancePrice.BulkAmount != insurancePlanPriceRequest.BulkAmount)
            {
                return false;
            }
            if (insurancePrice.BulkCount != insurancePlanPriceRequest.BulkCount)
            {
                return false;
            }

            if (insurancePrice.EnableCartBulkAmount != insurancePlanPriceRequest.EnableCartBulkAmount)
            {
                return false;
            }
            if (insurancePrice.CartBulkAmount != insurancePlanPriceRequest.CartBulkAmount)
            {
                return false;
            }
            if (insurancePrice.CartBulkCount != insurancePlanPriceRequest.CartBulkCount)
            {
                return false;
            }
            if (insurancePrice.MoneyUnit != insurancePlanPriceRequest.MoneyUnit)
            {
                return false;
            }

            return true;
        }

        public void AddInsuranceCompany(InsuranceCompanyModel insuranceCompany)
        {
            DBContext.InsuranceCompanies.Add(insuranceCompany);
        }
        public void UpdateInsuranceCompany(InsuranceCompanyModel insuranceCompany)
        {
            DBContext.InsuranceCompanies.Update(insuranceCompany);
        }
        public void AddInsuranceAudit(InsuranceAuditModel insuranceAudit)
        {
            DBContext.InsuranceAudits.Add(insuranceAudit);
        }
        public void AddInsurancePlanPrice(InsurancePriceModel insurancePrice)
        {
            DBContext.InsurancePrices.Add(insurancePrice);
        }

        public async Task<string> GenerateSearchName(string name, string prefixName = Constants.Empty, long ignorePlanId = 0)
        {
            DateTime today = DateTime.Now;
            int lengthAllowed = 190;

            string finalSearchName = Regex.Replace(name, "[^a-zA-Z0-9_]+", Constants.Underscore);
            if (!string.IsNullOrEmpty(prefixName) && (prefixName.Length > 0))
            {
                lengthAllowed -= (prefixName.Length + 1);
                finalSearchName = string.Join(Constants.BackSlash, prefixName, finalSearchName);
            }
            finalSearchName = finalSearchName.Substring(0, ((finalSearchName.Length < lengthAllowed) ? (finalSearchName.Length) : lengthAllowed)).ToLower();

            IQueryable<InsurancePlanModel> queryable = GetQueryable(t => t.SearchName == finalSearchName);
            if (ignorePlanId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignorePlanId);
            }

            bool check = await queryable.AnyAsync();

            if (check)
            {
                return string.Join(Constants.Underscore, finalSearchName.ToLower(), RandomGenerator.GetRandomString(6, false).ToLower());
            }
            else
            {
                return finalSearchName.ToLower();
            }
        }

        public async Task<InsurancePlanResponse> GetInsurancePlan(long id, string searchName, InsurancePlanResponse result)
        {
            IQueryable<InsurancePlanModel> queryable = GetQueryable(r => true);
            if (id > 0)
            {
                queryable = queryable.Where(r => r.Id == id);
            }
            if (!string.IsNullOrEmpty(searchName))
            {
                searchName = searchName.ToLower();
                queryable = queryable.Where(r => r.SearchName == searchName);
            }

            InsurancePlanModel insurancePlan = await queryable
                .Include(r => r.InsurancePlanAnswerAsTags)
                .Include(r => r.PlanCompanyExtraResults)
                .ThenInclude(r => r.CompanyExtra)
                .FirstOrDefaultAsync();
            if (insurancePlan == null)
            {
                return new InsurancePlanResponse();
            }

            InsurancePriceModel insurancePrice = await GetInsurancePlanPriceObject(insurancePlan.Id);

            result = new InsurancePlanResponse()
            {
                Id = insurancePlan.Id,
                Name = insurancePlan.Name,
                PartnerPlanId = insurancePlan.PartnerPlanId,
                PlanType = insurancePlan.PlanType,
                Company = insurancePlan.Company,
                HasFixedQuantity = (insurancePlan.Company == CompanyEnum.Hygeia),
                SearchName = insurancePlan.SearchName,
                Summary = WebUtility.HtmlDecode(insurancePlan.Summary),
                Amount = insurancePrice.Amount,
                MoneyUnit = insurancePrice.MoneyUnit,
                BulkAmount = insurancePrice.BulkAmount,
                BulkCount = insurancePrice.BulkCount,
                CartBulkAmount = insurancePrice.CartBulkAmount,
                CartBulkCount = insurancePrice.CartBulkCount,
                DiscountAmount = insurancePrice.DiscountAmount,
                EnableBulkAmount = insurancePrice.EnableBulkAmount,
                EnableCartBulkAmount = insurancePrice.EnableCartBulkAmount,
                EnableDiscount = insurancePrice.EnableDiscount,
                Featured = insurancePlan.Featured,
                Description = WebUtility.HtmlDecode(insurancePlan.Description),
                SEO = WebUtility.HtmlDecode(insurancePlan.SEO),
                PicturesJson = WebUtility.HtmlDecode(insurancePlan.PicturesJson),
                PublishStatus = insurancePlan.PublishStatus,
                Thumbnail = insurancePlan.Thumbnail,
                ViewCounter = insurancePlan.ViewCounter,
                MinMonthsRequired = insurancePlan.MinMonthsRequired,
                FrequencyPerMonthsAllowed = insurancePlan.FrequencyPerMonthsAllowed,
                HospitalPDFs = insurancePlan.HospitalPDFs,
                IsPublished = (insurancePlan.PublishStatus == PublishStatusEnum.Publish),
                Categories = await GetInsurancePlanCategories(insurancePlan.Id),
                OrderBy = insurancePlan.OrderBy,
                AnswerAsTags = insurancePlan.InsurancePlanAnswerAsTags.Select(t => new AnswerAsTagResponse()
                {
                    AnswerId = t.FindAPlanQuestionAnswerId,
                }).ToList(),
                CompanyPlanExtraResults = insurancePlan.PlanCompanyExtraResults.Select(t => new CompanyPlanExtraResultResponse()
                {
                    AnswerType = t.CompanyExtra.AnswerType,
                    CompanyExtraId = t.CompanyExtraId,
                    ResultInDateTime = t.ResultInDateTime,
                    ResultInDecimal = t.ResultInDecimal,
                    ResultInHTML = (string.IsNullOrEmpty(t.ResultInHTML)) ? (t.ResultInHTML) : (WebUtility.HtmlDecode(t.ResultInHTML)),
                    ResultInNumber = t.ResultInNumber,
                    ResultInString = t.ResultInString
                }).ToList()
            };

            result.MoneyUnitName = result.MoneyUnit.DisplayName();
            result.PlanTypeName = result.PlanType.DisplayName();
            result.CompanyName = (result.Company > 0) ? (result.Company.DisplayName()) : (string.Empty);
            return result;
        }

        public void AddCompanyExtra(CompanyExtraModel companyExtra)
        {
            DBContext.CompanyExtras.Add(companyExtra);
        }
        public void UpdateCompanyExtra(CompanyExtraModel companyExtra)
        {
            DBContext.CompanyExtras.Update(companyExtra);
        }
        public void DeleteCompanyExtra(CompanyExtraModel companyExtra)
        {
            DBContext.CompanyExtras.Remove(companyExtra);
        }
    }
}
