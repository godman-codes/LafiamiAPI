using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IOrderService : IRepositoryBase<OrderModel, Guid>
    {
        Task<string> GetOrderNameAsync(Guid orderId);
        Task<List<OrderItemsResponse>> GetFullOrderItems(Guid? orderid, Guid? paymentId = null);
        Task<string> GetOrderEmailAsync(Guid orderId);
        Task<Guid?> GetOrderIdOfACart(Guid cartId);
        Task<bool> DoesHygeiaPrincipalMemberIdExist(string memberid);
        Task<bool> IsOrderSuccessful(Guid orderId);
        string GenerateOrderId();
        Task<OrderModel> GetOrderAsync(Guid orderId, string userId);
        Task<LiteOrderContactDetail> GetOrderContactAsync(Guid orderId, string userId);
        decimal GetVatAmount(OrderSettings orderSettings, decimal billingAmount);
    }
}
