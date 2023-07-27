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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class StateController : BaseController<StateController>
    {
        public const string ControllerName = ControllerConstant.State;
        public StateController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<StateController> logger) : base(memoryCache, logger, _systemUnitofWork)
        {
        }

        [HttpGet]
        [Route("GetStates")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<EditableStateResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStates(int countryId, int page = 0, int pageSize = 0)
        {
            if (countryId <= 0)
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, Constants.Country));
            }

            string cachename = GetMethodName() + countryId + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<EditableStateResponse> states = (List<EditableStateResponse>)GetFromCache(cachename);
            if (states == null)
            {
                Expression<Func<StateModel, bool>> where = (r => r.CountryId == countryId);
                IQueryable<StateModel> queryable = systemUnitofWork.StateService.GetStateQueryable(page, pageSize, where);

                states = await queryable.AsNoTracking()
                    .Select(r => new EditableStateResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        CountryId = r.CountryId,
                        Enable = r.Enable
                    })
                    .ToListAsync();

                SavetoCache(states, cachename);
            }

            return Ok(states);
        }



        [HttpGet]
        [Route("GetActiveStates")]
        [ProducesResponseType(typeof(List<StateResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveStates(int countryId)
        {
            if (countryId <= 0)
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, Constants.Country));
            }

            string cachename = GetMethodName() + countryId;
            List<StateResponse> result = (List<StateResponse>)GetFromCache(cachename);
            if (result == null)
            {
                Expression<Func<StateModel, bool>> where = (r => r.Enable && (r.CountryId == countryId));
                IQueryable<StateModel> queryable = systemUnitofWork.StateService.GetStateQueryable(0, 0, where);

                result = await queryable.AsNoTracking()
                    .Select(r => new StateResponse()
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("CreateState")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateState([FromBody] NewStateRequest model)
        {
            if (!await systemUnitofWork.CountryService.DoesCountryExist(model.CountryId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Country));
            }

            model.Name = model.Name.Trim();
            StateModel state = new StateModel()
            {
                Name = model.Name,
                CountryId = model.CountryId,
                Enable = model.Enable,
            };

            systemUnitofWork.StateService.Add(state);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<int>(state.Id, string.Format(Constants.ActionResponse, Constants.State, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateState")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateState([FromBody] ExistingStateRequest model)
        {
            if (!await systemUnitofWork.CountryService.DoesCountryExist(model.Id))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Country));
            }
            if (await systemUnitofWork.StateService.DoesStateNameExist(model.Name, model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.State));
            }

            if (await systemUnitofWork.StateService.IsStateInUse(model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.State, Constants.Update));
            }


            StateModel state = await systemUnitofWork.StateService.GetState(model.Id);
            if (state == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.State));
            }

            state.Name = model.Name;
            state.CountryId = model.CountryId;
            state.UpdatedDate = DateTime.Now;

            systemUnitofWork.StateService.Update(state);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.State, Constants.Updated));
        }

        [HttpPost]
        [Route("DeleteState")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteState([FromBody][Required] int id)
        {
            //if (await systemUnitofWork.StateService.IsStateInUse(id))
            //{
            //    throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.State, Constants.Delete));
            //}

            StateModel state = await systemUnitofWork.StateService.GetState(id);
            if (state == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.State));
            }

            state.ToDeletedEntity();

            systemUnitofWork.StateService.Update(state);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.State, Constants.Deleted));
        }

        [HttpPost]
        [Route("EnableState")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableState([FromBody] int id)
        {
            if (!await systemUnitofWork.StateService.DoesStateExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.State));
            }



            StateModel state = await systemUnitofWork.StateService.GetState(id);
            state.Enable = true;

            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.State, Constants.Enabled));
        }

        [HttpPost]
        [Route("DisableState")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableState([FromBody] int id)
        {
            if (!await systemUnitofWork.StateService.DoesStateExist(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.State));
            }

            StateModel state = await systemUnitofWork.StateService.GetState(id);
            state.Enable = false;

            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.State, Constants.Disabled));
        }
    }
}
