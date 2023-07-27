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
    public class CategoryController : BaseController<CategoryController>
    {
        public readonly IBusinessUnitofWork businessUnitofWork;
        public CategoryController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<CategoryController> logger, IBusinessUnitofWork _businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            businessUnitofWork = _businessUnitofWork;
        }

        [HttpGet]
        [Route("GetCategories")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteCategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories(int parentCategoryId = 0, bool isVariationCategory = false)
        {
            string cachename = GetMethodName() + parentCategoryId + Constants.Underscore + isVariationCategory;
            List<LiteCategoryResponse> result = (List<LiteCategoryResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<CategoryModel> queryable = businessUnitofWork.CategoryService.GetQueryable((r => r.UseforVariation == isVariationCategory));
                if (parentCategoryId > 0)
                {
                    queryable = queryable.Where(r => r.ParentId == parentCategoryId);
                }
                else
                {
                    queryable = queryable.Where(r => !r.ParentId.HasValue || (r.ParentId == 0));
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteCategoryResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        //ShowInMenu = r.ShowInMenu,
                        ImageURL = r.ImageURL,
                        ParentId = r.ParentId ?? 0,
                        OrderBy = r.OrderBy,
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LiteCategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories(bool isVariationCategory = false)
        {
            string cachename = GetMethodName() + Constants.Underscore + isVariationCategory;
            List<LiteCategoryResponse> result = (List<LiteCategoryResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<CategoryModel> queryable = businessUnitofWork.CategoryService.GetQueryable((r => r.UseforVariation == isVariationCategory));

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteCategoryResponse()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ImageURL = r.ImageURL,
                        ParentId = r.ParentId ?? 0,
                        OrderBy = r.OrderBy,
                        UseAsFilter = r.UseAsFilter,
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }


        [HttpGet]
        [Route("GetCategoryById")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            string cachename = GetMethodName() + id;
            CategoryResponse result = (CategoryResponse)GetFromCache(cachename);
            if (result == null)
            {
                CategoryModel category = await businessUnitofWork.CategoryService.GetCategory(id);

                result = new CategoryResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = WebUtility.HtmlDecode(category.Description),
                    DefaultVariations = WebUtility.HtmlDecode(category.DefaultVariations),
                    IconURl = category.IconURl,
                    ImageURL = category.ImageURL,
                    OrderBy = category.OrderBy,
                    ParentId = category.ParentId,
                    UseAsFilter = category.UseAsFilter,
                    UseforVariation = category.UseforVariation
                };

                SavetoCache(result, cachename);
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("GetSubCategoryCountById")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubCategoryCountById(int parentId)
        {
            string cachename = GetMethodName() + parentId;
            int? result = (int?)GetFromCache(cachename);
            if (!result.HasValue || (result <= 0))
            {
                result = await businessUnitofWork.CategoryService.GetSubCategoryCount(parentId);

                if (result > 0)
                {
                    SavetoCache(result, cachename);
                }
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateCategory")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCategory([FromBody] NewCategoryRequest model)
        {
            if (model.ParentId.HasValue && (model.ParentId > 0) && !await businessUnitofWork.CategoryService.DoesCategoryExist(model.ParentId.Value))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.ParentCategory));
            }
            if (await businessUnitofWork.CategoryService.IsNameInUse(model.Name))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.Category));
            }

            CategoryModel category = new CategoryModel()
            {
                DefaultVariations = WebUtility.HtmlEncode(model.DefaultVariations),
                Name = model.Name,
                Description = WebUtility.HtmlEncode(model.Description),
                IconURl = model.IconURl,
                ImageURL = model.ImageURL,
                ParentId = ((model.ParentId == 0) ? null : (model.ParentId)),
                OrderBy = model.OrderBy,
                UseAsFilter = model.UseAsFilter,
                UseforVariation = model.UseforVariation,
            };

            businessUnitofWork.CategoryService.Add(category);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(new NewItemResponse<long>(category.Id, string.Format(Constants.ActionResponse, Constants.Category, Constants.Created)));
        }

        [HttpPost]
        [Route("UpdateCategory")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCategory([FromBody] ExistingCategoryRequest model)
        {
            if (model.ParentId.HasValue && (model.ParentId > 0) && !await businessUnitofWork.CategoryService.DoesCategoryExist(model.ParentId.Value))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.ParentCategory));
            }
            if (await businessUnitofWork.CategoryService.IsNameInUse(model.Name, model.Id))
            {
                throw new WebsiteException(string.Format(Constants.ItemNameInUseResponse, Constants.Category));
            }

            CategoryModel category = await businessUnitofWork.CategoryService.GetCategory(model.Id);

            category.DefaultVariations = WebUtility.HtmlEncode(model.DefaultVariations);
            category.Name = model.Name;
            category.Description = WebUtility.HtmlEncode(model.Description);
            category.IconURl = model.IconURl;
            category.ImageURL = model.ImageURL;
            category.ParentId = ((model.ParentId == 0) ? null : (model.ParentId));
            category.OrderBy = model.OrderBy;
            category.UseAsFilter = model.UseAsFilter;
            category.UseforVariation = model.UseforVariation;
            category.UpdatedDate = DateTime.Now;

            businessUnitofWork.CategoryService.Update(category);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Category, Constants.Updated));
        }

        [HttpPost]
        [Route("UpdateCategoryPosition")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCategoryPosition([FromBody] CategoryPositionRequest model)
        {
            CategoryModel category = await businessUnitofWork.CategoryService.GetCategory(model.Id);

            category.OrderBy = model.OrderBy;
            category.UpdatedDate = DateTime.Now;

            businessUnitofWork.CategoryService.Update(category);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Category, Constants.Updated));
        }

        [HttpPost]
        [Route("DeleteCategory")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCategory([FromBody] int id)
        {
            CategoryModel category = await businessUnitofWork.CategoryService.GetCategory(id);
            if (category == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Category));
            }

            category.ToDeletedEntity();

            businessUnitofWork.CategoryService.Update(category);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new InsurancePlanController(cache, systemUnitofWork, null, businessUnitofWork)).ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Category, Constants.Deleted));
        }
    }
}
