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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class CityController : BaseController<CityController>
    {
        public const string ControllerName = ControllerConstant.City;
        public CityController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<CityController> logger) : base(memoryCache, logger, _systemUnitofWork)
        {
        }

        [HttpGet]
        [Route("GetCities")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<EditableCityResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCities(int countryId = 0, int stateId = 0, int page = 0, int pageSize = 0)
        {
            if (stateId <= 0)
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, Constants.State));
            }

            string cachename = GetMethodName() + countryId + stateId + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<EditableCityResponse> cities = (List<EditableCityResponse>)GetFromCache(cachename);
            if (cities == null)
            {
                IQueryable<CityModel> queryable = systemUnitofWork.CityService.GetQueryable(r => true);
                if (countryId > 0)
                {
                    queryable = queryable.Where(r => r.State.CountryId == countryId);
                }
                if (stateId > 0)
                {
                    queryable = queryable.Where(r => r.StateId == stateId);
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


                cities = await queryable.AsNoTracking()
                    .Select(r => new EditableCityResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        CountryId = r.State.CountryId,
                        StateId = r.StateId,
                        Enable = r.Enable
                    })
                    .ToListAsync();

                SavetoCache(cities, cachename);
            }

            return Ok(cities);
        }


        [HttpGet]
        [Route("GetActiveCities")]
        [ProducesResponseType(typeof(List<CityResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveCities(int stateId)
        {
            if (stateId <= 0)
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, Constants.State));
            }

            string cachename = GetMethodName() + stateId;
            List<CityResponse> result = (List<CityResponse>)GetFromCache(cachename);
            if (result == null)
            {
                Expression<Func<CityModel, bool>> where = (r => r.Enable && (r.StateId == stateId));
                IQueryable<CityModel> queryable = GetCityQueryable(0, 0, where);

                result = await queryable.AsNoTracking()
                    .Select(r => new CityResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        private IQueryable<CityModel> GetCityQueryable(int page, int pageSize, Expression<Func<CityModel, bool>> where)
        {
            IQueryable<CityModel> queryable = systemUnitofWork.CityService.GetQueryable(where);
            if (pageSize > 0)
            {
                queryable = queryable.OrderByDescending(r => r.CreatedDate)
                    .Skip(page * pageSize)
                    .Take(pageSize);
            }

            return queryable;
        }


        [HttpPost]
        [Route("CreateCity")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCity([FromBody] NewCityRequest model)
        {
            if (!await systemUnitofWork.StateService.DoesStateExist(model.StateId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.State));
            }

            model.Name = model.Name.Trim();
            CityModel city = new CityModel()
            {
                Name = model.Name,
                StateId = model.StateId,
                Enable = model.Enable,
            };

            systemUnitofWork.CityService.Add(city);

            await systemUnitofWork.SaveAsync();


            ClearCache();

            return Ok(new NewItemResponse<int>(city.Id, string.Format(Constants.ActionResponse, Constants.City, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateCity")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCity([FromBody] ExistingCityRequest model)
        {
            if (!await systemUnitofWork.StateService.DoesStateExist(model.StateId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.State));
            }
            if (await systemUnitofWork.CityService.DoesCityNameExist(model.Name, model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.City));
            }

            if (await systemUnitofWork.CityService.IsCityInUse(model.Id))
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.City, Constants.Update));
            }

            CityModel city = await systemUnitofWork.CityService.GetCity(model.Id);

            if (city == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.City));
            }

            city.Name = model.Name;
            city.StateId = model.StateId;
            city.UpdatedDate = DateTime.Now;

            systemUnitofWork.CityService.Update(city);
            await systemUnitofWork.SaveAsync();

            ClearCache();

            return Ok(string.Format(Constants.ActionResponse, Constants.City, Constants.Updated));
        }

        [HttpPost]
        [Route("DeleteCity")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCity([FromBody] int id)
        {
            if (await systemUnitofWork.CityService.IsCityInUse(id))
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.City, Constants.Update));
            }

            CityModel city = await systemUnitofWork.CityService.GetCity(id);
            if (city == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.City));
            }

            city.ToDeletedEntity();

            systemUnitofWork.CityService.Update(city);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.City, Constants.Deleted));
        }



        [HttpPost]
        [Route("EnableCity")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableCity([FromBody] int id)
        {
            if (!await systemUnitofWork.CityService.DoesCityExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.City));
            }

            CityModel city = await systemUnitofWork.CityService.GetCity(id);
            city.Enable = true;

            systemUnitofWork.CityService.Update(city);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.City, Constants.Enabled));
        }

        [HttpPost]
        [Route("DisableCity")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableCity([FromBody] int id)
        {
            if (!await systemUnitofWork.CityService.DoesCityExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.City));
            }

            CityModel city = await systemUnitofWork.CityService.GetCity(id);
            city.Enable = false;

            systemUnitofWork.CityService.Update(city);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.City, Constants.Disabled));
        }
    }
}
