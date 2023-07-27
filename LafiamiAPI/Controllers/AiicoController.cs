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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class AiicoController : BaseController<AiicoController>
    {
        public readonly IWebAPI webAPI;
        public readonly IBusinessUnitofWork businessUnitofWork;
        public AiicoController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<AiicoController> logger, IBusinessUnitofWork _businessUnitofWork, IWebAPI _webAPI) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
            webAPI = _webAPI;
        }

        [HttpGet]
        [Route("GetTitles")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTitles()
        {
            string cachename = GetMethodName();
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                AiicoBasicResponse _result = await webAPI.Get<AiicoBasicResponse>("UtilitiyService/GetTitles", CompanyEnum.Aiico);
                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.id,
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetGenders")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenders()
        {
            string cachename = GetMethodName();
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                AiicoBasicResponse _result = await webAPI.Get<AiicoBasicResponse>("UtilitiyService/GetGenders", CompanyEnum.Aiico);
                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.id,
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetLGAs")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLGAs()
        {
            string cachename = GetMethodName();
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                AiicoBasicResponse _result = await webAPI.Get<AiicoBasicResponse>("UtilitiyService/GetAllLgas", CompanyEnum.Aiico);
                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.id,
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetHospitals")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitals(string lgaId)
        {
            string cachename = GenerateCacheName(GetMethodName(), lgaId);
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                string queryPath = CreateUrlQuery(nameof(lgaId).EqualJoin(lgaId));
                AiicoBasicResponse _result = await webAPI.Get<AiicoBasicResponse>("UtilitiyService/GetHospitalsForLga" + queryPath, CompanyEnum.Aiico);
                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.id,
                    Name = r.name
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetProducts")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            string cachename = GetMethodName();
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                AiicoBasicResponse _result = await webAPI.Get<AiicoBasicResponse>("ProductService/GetProducts", CompanyEnum.Aiico);
                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.id,
                    Name = r.name
                }).ToList();
                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetProductSubClassCoverTypes")]
        [ProducesResponseType(typeof(List<AiicoIdNameResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductSubClassCoverTypes(string productId)
        {
            string cachename = GenerateCacheName(GetMethodName(), productId);
            List<AiicoIdNameResponse> result = (List<AiicoIdNameResponse>)GetFromCache(cachename);
            if (result == null)
            {
                string queryPath = CreateUrlQuery(nameof(productId).EqualJoin(productId));
                AiicoBasicCoverResponse _result = await webAPI.Get<AiicoBasicCoverResponse>("ProductService/GetProductSubClassCoverTypes" + queryPath, CompanyEnum.Aiico);

                result = _result.result.Select(r => new AiicoIdNameResponse()
                {
                    Id = r.subClassCoverTypes.id,
                    Name = r.subClassCoverTypes.coverTypeName
                }).ToList();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

    }
}
