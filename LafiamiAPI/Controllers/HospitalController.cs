using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
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
    public class HospitalController : BaseController<HospitalController>
    {
        public readonly IBusinessUnitofWork businessUnitofWork;
        public HospitalController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<HospitalController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetLiteHospitalsWithInsuranceStatus")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteHospitalWithInsurnaceStatusResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLiteHospitalsWithInsuranceStatus(long insurnacePlanId, int stateId, int? cityId = null)
        {
            string cachename = GenerateCacheName(GetMethodName(), insurnacePlanId, stateId, cityId ?? 0);
            List<LiteHospitalWithInsurnaceStatusResponse> result = (List<LiteHospitalWithInsurnaceStatusResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<HospitalModel> queryable = systemUnitofWork.HospitalService.GetQueryable((r => (r.StateId == stateId))); ;
                if (cityId.HasValue)
                {
                    queryable = queryable.Where(r => r.CityId == cityId.Value);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteHospitalWithInsurnaceStatusResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Status = r.HospitalInsurancePlans.Any(t => t.InsurancePlanId == insurnacePlanId)
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetInsurancePlanHospitals")]
        [Authorize]
        [ProducesResponseType(typeof(List<LiteHospitalResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanHospitals(long insurnacePlanId, int stateId, int? cityId = null)
        {
            string cachename = GenerateCacheName(GetMethodName(), stateId, cityId ?? 0);
            List<LiteHospitalResponse> result = (List<LiteHospitalResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<HospitalModel> queryable = systemUnitofWork.HospitalService.GetQueryable((r => (r.StateId == stateId) && (r.HospitalInsurancePlans.Any(t => t.InsurancePlanId == insurnacePlanId))));
                if (cityId.HasValue)
                {
                    queryable = queryable.Where(r => r.CityId == cityId.Value);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteHospitalResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetHospitals")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<HospitalResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitals(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword, page, pageSize);
            List<HospitalResponse> result = (List<HospitalResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<HospitalModel> queryable = systemUnitofWork.HospitalService.GetQueryable((r => true));
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
                else
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new HospitalResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Address = r.Address,
                        CityId = r.CityId,
                        CityName = r.CityId.HasValue ? r.City.Name : string.Empty,
                        StateId = r.StateId,
                        StateName = r.State.Name,
                        CountryId = r.State.CountryId,
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpPost]
        [Route("CreateHospital")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateHospital([FromBody] NewHospitalRequest model)
        {
            if (model.CityId.HasValue && (model.CityId > 0) && !await systemUnitofWork.CityService.DoesCityExist(model.CityId.Value))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.City));
            }
            if (!await systemUnitofWork.StateService.DoesStateExist(model.StateId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.State));
            }
            if (await systemUnitofWork.HospitalService.IsHospitalNameInUse(model.Name, model.StateId, model.CityId))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.Hospital));
            }

            HospitalModel Hospital = new HospitalModel()
            {
                Name = model.Name,
                Address = model.Address,
                StateId = model.StateId,
                CityId = model.CityId.HasValue ? (model.CityId > 0 ? model.CityId.Value : (int?)null) : null,
            };

            systemUnitofWork.HospitalService.Add(Hospital);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(new NewItemResponse<int>(Hospital.Id, string.Format(Constants.ActionResponse, Constants.Hospital, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateHospital")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateHospital([FromBody] ExistingHospitalRequest model)
        {
            if (model.CityId.HasValue && (model.CityId > 0) && !await systemUnitofWork.CityService.DoesCityExist(model.CityId.Value))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.City));
            }
            if (!await systemUnitofWork.StateService.DoesStateExist(model.StateId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.State));
            }
            if (await systemUnitofWork.HospitalService.IsHospitalNameInUse(model.Name, model.StateId, model.CityId, model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.Hospital));
            }

            HospitalModel Hospital = await systemUnitofWork.HospitalService.GetQueryable(r => r.Id == model.Id).SingleOrDefaultAsync();
            if (Hospital == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Hospital));
            }

            Hospital.Name = model.Name;
            Hospital.Address = model.Address;
            Hospital.CityId = model.CityId.HasValue ? (model.CityId > 0 ? model.CityId.Value : (int?)null) : null;
            Hospital.StateId = model.StateId;
            Hospital.UpdatedDate = DateTime.Now;

            systemUnitofWork.HospitalService.Update(Hospital);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Hospital, Constants.Updated));
        }

        [HttpPost]
        [Route("AddInsurancePlantoHospital")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddInsurancePlantoHospital([FromBody] HospitalInsurancePlansRequest model)
        {
            if (!await businessUnitofWork.InsuranceService.DoesInsurancePlanExist(model.InsurancePlanId, null))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.InsurancePlan));
            }
            if (!await systemUnitofWork.HospitalService.DoesHospitalExist(model.HospitalId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Hospital));
            }

            HospitalInsurancePlanModel hospitalInsurancePlan = await systemUnitofWork.HospitalService.GetHospitalInsurancePlanQueryable().SingleOrDefaultAsync(r => (r.HospitalId == model.HospitalId) && (r.InsurancePlanId == model.InsurancePlanId));

            if (hospitalInsurancePlan == null)
            {
                hospitalInsurancePlan = new HospitalInsurancePlanModel()
                {
                    InsurancePlanId = model.InsurancePlanId,
                    HospitalId = model.HospitalId
                };

                systemUnitofWork.HospitalService.AddHospitalInsurancePlan(hospitalInsurancePlan);
                await systemUnitofWork.SaveAsync();
            }

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, string.Join(Constants.Space, Constants.InsurancePlan, Constants.To, Constants.Hospital), Constants.Added));
        }

        [HttpPost]
        [Route("RemoveInsurancePlanFromHospital")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveInsurancePlanFromHospital([FromBody] HospitalInsurancePlansRequest model)
        {
            if (!await businessUnitofWork.InsuranceService.DoesInsurancePlanExist(model.InsurancePlanId, null))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.InsurancePlan));
            }
            if (!await systemUnitofWork.HospitalService.DoesHospitalExist(model.HospitalId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Hospital));
            }

            HospitalInsurancePlanModel hospitalInsurancePlan = await systemUnitofWork.HospitalService.GetHospitalInsurancePlanQueryable().SingleOrDefaultAsync(r => (r.HospitalId == model.HospitalId) && (r.InsurancePlanId == model.InsurancePlanId));

            if (hospitalInsurancePlan != null)
            {
                systemUnitofWork.HospitalService.RemoveHospitalInsurancePlan(hospitalInsurancePlan);
                await systemUnitofWork.SaveAsync();
            }

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, string.Join(Constants.Space, Constants.InsurancePlan, Constants.To, Constants.Hospital), Constants.Removed));
        }


        [HttpPost]
        [Route("DeleteHospital")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteHospital([FromBody] IdRequest<int> model)
        {
            HospitalModel Hospital = await systemUnitofWork.HospitalService.GetQueryable(r => r.Id == model.Id).SingleOrDefaultAsync();
            if (Hospital == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Hospital));
            }

            Hospital.ToDeletedEntity();

            systemUnitofWork.HospitalService.Update(Hospital);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Hospital, Constants.Deleted));
        }


    }
}
