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
    public class VisitController : BaseController<VisitController>
    {
        public readonly IBusinessUnitofWork businessUnitofWork;
        public VisitController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<VisitController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetPageTypes")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPageTypes()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(PageTypeEnum)).Cast<PageTypeEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPeriods")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPeriods()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(PeriodEnum)).Cast<PeriodEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetVisitSummaries")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<VisitSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVisitSummaries(PeriodEnum period, int dayLength = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), period, dayLength);
            List<VisitSummaryResponse> result = (List<VisitSummaryResponse>)GetFromCache(cachename);

            if (result == null)
            {
                result = new List<VisitSummaryResponse>();
                if (dayLength > 0)
                {
                    DateTime today = DateTime.Today;
                    DateTime startDate;
                    DateTime endDate;
                    for (int i = 0; i < dayLength; i++)
                    {
                        VisitSummaryResponse visit = new VisitSummaryResponse()
                        {
                            OrderBy = i
                        };

                        switch (period)
                        {
                            case PeriodEnum.Daily:
                                visit.Date = today.AddDays(0 - i);
                                startDate = today.AddDays(0 - i);//Start of the day
                                endDate = startDate.AddDays(1).AddSeconds(0 - 1);//End of the day
                                break;
                            case PeriodEnum.Weekly:
                                startDate = today.AddDays(0 - ((int)today.DayOfWeek)); //Start of the Week
                                endDate = startDate.AddDays(6);//End of the Week
                                visit.Week = string.Join(Constants.Space, Constants.From, startDate.ToString(Constants.DisplayDateFormatMMMddyyyy), Constants.To, endDate.ToString(Constants.DisplayDateFormatMMMddyyyy));
                                break;
                            case PeriodEnum.Monthly:
                                startDate = new DateTime(today.Year, today.Month, 1);//Start of the Month
                                endDate = startDate.AddMonths(1).AddDays(-1);//End of the Month
                                visit.Month = startDate.ToString(Constants.DisplayMMMyyyy);
                                break;
                            case PeriodEnum.Quaterly:
                                startDate = new DateTime(today.Year, today.Month, 1);//Start of the Quater
                                endDate = startDate.AddMonths(3).AddDays(-1);//End of the Quater
                                visit.QuaterName = string.Join(Constants.Space, Constants.From, startDate.ToString(Constants.DisplayMMMyyyy), Constants.To, endDate.ToString(Constants.DisplayMMMyyyy));
                                break;
                            case PeriodEnum.Yearly:
                                startDate = new DateTime(today.Year, 1, 1);//Start of the Year
                                endDate = startDate.AddMonths(1).AddDays(-1);//End of the Year
                                break;
                            default:
                                visit.Date = today.AddDays(0 - i);
                                startDate = today.AddDays(0 - i);//Start of the day
                                endDate = startDate.AddDays(1).AddSeconds(0 - 1);//End of the day
                                visit.Year = startDate.ToString(Constants.Displayyyyy);
                                break;
                        }

                        visit.UserCount = await systemUnitofWork.VisitAuditService.GetUserCount(startDate, endDate);
                        visit.UniqueUserCount = await systemUnitofWork.VisitAuditService.GetUserCount(startDate, endDate, true);
                        visit.UserBounceRate = decimal.Multiply(decimal.Divide((await systemUnitofWork.VisitAuditService.GetUserBounceCount(startDate, endDate)), visit.UserCount), 100);
                        visit.UniqueUserBounceRate = decimal.Multiply(decimal.Divide((await systemUnitofWork.VisitAuditService.GetUserBounceCount(startDate, endDate, true)), visit.UniqueUserCount), 100);

                        result.Add(visit);
                    }
                }

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetInsurancePlanVisitSummaries")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<PlanSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurancePlanVisitSummaries(PeriodEnum period, int topXPlans, int dayLength = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), period, topXPlans, dayLength);
            List<PlanSummaryResponse> result = (List<PlanSummaryResponse>)GetFromCache(cachename);

            if (result == null)
            {
                result = new List<PlanSummaryResponse>();
                if (dayLength > 0)
                {
                    DateTime today = DateTime.Today;
                    DateTime startDate;
                    DateTime endDate;
                    for (int i = 0; i < dayLength; i++)
                    {
                        PlanSummaryResponse visit = new PlanSummaryResponse()
                        {
                            OrderBy = i
                        };

                        switch (period)
                        {
                            case PeriodEnum.Daily:
                                visit.Date = today.AddDays(0 - i);
                                startDate = today.AddDays(0 - i);//Start of the day
                                endDate = startDate.AddDays(1).AddSeconds(0 - 1);//End of the day
                                break;
                            case PeriodEnum.Weekly:
                                startDate = today.AddDays(0 - ((int)today.DayOfWeek)); //Start of the Week
                                endDate = startDate.AddDays(6);//End of the Week
                                visit.Week = string.Join(Constants.Space, Constants.From, startDate.ToString(Constants.DisplayDateFormatMMMddyyyy), Constants.To, endDate.ToString(Constants.DisplayDateFormatMMMddyyyy));
                                break;
                            case PeriodEnum.Monthly:
                                startDate = new DateTime(today.Year, today.Month, 1);//Start of the Month
                                endDate = startDate.AddMonths(1).AddDays(-1);//End of the Month
                                visit.Month = startDate.ToString(Constants.DisplayMMMyyyy);
                                break;
                            case PeriodEnum.Quaterly:
                                startDate = new DateTime(today.Year, today.Month, 1);//Start of the Quater
                                endDate = startDate.AddMonths(3).AddDays(-1);//End of the Quater
                                visit.QuaterName = string.Join(Constants.Space, Constants.From, startDate.ToString(Constants.DisplayMMMyyyy), Constants.To, endDate.ToString(Constants.DisplayMMMyyyy));
                                break;
                            case PeriodEnum.Yearly:
                                startDate = new DateTime(today.Year, 1, 1);//Start of the Year
                                endDate = startDate.AddMonths(1).AddDays(-1);//End of the Year
                                break;
                            default:
                                visit.Date = today.AddDays(0 - i);
                                startDate = today.AddDays(0 - i);//Start of the day
                                endDate = startDate.AddDays(1).AddSeconds(0 - 1);//End of the day
                                visit.Year = startDate.ToString(Constants.Displayyyyy);
                                break;
                        }

                        visit.PlanCounts = await systemUnitofWork.VisitAuditService.GetPlanUserCount(startDate, endDate, topXPlans);
                        result.Add(visit);
                    }
                }

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpPost]
        [Route("LogVisit")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LogVisit([FromBody] NewVisitRequest model)
        {
            if (model.InsurancePlanId.HasValue && !await businessUnitofWork.InsuranceService.DoesInsurancePlanExist(model.InsurancePlanId.Value))
            {
                model.PageType = PageTypeEnum.UnknownPage;
                model.InsurancePlanId = null;
            }

            VisitModel visit = new VisitModel()
            {
                Id = Guid.NewGuid(),
                InsurancePlanId = model.InsurancePlanId,
                IPAddress = model.IPAddress,
                PageType = model.PageType,
            };

            string userId = GetUserId();
            UserSessionVisitModel userSessionVisit = await systemUnitofWork.VisitAuditService.GetTodayUserSessionVisitAsync(model, userId);
            if (userSessionVisit == null)
            {
                visit.UserSessionVisit = new UserSessionVisitModel()
                {
                    Id = Guid.NewGuid(),
                    SessionId = model.SessionId,
                    SessionVisitCount = 1,
                    SystemUserId = model.SystemUserId,
                    UserId = userId,
                };
            }
            else
            {
                userSessionVisit.SessionVisitCount = userSessionVisit.SessionVisitCount + 1;
                visit.UserSessionVisitId = userSessionVisit.Id;
            }

            if (model.InsurancePlanId.HasValue)
            {
                PlanVisitModel planVisit = await systemUnitofWork.VisitAuditService.GetTodayPlanVisitAsync(model.InsurancePlanId.Value, model.SessionId);
                if (planVisit == null)
                {
                    visit.PlanVisit = new PlanVisitModel()
                    {
                        Id = Guid.NewGuid(),
                        InsurancePlanId = model.InsurancePlanId.Value,
                        VisitCount = 1,
                    };
                }
                else
                {
                    planVisit.VisitCount = planVisit.VisitCount + 1;
                    visit.PlanVisitId = planVisit.Id;
                }
            }

            systemUnitofWork.VisitAuditService.Add(visit);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(visit.Id, string.Format(Constants.ActionResponse, Constants.Visit, Constants.Logged)));
        }

        [HttpPost]
        [Route("LogPlanForComparison")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LogPlanForComparison([FromBody] NewPlanForComparisonRequest model)
        {
            if (!await businessUnitofWork.InsuranceService.DoesInsurancePlanExist(model.InsurancePlanId))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.InsurancePlan));
            }

            CompareLogModel compareLog = new CompareLogModel()
            {
                Id = Guid.NewGuid(),
                InsurancePlanId = model.InsurancePlanId,
                IsUseInComparison = model.IsUseInComparison,
                VisitId = model.VisitId
            };

            systemUnitofWork.VisitAuditService.AddCompareLog(compareLog);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(compareLog.Id, string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Logged)));
        }


    }
}
