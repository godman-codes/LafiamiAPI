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
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class FindAPlanController : BaseController<FindAPlanController>
    {
        public const string ControllerName = ControllerConstant.FindAPlan;
        public readonly IBusinessUnitofWork businessUnitofWork;
        public FindAPlanController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<FindAPlanController> logger, IBusinessUnitofWork businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
        }


        [HttpGet]
        [Route("GetFindAPlans")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteFindAPlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFindAPlans(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, page, pageSize);
            List<LiteFindAPlanResponse> liteFindAPlans = (List<LiteFindAPlanResponse>)GetFromCache(cachename);
            if (liteFindAPlans == null)
            {
                IQueryable<FIndAPlanQuestionModel> queryable = businessUnitofWork.FindAPlanService.GetQueryable(r => true);
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r => r.Question.Contains(keyword));
                }

                liteFindAPlans = await queryable.AsNoTracking()
                    .Select(r => new LiteFindAPlanResponse()
                    {
                        Id = r.Id,
                        Question = r.Question,
                        IsActive = r.IsActive,
                        AnswerCount = r.FindAPlanQuestionAnswers.Count()
                    })
                    .ToListAsync();

                liteFindAPlans.ForEach(r => r.Question = WebUtility.HtmlDecode(r.Question));
                SavetoCache(liteFindAPlans, cachename);
            }

            return Ok(liteFindAPlans);
        }



        [HttpGet]
        [Route("GetFindAPlanById")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(FindAPlanResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFindAPlanById(long id)
        {
            string cachename = GetMethodName() + id;
            FindAPlanResponse result = (FindAPlanResponse)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<FIndAPlanQuestionModel> queryable = businessUnitofWork.FindAPlanService.GetQueryable(r => r.Id == id);

                result = await queryable.AsNoTracking()
                    .Select(r => new FindAPlanResponse()
                    {
                        Id = r.Id,
                        Question = r.Question,
                        IsActive = r.IsActive,
                        Note = r.Note,
                        OrderBy = r.OrderBy,
                        HasDependency = r.HasDependency,
                        Answers = r.FindAPlanQuestionAnswers.Select(t => new FindAPlanAnswerResponse()
                        {
                            Id = t.Id,
                            Answer = t.Answer,
                            Explanation = t.Explanation,
                            OrderBy = t.OrderBy,
                            DependentQuestionId = t.DependentQuestionId
                        }).ToList()
                    })
                    .SingleOrDefaultAsync();

                if (result != null)
                {
                    result.Question = WebUtility.HtmlDecode(result.Question);
                    result.Note = WebUtility.HtmlDecode(result.Note);
                    result.Answers.ForEach(r => r.Answer = WebUtility.HtmlDecode(r.Answer));
                    result.Answers.ForEach(r => r.Explanation = WebUtility.HtmlDecode(r.Explanation));
                    SavetoCache(result, cachename);
                }
                else
                {
                    result = new FindAPlanResponse();
                }
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetActiveFindAPlanQuestions")]
        [ProducesResponseType(typeof(List<FindAPlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveFindAPlanQuestions()
        {
            string cachename = GetMethodName();
            List<FindAPlanResponse> result = (List<FindAPlanResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<FIndAPlanQuestionModel> queryable = businessUnitofWork.FindAPlanService.GetQueryable(r => r.IsActive);

                result = await queryable.AsNoTracking()
                    .Select(r => new FindAPlanResponse()
                    {
                        Id = r.Id,
                        IsActive = r.IsActive,
                        Note = r.Note,
                        Question = r.Question,
                        OrderBy = r.OrderBy,
                        HasDependency = r.HasDependency,
                        Answers = r.FindAPlanQuestionAnswers.Select(t => new FindAPlanAnswerResponse()
                        {
                            Answer = t.Answer,
                            Explanation = t.Explanation,
                            Id = t.Id,
                            OrderBy = t.OrderBy,
                            DependentQuestionId = t.DependentQuestionId
                        }).ToList()
                    })
                    .ToListAsync();

                foreach (FindAPlanResponse plan in result)
                {
                    plan.Question = WebUtility.HtmlDecode(plan.Question);
                    plan.Note = WebUtility.HtmlDecode(plan.Note);
                    plan.Answers.ForEach(r => r.Answer = WebUtility.HtmlDecode(r.Answer));
                    plan.Answers.ForEach(r => r.Explanation = WebUtility.HtmlDecode(r.Explanation));
                }
                SavetoCache(result, cachename);
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("GetActiveDependentQuestions")]
        [ProducesResponseType(typeof(List<FindAPlanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveDependentQuestions()
        {
            string cachename = GetMethodName();
            List<FindAPlanResponse> result = (List<FindAPlanResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<FIndAPlanQuestionModel> queryable = businessUnitofWork.FindAPlanService.GetQueryable(r => r.IsActive && r.HasDependency);

                result = await queryable.AsNoTracking()
                    .Select(r => new FindAPlanResponse()
                    {
                        Id = r.Id,
                        IsActive = r.IsActive,
                        Note = r.Note,
                        Question = r.Question,
                        OrderBy = r.OrderBy,
                        Answers = r.FindAPlanQuestionAnswers.Select(t => new FindAPlanAnswerResponse()
                        {
                            Answer = t.Answer,
                            Explanation = t.Explanation,
                            Id = t.Id,
                            OrderBy = t.OrderBy
                        }).ToList()
                    })
                    .ToListAsync();

                foreach (FindAPlanResponse plan in result)
                {
                    plan.Question = WebUtility.HtmlDecode(plan.Question);
                    plan.Note = WebUtility.HtmlDecode(plan.Note);
                    plan.Answers.ForEach(r => r.Answer = WebUtility.HtmlDecode(r.Answer));
                    plan.Answers.ForEach(r => r.Explanation = WebUtility.HtmlDecode(r.Explanation));
                }
                SavetoCache(result, cachename);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateFindAPlanQuestion")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateFindAPlanQuestion([FromBody] NewFIndAPlanRequest model)
        {
            FIndAPlanQuestionModel fIndAPlanQuestion = new FIndAPlanQuestionModel()
            {
                IsActive = model.IsActive,
                Note = WebUtility.HtmlEncode(model.Note),
                OrderBy = model.OrderBy,
                Question = WebUtility.HtmlEncode(model.Question),
                HasDependency = model.HasDependency,
                FindAPlanQuestionAnswers = model.Answers.Select(r => new FindAPlanQuestionAnswerModel()
                {
                    Answer = WebUtility.HtmlEncode(r.Answer),
                    Explanation = WebUtility.HtmlEncode(r.Explanation),
                    OrderBy = r.OrderBy,
                    DependentQuestionId = r.DependentQuestionId
                }).ToList(),
            };

            businessUnitofWork.FindAPlanService.Add(fIndAPlanQuestion);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependent();
            return Ok(new NewItemResponse<int>(fIndAPlanQuestion.Id, string.Format(Constants.ActionResponse, Constants.Question, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateFindAPlanQuestion")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateFindAPlanQuestion([FromBody] ExistingFIndAPlanRequest model)
        {
            FIndAPlanQuestionModel question = await businessUnitofWork.FindAPlanService.GetQueryable(r => r.Id == model.Id).Include(r => r.FindAPlanQuestionAnswers).SingleOrDefaultAsync();
            if (question == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Question));
            }

            question.IsActive = model.IsActive;
            question.Note = WebUtility.HtmlEncode(model.Note);
            question.OrderBy = model.OrderBy;
            question.Question = WebUtility.HtmlEncode(model.Question);
            question.HasDependency = model.HasDependency;
            question.UpdatedDate = DateTime.Now;


            foreach (FindAPlanQuestionAnswerModel answer in question.FindAPlanQuestionAnswers)
            {
                if (!model.Answers.Any(r => r.Id == answer.Id))
                {
                    businessUnitofWork.FindAPlanService.DeleteFindAPlanQuestionAnswer(answer);
                }
            }

            foreach (FindAPlanAnswerRequest answer in model.Answers)
            {
                FindAPlanQuestionAnswerModel dbAnswer = question.FindAPlanQuestionAnswers.FirstOrDefault(r => r.Id == answer.Id);

                if (dbAnswer == null)
                {
                    question.FindAPlanQuestionAnswers.Add(new FindAPlanQuestionAnswerModel()
                    {
                        Answer = WebUtility.HtmlEncode(answer.Answer),
                        Explanation = WebUtility.HtmlEncode(answer.Explanation),
                        OrderBy = answer.OrderBy,
                        DependentQuestionId = answer.DependentQuestionId,
                    });
                }
                else
                {
                    dbAnswer.Answer = WebUtility.HtmlEncode(answer.Answer);
                    dbAnswer.Explanation = WebUtility.HtmlEncode(answer.Explanation);
                    dbAnswer.OrderBy = answer.OrderBy;
                    dbAnswer.DependentQuestionId = answer.DependentQuestionId;
                }
            }


            businessUnitofWork.FindAPlanService.Update(question);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependent();
            return Ok(string.Format(Constants.ActionResponse, Constants.Question, Constants.Updated));
        }

        private void ClearCacheWithDependent()
        {
            ClearCache();
            (new InsurancePlanController(cache, null, null, null)).ClearCache();
        }

        [HttpPost]
        [Route("DeleteFindAPlanQuestion")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFindAPlanQuestion([FromBody] int id)
        {
            if (await businessUnitofWork.FindAPlanService.IsQuestionInUse(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.Question, Constants.Delete));
            }

            FIndAPlanQuestionModel question = await businessUnitofWork.FindAPlanService.GetQueryable(r => r.Id == id).SingleOrDefaultAsync();
            if (question == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Question));
            }

            question.ToDeletedEntity();

            businessUnitofWork.FindAPlanService.Update(question);
            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependent();
            return Ok(string.Format(Constants.ActionResponse, Constants.Question, Constants.Deleted));
        }

        [HttpPost]
        [Route("EnableFindAPlanQuestion")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableFindAPlanQuestion([FromBody] int id)
        {
            if (await businessUnitofWork.FindAPlanService.IsQuestionInUse(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.Question, Constants.Delete));
            }

            FIndAPlanQuestionModel question = await businessUnitofWork.FindAPlanService.GetQueryable(r => r.Id == id).SingleOrDefaultAsync();
            if (question == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Question));
            }

            question.IsActive = true;

            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependent();
            return Ok(string.Format(Constants.ActionResponse, Constants.Question, Constants.Enabled));
        }

        [HttpPost]
        [Route("DisableFindAPlanQuestion")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableFindAPlanQuestion([FromBody] int id)
        {

            if (await businessUnitofWork.FindAPlanService.IsQuestionInUse(id))
            {
                throw new WebsiteException(string.Format(Constants.ItemAlreadyInUseResponse, Constants.Question, Constants.Delete));
            }

            FIndAPlanQuestionModel question = await businessUnitofWork.FindAPlanService.GetQueryable(r => r.Id == id).SingleOrDefaultAsync();
            if (question == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Question));
            }

            question.IsActive = false;

            await businessUnitofWork.SaveAsync();

            ClearCacheWithDependent();
            return Ok(string.Format(Constants.ActionResponse, Constants.Question, Constants.Disabled));
        }
    }
}
