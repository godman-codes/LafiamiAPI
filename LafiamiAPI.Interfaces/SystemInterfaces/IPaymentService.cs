using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using System;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IPaymentService : IRepositoryBase<PaymentModel, Guid>
    {
        Task<string> GetPaymentEmailAsync(Guid paymentId);
        Task<PaymentModel> GetPayment(Guid paymentid, bool includeOrder = false);
        Task<string> GetPaymentUserId(Guid paymentid);
        Task<PaymentLiteInformation> GetLastLitePayment(Guid OrderId);
        string GeneratePaymentTransactionId();
        Task<PaymentModel> GetLastPayment(Guid OrderId);
    }
}
