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
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class ReviewController : BaseController<ReviewController>
    {
        public readonly IBusinessUnitofWork businessUnitofWork;
        public ReviewController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<ReviewController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetReviews")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ReviewResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviews(long insurancePlanId, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<ReviewResponse> results = (List<ReviewResponse>)GetFromCache(cachename);
            if (results == null)
            {
                IQueryable<ReviewModel> queryable = businessUnitofWork.ReviewService.GetQueryable((r => (r.InsurancePlanId == insurancePlanId) && (r.Status == StatusEnum.Approve)));

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

                results = await queryable.AsNoTracking()
                    .Select(r => new ReviewResponse()
                    {
                        Id = r.Id,
                        Comment = r.Comment,
                        Name = r.Name,
                        PostedDate = r.CreatedDate
                    })
                    .ToListAsync();

                results.ForEach(r => r.Comment = WebUtility.HtmlDecode(r.Comment));
                SavetoCache(results, cachename);
            }

            return Ok(results);
        }

        [HttpGet]
        [Route("GetPendingReviews")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<FullReviewResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingReviews(int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<FullReviewResponse> results = (List<FullReviewResponse>)GetFromCache(cachename);
            if (results == null)
            {
                IQueryable<ReviewModel> queryable = businessUnitofWork.ReviewService.GetQueryable((r => r.Status == StatusEnum.Pending));

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

                results = await queryable.AsNoTracking()
                    .Select(r => new FullReviewResponse()
                    {
                        Id = r.Id,
                        Comment = r.Comment,
                        Name = r.Name,
                        PostedDate = r.CreatedDate,
                        InsurancePlanName = r.InsurancePlan.Name,
                        Status = r.Status,
                    })
                    .ToListAsync();

                results.ForEach(r => r.Comment = WebUtility.HtmlDecode(r.Comment));
                results.ForEach(r => r.StatusValue = r.Status.DisplayName());
                SavetoCache(results, cachename);
            }

            return Ok(results);
        }


        [HttpPost]
        [Route("CreateReview")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateReview([FromBody] NewReviewRequest model)
        {
            if (!await businessUnitofWork.InsuranceService.DoesInsuranceCompanyExist(model.InsurancePlanId))
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.InsurancePlan));
            }

            ReviewModel review = new ReviewModel()
            {
                Comment = WebUtility.HtmlEncode(model.Comment),
                InsurancePlanId = model.InsurancePlanId,
                Name = model.Name,
                Id = Guid.NewGuid(),
                Status = StatusEnum.Pending
            };

            businessUnitofWork.ReviewService.Add(review);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(review.Id, string.Format(Constants.ActionResponse, Constants.Review, Constants.Submitted)));
        }

        [HttpPost]
        [Route("DeleteReview")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteReview([FromBody] IdRequest<Guid> model)
        {
            ReviewModel review = await businessUnitofWork.ReviewService.GetQueryable(r => r.Id == model.Id).SingleOrDefaultAsync();
            if (review == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Review));
            }

            if (review.Status != StatusEnum.Pending)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Review, Constants.Delete));
            }

            review.ToDeletedEntity();

            businessUnitofWork.ReviewService.Update(review);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Review, Constants.Deleted));
        }

        [HttpPost]
        [Route("ApproveReview")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveReview([FromBody] IdRequest<Guid> model)
        {
            ReviewModel review = await businessUnitofWork.ReviewService.GetQueryable(r => r.Id == model.Id).SingleOrDefaultAsync();

            if (review == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Review));
            }

            review.Status = StatusEnum.Approve;
            review.UpdatedDate = DateTime.Now;

            businessUnitofWork.ReviewService.Update(review);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Review, Constants.Deleted));
        }


        [HttpPost]
        [Route("RejectReview")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectReview([FromBody] IdRequest<Guid> model)
        {
            ReviewModel review = await businessUnitofWork.ReviewService.GetQueryable(r => r.Id == model.Id).SingleOrDefaultAsync();

            if (review == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Review));
            }

            review.Status = StatusEnum.Reject;
            review.UpdatedDate = DateTime.Now;

            businessUnitofWork.ReviewService.Update(review);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Review, Constants.Deleted));
        }

    }
}
