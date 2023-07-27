using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Internals;
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
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class PaymentController : BaseController<PaymentController>
    {
        public const string ControllerName = ControllerConstant.Payment;
        private readonly IBusinessUnitofWork businessUnitofWork;
        private readonly FlutterwaveSettings flutterwaveSettings;
        private readonly PaystackSettings paystackSettings;
        private readonly PaymentSettings paymentSettings;

        public PaymentController(IMemoryCache memoryCache, ILogger<PaymentController> logger, ISystemUnitofWork systemUnitofWork, IBusinessUnitofWork businessUnitofWork, IOptions<FlutterwaveSettings> _flutterconfig, IOptions<PaystackSettings> _paystackconfig, IOptions<PaymentSettings> _paymentconfig) : base(memoryCache, logger, systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
            flutterwaveSettings = _flutterconfig.Value;
            paystackSettings = _paystackconfig.Value;
            paymentSettings = _paymentconfig.Value;
        }

        [HttpGet]
        [Route("GetPayments")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LitePaymentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayments(int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<LitePaymentResponse> payments = (List<LitePaymentResponse>)GetFromCache(cachename);
            if (payments == null)
            {

                Expression<Func<PaymentModel, bool>> where = (r => true);

                IQueryable<PaymentModel> queryable = systemUnitofWork.PaymentService.GetQueryable(where);

                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                var _payments = await queryable.AsNoTracking()
                        .OrderByDescending(r => r.CreatedDate)
                         .Select(r => new
                         {
                             r.Id,
                             r.Amount,
                             r.TransactionId,
                             r.Order.OrderId,
                             r.PaymentCompletedDate,
                             r.PaymentStatus,
                             r.CreatedDate,
                             r.PaymentGateway,

                             Email = r.Order.ForSomeoneElse ? r.Order.EmailAddress : r.Order.User.Email,
                             Phone = r.Order.ForSomeoneElse ? r.Order.PhoneNumber : r.Order.User.PhoneNumber,
                             Firstname = r.Order.ForSomeoneElse ? r.Order.Firstname : r.Order.User.Firstname,
                             Surname = r.Order.ForSomeoneElse ? r.Order.Surname : r.Order.User.Surname,
                         })
                        .ToListAsync();

                payments = new List<LitePaymentResponse>();

                foreach (var r in _payments)
                {
                    LitePaymentResponse paymentResponse = new LitePaymentResponse()
                    {
                        Id = r.Id,
                        Amount = r.Amount,
                        TransactionId = r.TransactionId,
                        PaymentCompletedDate = r.PaymentCompletedDate,
                        HasPaid = (r.PaymentStatus == PaymentStatusEnum.Paid),
                        PaymentStatus = r.PaymentStatus.DisplayName(),
                        Email = r.Email,
                        Phone = r.Phone,
                        Surname = r.Surname,
                        Firstname = r.Firstname,
                        OrderId = r.OrderId,
                        PaymentGateway = r.PaymentGateway,
                        CanManuallyConfirm = (r.PaymentGateway == PaymentGatewayEnum.Cash),
                        ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(null, r.Id)
                    };

                    payments.Add(paymentResponse);
                }

                SavetoCache(payments, cachename);
            }

            return Ok(payments);
        }

        [HttpGet]
        [Route("GetAiicoPayments")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteAiicoPaymentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAiicoPayments(int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<LiteAiicoPaymentResponse> payments = (List<LiteAiicoPaymentResponse>)GetFromCache(cachename);
            if (payments == null)
            {

                Expression<Func<PaymentModel, bool>> where = (r => r.Order.Company == CompanyEnum.Aiico);
                IQueryable<PaymentModel> queryable = systemUnitofWork.PaymentService.GetQueryable(where);

                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                var _payments = await queryable.AsNoTracking()
                        .OrderByDescending(r => r.CreatedDate)
                         .Select(r => new
                         {
                             r.Id,
                             r.Amount,
                             r.TransactionId,
                             r.Order.OrderId,
                             r.PaymentCompletedDate,
                             r.PaymentStatus,
                             r.CreatedDate,
                             r.PaymentGateway,
                             Email = r.Order.ForSomeoneElse ? r.Order.EmailAddress : r.Order.User.Email,
                             Phone = r.Order.ForSomeoneElse ? r.Order.PhoneNumber : r.Order.User.PhoneNumber,
                             Firstname = r.Order.ForSomeoneElse ? r.Order.Firstname : r.Order.User.Firstname,
                             Surname = r.Order.ForSomeoneElse ? r.Order.Surname : r.Order.User.Surname,
                             r.PartnerPaymentJsonResponse,
                             r.IntegrationStatus,
                             r.IntegrationErrorMessage,
                         })
                        .ToListAsync();

                payments = new List<LiteAiicoPaymentResponse>();

                foreach (var r in _payments)
                {
                    LiteAiicoPaymentResponse paymentResponse = new LiteAiicoPaymentResponse()
                    {
                        Id = r.Id,
                        Amount = r.Amount,
                        TransactionId = r.TransactionId,
                        PaymentCompletedDate = r.PaymentCompletedDate,
                        HasPaid = (r.PaymentStatus == PaymentStatusEnum.Paid),
                        PaymentStatus = r.PaymentStatus.DisplayName(),
                        Email = r.Email,
                        Phone = r.Phone,
                        Surname = r.Surname,
                        Firstname = r.Firstname,
                        OrderId = r.OrderId,
                        PaymentGateway = r.PaymentGateway,
                        CanManuallyConfirm = (r.PaymentGateway == PaymentGatewayEnum.Cash),
                        ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(null, r.Id),
                        IntegrationStatus = r.IntegrationStatus,
                        IntegrationStatusName = r.IntegrationStatus.DisplayName(),
                        IntegrationErrorMessage = WebUtility.HtmlDecode(r.IntegrationErrorMessage),
                        FinalizePartnerJsonResponse = WebUtility.HtmlDecode(r.PartnerPaymentJsonResponse),
                        IsIntegrationCompleted = (r.IntegrationStatus == IntegrationStatusEnum.Completed),
                        IsIntegrationFailed = (r.IntegrationStatus == IntegrationStatusEnum.Failed),
                        IsIntegrationPending = (r.IntegrationStatus == IntegrationStatusEnum.Pending)
                    };

                    payments.Add(paymentResponse);
                }

                SavetoCache(payments, cachename);
            }

            return Ok(payments);
        }

        [HttpGet]
        [Route("GetMyPayments")]
        [Authorize]
        [ProducesResponseType(typeof(List<MyLitePaymentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyPayments(int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), page, pageSize);
            List<MyLitePaymentResponse> payments = (List<MyLitePaymentResponse>)GetFromCache(cachename);
            if (payments == null)
            {
                string userId = GetUserId();
                Expression<Func<PaymentModel, bool>> where = (r =>
                    (r.Order.UserId == userId)
                );

                IQueryable<PaymentModel> queryable = systemUnitofWork.PaymentService.GetQueryable(where);

                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                var _payments = await queryable.AsNoTracking()
                        .OrderByDescending(r => r.CreatedDate)
                        .Select(r => new
                        {
                            r.Id,
                            r.Amount,
                            r.TransactionId,
                            r.PaymentCompletedDate,
                            r.PaymentStatus,
                            r.PaymentGateway,
                            r.Order.OrderId
                        })
                        .ToListAsync();

                payments = new List<MyLitePaymentResponse>();

                foreach (var r in _payments)
                {
                    MyLitePaymentResponse paymentResponse = new MyLitePaymentResponse()
                    {
                        Id = r.Id,
                        Amount = r.Amount,
                        TransactionId = r.TransactionId,
                        PaymentCompletedDate = r.PaymentCompletedDate,
                        HasPaid = (r.PaymentStatus == PaymentStatusEnum.Paid),
                        PaymentStatus = r.PaymentStatus.DisplayName(),
                        PaymentGateway = r.PaymentGateway,
                        OrderId = r.OrderId,
                        ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(null, r.Id)
                    };

                    payments.Add(paymentResponse);
                }

                SavetoCache(payments, cachename);
            }

            return Ok(payments);
        }


        [HttpGet]
        [Route("GetPaymentTypes")]
        [ProducesResponseType(typeof(List<PaymentTypeResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPaymentTypes()
        {
            List<PaymentTypeResponse> result = Enum.GetValues(typeof(PaymentTypeEnum)).Cast<PaymentTypeEnum>().Select(r => new PaymentTypeResponse()
            {
                Id = (byte)r,
                Name = r.DisplayName()
            }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPaymentStatuses")]
        [ProducesResponseType(typeof(List<PaymentStatusResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPaymentStatuses()
        {
            List<PaymentStatusResponse> result = Enum.GetValues(typeof(PaymentStatusEnum)).Cast<PaymentStatusEnum>().Select(r => new PaymentStatusResponse()
            {
                Id = (byte)r,
                Name = r.DisplayName()
            }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPaymentGateways")]
        [ProducesResponseType(typeof(List<PaymentGatewayResponse>), StatusCodes.Status200OK)]
        public IActionResult GetPaymentGateways()
        {
            List<PaymentGatewayResponse> result = Enum.GetValues(typeof(PaymentGatewayEnum)).Cast<PaymentGatewayEnum>().Select(r => new PaymentGatewayResponse()
            {
                Id = (byte)r,
                Name = r.DisplayName()
            }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMyPaystackPublicKey")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetMyPaystackPublicKey()
        {
            string result = Paystack.GetPublicKey(paystackSettings);
            return Ok(result ?? string.Empty);
        }

        [HttpGet]
        [Route("GetMyFlutterwavePublicKey")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetMyFlutterwavePublicKey()
        {
            string result = ((flutterwaveSettings.GoLive) ? (flutterwaveSettings.PublicKey) : (flutterwaveSettings.TestPublicKey));
            return Ok(result ?? string.Empty);
        }

        [HttpPost]
        [Route("UpdatePaymentGatewayInformation")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePaymentGatewayInformation([FromBody] PaymentGatewayInfoRequest model)
        {
            PaymentModel payment = new PaymentModel();
            if (model.PaymentId != Guid.Empty)
            {
                payment = await systemUnitofWork.PaymentService.GetPayment(model.PaymentId);
                if (payment == null)
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
                }
                if (!((payment.PaymentStatus == PaymentStatusEnum.Pending) || (payment.PaymentStatus == PaymentStatusEnum.AwaitingVerification)))
                {
                    throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Payment, Constants.Update));
                }

                payment.PaymentGateway = model.Gateway;
                payment.PaymentType = ((model.Gateway == PaymentGatewayEnum.Cash) ? (PaymentTypeEnum.CashPayment) : (PaymentTypeEnum.OnlinePayment));
                payment.PaymentStatus = PaymentStatusEnum.AwaitingVerification;
                systemUnitofWork.PaymentService.Update(payment);
            }
            else
            {
                OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.OrderId, null);
                payment = new PaymentModel()
                {
                    Amount = order.TotalAmount,
                    Id = Guid.NewGuid(),
                    OrderId = model.OrderId,
                    PaymentGateway = model.Gateway,
                    PaymentStatus = PaymentStatusEnum.AwaitingVerification,
                    PaymentType = ((model.Gateway == PaymentGatewayEnum.Cash) ? (PaymentTypeEnum.CashPayment) : (PaymentTypeEnum.OnlinePayment)),
                    TransactionId = systemUnitofWork.PaymentService.GeneratePaymentTransactionId(),
                };
                systemUnitofWork.PaymentService.Add(payment);

                if ((payment.PaymentGateway == PaymentGatewayEnum.Cash))
                {
                    await businessUnitofWork.EmailService.PaymentEmail(payment.Id, EmailTypeEnums.AwaitingPaymentConfirmation, paymentSettings.NewPaymentNotificationEmail);
                    await businessUnitofWork.SaveAsync();
                }
            }

            await systemUnitofWork.SaveAsync();


            LiteOrderContactDetail liteOrder = await businessUnitofWork.OrderService.GetOrderContactAsync(model.OrderId, null);
            PaymentResponse paymentResponse = new PaymentResponse()
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Code = payment.Code,
                PaymentGateway = payment.PaymentGateway.Value.DisplayName(),
                TransactionId = payment.TransactionId,
                HasPaid = (payment.PaymentStatus == PaymentStatusEnum.Paid),
                IsAwaitingVerification = (payment.PaymentStatus == PaymentStatusEnum.AwaitingVerification),
                PaymentType = payment.PaymentType.DisplayName(),
                IsOnlinePayment = (payment.PaymentType == PaymentTypeEnum.OnlinePayment),
                PaymentStatus = payment.PaymentStatus.DisplayName(),
                CreatedDate = payment.CreatedDate,
                Email = liteOrder.Email,
                Phone = liteOrder.Phone,
                Name = string.Join(Constants.Space, liteOrder.Surname, liteOrder.Firstname),
                PaymentItems = await businessUnitofWork.OrderService.GetFullOrderItems(payment.OrderId.Value)
            };

            ClearCacheWithDependents();
            return Ok(paymentResponse);
        }

        [HttpPost]
        [Route("CompleteWithWalletPayment")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompleteWithWalletPayment([FromBody] PaymentGatewayInfoRequest model)
        {
            string userId = GetUserId();

            PaymentModel payment;
            string orderId;
            if (model.PaymentId != Guid.Empty)
            {
                payment = await systemUnitofWork.PaymentService.GetPayment(model.PaymentId, true);
                if (payment == null)
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
                }
                if (payment.PaymentStatus != PaymentStatusEnum.Pending)
                {
                    throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Update, Constants.PaymentGateway));
                }

                payment.PaymentGateway = PaymentGatewayEnum.Wallet;
                payment.PaymentType = PaymentTypeEnum.OnlinePayment;
                payment.PaymentStatus = PaymentStatusEnum.Paid;
                payment.PaymentCompletedDate = DateTime.Now;
                payment.Order.OrderStatus = OrderStatusEnum.Approved;
                payment.RunBackgroundService = (payment.Order.Company == CompanyEnum.Aiico);
                payment.IntegrationStatus = IntegrationStatusEnum.Pending;

                orderId = payment.Order.OrderId;

                systemUnitofWork.PaymentService.Update(payment);
            }
            else
            {
                OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.OrderId, null);
                order.OrderStatus = OrderStatusEnum.Approved;
                payment = new PaymentModel()
                {
                    Amount = order.TotalAmount,
                    Id = Guid.NewGuid(),
                    Order = order,
                    PaymentGateway = model.Gateway,
                    PaymentStatus = PaymentStatusEnum.Paid,
                    PaymentType = PaymentTypeEnum.OnlinePayment,
                    PaymentCompletedDate = DateTime.Now,
                    TransactionId = systemUnitofWork.PaymentService.GeneratePaymentTransactionId(),
                    RunBackgroundService = (order.Company == CompanyEnum.Aiico),
                    IntegrationStatus = IntegrationStatusEnum.Pending
                };

                orderId = order.OrderId;
                systemUnitofWork.PaymentService.Add(payment);
            }

            if (!await businessUnitofWork.WalletService.DoesUserHasEnoughCredit(userId, payment.Amount))
            {
                throw new WebsiteException(Constants.InsufficientCredit);
            }

            //Update Wallet Balance and also Generate a Wallet Transaction 
            await businessUnitofWork.WalletService.UpdateUserWalletAsync(userId, payment.Amount, TransactionTypeEnum.Debit, string.Format(Constants.PaymentForOrder, orderId));

            await systemUnitofWork.SaveAsync();

            await PaymentNotification(payment);
            ClearCacheWithDependents();
            return Ok(string.Format(Constants.ActionResponse, Constants.Payment, Constants.Completed));
        }

        [HttpPost]
        [Route("ConfirmPaystackPayment")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmPaystackPayment([FromBody] Guid paymentId)
        {
            PaymentModel payment = await systemUnitofWork.PaymentService.GetPayment(paymentId, true);
            if (payment == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }

            if (payment.PaymentGateway != PaymentGatewayEnum.Paystack)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }

            string paystatuscontent = "";
            PayStackStatusResponse paystatus = new PayStackStatusResponse();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient cl = new HttpClient())
            {
                cl.BaseAddress = new Uri(Paystack.PayStackStatusUrlContent);
                cl.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, Paystack.GetSecretKey(paystackSettings));

                // Add a new Request Message
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, payment.TransactionId);

                using (HttpResponseMessage response = cl.SendAsync(requestMessage).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string res = await content.ReadAsStringAsync();
                        paystatuscontent = res.Trim();
                        paystatus = System.Text.Json.JsonSerializer.Deserialize<PayStackStatusResponse>(res.Trim());
                    }
                }
            }

            if (paystatus.data != null)
            {
                payment.PaymentStatus = (paystatus.data.status == "success") ? PaymentStatusEnum.Paid : PaymentStatusEnum.Failed;
                payment.Reason = paystatus.data.gateway_response;
            }
            else
            {
                payment.PaymentStatus = PaymentStatusEnum.Failed;
                payment.Reason = paystatus.message;
            }
            payment.PaymentCompletedDate = DateTime.Now;
            payment.PaymentJson = WebUtility.HtmlEncode(paystatuscontent);
            payment.PaymentCompletedDate = DateTime.Now;

            if (payment.PaymentStatus == PaymentStatusEnum.Paid)
            {
                payment.Order.OrderStatus = OrderStatusEnum.Approved;
                if (payment.Order.Company.HasValue)
                {
                    payment.RunBackgroundService = (payment.Order.Company == CompanyEnum.Aiico);
                    payment.IntegrationStatus = IntegrationStatusEnum.Pending;
                }
            }
            else
            {
                payment.Order.OrderStatus = OrderStatusEnum.Failed;
            }

            systemUnitofWork.PaymentService.Update(payment);
            await systemUnitofWork.SaveAsync();

            await PaymentNotification(payment);
            ClearCacheWithDependents();
            return Ok((payment.PaymentStatus == PaymentStatusEnum.Paid));

        }

        private void ClearCacheWithDependents()
        {
            ClearCache();
            (new OrderController(cache, null, null, null, null)).ClearCache();
        }

        private async Task PaymentNotification(PaymentModel paymentTransaction)
        {
            try
            {
                string userId = await systemUnitofWork.PaymentService.GetPaymentUserId(paymentTransaction.Id);
                string paymentEmail = await systemUnitofWork.PaymentService.GetPaymentEmailAsync(paymentTransaction.Id);
                if (paymentTransaction.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    await businessUnitofWork.EmailService.PaymentEmail(paymentTransaction.Id, EmailTypeEnums.NewSuccessfulPayment, paymentEmail);
                }
                else
                {
                    await businessUnitofWork.EmailService.PaymentEmail(paymentTransaction.Id, EmailTypeEnums.NewFailedPayment, paymentEmail);
                }
                await businessUnitofWork.SaveAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        [HttpPost]
        [Route("ConfirmFlutterwavePayment")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmFlutterwavePayment([FromBody] Guid paymentId)
        {
            PaymentModel payment = await systemUnitofWork.PaymentService.GetPayment(paymentId, true);
            if (payment == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }

            if (payment.PaymentGateway != PaymentGatewayEnum.Flutterwave)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }

            var data = new
            {
                txref = payment.TransactionId,
                SECKEY = flutterwaveSettings.GoLive ? flutterwaveSettings.SecretKey : flutterwaveSettings.TestSecretKey
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));


            StringContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data));
            content.Headers.ContentType = new MediaTypeHeaderValue(Constants.ApplicationJson);

            HttpResponseMessage responseMessage = await client.PostAsync(Constants.VerifyFlutterPaymentURL, content);
            //please make sure to change this to production url when you go live
            string responseStr = responseMessage.Content.ReadAsStringAsync().Result;
            FlutterwaveStatusResponse response = System.Text.Json.JsonSerializer.Deserialize<FlutterwaveStatusResponse>(responseStr);

            payment.PaymentJson = WebUtility.HtmlEncode(responseStr);
            if (response.data.status == Constants.Successful && response.data.amount == payment.Amount && response.data.chargecode == Constants.DoubleZeroString)
            {
                payment.PaymentStatus = PaymentStatusEnum.Paid;
                payment.Reason = response.data.status;
                if (payment.Order.Company.HasValue)
                {
                    payment.RunBackgroundService = (payment.Order.Company == CompanyEnum.Aiico);
                    payment.IntegrationStatus = IntegrationStatusEnum.Pending;
                }
            }
            else
            {
                payment.PaymentStatus = PaymentStatusEnum.Failed;
                payment.Reason = response.data.status;
            }
            payment.PaymentCompletedDate = DateTime.Now;
            if (payment.PaymentStatus == PaymentStatusEnum.Paid)
            {
                payment.Order.OrderStatus = OrderStatusEnum.Approved;
            }
            else
            {
                payment.Order.OrderStatus = OrderStatusEnum.Failed;
            }

            systemUnitofWork.PaymentService.Update(payment);
            await systemUnitofWork.SaveAsync();

            await PaymentNotification(payment);
            ClearCacheWithDependents();

            return Ok((payment.PaymentStatus == PaymentStatusEnum.Paid));

        }

        [HttpPost]
        [Route("ConfirmManualPayment")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Authorize(Roles = SystemRole.Administrator)]
        public async Task<IActionResult> ConfirmManualPayment([FromBody] ConfirmManualPaymentRequest model)
        {
            PaymentModel payment = await systemUnitofWork.PaymentService.GetPayment(model.Id, true);
            if (payment == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }

            if (!(payment.PaymentGateway == PaymentGatewayEnum.Cash))
            {
                throw new WebsiteException(Constants.NotAllowedToManuallyConfirmPayment);
            }

            payment.PaymentStatus = (model.PaymentStatus) ? PaymentStatusEnum.Paid : PaymentStatusEnum.Failed;
            payment.Reason = string.Format(Constants.ProcessedFor, GetUsername());
            payment.PaymentCompletedDate = DateTime.Now;

            if (payment.PaymentStatus == PaymentStatusEnum.Paid)
            {
                payment.Order.OrderStatus = OrderStatusEnum.Approved;
                if (payment.Order.Company.HasValue)
                {
                    payment.RunBackgroundService = (payment.Order.Company == CompanyEnum.Aiico);
                    payment.IntegrationStatus = IntegrationStatusEnum.Pending;
                }
            }
            else
            {
                payment.Order.OrderStatus = OrderStatusEnum.Rejected;
            }

            systemUnitofWork.PaymentService.Update(payment);
            await systemUnitofWork.SaveAsync();

            await PaymentNotification(payment);
            ClearCacheWithDependents();

            return Ok((payment.PaymentStatus == PaymentStatusEnum.Paid) ? (string.Format(Constants.ActionResponse, Constants.Payment, Constants.Confirmed)) : (string.Format(Constants.ActionResponseNot, Constants.Payment, Constants.Confirmed)));
        }


        [HttpPost]
        [Route("RepostAiicoPayment")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RepostAiicoPayment([FromBody] IdRequest<Guid> model)
        {
            PaymentModel payment = await systemUnitofWork.PaymentService.GetPayment(model.Id);
            if (payment == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Payment));
            }
            if (payment.IntegrationStatus != IntegrationStatusEnum.Failed)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Payment, Constants.Repost));
            }

            payment.IntegrationStatus = IntegrationStatusEnum.Pending;
            payment.RunBackgroundService = true;
            payment.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Payment, Constants.Reposted));
        }
    }
}
