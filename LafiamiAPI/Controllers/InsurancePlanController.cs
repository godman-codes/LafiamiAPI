using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Attributes;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Queries;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class InsurancePlanController : BaseController<InsurancePlanController>
    {
        public const string ControllerName = ControllerConstant.InsurancePlan;
        public readonly IBusinessUnitofWork businessUnitofWork;
        public InsurancePlanController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<InsurancePlanController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetSortingType")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetSortingType()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(SortingTypeEnum)).Cast<SortingTypeEnum>()
                .Where(r => ((r.GetAttribute<UtilityDisplayAttribute>() != null) ? (!r.GetAttribute<UtilityDisplayAttribute>().IsPrivate) : (true)))
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPlanTypes")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPlanTypes()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(PlanTypeEnum)).Cast<PlanTypeEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMoneyUnits")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetMoneyUnits()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(MoneyUnitEnum)).Cast<MoneyUnitEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPublishStatuses")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPublishStatuses()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(PublishStatusEnum)).Cast<PublishStatusEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsurancePlans")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteInsurancePlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlans(string keyword, PublishStatusEnum? publishStatus, int category = 0, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, (publishStatus.HasValue ? publishStatus.Value.ToString() : string.Empty), category, page, pageSize);
            List<LiteInsurancePlanResponse> result = (List<LiteInsurancePlanResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsurancePlanModel> queryable = businessUnitofWork.InsuranceService.GetQueryable((r => true));
                if (publishStatus.HasValue)
                {
                    queryable = queryable.Where(r => r.PublishStatus == publishStatus.Value);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r => r.Name.Contains(keyword));
                }
                if (category > 0)
                {
                    queryable = queryable.Where(r => r.InsuranceCategories.Any(t => t.CategoryId == category));
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }
                else
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteInsurancePlanResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        SearchName = r.SearchName,
                        Thumbnail = r.Thumbnail,
                        IsPublished = (r.PublishStatus == PublishStatusEnum.Publish)
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsurancePlansWithSystemHospitalStatus")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteInsurancePlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlansWithSystemHospitalStatus(string keyword, PublishStatusEnum? publishStatus, int category = 0, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, (publishStatus.HasValue ? publishStatus.Value.ToString() : string.Empty), category, page, pageSize);
            List<LiteInsurancePlanResponse> result = (List<LiteInsurancePlanResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsurancePlanModel> queryable = businessUnitofWork.InsuranceService.GetQueryable(r => (r.Company == CompanyEnum.RelainceHMO) || (r.Company == CompanyEnum.Lafiami));

                if (publishStatus.HasValue)
                {
                    queryable = queryable.Where(r => r.PublishStatus == publishStatus.Value);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r => r.Name.Contains(keyword));
                }
                if (category > 0)
                {
                    queryable = queryable.Where(r => r.InsuranceCategories.Any(t => t.CategoryId == category));
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }
                else
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteInsurancePlanResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        SearchName = r.SearchName,
                        Thumbnail = r.Thumbnail,
                        IsPublished = (r.PublishStatus == PublishStatusEnum.Publish)
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetInsurancePlanNames")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LiteInsurancePlanNamesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanNames(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, page, pageSize);
            List<LiteInsurancePlanNamesResponse> result = (List<LiteInsurancePlanNamesResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsurancePlanModel> queryable = businessUnitofWork.InsuranceService.GetQueryable((r => r.PublishStatus == PublishStatusEnum.Publish));
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r => r.Name.Contains(keyword));
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteInsurancePlanNamesResponse()
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetInsurancePlanById")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(InsurancePlanResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanById(long id)
        {
            string cachename = GetMethodName() + id;
            InsurancePlanResponse result = (InsurancePlanResponse)GetFromCache(cachename);
            if (result == null)
            {
                result = await businessUnitofWork.InsuranceService.GetInsurancePlan(id, null, result);
                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsurancePlanByName")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(InsurancePlanResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanByName(string searchName)
        {
            string cachename = GetMethodName() + searchName;
            InsurancePlanResponse result = (InsurancePlanResponse)GetFromCache(cachename);
            if (result == null)
            {
                result = await businessUnitofWork.InsuranceService.GetInsurancePlan(0, searchName, result);
                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsurancePlanForAddToCart")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(InsurancePlanForAddtoCartResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanForAddToCart(long id)
        {
            string cachename = GetMethodName() + id;
            InsurancePlanForAddtoCartResponse result = (InsurancePlanForAddtoCartResponse)GetFromCache(cachename);
            if (result == null)
            {
                InsurancePlanModel insurancePlan = await businessUnitofWork.InsuranceService.GetQueryable(r => r.Id == id).FirstOrDefaultAsync();
                InsurancePriceModel productPrice = await businessUnitofWork.InsuranceService.GetInsurancePlanPriceObject(insurancePlan.Id);

                result = new InsurancePlanForAddtoCartResponse()
                {
                    Id = insurancePlan.Id,
                    Name = insurancePlan.Name,
                    Amount = productPrice.Amount,
                    BulkAmount = productPrice.BulkAmount,
                    BulkCount = productPrice.BulkCount,
                    CartBulkAmount = productPrice.CartBulkAmount,
                    CartBulkCount = productPrice.CartBulkCount,
                    EnableBulkAmount = productPrice.EnableBulkAmount,
                    EnableCartBulkAmount = productPrice.EnableCartBulkAmount,
                };

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("GetPublicInsurancePlans")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ViewInsurancePlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicInsurancePlans([FromBody] InsurancePlanSeachRequest model)
        {
            string cachename = GenerateCacheNameWithbject(GetMethodName(), model);

            List<ViewInsurancePlanResponse> insurancePlans = (List<ViewInsurancePlanResponse>)GetFromCache(cachename);
            if (insurancePlans == null)
            {
                await IsValidCategories(model);
                await GetSearchParametersBasedOnReferenceProductId(model);

                IQueryable<InsurancePlanModel> queryable = SearchPlanQuery(model);

                queryable = model.SortingType switch
                {
                    SortingTypeEnum.Latest => queryable.OrderByDescending(r => r.CreatedDate),
                    SortingTypeEnum.Oldest => queryable.OrderBy(r => r.CreatedDate),
                    SortingTypeEnum.LowestPrice => queryable.OrderBy(r => r.SortingAmount),
                    SortingTypeEnum.HighestPrice => queryable.OrderByDescending(r => r.SortingAmount),
                    SortingTypeEnum.Popular => queryable.OrderByDescending(r => r.ViewCounter),
                    SortingTypeEnum.Random => queryable.OrderBy(r => Guid.NewGuid()),
                    _ => queryable.OrderByDescending(r => r.CreatedDate),
                };

                if (model.PageSize > 0)
                {
                    queryable = queryable.Skip(model.Page * model.PageSize).Take(model.PageSize);
                }

                insurancePlans = await queryable.AsNoTracking()
                    .Select(r => new ViewInsurancePlanResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        SearchName = r.SearchName,
                        Thumbnail = r.Thumbnail,
                        PicturesJson = WebUtility.HtmlDecode(r.PicturesJson),
                        Summary = r.Summary,
                        OrderBy = r.OrderBy,
                        HasFixedQuantity = (r.Company == CompanyEnum.Hygeia)
                    })
                    .ToListAsync();

                foreach (ViewInsurancePlanResponse viewInsurancePlan in insurancePlans)
                {
                    AmountObjectModel moneyAmount = await businessUnitofWork.InsuranceService.GetAmount(viewInsurancePlan.Id);
                    viewInsurancePlan.Amount = moneyAmount.Amount;
                    viewInsurancePlan.MoneyUnit = moneyAmount.MoneyUnit;
                    viewInsurancePlan.MoneyUnitName = viewInsurancePlan.MoneyUnit.DisplayName();
                    AmountObjectModel actualMoneyAmount = await businessUnitofWork.InsuranceService.GetActualAmount(viewInsurancePlan.Id);
                    viewInsurancePlan.ActualAmount = actualMoneyAmount.Amount;
                    viewInsurancePlan.EnableDiscount = await businessUnitofWork.InsuranceService.GetEnableDiscountStatus(viewInsurancePlan.Id);
                    viewInsurancePlan.Summary = WebUtility.HtmlDecode(viewInsurancePlan.Summary);
                }
                SavetoCache(insurancePlans, cachename);
            }

            await SaveInsuranceAudit("All Products", insurancePlans.Count(), model);
            return Ok(insurancePlans);
        }

        private async Task SaveInsuranceAudit(string action, int resultCount, InsurancePlanSeachRequest model)
        {
            try
            {
                businessUnitofWork.InsuranceService.AddInsuranceAudit(new InsuranceAuditModel()
                {
                    Action = action,
                    HasResult = (resultCount > 0),
                    ResultCount = resultCount,
                    Id = Guid.NewGuid(),
                    Keyword = model.Keyword,
                    UserId = GetUserId(),
                    InsuranceAuditCategories = (model.Categories.Any()) ? (model.Categories.Select(r => new InsuranceAuditCategoryModel()
                    {
                        CategoryId = r
                    }).ToList()) : (new List<InsuranceAuditCategoryModel>()),
                    InsuranceAuditQuestionAnswers = (model.AnswerAsTags.Any()) ? (model.AnswerAsTags.Select(r => new InsuranceAuditQuestionAnswerModel()
                    {
                        FindAPlanQuestionAnswerId = r
                    }).ToList()) : (new List<InsuranceAuditQuestionAnswerModel>()),
                });
                await businessUnitofWork.SaveAsync();

                (new InsuranceAuditController(cache, null, null, null)).ClearCache();
            }
            catch (Exception)
            {
            }
        }

        private string GenerateCacheNameWithbject(string methodName, InsurancePlanSeachRequest model)
        {
            return GenerateCacheName(methodName, model.Keyword ?? string.Empty, model.SortingType, string.Join(Constants.Comma, model.Categories), string.Join(Constants.Comma, model.Companies), string.Join(Constants.Comma, model.AnswerAsTags), model.ReferenceInsurancePlanId, model.ForSimilarPlans, (model.PlanType.HasValue ? model.PlanType.Value.ToString() : string.Empty), model.Page, model.PageSize);
        }

        private async Task IsValidCategories(InsurancePlanSeachRequest model)
        {
            if ((model.Categories.Count() > 0) && !await businessUnitofWork.CategoryService.DoesCategoriesExist(model.Categories))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Category));
            }
        }

        private async Task GetSearchParametersBasedOnReferenceProductId(InsurancePlanSeachRequest model)
        {
            if (model.ReferenceInsurancePlanId > 0)
            {
                model.Categories = await businessUnitofWork.CategoryService.GetQueryable((r => r.InsuranceCategories.Any(t => t.InsurancePlanId == model.ReferenceInsurancePlanId)))
                    .Select(r => r.Id)
                    .ToListAsync();
                model.SortingType = SortingTypeEnum.Random;
            }
        }


        [HttpPost]
        [Route("CountInsurancePlans")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CountInsurancePlans([FromBody] InsurancePlanSeachRequest model)
        {
            string cachename = GenerateCacheNameWithbject(GetMethodName(), model);

            int? count = (int?)GetFromCache(cachename);
            if (count == null)
            {
                await IsValidCategories(model);
                await GetSearchParametersBasedOnReferenceProductId(model);

                IQueryable<InsurancePlanModel> queryable = SearchPlanQuery(model);

                count = await queryable.AsNoTracking().CountAsync();
                SavetoCache(count, cachename);
            }

            return Ok(count);
        }

        private IQueryable<InsurancePlanModel> SearchPlanQuery(InsurancePlanSeachRequest model)
        {
            IQueryable<InsurancePlanModel> queryable = businessUnitofWork.InsuranceService.GetQueryable(r => (r.PublishStatus == PublishStatusEnum.Publish));
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                string[] words = model.Keyword.Split(Constants.Space);
                queryable = queryable.Where(r => words.Contains(r.Name));
            }

            if (model.Companies.Count() > 0)
            {
                Expression<Func<InsurancePlanModel, bool>> expression = r => false;
                foreach (CompanyEnum company in model.Companies)
                {
                    expression = QueryCombinator.MergeWithOr(expression, (r => r.Company == company));
                }
                queryable = queryable.Where(expression);
            }

            if (model.PlanType.HasValue)
            {
                queryable = queryable.Where(r => r.PlanType == model.PlanType.Value);
            }

            if (model.Categories.Count() > 0)
            {
                //note I am using -1 because Category Id will never be -1 and it wont be in the category list.
                if ((model.ReferenceInsurancePlanId > 0) && !model.ForSimilarPlans)
                {
                    queryable = queryable.Where(r => !r.InsuranceCategories.Any(t => model.Categories.Contains(t.Category.Id) || model.Categories.Contains(t.Category.ParentId ?? -1)));
                }
                else
                {
                    queryable = queryable.Where(r => r.InsuranceCategories.Any(t => model.Categories.Contains(t.Category.Id) || model.Categories.Contains(t.Category.ParentId ?? -1)));
                }

                if (model.ReferenceInsurancePlanId > 0)
                {
                    queryable = queryable.Where(r => r.Id != model.ReferenceInsurancePlanId);
                }
            }

            if (model.AnswerAsTags.Count() > 0)
            {
                decimal _75Percent = decimal.Divide(decimal.Multiply(75, model.AnswerAsTags.Count()), 100M);
                queryable = queryable.Where(r => r.InsurancePlanAnswerAsTags.Count(u => model.AnswerAsTags.Any(t => t == u.FindAPlanQuestionAnswerId)) >= _75Percent);
            }

            return queryable;
        }


        [HttpPost]
        [Route("GetFeaturedInsurancePlans")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ViewInsurancePlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeaturedInsurancePlans([FromBody] InsurancePlanSeachRequest model)
        {
            string cachename = GenerateCacheNameWithbject(GetMethodName(), model);

            List<ViewInsurancePlanResponse> insurancePlans = (List<ViewInsurancePlanResponse>)GetFromCache(cachename);
            if (insurancePlans == null)
            {
                if ((model.Categories.Count() > 0) && !await businessUnitofWork.CategoryService.DoesCategoriesExist(model.Categories))
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Category));
                }

                IQueryable<InsurancePlanModel> queryable = SearchPlanQuery(model);
                queryable = queryable.Where(r => r.Featured);

                queryable = model.SortingType switch
                {
                    SortingTypeEnum.Latest => queryable.OrderByDescending(r => r.CreatedDate),
                    SortingTypeEnum.Oldest => queryable.OrderBy(r => r.CreatedDate),
                    SortingTypeEnum.LowestPrice => queryable.OrderBy(r => r.SortingAmount),
                    SortingTypeEnum.HighestPrice => queryable.OrderByDescending(r => r.SortingAmount),
                    SortingTypeEnum.Popular => queryable.OrderByDescending(r => r.ViewCounter),
                    _ => queryable.OrderByDescending(r => r.CreatedDate),
                };

                if (model.PageSize > 0)
                {
                    queryable = queryable.Skip(model.Page * model.PageSize).Take(model.PageSize);
                }

                insurancePlans = await queryable.AsNoTracking()
                    .Select(r => new ViewInsurancePlanResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Thumbnail = r.Thumbnail,
                        SearchName = r.SearchName,
                        PicturesJson = r.PicturesJson,
                        Summary = r.Summary,
                        OrderBy = r.OrderBy,
                        HasFixedQuantity = (r.Company == CompanyEnum.Hygeia)
                    })
                    .ToListAsync();

                foreach (ViewInsurancePlanResponse insurancePlan in insurancePlans)
                {
                    insurancePlan.PicturesJson = WebUtility.HtmlDecode(insurancePlan.PicturesJson);
                    AmountObjectModel moneyAmount = await businessUnitofWork.InsuranceService.GetAmount(insurancePlan.Id);
                    insurancePlan.Amount = moneyAmount.Amount;
                    insurancePlan.MoneyUnit = moneyAmount.MoneyUnit;
                    insurancePlan.MoneyUnitName = insurancePlan.MoneyUnit.DisplayName();
                    AmountObjectModel actualMoneyAmount = await businessUnitofWork.InsuranceService.GetActualAmount(insurancePlan.Id);
                    insurancePlan.ActualAmount = actualMoneyAmount.Amount;
                    insurancePlan.EnableDiscount = await businessUnitofWork.InsuranceService.GetEnableDiscountStatus(insurancePlan.Id);
                    insurancePlan.Summary = WebUtility.HtmlDecode(insurancePlan.Summary);
                }
                SavetoCache(insurancePlans, cachename);
            }

            return Ok(insurancePlans);
        }


        [HttpPost]
        [Route("FindAPlanResults")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ViewInsurancePlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindAPlanResults([FromBody] InsurancePlanSeachRequest model)
        {
            string cachename = GenerateCacheNameWithbject(GetMethodName(), model);
            List<ViewInsurancePlanResponse> insurancePlans = (List<ViewInsurancePlanResponse>)GetFromCache(cachename);
            if (insurancePlans == null)
            {
                IQueryable<InsurancePlanModel> queryable = SearchPlanQuery(model);

                queryable = model.SortingType switch
                {
                    SortingTypeEnum.Latest => queryable.OrderByDescending(r => r.CreatedDate),
                    SortingTypeEnum.Oldest => queryable.OrderBy(r => r.CreatedDate),
                    SortingTypeEnum.LowestPrice => queryable.OrderBy(r => r.SortingAmount),
                    SortingTypeEnum.HighestPrice => queryable.OrderByDescending(r => r.SortingAmount),
                    SortingTypeEnum.Popular => queryable.OrderByDescending(r => r.ViewCounter),
                    SortingTypeEnum.Random => queryable.OrderBy(r => Guid.NewGuid()),
                    _ => queryable.OrderByDescending(r => r.CreatedDate),
                };

                if (model.PageSize > 0)
                {
                    queryable = queryable.Skip(model.Page * model.PageSize).Take(model.PageSize);
                }

                insurancePlans = await queryable.AsNoTracking()
                    .Select(r => new ViewInsurancePlanResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Thumbnail = r.Thumbnail,
                        SearchName = r.SearchName,
                        PicturesJson = WebUtility.HtmlDecode(r.PicturesJson),
                        Summary = r.Summary,
                        OrderBy = r.OrderBy
                    })
                    .ToListAsync();

                foreach (ViewInsurancePlanResponse insurancePlan in insurancePlans)
                {
                    AmountObjectModel moneyAmount = await businessUnitofWork.InsuranceService.GetAmount(insurancePlan.Id);
                    insurancePlan.Amount = moneyAmount.Amount;
                    insurancePlan.MoneyUnit = moneyAmount.MoneyUnit;
                    insurancePlan.MoneyUnitName = insurancePlan.MoneyUnit.DisplayName();
                    AmountObjectModel actualMoneyAmount = await businessUnitofWork.InsuranceService.GetActualAmount(insurancePlan.Id);
                    insurancePlan.ActualAmount = actualMoneyAmount.Amount;
                    insurancePlan.EnableDiscount = await businessUnitofWork.InsuranceService.GetEnableDiscountStatus(insurancePlan.Id);
                    insurancePlan.Summary = WebUtility.HtmlDecode(insurancePlan.Summary);
                }
                SavetoCache(insurancePlans, cachename);
            }

            await SaveInsuranceAudit("Find a Plan", insurancePlans.Count(), model);
            return Ok(insurancePlans);
        }


        [HttpPost]
        [Route("CreateInsurancePlan")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<long>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateInsurancePlan([FromBody] NewInsurancePlanRequest model)
        {
            InsurancePlanModel insurancePlan = await ProcessInsurancePlan(model);
            insurancePlan.PublishStatus = model.PublishStatus;

            businessUnitofWork.InsuranceService.Add(insurancePlan);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependents();
            return Ok(new NewItemResponse<long>(insurancePlan.Id, string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Created)));
        }

        private decimal GetSortingAmountPerMonth(AmountObjectModel amountObject)
        {
            return decimal.Divide(amountObject.Amount, amountObject.MoneyUnit.GetAttribute<UtilityDisplayAttribute>().MountCount);
        }
        private async Task<InsurancePlanModel> ProcessInsurancePlan(CommonInsurancePlanRequest model)
        {
            await ValidateInsurancePlanDetails(model);
            if (await businessUnitofWork.InsuranceService.IsInsurancePlanNameInUse(model.Name))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.InsurancePlan));
            }

            InsurancePriceModel insurancePrice = new InsurancePriceModel()
            {
                Amount = model.Amount,
                BulkAmount = model.BulkAmount,
                BulkCount = model.BulkCount,
                CartBulkAmount = model.CartBulkAmount,
                CartBulkCount = model.CartBulkCount,
                DiscountAmount = model.DiscountAmount,
                EnableBulkAmount = model.EnableBulkAmount,
                EnableCartBulkAmount = model.EnableCartBulkAmount,
                EnableDiscount = model.EnableDiscount,
                MoneyUnit = model.MoneyUnit,
                Id = Guid.NewGuid(),
            };

            InsurancePlanModel insurancePlan = new InsurancePlanModel()
            {
                PlanType = model.PlanType,
                OrderBy = model.OrderBy,
                Company = model.Company,
                Featured = model.Featured,
                Description = WebUtility.HtmlEncode(model.Description),
                SEO = WebUtility.HtmlEncode(model.SEO),
                Name = model.Name,
                SearchName = await businessUnitofWork.InsuranceService.GenerateSearchName(model.Name, string.Empty),
                PicturesJson = WebUtility.HtmlEncode(model.PicturesJson),
                SortingAmount = GetSortingAmountPerMonth(businessUnitofWork.InsuranceService.GetAmount(insurancePrice, 0)),
                Summary = WebUtility.HtmlEncode(model.Summary),
                Thumbnail = model.Thumbnail,
                MinMonthsRequired = model.MinMonthsRequired,
                FrequencyPerMonthsAllowed = model.FrequencyPerMonthsAllowed,
                HospitalPDFs = model.HospitalPDFs,
                InsurancePrices = new List<InsurancePriceModel>() { insurancePrice },
                InsuranceCategories = model.Categories.Select(r => new InsuranceCategory()
                {
                    CategoryId = r.Id,
                    IsCovered = r.IsCovered,
                    CoveredNote = r.CoveredNote
                }).ToList(),
                InsurancePlanAnswerAsTags = model.AnswerAsTags.Select(r => new InsurancePlanAnswerAsTagModel()
                {
                    FindAPlanQuestionAnswerId = r.AnswerId,
                }).ToList(),
                PlanCompanyExtraResults = model.CompanyPlanExtraResults.Select(r => new PlanCompanyExtraResultModel()
                {
                    CompanyExtraId = r.CompanyExtraId,
                    ResultInDateTime = r.ResultInDateTime,
                    ResultInDecimal = r.ResultInDecimal,
                    ResultInHTML = WebUtility.HtmlEncode(r.ResultInHTML),
                    ResultInNumber = r.ResultInNumber,
                    ResultInString = r.ResultInString,
                    ResultInBool = r.ResultInBool
                }).ToList(),
            };

            return insurancePlan;
        }

        private async Task ValidateInsurancePlanDetails(CommonInsurancePlanRequest model)
        {
            if (!await businessUnitofWork.CategoryService.DoesCategoriesExist(model.Categories.Select(r => r.Id).ToList()))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Category));
            }


            if (!await businessUnitofWork.FindAPlanService.DoesAnswersExist(model.AnswerAsTags.Select(r => r.AnswerId).ToList()))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.AnswerAsTag));
            }

        }

        [HttpPost]
        [Route("UpdateInsurancePlan")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInsurancePlan([FromBody] ExistingInsurancePlanRequest model)
        {
            await ValidateInsurancePlanDetails(model);
            InsurancePlanModel insurancePlan = await businessUnitofWork.InsuranceService.GetInsurancePlan(model.Id, null);


            insurancePlan = await ProcessUpdatingProduct(model, insurancePlan);
            insurancePlan.PublishStatus = model.PublishStatus;

            businessUnitofWork.InsuranceService.Update(insurancePlan);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependents();
            return Ok(string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Updated));
        }

        private async Task<InsurancePlanModel> ProcessUpdatingProduct(CommonInsurancePlanRequest model, InsurancePlanModel insurancePlan)
        {
            if (await businessUnitofWork.InsuranceService.IsInsurancePlanNameInUse(model.Name, insurancePlan.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.InsurancePlan));
            }

            if (!await businessUnitofWork.InsuranceService.DoesLastInsurancePlanPriceTallies(insurancePlan.Id, model))
            {
                InsurancePriceModel insurancePrice = new InsurancePriceModel()
                {
                    Amount = model.Amount,
                    BulkAmount = model.BulkAmount,
                    BulkCount = model.BulkCount,
                    CartBulkAmount = model.CartBulkAmount,
                    CartBulkCount = model.CartBulkCount,
                    DiscountAmount = model.DiscountAmount,
                    EnableBulkAmount = model.EnableBulkAmount,
                    EnableCartBulkAmount = model.EnableCartBulkAmount,
                    EnableDiscount = model.EnableDiscount,
                    Id = Guid.NewGuid(),
                    InsurancePlanId = insurancePlan.Id,
                    MoneyUnit = model.MoneyUnit
                };

                businessUnitofWork.InsuranceService.AddInsurancePlanPrice(insurancePrice);
                insurancePlan.SortingAmount = GetSortingAmountPerMonth(businessUnitofWork.InsuranceService.GetAmount(insurancePrice, 0));
            }

            insurancePlan.PlanType = model.PlanType;
            insurancePlan.OrderBy = model.OrderBy;
            //insurancePlan.PartnerPlanId = model.PartnerPlanId;
            insurancePlan.Company = model.Company;
            insurancePlan.Featured = model.Featured;
            insurancePlan.Description = WebUtility.HtmlEncode(model.Description);
            insurancePlan.SEO = WebUtility.HtmlEncode(model.SEO);
            insurancePlan.Name = model.Name;
            insurancePlan.PicturesJson = WebUtility.HtmlEncode(model.PicturesJson);
            insurancePlan.Summary = WebUtility.HtmlEncode(model.Summary);
            insurancePlan.Thumbnail = model.Thumbnail;
            insurancePlan.MinMonthsRequired = model.MinMonthsRequired;
            insurancePlan.FrequencyPerMonthsAllowed = model.FrequencyPerMonthsAllowed;
            insurancePlan.HospitalPDFs = model.HospitalPDFs;
            insurancePlan.SearchName = await businessUnitofWork.InsuranceService.GenerateSearchName(model.Name, string.Empty, insurancePlan.Id);


            foreach (InsuranceCategory insuranceCategory in insurancePlan.InsuranceCategories)
            {
                if (!model.Categories.Any(r => r.Id == insuranceCategory.CategoryId))
                {
                    businessUnitofWork.InsuranceService.DeleteInsuranceCategory(insuranceCategory);
                }
            }

            foreach (InsuranceCategoryRequest category in model.Categories)
            {
                InsuranceCategory dbCategory = insurancePlan.InsuranceCategories.FirstOrDefault(r => r.CategoryId == category.Id);

                if (dbCategory == null)
                {
                    insurancePlan.InsuranceCategories.Add(new InsuranceCategory()
                    {
                        CategoryId = category.Id,
                        IsCovered = category.IsCovered,
                        CoveredNote = category.CoveredNote
                    });
                }
                else
                {
                    dbCategory.IsCovered = category.IsCovered;
                    dbCategory.CoveredNote = category.CoveredNote;
                }
            }

            foreach (InsurancePlanAnswerAsTagModel answer in insurancePlan.InsurancePlanAnswerAsTags)
            {
                if (!model.AnswerAsTags.Any(r => r.AnswerId == answer.FindAPlanQuestionAnswerId))
                {
                    businessUnitofWork.FindAPlanService.DeleteAnswerAsTag(answer);
                }
            }

            foreach (AnswerAsTagRequest answer in model.AnswerAsTags)
            {
                if (!insurancePlan.InsurancePlanAnswerAsTags.Any(r => r.FindAPlanQuestionAnswerId == answer.AnswerId))
                {
                    insurancePlan.InsurancePlanAnswerAsTags.Add(new InsurancePlanAnswerAsTagModel()
                    {
                        FindAPlanQuestionAnswerId = answer.AnswerId
                    });
                }
            }

            foreach (PlanCompanyExtraResultModel extra in insurancePlan.PlanCompanyExtraResults)
            {
                if (!model.CompanyPlanExtraResults.Any(r => r.CompanyExtraId == extra.CompanyExtraId))
                {
                    businessUnitofWork.InsuranceService.DeletePlanCompanyExtraResult(extra);
                }
            }

            foreach (CompanyPlanExtraResultRequest extra in model.CompanyPlanExtraResults)
            {
                PlanCompanyExtraResultModel dbExtra = insurancePlan.PlanCompanyExtraResults.Where(r => r.CompanyExtraId == extra.CompanyExtraId).SingleOrDefault();
                if (dbExtra == null)
                {
                    insurancePlan.PlanCompanyExtraResults.Add(new PlanCompanyExtraResultModel()
                    {
                        CompanyExtraId = extra.CompanyExtraId,
                        ResultInDateTime = extra.ResultInDateTime,
                        ResultInDecimal = extra.ResultInDecimal,
                        ResultInHTML = WebUtility.HtmlEncode(extra.ResultInHTML),
                        ResultInNumber = extra.ResultInNumber,
                        ResultInString = extra.ResultInString,
                        ResultInBool = extra.ResultInBool
                    });
                }
                else
                {
                    dbExtra.ResultInDateTime = extra.ResultInDateTime;
                    dbExtra.ResultInDecimal = extra.ResultInDecimal;
                    dbExtra.ResultInHTML = WebUtility.HtmlEncode(extra.ResultInHTML);
                    dbExtra.ResultInNumber = extra.ResultInNumber;
                    dbExtra.ResultInString = extra.ResultInString;
                    dbExtra.ResultInBool = extra.ResultInBool;
                }
            }

            return insurancePlan;
        }

        private void ClearCacheWithDependents()
        {
            ClearCache();
            (new CartController(cache, null, null, null)).ClearCache();
        }

        [HttpPost]
        [Route("DeleteInsurancePlan")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteInsurancePlan([FromBody] long id)
        {
            InsurancePlanModel product = await businessUnitofWork.InsuranceService.GetInsurancePlan(id, null);
            if (product == null)
            {
                throw new WebsiteException(Constants.RecordDoesNotExist);
            }
            if (await businessUnitofWork.InsuranceService.IsInsurancePlanInUse(id, null))
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.InsurancePlan, Constants.Delete));
            }

            product.ToDeletedEntity();

            businessUnitofWork.InsuranceService.Update(product);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependents();

            return Ok(string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Deleted));
        }

        [HttpPost]
        [Route("UnPublishInsurancePlan")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnPublishInsurancePlan([FromBody] long id)
        {
            InsurancePlanModel product = await businessUnitofWork.InsuranceService.GetInsurancePlan(id, null);
            if (product == null)
            {
                throw new WebsiteException(Constants.RecordDoesNotExist);
            }

            product.PublishStatus = PublishStatusEnum.Unpublished;
            product.UpdatedDate = DateTime.Now;

            businessUnitofWork.InsuranceService.Update(product);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependents();
            return Ok(string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Unpublished));
        }

        [HttpPost]
        [Route("PublishInsurancePlan")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> PublishInsurancePlan([FromBody] long id)
        {
            InsurancePlanModel product = await businessUnitofWork.InsuranceService.GetInsurancePlan(id, null);
            if (product == null)
            {
                throw new WebsiteException(Constants.RecordDoesNotExist);
            }

            product.PublishStatus = PublishStatusEnum.Publish;
            product.UpdatedDate = DateTime.Now;

            businessUnitofWork.InsuranceService.Update(product);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependents();
            return Ok(string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Published));
        }

    }
}
