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
    public class CountryController : BaseController<CountryController>
    {
        public const string ControllerName = ControllerConstant.Country;
        public CountryController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<CountryController> logger) : base(memoryCache, logger, _systemUnitofWork)
        {
        }

        [HttpGet]
        [Route("GetCountries")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<EditableCountryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCountries(int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + page + Constants.Underscore + pageSize;
            List<EditableCountryResponse> countries = (List<EditableCountryResponse>)GetFromCache(cachename);
            if (countries == null)
            {
                Expression<Func<CountryModel, bool>> where = (r => true);
                IQueryable<CountryModel> queryable = systemUnitofWork.CountryService.GetCountryQueryable(page, pageSize, where);

                countries = await queryable.AsNoTracking()
                    .Select(r => new EditableCountryResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ThreeLetterIsoCode = r.ThreeLetterIsoCode,
                        TwoLetterIsoCode = r.TwoLetterIsoCode,
                        Enable = r.Enable
                    })
                    .ToListAsync();

                SavetoCache(countries, cachename);
            }

            return Ok(countries);
        }


        [HttpGet]
        [Route("GetActiveCountries")]
        [ProducesResponseType(typeof(List<CountryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveCountries()
        {
            string cachename = GetMethodName();
            List<CountryResponse> result = (List<CountryResponse>)GetFromCache(cachename);
            if (result == null)
            {
                Expression<Func<CountryModel, bool>> where = (r => r.Enable);
                IQueryable<CountryModel> queryable = systemUnitofWork.CountryService.GetCountryQueryable(0, 0, where);

                result = await queryable.AsNoTracking()
                    .Select(r => new CountryResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ThreeLetterIsoCode = r.ThreeLetterIsoCode,
                        TwoLetterIsoCode = r.TwoLetterIsoCode
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateCountry")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCountry([FromBody] NewCountryRequest model)
        {
            model.Name = model.Name.Trim();

            CountryModel country = new CountryModel()
            {
                Name = model.Name,
                ThreeLetterIsoCode = model.ThreeLetterIsoCode,
                TwoLetterIsoCode = model.TwoLetterIsoCode,
                Enable = model.Enable,
            };

            systemUnitofWork.CountryService.Add(country);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<int>(country.Id, string.Format(Constants.ActionResponse, Constants.Country, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateCountry")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCountry([FromBody] ExistingCountryRequest model)
        {
            if (await systemUnitofWork.CountryService.DoesCountryNameExist(model.Name, model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.Country));
            }

            if (await systemUnitofWork.CountryService.IsCountryInUse(model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.Country, Constants.Update));
            }

            CountryModel country = await systemUnitofWork.CountryService.GetCountry(model.Id);
            if (country == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Country));
            }

            country.Name = model.Name;
            country.ThreeLetterIsoCode = model.ThreeLetterIsoCode;
            country.TwoLetterIsoCode = model.TwoLetterIsoCode;
            country.UpdatedDate = DateTime.Now;

            systemUnitofWork.CountryService.Update(country);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Country, Constants.Updated));
        }

        [HttpPost]
        [Route("DeleteCountry")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCountry([FromBody] int id)
        {
            if (await systemUnitofWork.CountryService.IsCountryInUse(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.Country, Constants.Delete));
            }

            CountryModel country = await systemUnitofWork.CountryService.GetCountry(id);
            if (country == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Country));
            }

            country.ToDeletedEntity();

            systemUnitofWork.CountryService.Update(country);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Country, Constants.Deleted));
        }

        [HttpPost]
        [Route("EnableCountry")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableCountry([FromBody] int id)
        {
            if (!await systemUnitofWork.CountryService.DoesCountryExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Country));
            }

            CountryModel country = await systemUnitofWork.CountryService.GetCountry(id);
            country.Enable = true;

            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Country, Constants.Enabled));
        }

        [HttpPost]
        [Route("DisableCountry")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableCountry([FromBody] int id)
        {
            if (!await systemUnitofWork.CountryService.DoesCountryExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Country));
            }

            CountryModel country = await systemUnitofWork.CountryService.GetCountry(id);
            country.Enable = false;

            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Country, Constants.Disabled));
        }

    }
}
