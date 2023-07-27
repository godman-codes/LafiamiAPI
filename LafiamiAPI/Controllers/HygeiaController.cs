using LafiamiAPI.Interfaces;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class HygeiaController : BaseController<HygeiaController>
    {
        public readonly IWebAPI webAPI;
        public readonly IBusinessUnitofWork businessUnitofWork;
        public HygeiaController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<HygeiaController> logger, IBusinessUnitofWork _businessUnitofWork, IWebAPI _webAPI) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
            webAPI = _webAPI;
        }

        [HttpGet]
        [Route("GetHygeiaPlans")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public IActionResult GetHygeiaPlans()
        {
            List<HygeiaIdNameResponse> result = Enum.GetValues(typeof(HygeiaPlanEnum)).Cast<HygeiaPlanEnum>()
                .Select(r => new HygeiaIdNameResponse()
                {
                    Id = ((int)r).ToString(),
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetTitles")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTitles()
        {
            string cachename = GetMethodName();
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaRawIdNameResponse> _result = await webAPI.Get<List<HygeiaRawIdNameResponse>>("tpi-ms/api/reg-titles", CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.id.ToString(),
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMaritalStatuses")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaritalStatuses()
        {
            string cachename = GetMethodName();
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaRawIdNameResponse> _result = await webAPI.Get<List<HygeiaRawIdNameResponse>>("tpi-ms/api/reg-maritalstatus", CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.id.ToString(),
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetConvergeClasses")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConvergeClasses()
        {
            string cachename = GetMethodName();
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaRawIdNameResponse> _result = await webAPI.Get<List<HygeiaRawIdNameResponse>>("tpi-ms/api/CoverageClass/GetCoverageClass", CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.id.ToString(),
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetDependentTypes")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDependentTypes()
        {
            string cachename = GetMethodName();
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaRawIdNameResponse> _result = await webAPI.Get<List<HygeiaRawIdNameResponse>>("tpi-ms/api/Dept/DeptTypes", CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.id.ToString(),
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetStates")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStates()
        {
            string cachename = GetMethodName();
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaStateResponse> _result = await webAPI.Get<List<HygeiaStateResponse>>("hygeiaapiservice/api/Services/States", CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.stateid.ToString(),
                    Name = r.statename
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetLGAs")]
        [ProducesResponseType(typeof(List<HygeiaIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLGAs(int stateId)
        {
            string cachename = GetMethodName() + stateId;
            List<HygeiaIdNameResponse> result = (List<HygeiaIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                List<HygeiaLGAResponse> _result = await webAPI.Get<List<HygeiaLGAResponse>>("hygeiaapiservice/api/Services/Cities/" + stateId, CompanyEnum.Hygeia);
                result = _result.Select(r => new HygeiaIdNameResponse()
                {
                    Id = r.cityId.ToString(),
                    Name = r.cityName
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }


    }
}
