using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class OrderService : RepositoryBase<OrderModel, Guid>, IOrderService
    {
        public OrderService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task<List<Guid>> GetNewOrdersNotificationYetToBeGenerated(int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            var results = await GetQueryable(r => (r.Email == null))
                .Where(r => r.DueDate >= _2DaysAgo)
                .OrderBy(r => r.DueDate)
                .Select(r => new
                {
                    OrderId = r.Id,
                })
                .Skip(page)
                .Take((page + 1) * pageSize)
                .ToListAsync();

            if (results == null)
            {
                return new List<Guid>();
            }

            return results.Select(r => r.OrderId).ToList();
        }

        public string GenerateOrderId()
        {
            return string.Join(string.Empty, Constants.Lafiami.ToLower(), Constants.Order, DateTime.Now.ToString(Constants.DateFormatForIdGeneration), RandomGenerator.GetRandomString(3, true));
        }

        public async Task<OrderModel> GetOrderAsync(Guid orderId, string userId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Id == orderId));
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }
            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<Guid?> GetOrderIdOfACart(Guid cartId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Cart.Id == cartId));
            return await queryable.Select(r => r.Id).SingleOrDefaultAsync();
        }

        public async Task<bool> IsOrderSuccessful(Guid orderId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Id == orderId));
            return await queryable.Select(r => (r.OrderStatus == OrderStatusEnum.Approved)).SingleOrDefaultAsync();
        }

        public async Task<LiteOrderContactDetail> GetOrderContactAsync(Guid orderId, string userId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Id == orderId));
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }
            return await queryable
                .Select(r => new LiteOrderContactDetail()
                {
                    Firstname = r.User.Firstname,
                    Surname = r.User.Surname,
                    Phone = r.User.PhoneNumber,
                    Email = r.User.Email,
                })
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetOrderNameAsync(Guid orderId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Id == orderId));


            return await queryable
                .Select(r => (r.User.Surname + Constants.Space + r.User.Firstname))
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetOrderEmailAsync(Guid orderId)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.Id == orderId));
            return await queryable
                .Select(r => r.User.Email)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> DoesHygeiaPrincipalMemberIdExist(string memberid)
        {
            IQueryable<OrderModel> queryable = GetQueryable(r => (r.HygeiaMemberId == memberid));
            return await queryable.AnyAsync();
        }

        public async Task<List<OrderItemsResponse>> GetFullOrderItems(Guid? orderid, Guid? paymentId = null)
        {
            List<OrderItemsResponse> orderItems = new List<OrderItemsResponse>();

            CartService cartService = new CartService(DBContext);
            await cartService.GetCartItems(paymentId, orderid, orderItems);

            return orderItems;
        }

        public decimal GetVatAmount(OrderSettings orderSettings, decimal billingAmount)
        {
            return ((orderSettings.VATInPercent > 0) ? (decimal.Multiply(billingAmount, decimal.Divide(orderSettings.VATInPercent, 100))) : (0M));
        }



    }
}
