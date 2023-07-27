using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class PaymentService : RepositoryBase<PaymentModel, Guid>, IPaymentService
    {
        public PaymentService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }


        public async Task<string> GetPaymentEmailAsync(Guid paymentId)
        {
            IQueryable<PaymentModel> queryable = GetQueryable(r => (r.Id == paymentId));
            return await queryable
                .Select(r => r.Order.User.Email)
                .SingleOrDefaultAsync();
        }

        public async Task<DefaultQuestionValue> GetPaymentDefaultQuestionAnswers(Guid paymentId)
        {
            return await GetQueryable(r => r.Id == paymentId)
                .Select(r => new DefaultQuestionValue()
                {
                    Surname = r.Order.User.Surname,
                    FirstName = r.Order.User.Firstname,
                    EmailAddress = r.Order.User.Email,
                    PhoneNumber = r.Order.User.PhoneNumber,
                    Address = r.Order.User.Address,
                    Amount = r.Amount,
                    DueDate = r.Order.DueDate,
                })
                .SingleOrDefaultAsync();
        }

        public async Task<List<ObjectId<Guid>>> GetNewPaymentsYetToBeNotified(int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            var results = await GetQueryable(r => (r.Email == null))
                .Where(r => r.CreatedDate >= _2DaysAgo)
                .OrderBy(r => r.CreatedDate)
                .Select(r => new
                {
                    r.Id,
                    EmailId = (r.Email.Id)
                })
                .Skip(page)
                .Take((page + 1) * pageSize)
                .ToListAsync();

            return results.Select(r => new ObjectId<Guid>()
            {
                Id = r.Id,
                GenerateEmail = (r.EmailId == Guid.Empty)
            }).ToList();
        }

        public async Task<List<Guid>> GetPendingPayments(int daysFromSentDate, int page, int pageSize)
        {
            DateTime daysFromToday = DateTime.Today.AddDays(0 - daysFromSentDate);
            return await GetQueryable(r => (r.PaymentStatus == PaymentStatusEnum.Pending) && r.Email.Sentdate.HasValue && (r.Email.Sentdate.Value.Date == daysFromToday.Date))
                .OrderBy(r => r.CreatedDate)
                .Select(r => r.Id)
                .Skip(page)
                .Take((page + 1) * pageSize)
                .ToListAsync();
        }

        public async Task<PaymentModel> GetPayment(Guid paymentid, bool includeOrder = false)
        {
            IQueryable<PaymentModel> queryable = GetQueryable(r => r.Id == paymentid);
            if (includeOrder)
            {
                queryable = queryable.Include(r => r.Order);
            }

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<string> GetPaymentUserId(Guid paymentid)
        {
            IQueryable<PaymentModel> queryable = GetQueryable(r => r.Id == paymentid);
            return await queryable.Select(r => r.Order.UserId).FirstOrDefaultAsync();
        }

        public async Task<PaymentLiteInformation> GetLastLitePayment(Guid OrderId)
        {
            PaymentLiteInformation payment = await GetQueryable((r => r.OrderId == OrderId))
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new PaymentLiteInformation()
                {
                    Amount = r.Amount,
                    PaymentId = (r.Id),
                    PaymentStatus = r.PaymentStatus,
                    PaymentGateway = r.PaymentGateway,
                    GatewayTransactionId = r.GatewayTransactionId
                })
                .FirstOrDefaultAsync();

            if (payment != null)
            {
                if (payment.PaymentStatus == PaymentStatusEnum.Failed)
                {
                    payment.PaymentId = Guid.Empty;
                }
            }

            return payment;
        }

        public async Task<PaymentModel> GetLastPayment(Guid OrderId)
        {
            return await GetQueryable((r => r.OrderId == OrderId))
                .OrderByDescending(r => r.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public string GeneratePaymentTransactionId()
        {
            return string.Join(string.Empty, Constants.Lafiami.ToLower().Trim(), Constants.Pay, DateTime.Now.ToString(Constants.DateFormatForIdGeneration), RandomGenerator.GetRandomString(3, true));
        }
    }
}
