using LafiamiAPI.Datas.Models;
using LafiamiAPI.Utilities.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IWalletService : IRepositoryBase<WalletModel, Guid>
    {
        Task<string> GetWalletEmailAsync(Guid walletId);
        void FinalizedBookCredit(WalletTransactionModel walletTransaction, TransactionStatusEnum transactionStatus);
        Task<bool> DoesUserHasEnoughCredit(string userId, decimal amount);
        IQueryable<WalletTransactionModel> GetWalletTransactionQueryable();
        Task UpdateUserWalletAsync(string userId, decimal amount, TransactionTypeEnum transactionType, string reason, OrderModel order = null);
    }
}
