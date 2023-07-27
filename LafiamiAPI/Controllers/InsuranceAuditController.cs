using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class InsuranceAuditController : BaseController<InsuranceAuditController>
    {
        //public const string ControllerName = ControllerConstant.InsurancePlan;
        public readonly IBusinessUnitofWork businessUnitofWork;
        public InsuranceAuditController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<InsuranceAuditController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetInsuranceSearchAudits")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<InsuranceSearchAuditResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsuranceSearchAudits(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, page, pageSize);
            List<InsuranceSearchAuditResponse> result = (List<InsuranceSearchAuditResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<InsuranceAuditModel> queryable = businessUnitofWork.InsuranceService.GetInsuranceAuditsQueryable();
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r => r.Keyword.Contains(keyword));
                    queryable = queryable.Where(r => r.InsuranceAuditCategories.Any(t => t.Category.Name.Contains(keyword)));
                    queryable = queryable.Where(r => r.InsuranceAuditQuestionAnswers.Any(t => t.FindAPlanQuestionAnswer.Answer.Contains(keyword)));
                }

                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }
                result = new List<InsuranceSearchAuditResponse>();

                var _results = await queryable.AsNoTracking()
                    .Select(r => new
                    {
                        r.Id,
                        r.Action,
                        FullName = (r.User != null) ? (r.User.Surname + Constants.Space + r.User.Firstname) : (string.Empty),
                        r.HasResult,
                        r.ResultCount,
                        r.Keyword,
                        Answers = r.InsuranceAuditQuestionAnswers.Select(r => r.FindAPlanQuestionAnswer.Answer).ToList(),
                        Categories = r.InsuranceAuditCategories.Select(r => r.Category.Name).ToList()
                    })
                    .ToListAsync();

                if (_results == null)
                {
                    result = new List<InsuranceSearchAuditResponse>();
                }
                else
                {
                    foreach (var r in _results)
                    {
                        string answers = string.Join(Constants.Comma, r.Answers);
                        string categories = string.Join(Constants.Comma, r.Categories);

                        string finalsearch = string.Empty;
                        if (!string.IsNullOrEmpty(r.Keyword))
                        {
                            finalsearch = ((string.IsNullOrEmpty(finalsearch)) ? (r.Keyword) : (string.Join(Constants.Comma, finalsearch, r.Keyword)));
                        }

                        if (!string.IsNullOrEmpty(answers))
                        {
                            finalsearch = ((string.IsNullOrEmpty(finalsearch)) ? (answers) : (string.Join(Constants.Comma, finalsearch, answers)));
                        }

                        if (!string.IsNullOrEmpty(categories))
                        {
                            finalsearch = ((string.IsNullOrEmpty(finalsearch)) ? (categories) : (string.Join(Constants.Comma, finalsearch, categories)));
                        }

                        result.Add(new InsuranceSearchAuditResponse()
                        {
                            Action = r.Action,
                            FullName = r.FullName,
                            HasResult = r.HasResult,
                            ResultCount = r.ResultCount,
                            SearchValues = finalsearch
                        });
                    }
                    SavetoCache(result, cachename);
                }
            }


            return Ok(result);
        }


    }
}
