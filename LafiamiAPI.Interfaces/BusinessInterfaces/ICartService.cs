using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface ICartService : IRepositoryBase<CartModel, Guid>
    {
        IQueryable<CartModel> GetMyCartQueryable(string userId);
        Task<CartModel> GetCart(NewCartRequest model, string userId);
        Task<bool> DoesCartExist(NewCartRequest model, string userId);
        Task<bool> DoesCartExist(Guid id);
        Task<CartModel> GetCart(Guid id);
        Task<List<CartModel>> GetCarts(List<Guid> ids);
        Task<bool> DoesCartsExist(List<Guid> ids);
        Task GetCartItems(Guid? paymentId, Guid? orderId, List<OrderItemsResponse> paymentItems);
        Task<string> GetOrderItemNames(Guid? orderId, Guid? paymentId = null);
    }
}
