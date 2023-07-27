using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class CartService : RepositoryBase<CartModel, Guid>, ICartService
    {
        public CartService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<CartModel> GetMyCartQueryable(string userId)
        {
            IQueryable<CartModel> queryable = GetQueryable((r => !r.OrderId.HasValue));
            queryable = queryable.Where(r => r.UserId == userId);

            return queryable;
        }

        public async Task<bool> DoesCartExist(NewCartRequest model, string userId)
        {
            IQueryable<CartModel> queryable = GetQueryable((r => (r.InsurancePlanId == model.InsurancePlanId) && (!r.OrderId.HasValue)));
            queryable = queryable.Where(r => r.UserId == userId);

            return await queryable.AnyAsync();
        }

        public async Task<CartModel> GetCart(NewCartRequest model, string userId)
        {
            IQueryable<CartModel> queryable = GetQueryable((r => (r.InsurancePlanId == model.InsurancePlanId) && (!r.OrderId.HasValue)));
            queryable = queryable.Where(r => r.UserId == userId);

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<bool> DoesCartExist(Guid id)
        {
            IQueryable<CartModel> queryable = GetQueryable((r => r.Id == id));
            return await queryable.AnyAsync();
        }

        public async Task<bool> DoesCartsExist(List<Guid> ids)
        {
            foreach (Guid id in ids)
            {
                if (!await DoesCartExist(id))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<CartModel> GetCart(Guid id)
        {
            IQueryable<CartModel> queryable = GetQueryable((r => r.Id == id));
            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<List<CartModel>> GetCarts(List<Guid> ids)
        {
            List<CartModel> carts = new List<CartModel>();
            foreach (Guid id in ids)
            {
                CartModel productCart = await GetCart(id);
                if (productCart != null)
                {
                    carts.Add(productCart);
                }
            }

            return carts;
        }

        public async Task GetCartItems(Guid? paymentId, Guid? orderId, List<OrderItemsResponse> paymentItems)
        {
            if (!paymentId.HasValue && !orderId.HasValue)
            {
                return;
            }

            IQueryable<CartModel> queryable = GetQueryable(r => true);
            if (paymentId.HasValue)
            {
                queryable = queryable.Where(r => r.Order.Payments.Any(t => t.Id == paymentId.Value));
            }
            if (orderId.HasValue)
            {
                queryable = queryable.Where(r => r.OrderId == orderId.Value);
            }

            var paymentItemsfromProduct = await queryable.Select(r => new
            {
                Name = (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Name) : (r.ItemName),
                r.QuatityOrder,
                r.InsurancePlanId,
                r.Amount,
                OrderDate = (r.OrderId.HasValue) ? (r.Order.CreatedDate) : ((DateTime?)null),
                Cartcount = r.Order.Cart.QuatityOrder,
            })
            .ToListAsync();

            InsuranceService insuranceService = new InsuranceService(DBContext);
            foreach (var paymentItem in paymentItemsfromProduct)
            {
                AmountObjectModel amountObject = new AmountObjectModel()
                {
                    Amount = paymentItem.Amount,
                    MoneyUnit = MoneyUnitEnum.OneOff
                };

                if (paymentItem.InsurancePlanId.HasValue)
                {
                    amountObject = await insuranceService.GetAmount(paymentItem.InsurancePlanId.Value, paymentItem.OrderDate, paymentItem.QuatityOrder, paymentItem.Cartcount);
                }
                //decimal amount = (paymentItem.InsurancePlanId.HasValue) ? (await insuranceService.GetAmount(paymentItem.InsurancePlanId.Value, paymentItem.OrderDate, paymentItem.QuatityOrder, paymentItem.Cartcount)).Amount : (paymentItem.Amount);

                paymentItems.Add(new OrderItemsResponse()
                {
                    Amount = decimal.Multiply(paymentItem.QuatityOrder, amountObject.Amount),
                    MoneyUnit = amountObject.MoneyUnit,
                    Name = paymentItem.Name,
                    Quantity = paymentItem.QuatityOrder,
                });
            }
        }

        public async Task<string> GetOrderItemNames(Guid? orderId, Guid? paymentId = null)
        {
            if (!paymentId.HasValue && !orderId.HasValue)
            {
                return string.Empty;
            }

            IQueryable<CartModel> queryable = GetQueryable(r => true);
            if (paymentId.HasValue)
            {
                queryable = queryable.Where(r => r.Order.Payments.Any(t => t.Id == paymentId.Value));
            }
            if (orderId.HasValue)
            {
                queryable = queryable.Where(r => r.OrderId == orderId.Value);
            }

            List<string> productNames = await queryable
                .Select(r => (r.InsurancePlanId.HasValue) ? (r.InsurancePlan.Name) : (r.ItemName))
                .ToListAsync();

            string result = string.Empty;
            if (productNames.Count() <= 2)
            {
                result = string.Join(Constants.Comma, productNames);
            }
            else
            {
                result = string.Join(Constants.Comma, productNames.Take(2).ToList(), "and co.");
            }

            return result;
        }


    }
}
