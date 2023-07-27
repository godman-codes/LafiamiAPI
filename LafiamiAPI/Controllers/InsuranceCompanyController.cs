using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
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
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class InsuranceCompanyController : BaseController<InsuranceCompanyController>
    {
        public readonly IBusinessUnitofWork businessUnitofWork;
        public InsuranceCompanyController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<InsuranceCompanyController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetAnswerTypes")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetAnswerTypes()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(AnswerTypeEnum)).Cast<AnswerTypeEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetLiteCompanies")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetLiteCompanies()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(CompanyEnum)).Cast<CompanyEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }


        [HttpGet]
        [Route("GetInsuranceCompanies")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LiteInsuranceCompanyResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsuranceCompanies(int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<LiteInsuranceCompanyResponse> result = (List<LiteInsuranceCompanyResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsuranceCompanyModel> queryable = businessUnitofWork.InsuranceService.GetInsuranceCompaniesQueryable().Where((r => true));

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
                    .Select(r => new LiteInsuranceCompanyResponse()
                    {
                        CompanyId = r.Company,
                        Logo = r.Logo,
                        OrderBy = r.OrderBy,
                        HasExtra = r.CompanyExtras.Where(r => r.ForProductSetup).Any()
                    })
                    .ToListAsync();

                result.ForEach(r => r.CompanyName = r.CompanyId.DisplayName());

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsuranceCompanyById")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(InsuranceCompanyResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsuranceCompanyById(CompanyEnum companyId)
        {
            string cachename = GenerateCacheName(GetMethodName(), companyId);
            InsuranceCompanyResponse result = (InsuranceCompanyResponse)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsuranceCompanyModel> queryable = businessUnitofWork.InsuranceService.GetInsuranceCompaniesQueryable()
                                                              .Where((r => r.Company == companyId));

                result = await queryable.AsNoTracking()
                    .Select(r => new InsuranceCompanyResponse()
                    {
                        CompanyId = r.Company,
                        Logo = r.Logo,
                        OrderBy = r.OrderBy,
                        RequiredNextOfKin = r.RequiredNextOfKin,
                        RequiredJobInformation = r.RequiredJobInformation,
                        RequiredIdenitication = r.RequiredIdenitication,
                        RequiredBankInformation = r.RequiredBankInformation,
                        RequiredExtraInformation = r.HasExtraInformation,
                        HasFixedQuantity = (r.Company == CompanyEnum.Hygeia),
                        CompanyOrderExtras = r.CompanyExtras.Where(r => !r.ForProductSetup && !r.IsDeleted).Select(t => new CompanyExtraResponse()
                        {
                            AnswerType = t.AnswerType,
                            DisplayName = t.DisplayName,
                            Id = t.Id,
                            Name = t.Name,
                            OrderBy = t.OrderBy,
                            DependentName = t.DependentName,
                            HasDependent = t.HasDependent,
                            LoadingToBeTrigger = t.LoadingToBeTrigger,
                            DropdownListGetUrl = t.DropdownListGetUrl
                        }).ToList(),
                        CompanyProductExtras = r.CompanyExtras.Where(r => r.ForProductSetup && !r.IsDeleted).Select(t => new CompanyExtraResponse()
                        {
                            AnswerType = t.AnswerType,
                            DisplayName = t.DisplayName,
                            Id = t.Id,
                            Name = t.Name,
                            OrderBy = t.OrderBy,
                            DependentName = t.DependentName,
                            HasDependent = t.HasDependent,
                            LoadingToBeTrigger = t.LoadingToBeTrigger,
                            DropdownListGetUrl = t.DropdownListGetUrl
                        }).ToList()
                    })
                    .SingleOrDefaultAsync();

                if (result != null)
                {
                    result.CompanyName = result.CompanyId.DisplayName();
                }

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        //[HttpGet]
        //[Route("FindInsuranceCompanyByName")]
        //[AllowAnonymous]
        //[ProducesResponseType(typeof(List<LiteInsuranceCompanyResponse>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> FindInsuranceCompanyByName(string searchName)
        //{
        //    string cachename = GetMethodName() + searchName;
        //    List<LiteInsuranceCompanyResponse> result = (List<LiteInsuranceCompanyResponse>)GetFromCache(cachename);
        //    if (result == null)
        //    {
        //        IQueryable<InsuranceCompanyModel> queryable = businessUnitofWork.InsuranceService.GetInsuranceCompaniesQueryable().Where((r => true));
        //        if (!string.IsNullOrEmpty(searchName))
        //        {
        //            queryable = queryable.Where(r =>
        //                r.Name.Contains(searchName)
        //            );
        //        }
        //        queryable = queryable.OrderByDescending(r => r.CreatedDate).Take(20);

        //        result = await queryable.AsNoTracking()
        //            .Select(r => new LiteInsuranceCompanyResponse()
        //            {
        //                Id = r.Id,
        //                CompanyName = r.Name,
        //                OrderBy = r.OrderBy,
        //                RequiredNextOfKin = r.RequiredNextOfKin,
        //                RequiredJobInformation = r.RequiredJobInformation,
        //                RequiredIdenitication = r.RequiredIdenitication,
        //                RequiredBankInformation = r.RequiredBankInformation
        //            })
        //            .ToListAsync();

        //        SavetoCache(result, cachename);
        //    }

        //    return Ok(result);
        //}

        //[HttpPost]
        //[Route("CreateInsuranceCompany")]
        //[Authorize(Roles = SystemRole.Administrator)]
        //[ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> CreateInsuranceCompany([FromBody] NewInsuranceCompanyRequest model)
        //{
        //    if (await businessUnitofWork.InsuranceService.IsInsuranceCompanyNameInUse(model.Name))
        //    {
        //        throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.InsuranceCompany));
        //    }

        //    InsuranceCompanyModel InsuranceCompany = new InsuranceCompanyModel()
        //    {
        //        Name = model.Name,
        //        OrderBy = model.OrderBy,
        //        RequiredNextOfKin = model.RequiredNextOfKin,
        //        RequiredJobInformation = model.RequiredJobInformation,
        //        RequiredIdenitication = model.RequiredIdenitication,
        //        RequiredBankInformation = model.RequiredBankInformation,
        //        CompanyExtras = model.CompanyExtras.Select(r=> new CompanyExtraModel() { 
        //            AnswerType = r.AnswerType,
        //            DisplayName = r.DisplayName,
        //            Name = r.Name,
        //            OrderBy = r.OrderBy,                    
        //        }).ToList()
        //    };

        //    businessUnitofWork.InsuranceService.AddInsuranceCompany(InsuranceCompany);
        //    await businessUnitofWork.SaveAsync();

        //    ClearCache();
        //    (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
        //    return Ok(new NewItemResponse<long>(InsuranceCompany.Id, string.Format(Constants.ActionResponse, Constants.InsuranceCompany, Constants.Created)));
        //}

        [HttpPost]
        [Route("UpdateInsuranceCompany")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInsuranceCompany([FromBody] InsuranceCompanyUpdateRequest model)
        {
            InsuranceCompanyModel InsuranceCompany = await businessUnitofWork.InsuranceService.GetInsuranceCompaniesQueryable()
                                                     .Where(r => r.Company == model.CompanyId)
                                                     .SingleOrDefaultAsync();

            InsuranceCompany.Logo = model.Logo;
            InsuranceCompany.UpdatedDate = DateTime.Now;

            businessUnitofWork.InsuranceService.UpdateInsuranceCompany(InsuranceCompany);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.InsuranceCompany, Constants.Updated));
        }


    }
}
