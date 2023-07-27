using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Internals;
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
    public class WalletService : RepositoryBase<WalletModel, Guid>, IWalletService
    {
        public WalletService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }


        public async Task<string> GetWalletEmailAsync(Guid walletId)
        {
            IQueryable<WalletModel> queryable = GetQueryable(r => (r.Id == walletId));
            return await queryable
                .Select(r => r.User.Email)
                .SingleOrDefaultAsync();
        }

        public async Task<DefaultQuestionValue> GetWalletDefaultQuestionAnswers(Guid wallettId)
        {
            return await GetQueryable(r => r.Id == wallettId)
                .Select(r => new DefaultQuestionValue()
                {
                    Surname = r.User.Surname,
                    FirstName = r.User.Firstname,
                    EmailAddress = r.User.Email,
                    PhoneNumber = r.User.PhoneNumber,
                    Address = r.User.Address,
                    Amount = r.Balance,
                })
                .SingleOrDefaultAsync();
        }

        public async Task<List<ObjectId<Guid>>> GetRecentWalletYetToBeNotified(int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            var results = await DBContext.WalletTransactions
                .Where(r => (r.Email == null) && (r.CreatedDate >= _2DaysAgo))
                .OrderBy(r => r.CreatedDate)
                .Select(r => new
                {
                    r.Id,
                })
                .Skip(page)
                .Take((page + 1) * pageSize)
                .ToListAsync();

            if (results == null)
            {
                return new List<ObjectId<Guid>>();
            }

            return results.Select(r => new ObjectId<Guid>()
            {
                Id = r.Id,
                GenerateEmail = true
            }).ToList();
        }

        public IQueryable<WalletTransactionModel> GetWalletTransactionQueryable()
        {
            return DBContext.WalletTransactions.Where(r => !r.IsDeleted);
        }

        public async Task<bool> DoesUserHasEnoughCredit(string userId, decimal amount)
        {
            return await GetQueryable((r => (r.UserId == userId) && (r.Balance >= amount))).AnyAsync();
        }

        public async Task UpdateUserWalletAsync(string userId, decimal amount, TransactionTypeEnum transactionType, string reason, OrderModel order = null)
        {
            bool user = await DBContext.Users.AnyAsync(r => (r.Id == userId));
            if (!user)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.User));
            }

            WalletModel wallet = await GetQueryable((r => r.UserId == userId)).SingleOrDefaultAsync();
            if (wallet == null)
            {
                wallet = new WalletModel()
                {
                    Balance = (transactionType == TransactionTypeEnum.BookCredit) ? (0) : (amount),
                    BookBalance = amount,
                    Id = Guid.NewGuid(),
                    UserId = userId,
                };
            }
            else
            {
                wallet.Balance = (transactionType == TransactionTypeEnum.BookCredit) ? (wallet.Balance) : (decimal.Add(wallet.Balance, amount));
                wallet.BookBalance = decimal.Add(wallet.Balance, amount);
            }

            DBContext.WalletTransactions.Add(new WalletTransactionModel()
            {
                Amount = amount,
                Id = Guid.NewGuid(),
                TransactionCompletedDate = (transactionType == TransactionTypeEnum.BookCredit) ? ((DateTime?)null) : (DateTime.Now),
                TransactionStatus = (transactionType == TransactionTypeEnum.BookCredit) ? (TransactionStatusEnum.Pending) : (TransactionStatusEnum.Completed),
                TransactionType = transactionType,
                Wallet = wallet,
                Order = order,
            });
        }


        public void FinalizedBookCredit(WalletTransactionModel walletTransaction, TransactionStatusEnum transactionStatus)
        {
            if (transactionStatus == TransactionStatusEnum.Completed)
            {
                walletTransaction.Wallet.Balance += walletTransaction.Amount;
            }
            else
            {
                walletTransaction.Wallet.BookBalance = decimal.Subtract(walletTransaction.Wallet.BookBalance, walletTransaction.Amount);
            }
            walletTransaction.TransactionStatus = transactionStatus;
            walletTransaction.TransactionType = TransactionTypeEnum.Credit;
            walletTransaction.TransactionCompletedDate = DateTime.Now;
        }

        public async Task<List<WalletTransactionModel>> GetCompletedOrderBookCreditTransactions(int page, int pageSize)
        {
            return await DBContext.WalletTransactions
                 .Where(r => !r.IsDeleted && (r.Order.OrderStatus == OrderStatusEnum.Approved) && (r.TransactionType == TransactionTypeEnum.BookCredit))
                 .Include(r => r.Wallet)
                 .OrderBy(r => r.CreatedDate)
                 .Skip(page * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<List<WalletTransactionModel>> GetCancelledOrderBookCreditTransactions(int page, int pageSize)
        {
            return await DBContext.WalletTransactions
                 .Where(r => !r.IsDeleted && ((r.Order.OrderStatus == OrderStatusEnum.Cancelled) || (r.Order.OrderStatus == OrderStatusEnum.Rejected)) && (r.TransactionType == TransactionTypeEnum.BookCredit))
                  .Include(r => r.Wallet)
                 .OrderBy(r => r.CreatedDate)
                 .Skip(page * pageSize)
                .Take(pageSize)
                 .ToListAsync();
        }
    }
}
