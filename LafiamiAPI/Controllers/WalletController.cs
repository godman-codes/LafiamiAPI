using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
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
    public class WalletController : BaseController<WalletController>
    {
        public const string ControllerName = ControllerConstant.Wallet;
        private readonly IBusinessUnitofWork businessUnitofWork;
        public WalletController(IMemoryCache memoryCache, ILogger<WalletController> logger, ISystemUnitofWork systemUnitofWork, IBusinessUnitofWork businessUnitofWork) : base(memoryCache, logger, systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
        }

        [HttpGet]
        [Route("GetWalletTransactions")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<WalletTransactionResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWalletTransactions(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + (string.IsNullOrEmpty(keyword) ? string.Empty : keyword) + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<WalletTransactionResponse> walletTransactions = (List<WalletTransactionResponse>)GetFromCache(cachename);
            if (walletTransactions == null)
            {
                IQueryable<WalletTransactionModel> queryable = businessUnitofWork.WalletService.GetWalletTransactionQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.Wallet.User.Surname.Contains(keyword) ||
                        r.Wallet.User.Firstname.Contains(keyword)
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

                walletTransactions = await queryable.AsNoTracking()
                    .Select(r => new WalletTransactionResponse()
                    {
                        Id = r.Id,
                        Email = r.Wallet.User.Email,
                        PhoneNumber = r.Wallet.User.PhoneNumber,
                        Firstname = r.Wallet.User.Firstname,
                        Surname = r.Wallet.User.Surname,
                        Amount = r.Amount,
                        IsPending = (r.TransactionStatus == TransactionStatusEnum.Pending),
                        TransactionCompletedDate = r.TransactionCompletedDate,
                        TransactionStatus = r.TransactionStatus.DisplayName(),
                        TransactionType = r.TransactionType.DisplayName(),
                        CreatedDate = r.CreatedDate
                    })
                    .ToListAsync();

                SavetoCache(walletTransactions, cachename);
            }

            return Ok(walletTransactions);
        }

        [HttpGet]
        [Route("GetMyWalletTransactions")]
        [Authorize]
        [ProducesResponseType(typeof(List<MyWalletTransactionResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWalletTransactions(int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + page + Constants.Underscore + pageSize;
            List<MyWalletTransactionResponse> myWalletTransactions = (List<MyWalletTransactionResponse>)GetFromCache(cachename);
            if (myWalletTransactions == null)
            {
                IQueryable<WalletTransactionModel> queryable = businessUnitofWork.WalletService.GetWalletTransactionQueryable();

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

                myWalletTransactions = await queryable.AsNoTracking()
                    .Select(r => new MyWalletTransactionResponse()
                    {
                        Id = r.Id,
                        Amount = r.Amount,
                        IsPending = (r.TransactionStatus == TransactionStatusEnum.Pending),
                        TransactionCompletedDate = r.TransactionCompletedDate,
                        TransactionStatus = r.TransactionStatus.DisplayName(),
                        TransactionType = r.TransactionType.DisplayName(),
                        CreatedDate = r.CreatedDate
                    })
                    .ToListAsync();

                SavetoCache(myWalletTransactions, cachename);
            }


            return Ok(myWalletTransactions);
        }

        [HttpGet]
        [Route("GetMyWallet")]
        [Authorize]
        [ProducesResponseType(typeof(MyWalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWallet()
        {
            string cachename = GetMethodName();
            MyWalletResponse walletResponse = (MyWalletResponse)GetFromCache(cachename);
            if (walletResponse == null)
            {
                string userId = GetUserId();
                IQueryable<WalletModel> queryable = businessUnitofWork.WalletService.GetQueryable((r => r.UserId == userId));

                walletResponse = await queryable.AsNoTracking()
                    .Select(r => new MyWalletResponse()
                    {
                        Id = r.Id,
                        BookBalance = r.BookBalance,
                        Balance = r.Balance,
                    })
                    .FirstOrDefaultAsync();
                if (walletResponse != null)
                {
                    SavetoCache(walletResponse, cachename);
                }
                else
                {
                    walletResponse = new MyWalletResponse();
                }
            }


            return Ok(walletResponse);
        }




        [HttpPost]
        [Route("AddFundToClientWallet")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddFundToClientWallet([FromBody] TopUpRequest model)
        {
            string userId = await systemUnitofWork.UserService.GetUserId(model.EmailAddress);

            string orderId = await TopUp(model.Amount, userId);

            ClearCache();
            return Ok(string.Join(string.Empty, Constants.DefaultOrderPath, Constants.OrderQueryParameter, orderId));
        }

        [HttpPost]
        [Route("AddFundToMyWallet")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddFundToMyWallet([FromBody] MyTopUpRequest model)
        {
            string userId = GetUserId();

            string orderId = await TopUp(model.Amount, userId);

            ClearCache();
            return Ok(Constants.DefaultOrderPath + Constants.OrderQueryParameter + orderId);
        }

        private async Task<string> TopUp(decimal amount, string userId)
        {
            DateTime orderDate = DateTime.Now;
            OrderModel order = new OrderModel()
            {
                Id = Guid.NewGuid(),
                OrderId = businessUnitofWork.OrderService.GenerateOrderId(),
                OrderStatus = OrderStatusEnum.Pending,
                TotalAmount = amount,
                Vat = 0,
                UserId = userId,
                DueDate = orderDate,
                Payments = new List<PaymentModel>() { new PaymentModel()
                    {
                        Id = Guid.NewGuid(),
                        Amount = amount,
                        PaymentStatus = PaymentStatusEnum.Pending,
                        TransactionId = systemUnitofWork.PaymentService.GeneratePaymentTransactionId(),
                    }
                },
                Cart = new CartModel()
                {
                    ItemName = "Credit Top Up",
                    QuatityOrder = 1,
                    Amount = amount,
                    Id = Guid.NewGuid(),
                    UserId = userId,
                },
            };

            await businessUnitofWork.WalletService.UpdateUserWalletAsync(userId, amount, TransactionTypeEnum.BookCredit, "Wallet Top Up", order);
            await businessUnitofWork.SaveAsync();
            return order.OrderId;
        }


    }
}
