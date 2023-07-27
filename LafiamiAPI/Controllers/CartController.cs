using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
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
    public class CartController : BaseController<CartController>
    {
        public const string ControllerName = ControllerConstant.Cart;
        private readonly IBusinessUnitofWork businessUnitofWork;
        public CartController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<CartController> logger, IBusinessUnitofWork businessUnitofWork) : base(memoryCache, logger, _systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
        }

        [HttpGet]
        [Route("GetCarts")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<CartResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCarts(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + (string.IsNullOrEmpty(keyword) ? string.Empty : keyword) + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<CartResponse> carts = (List<CartResponse>)GetFromCache(cachename);
            if (carts == null)
            {
                IQueryable<CartModel> queryable = businessUnitofWork.CartService.GetQueryable((r => !r.OrderId.HasValue));

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.InsurancePlan.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                    );
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

                carts = await queryable.AsNoTracking()
                    .Select(r => new CartResponse()
                    {
                        Id = r.Id,
                        InsurancePlanId = r.InsurancePlanId,
                        ItemName = (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Name) : (r.ItemName),
                        UnitAmount = r.Amount,
                        Picture = (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Thumbnail) : (string.Empty),
                        UserId = r.UserId,
                        Email = ((r.UserId != null) ? (r.User.Email) : (string.Empty)),
                        Phone = ((r.UserId != null) ? (r.User.PhoneNumber) : (string.Empty)),
                        Firstname = ((r.UserId != null) ? (r.User.Firstname) : (string.Empty)),
                        Surname = ((r.UserId != null) ? (r.User.Surname) : (string.Empty)),
                        QuatityOrder = r.QuatityOrder,
                    })
                    .ToListAsync();

                foreach (CartResponse cart in carts)
                {
                    if (cart.InsurancePlanId.HasValue)
                    {
                        AmountObjectModel amountObject = await businessUnitofWork.InsuranceService.GetAmount(cart.InsurancePlanId.Value, null, 1, carts.Sum(r => r.QuatityOrder));
                        cart.UnitAmount = amountObject.Amount;
                        cart.MoneyUnit = amountObject.MoneyUnit;
                    }
                }

                SavetoCache(carts, cachename);
            }


            return Ok(carts);
        }

        [HttpGet]
        [Route("GetCartById")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartById(Guid id)
        {
            string cachename = GetMethodName() + id;
            CartResponse cart = (CartResponse)GetFromCache(cachename);
            if (cart == null)
            {
                IQueryable<CartModel> queryable = businessUnitofWork.CartService.GetQueryable((r => r.Id == id));

                cart = await queryable.AsNoTracking()
                    .Select(r => new CartResponse()
                    {
                        Id = r.Id,
                        InsurancePlanId = r.InsurancePlanId,
                        ItemName = (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Name) : (r.ItemName),
                        UnitAmount = r.Amount,
                        Picture = (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Thumbnail) : (string.Empty),
                        UserId = r.UserId,
                        Email = ((r.UserId != null) ? (r.User.Email) : (string.Empty)),
                        Phone = ((r.UserId != null) ? (r.User.PhoneNumber) : (string.Empty)),
                        Firstname = ((r.UserId != null) ? (r.User.Firstname) : (string.Empty)),
                        Surname = ((r.UserId != null) ? (r.User.Surname) : (string.Empty)),
                        QuatityOrder = r.QuatityOrder,
                    })
                    .SingleOrDefaultAsync();

                if (cart != null)
                {
                    if (cart.InsurancePlanId.HasValue)
                    {
                        AmountObjectModel amountObject = await businessUnitofWork.InsuranceService.GetAmount(cart.InsurancePlanId.Value, null, 1, 0);
                        cart.UnitAmount = amountObject.Amount;
                        cart.MoneyUnit = amountObject.MoneyUnit;
                    }
                    SavetoCache(cart, cachename);
                }
                else
                {
                    cart = new CartResponse();
                }
            }


            return Ok(cart);
        }

        [HttpGet]
        [Route("GetMyCart")]
        [Authorize]
        [ProducesResponseType(typeof(LiteCartResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCart()
        {
            string userId = GetUserId();
            string cachename = GetMethodName() + userId;
            LiteCartResponse cart = (LiteCartResponse)GetFromCache(cachename);
            if (cart == null)
            {
                IQueryable<CartModel> queryable = businessUnitofWork.CartService.GetMyCartQueryable(userId);

                cart = await queryable.AsNoTracking()
                    .Select(r => new LiteCartResponse()
                    {
                        Id = r.Id,
                        InsurancePlanId = r.InsurancePlanId,
                        ItemName = r.InsurancePlanId.HasValue ? r.InsurancePlan.Name : r.ItemName,
                        UnitAmount = r.Amount,
                        Picture = r.InsurancePlanId.HasValue ? r.InsurancePlan.Thumbnail : string.Empty,
                        UserId = r.UserId,
                        QuatityOrder = r.QuatityOrder,
                        HasFixedQuantity = r.InsurancePlanId.HasValue && (r.InsurancePlan.Company == CompanyEnum.Hygeia),
                        CompanyId = r.InsurancePlanId.HasValue ? r.InsurancePlan.Company : CompanyEnum.Lafiami,
                        UseSystemHospital = r.InsurancePlanId.HasValue && ((r.InsurancePlan.Company == CompanyEnum.RelainceHMO) || (r.InsurancePlan.Company == CompanyEnum.Lafiami)),
                    })
                    .SingleOrDefaultAsync();
                if (cart.InsurancePlanId.HasValue)
                {
                    AmountObjectModel amountObject = await businessUnitofWork.InsuranceService.GetAmount(cart.InsurancePlanId.Value, null, 1, cart.QuatityOrder);
                    cart.UnitAmount = amountObject.Amount;
                    cart.MoneyUnit = amountObject.MoneyUnit;

                }

                SavetoCache(cart, cachename);
            }


            return Ok(cart);
        }

        [HttpPost]
        [Route("AddToCart")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToCart([FromBody] NewCartRequest model)
        {
            string userId = GetUserId();
            if (!await businessUnitofWork.InsuranceService.DoesInsurancePlanExist(model.InsurancePlanId, null))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.InsurancePlan));
            }

            if (model.InsurancePlanId > 0)
            {
                await ConfirmMinRequirement(model.InsurancePlanId, model.QuatityOrder);
            }

            List<CartModel> existingCarts = await businessUnitofWork.CartService.GetMyCartQueryable(userId).Where(r => r.InsurancePlanId != model.InsurancePlanId).ToListAsync();
            foreach (CartModel existingCart in existingCarts)
            {
                existingCart.ToDeletedEntity();
                businessUnitofWork.CartService.Update(existingCart);
            }

            CartModel cart = await businessUnitofWork.CartService.GetCart(model, userId);
            if (cart == null)
            {
                cart = new CartModel()
                {
                    Id = Guid.NewGuid(),
                    InsurancePlanId = model.InsurancePlanId,
                    QuatityOrder = model.QuatityOrder,
                    UserId = ((string.IsNullOrEmpty(userId)) ? (null) : (userId)),
                };


                businessUnitofWork.CartService.Add(cart);
                await businessUnitofWork.SaveAsync();
            }
            else
            {
                cart.QuatityOrder = model.QuatityOrder;
                businessUnitofWork.CartService.Update(cart);
                await businessUnitofWork.SaveAsync();
            }

            ClearCache();
            return Ok(new NewItemResponse<Guid>(cart.Id, string.Format(Constants.ActionResponse, Constants.InsurancePlan, Constants.Added)));
        }

        private async Task ConfirmMinRequirement(long insurancePlanId, int quatityOrder)
        {
            int minRequired = await businessUnitofWork.InsuranceService.GetMinMonthsRequired(insurancePlanId);
            if (quatityOrder < minRequired)
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, string.Join(Constants.Space, Constants.Five, Constants.Months, Constants.ofthis, Constants.InsurancePlan)));
            }
        }

        [HttpPost]
        [Route("UpdateCart")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartRequest model)
        {
            CartModel cart = await businessUnitofWork.CartService.GetCart(model.Id);
            if (cart == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Cart));
            }
            else
            {
                if (cart.OrderId.HasValue)
                {
                    throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Cart, Constants.Update));
                }

                if (cart.InsurancePlanId.HasValue)
                {
                    await ConfirmMinRequirement(cart.InsurancePlanId.Value, model.QuatityOrder);
                }

                cart.QuatityOrder = model.QuatityOrder;
                businessUnitofWork.CartService.Update(cart);
                await businessUnitofWork.SaveAsync();
            }

            ClearCache();
            return Ok(new NewItemResponse<Guid>(cart.Id, string.Format(Constants.ActionResponse, Constants.Cart, Constants.Updated)));
        }

        [HttpPost]
        [Route("DeleteCart")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCart([FromBody] Guid id)
        {
            CartModel productCart = await businessUnitofWork.CartService.GetCart(id);
            if (productCart == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Cart));
            }
            if (productCart.OrderId.HasValue)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Cart, Constants.Update));
            }

            productCart.ToDeletedEntity();

            businessUnitofWork.CartService.Update(productCart);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(productCart.Id, string.Format(Constants.ActionResponse, Constants.Cart, Constants.Updated)));
        }


    }
}
