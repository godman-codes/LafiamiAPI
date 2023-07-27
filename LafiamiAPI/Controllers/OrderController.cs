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
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class OrderController : BaseController<OrderController>
    {
        public const string ControllerName = ControllerConstant.Order;
        private readonly IBusinessUnitofWork businessUnitofWork;
        private readonly OrderSettings orderSettings;
        public OrderController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<OrderController> logger, IBusinessUnitofWork businessUnitofWork, IOptions<OrderSettings> _config) : base(memoryCache, logger, _systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
            if (_config != null)
            {
                orderSettings = _config.Value;
            }
        }

        [HttpGet]
        [Route("GetIntegrationStatuses")]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetIntegrationStatuses()
        {
            List<EnumResponse> result = Enum.GetValues(typeof(IntegrationStatusEnum)).Cast<IntegrationStatusEnum>()
                .Select(r => new EnumResponse()
                {
                    Id = (byte)r,
                    Name = r.DisplayName()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetOrders")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, page, pageSize);
            List<LiteOrderResponse> orders = (List<LiteOrderResponse>)GetFromCache(cachename);
            if (orders == null)
            {
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => true));

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.User.Surname.Contains(keyword) ||
                        r.User.Firstname.Contains(keyword) ||
                        r.Surname.Contains(keyword) ||
                        r.Firstname.Contains(keyword) ||
                        (r.OrderId.Contains(keyword))
                    );
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                orders = await queryable.AsNoTracking()
                    .Select(r => new LiteOrderResponse()
                    {
                        Id = r.Id,
                        Email = r.ForSomeoneElse ? r.EmailAddress : r.User.Email,
                        PhoneNumber = r.ForSomeoneElse?r.PhoneNumber: r.User.PhoneNumber,
                        Firstname = r.ForSomeoneElse ? r.Firstname : r.User.Firstname,
                        Surname = r.ForSomeoneElse ? r.Surname : r.User.Surname,
                        OrderId = r.OrderId,
                        OrderStatusValue = r.OrderStatus.DisplayName(),
                        TotalAmount = r.TotalAmount,
                        IsApproved = (r.OrderStatus == OrderStatusEnum.Approved),
                        CreatedDate = r.CreatedDate,
                    })
                    .ToListAsync();

                foreach (LiteOrderResponse order in orders)
                {
                    order.ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(order.Id, null);
                }

                SavetoCache(orders, cachename);
            }


            return Ok(orders);
        }

        [HttpGet]
        [Route("GetHygeiaOrders")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteHygeiaOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHygeiaOrders(string keyword, IntegrationStatusEnum integrationStatus, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, integrationStatus, page, pageSize);
            List<LiteHygeiaOrderResponse> orders = (List<LiteHygeiaOrderResponse>)GetFromCache(cachename);
            if (orders == null)
            {
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => true));
                queryable = queryable.Where(r => (r.Company == CompanyEnum.Hygeia) && (r.IntegrationStatus == integrationStatus) && (r.OrderStatus == OrderStatusEnum.Approved));

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.User.Surname.Contains(keyword) ||
                        r.User.Firstname.Contains(keyword) ||
                        r.Surname.Contains(keyword) ||
                        r.Firstname.Contains(keyword) ||
                        (r.OrderId.Contains(keyword))
                    );
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                orders = await queryable.AsNoTracking()
                    .Select(r => new LiteHygeiaOrderResponse()
                    {
                        Id = r.Id,
                        Email = r.ForSomeoneElse ? r.EmailAddress : r.User.Email,
                        PhoneNumber = r.ForSomeoneElse ? r.PhoneNumber : r.User.PhoneNumber,
                        Firstname = r.ForSomeoneElse ? r.Firstname : r.User.Firstname,
                        Surname = r.ForSomeoneElse ? r.Surname : r.User.Surname,
                        OrderId = r.OrderId,
                        OrderStatusValue = r.OrderStatus.DisplayName(),
                        TotalAmount = r.TotalAmount,
                        IsApproved = (r.OrderStatus == OrderStatusEnum.Approved),
                        CreatedDate = r.CreatedDate,
                        IntegrationStatus = r.IntegrationStatus,
                        IntegrationErrorMessage = r.IntegrationErrorMessage,
                        HygeiaDependantId = r.HygeiaDependantId,
                        HygeiaLegacyCode = r.HygeiaLegacyCode,
                        HygeiaMemberId = r.HygeiaMemberId
                    })
                    .ToListAsync();

                foreach (LiteHygeiaOrderResponse order in orders)
                {
                    order.EnrolleeNumber = string.Join(Constants.BackSlash, order.HygeiaLegacyCode, order.HygeiaDependantId);
                    order.ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(order.Id, null);
                    order.IntegrationStatusName = order.IntegrationStatus.DisplayName();
                    order.IsIntegrationCompleted = (order.IntegrationStatus == IntegrationStatusEnum.Completed);
                    order.IsIntegrationFailed = (order.IntegrationStatus == IntegrationStatusEnum.Failed);
                    order.IsIntegrationPending = (order.IntegrationStatus == IntegrationStatusEnum.Pending);
                    order.IntegrationErrorMessage = WebUtility.HtmlDecode(order.IntegrationErrorMessage);
                }

                SavetoCache(orders, cachename);
            }


            return Ok(orders);
        }


        [HttpGet]
        [Route("GetAiicoOrders")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteAiicoOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAiicoOrders(string keyword, IntegrationStatusEnum integrationStatus, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, integrationStatus, page, pageSize);
            List<LiteAiicoOrderResponse> orders = (List<LiteAiicoOrderResponse>)GetFromCache(cachename);
            if (orders == null)
            {
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => true));
                queryable = queryable.Where(r => (r.Company == CompanyEnum.Aiico) && (r.IntegrationStatus == integrationStatus) && (r.OrderStatus == OrderStatusEnum.Approved));

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.User.Surname.Contains(keyword) ||
                        r.User.Firstname.Contains(keyword) ||
                        r.Surname.Contains(keyword) ||
                        r.Firstname.Contains(keyword) ||
                        (r.OrderId.Contains(keyword))
                    );
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                orders = await queryable.AsNoTracking()
                    .Select(r => new LiteAiicoOrderResponse()
                    {
                        Id = r.Id,
                        Email = r.ForSomeoneElse ? r.EmailAddress : r.User.Email,
                        PhoneNumber = r.ForSomeoneElse ? r.PhoneNumber : r.User.PhoneNumber,
                        Firstname = r.ForSomeoneElse ? r.Firstname : r.User.Firstname,
                        Surname = r.ForSomeoneElse ? r.Surname : r.User.Surname,
                        OrderId = r.OrderId,
                        OrderStatusValue = r.OrderStatus.DisplayName(),
                        TotalAmount = r.TotalAmount,
                        IsApproved = (r.OrderStatus == OrderStatusEnum.Approved),
                        CreatedDate = r.CreatedDate,
                        IntegrationStatus = r.IntegrationStatus,
                        IntegrationErrorMessage = r.IntegrationErrorMessage,
                        HospitalCashScheduleJsonResponse = r.IntegrationBackgroundJsonResponse,
                    })
                    .ToListAsync();

                foreach (LiteAiicoOrderResponse order in orders)
                {
                    order.ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(order.Id, null);
                    order.IntegrationStatusName = order.IntegrationStatus.DisplayName();
                    order.IsIntegrationCompleted = (order.IntegrationStatus == IntegrationStatusEnum.Completed);
                    order.IsIntegrationFailed = (order.IntegrationStatus == IntegrationStatusEnum.Failed);
                    order.IsIntegrationPending = (order.IntegrationStatus == IntegrationStatusEnum.Pending);
                    order.IntegrationErrorMessage = WebUtility.HtmlDecode(order.IntegrationErrorMessage);
                    order.HospitalCashScheduleJsonResponse = WebUtility.HtmlDecode(order.HospitalCashScheduleJsonResponse);
                }

                SavetoCache(orders, cachename);
            }


            return Ok(orders);
        }

        [HttpGet]
        [Route("GetRelianceHMOOrders")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteRelianceHMOOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRelianceHMOOrders(string keyword, IntegrationStatusEnum integrationStatus, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, integrationStatus, page, pageSize);
            List<LiteRelianceHMOOrderResponse> orders = (List<LiteRelianceHMOOrderResponse>)GetFromCache(cachename);
            if (orders == null)
            {
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => true));
                queryable = queryable.Where(r => (r.Company == CompanyEnum.RelainceHMO) && (r.IntegrationStatus == integrationStatus) && (r.OrderStatus == OrderStatusEnum.Approved));

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        r.User.Surname.Contains(keyword) ||
                        r.User.Firstname.Contains(keyword) ||
                        r.Surname.Contains(keyword) ||
                        r.Firstname.Contains(keyword) ||
                        (r.OrderId.Contains(keyword))
                    );
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                orders = await queryable.AsNoTracking()
                    .Select(r => new LiteRelianceHMOOrderResponse()
                    {
                        Id = r.Id,
                        Email = r.ForSomeoneElse ? r.EmailAddress : r.User.Email,
                        PhoneNumber = r.ForSomeoneElse ? r.PhoneNumber : r.User.PhoneNumber,
                        Firstname = r.ForSomeoneElse ? r.Firstname : r.User.Firstname,
                        Surname = r.ForSomeoneElse ? r.Surname : r.User.Surname,
                        HouseAddress = r.ForSomeoneElse ? r.Address : r.User.Address,
                        DateOfBirth = r.ForSomeoneElse ? r.DateOfBirth : r.User.DateOfBirth,
                        Sex = r.ForSomeoneElse ? r.Sex : r.User.Sex,
                        ProfilePicture = r.ForSomeoneElse ? r.Picture : r.User.Picture,

                        OrderId = r.OrderId,
                        OrderStatusValue = r.OrderStatus.DisplayName(),
                        TotalAmount = r.TotalAmount,
                        IsApproved = (r.OrderStatus == OrderStatusEnum.Approved),
                        IsIntegrationPending = (r.IntegrationStatus == IntegrationStatusEnum.Pending),
                        CreatedDate = r.CreatedDate,
                        IntegrationStatus = r.IntegrationStatus,
                        IntegrationErrorMessage = r.IntegrationErrorMessage,
                        Hospital = (r.HospitalId.HasValue) ? (r.Hospital.Name) : (string.Empty),
                        StateOfResidence = r.User.StateId.HasValue ? r.User.State.Name : string.Empty,
                    })
                    .ToListAsync();

                foreach (LiteRelianceHMOOrderResponse order in orders)
                {
                    if (order.Sex.HasValue)
                    {
                        order.Gender = order.Sex.Value.DisplayName();
                    }
                    order.ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(order.Id, null);
                    order.IntegrationStatusName = order.IntegrationStatus.DisplayName();
                    order.IntegrationErrorMessage = WebUtility.HtmlDecode(order.IntegrationErrorMessage);
                }

                SavetoCache(orders, cachename);
            }


            return Ok(orders);
        }

        [HttpGet]
        [Route("GetMyOrders")]
        [Authorize]
        [ProducesResponseType(typeof(List<MyLiteOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyOrders(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + (string.IsNullOrEmpty(keyword) ? string.Empty : keyword) + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<MyLiteOrderResponse> orders = (List<MyLiteOrderResponse>)GetFromCache(cachename);
            if (orders == null)
            {
                string userId = GetUserId();
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => r.UserId == userId));
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryable = queryable.Where(r =>
                        (r.OrderId.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    );
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                orders = await queryable.AsNoTracking()
                    .Select(r => new MyLiteOrderResponse()
                    {
                        Id = r.Id,
                        OrderId = r.OrderId,
                        OrderStatusValue = r.OrderStatus.DisplayName(),
                        TotalAmount = r.TotalAmount,
                        IsApproved = (r.OrderStatus == OrderStatusEnum.Approved),
                        CreatedDate = r.CreatedDate
                    })
                    .ToListAsync();

                foreach (MyLiteOrderResponse order in orders)
                {
                    order.ItemNames = await businessUnitofWork.CartService.GetOrderItemNames(order.Id, null);
                }

                SavetoCache(orders, cachename);
            }


            return Ok(orders);
        }


        [HttpGet]
        [Route("GetPendingOrderCounts")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingOrderCounts()
        {
            string cachename = GetMethodName();
            int? result = (int?)GetFromCache(cachename);
            if (result == null)
            {
                result = await businessUnitofWork.OrderService.GetQueryable((r => r.OrderStatus == OrderStatusEnum.Pending)).CountAsync();

                SavetoCache(result, cachename);
            }

            return Ok(result ?? 0);
        }

        [HttpGet]
        [Route("CanViewOrder")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanViewOrder(string orderId)
        {
            string userId = GetUserId();
            var order = await businessUnitofWork.OrderService.GetQueryable((r => r.OrderId == orderId))
                            .Select(r => new { r.Id, r.UserId })
                            .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            else
            {
                return Ok((string.IsNullOrEmpty(order.UserId)) ? (true) : (order.UserId == userId));
            }
        }


        [HttpGet]
        [Route("GetOrder")]
        [Authorize]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            string cachename = GetMethodName() + orderId;
            OrderResponse order = (OrderResponse)GetFromCache(cachename);
            if (order == null)
            {
                IQueryable<OrderModel> queryable = businessUnitofWork.OrderService.GetQueryable((r => r.OrderId == orderId));

                var _order = await queryable.AsNoTracking()
                    .Select(r => new
                    {
                        r.Id,
                        Email = r.ForSomeoneElse ? r.EmailAddress : r.User.Email,
                        PhoneNumber = r.ForSomeoneElse ? r.PhoneNumber : r.User.PhoneNumber,
                        Firstname = r.ForSomeoneElse ? r.Firstname : r.User.Firstname,
                        Surname = r.ForSomeoneElse ? r.Surname : r.User.Surname,
                        Address = r.ForSomeoneElse ? r.Address : r.User.Address,
                        r.OrderStatus,
                        r.TotalAmount,
                        Vat = r.Vat ?? 0,
                        r.OrderId,
                        r.CreatedDate,
                        r.UserId,
                        r.DueDate,
                    })
                    .SingleOrDefaultAsync();

                if (!await systemUnitofWork.UserService.IsAdminUser(GetUserId()))
                {
                    if (!string.IsNullOrEmpty(_order.UserId) && !(_order.UserId == GetUserId()))
                    {
                        throw new UnauthorizedAccessException("Kindly login to view this Order. Thank you.");
                    }
                }

                if (_order != null)
                {
                    PaymentLiteInformation payment = await systemUnitofWork.PaymentService.GetLastLitePayment(_order.Id);
                    List<OrderItemsResponse> orderItems = await businessUnitofWork.OrderService.GetFullOrderItems(_order.Id);

                    order = new OrderResponse()
                    {
                        Address = _order.Address,
                        Email = _order.Email,
                        Firstname = _order.Firstname,
                        Id = _order.Id,
                        IsApproved = (_order.OrderStatus == OrderStatusEnum.Approved),
                        IsPending = ((_order.OrderStatus == OrderStatusEnum.Pending) && ((payment != null) ? (payment.PaymentStatus == PaymentStatusEnum.Pending) : (true))),
                        IsAwaitingVerification = (payment != null) ? ((payment.PaymentStatus == PaymentStatusEnum.AwaitingVerification)) : (false),
                        IsFailed = (payment != null) ? ((payment.PaymentStatus == PaymentStatusEnum.Failed)) : (false),
                        OrderId = _order.OrderId,
                        OrderStatusValue = _order.OrderStatus.DisplayName(),
                        PhoneNumber = _order.PhoneNumber,
                        Surname = _order.Surname,
                        TotalAmount = _order.TotalAmount,
                        Vat = _order.Vat,
                        OrderItems = orderItems,
                        PaymentGatewayId = (payment != null) ? (payment.PaymentGateway) : (0),
                        PaymentId = (payment != null) ? (payment.PaymentId) : (Guid.Empty),
                        HasPaid = (payment != null) ? (payment.PaymentStatus == PaymentStatusEnum.Paid) : (false),
                        PaymentStatus = (payment != null) ? (payment.PaymentStatus.DisplayName()) : (PaymentStatusEnum.Pending.DisplayName()),
                        CreatedDate = _order.CreatedDate,
                        DueDate = _order.DueDate,
                        AmountInWords = _order.TotalAmount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit),
                    };
                    SavetoCache(order, cachename);
                }
                else
                {
                    order = new OrderResponse();
                }
            }

            return Ok(order);
        }


        [HttpPost]
        [Route("CreateOrder")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder([FromBody] NewOrderRequest model)
        {
            string userId = GetUserId();
            if (!await systemUnitofWork.UserService.DoesUserHasDateOfBirth(userId))
            {
                throw new WebsiteException("Your Date of Birth is required. Kindly go to your profile to update it. Thank you");
            }

            bool hasValidPrincipalMemberId = false;
            foreach (CompanyOrderExtraResultRequest companyExtraResult in model.CompanyExtraResults)
            {
                switch (companyExtraResult.AnswerType)
                {
                    case AnswerTypeEnum.String:
                        if (string.IsNullOrEmpty(companyExtraResult.ResultInString))
                        {
                            if (!await businessUnitofWork.InsuranceService.IsHygeiaPrincipalMemberIdCompanyExtra(companyExtraResult.CompanyExtraId)){
                                throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                            }
                        }
                        else {
                            if ((await businessUnitofWork.InsuranceService.IsHygeiaPrincipalMemberIdCompanyExtra(companyExtraResult.CompanyExtraId)))
                            {
                                if (await businessUnitofWork.OrderService.DoesHygeiaPrincipalMemberIdExist(companyExtraResult.ResultInString))
                                {
                                    hasValidPrincipalMemberId = true;
                                }
                                else
                                {
                                    throw new WebsiteException(string.Format(Constants.InvalidInformation, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                                }
                            }
                        }
                        break;
                    case AnswerTypeEnum.Dropdown:
                        if (string.IsNullOrEmpty(companyExtraResult.ResultInString))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    case AnswerTypeEnum.Number:
                        if (!companyExtraResult.ResultInNumber.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    case AnswerTypeEnum.Decimal:
                        if (!companyExtraResult.ResultInDecimal.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    case AnswerTypeEnum.DateTime:
                        if (!companyExtraResult.ResultInDateTime.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    case AnswerTypeEnum.HTML:
                        if (string.IsNullOrEmpty(companyExtraResult.ResultInHTML))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    case AnswerTypeEnum.Image:
                        if (string.IsNullOrEmpty(companyExtraResult.ResultInString))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, await businessUnitofWork.InsuranceService.GetCompanyExtraDisplayName(companyExtraResult.CompanyExtraId)));
                        }
                        break;
                    default:
                        break;
                }
            }

            CartModel cart = await businessUnitofWork.CartService.GetMyCartQueryable(userId)
                .Include(r => r.InsurancePlan)
                .SingleOrDefaultAsync();

            if (cart.InsurancePlanId.HasValue && (cart.InsurancePlan.Company == CompanyEnum.RelainceHMO) && (!model.HospitalId.HasValue || (model.HospitalId <= 0)))
            {
                throw new WebsiteException(string.Format(Constants.IsRequired, Constants.Hospital));
            }

            decimal totalAmount = 0M;
            decimal amount = (cart.InsurancePlanId.HasValue) ? (await businessUnitofWork.InsuranceService.GetAmount(cart.InsurancePlanId.Value, DateTime.Now, cart.QuatityOrder, cart.QuatityOrder)).Amount : (cart.Amount);
            totalAmount += decimal.Multiply(cart.QuatityOrder, amount);

            DateTime orderDate = DateTime.Now;
            decimal vatAmount = businessUnitofWork.OrderService.GetVatAmount(orderSettings, totalAmount);

            PaymentModel payment = new PaymentModel()
            {
                Id = Guid.NewGuid(),
                Amount = totalAmount + vatAmount,
                PaymentStatus = PaymentStatusEnum.Pending,
                TransactionId = systemUnitofWork.PaymentService.GeneratePaymentTransactionId(),
            };

            if (hasValidPrincipalMemberId) {
                payment.PaymentStatus = PaymentStatusEnum.Paid;
            }

            OrderModel order = new OrderModel()
            {
                Id = Guid.NewGuid(),
                OrderId = businessUnitofWork.OrderService.GenerateOrderId(),
                OrderStatus = (totalAmount == 0) ? OrderStatusEnum.Approved : (hasValidPrincipalMemberId? OrderStatusEnum.Approved:  OrderStatusEnum.Pending),
                TotalAmount = totalAmount + vatAmount,
                UserId = (string.IsNullOrEmpty(userId) ? null : userId),
                Vat = vatAmount,
                DueDate = orderDate,
                Company = cart.InsurancePlan.Company,
                HospitalId = (model.HospitalId > 0) ? (model.HospitalId) : null,
                RunBackgroundService = ((cart.InsurancePlan.Company == CompanyEnum.Aiico) || (cart.InsurancePlan.Company == CompanyEnum.Hygeia)),//Just For Aiico or Hygeia for now 
                IntegrationStatus = IntegrationStatusEnum.Pending,
                Payments = (totalAmount == 0) ? new List<PaymentModel>() : new List<PaymentModel>() { payment},
                Cart = cart,
                UserCompanyExtraResults = model.CompanyExtraResults.Select(r => new UserCompanyExtraResultModel()
                {
                    CompanyExtraId = r.CompanyExtraId,
                    ResultInDateTime = r.ResultInDateTime,
                    ResultInDecimal = r.ResultInDecimal,
                    ResultInHTML = r.ResultInHTML,
                    ResultInNumber = r.ResultInNumber,
                    UserId = userId,
                    ResultInString = r.ResultInString,
                    ResultInBool = r.ResultInBool
                }).ToList(),
                ForSomeoneElse = model.ForSomeoneElse
            };

            if (model.ForSomeoneElse)
            {
                order.Surname = model.SomeoneElse.Surname;
                order.Firstname = model.SomeoneElse.Firstname;
                order.PhoneNumber = model.SomeoneElse.PhoneNumber;
                order.EmailAddress = model.SomeoneElse.EmailAddress;
                order.Picture = model.SomeoneElse.Picture;
                order.MiddleName = model.SomeoneElse.MiddleName;
                order.Address = model.SomeoneElse.Address;
                order.Sex = model.SomeoneElse.Sex;
                order.DateOfBirth = model.SomeoneElse.DateOfBirth;

                if (model.NextOfKin != null)
                {
                    order.NextOfKin = model.NextOfKin.ToDBModel();
                    order.NextOfKin.UserId = null;
                }
                if (model.Bank != null)
                {
                    order.BankInformation = model.Bank.ToDBModel();
                    order.BankInformation.UserId = null;
                }
                if (model.Work != null)
                {
                    order.Job = model.Work.ToDBModel();
                    order.Job.UserId = null;
                }
                if (model.Identification != null)
                {
                    order.Identification = model.Identification.ToDBModel();
                    order.Identification.UserId = null;
                }
            }
            else
            {
                if (model.Bank != null)
                {
                    model.Bank.UserId = userId;
                    await businessUnitofWork.UserService.SaveOrUpdateBank(model.Bank);
                }
                if (model.Work != null)
                {
                    model.Work.UserId = userId;
                    await businessUnitofWork.UserService.SaveOrUpdateJob(model.Work);
                }
                if (model.NextOfKin != null)
                {
                    model.NextOfKin.UserId = userId;
                    await businessUnitofWork.UserService.SaveOrUpdateNextOfKins(model.NextOfKin);
                }
                if (model.Identification != null)
                {
                    model.Identification.UserId = userId;
                    await businessUnitofWork.UserService.SaveOrUpdateIdentification(model.Identification);
                }
            }

            businessUnitofWork.OrderService.Add(order);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            (new CartController(cache, null, null, null)).ClearCache();
            return Ok(string.Join(string.Empty, Constants.DefaultOrderPath, Constants.OrderQueryParameter, order.OrderId));
        }

        [HttpPost]
        [Route("CancelOrder")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrRejectOrderRequest model)
        {
            string userId = GetUserId();
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, userId);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            if (order.OrderStatus != OrderStatusEnum.Approved)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Order, Constants.Cancel));
            }

            order.OrderStatus = OrderStatusEnum.Cancelled;
            order.ReasonForCanceling = model.Reason;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Order, Constants.Cancelled));
        }

        [HttpPost]
        [Route("RejectOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectOrder([FromBody] CancelOrRejectOrderRequest model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            if (order.OrderStatus != OrderStatusEnum.Approved)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Order, Constants.Reject));
            }

            order.OrderStatus = OrderStatusEnum.Cancelled;
            order.ReasonForRejecting = model.Reason;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Order, Constants.Rejected));
        }

        [HttpPost]
        [Route("ApproveOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveOrder([FromBody] IdRequest<Guid> model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            if (order.OrderStatus != OrderStatusEnum.Pending)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Order, Constants.Approve));
            }

            order.OrderStatus = OrderStatusEnum.Approved;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Order, Constants.Approved));
        }



        [HttpPost]
        [Route("CancelRelianceHMOOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelRelianceHMOOrder([FromBody] CancelOrRejectOrderRequest model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            if (order.Company != CompanyEnum.RelainceHMO)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.RelainceHMOOrder));
            }

            if (order.IntegrationStatus != IntegrationStatusEnum.Pending)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, string.Join(Constants.Space, Constants.Order, Constants.Integration), Constants.Cancel));
            }

            order.IntegrationStatus = IntegrationStatusEnum.Failed;
            order.IntegrationErrorMessage = model.Reason;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, string.Join(Constants.Space, Constants.Order, Constants.Integration), Constants.Cancelled));
        }

        [HttpPost]
        [Route("CompleteRelianceHMOOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompleteRelianceHMOOrder([FromBody] IdRequest<Guid> model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }
            if (order.Company != CompanyEnum.RelainceHMO)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.RelainceHMOOrder));
            }
            if (order.IntegrationStatus != IntegrationStatusEnum.Pending)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, string.Join(Constants.Space, Constants.Order, Constants.Integration), Constants.Cancel));
            }

            order.IntegrationStatus = IntegrationStatusEnum.Completed;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, string.Join(Constants.Space, Constants.Order, Constants.Integration), Constants.Completed));
        }


        [HttpPost]
        [Route("RepostAiicoOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RepostAiicoOrder([FromBody] IdRequest<Guid> model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }

            if (order.Company != CompanyEnum.Aiico)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, string.Join(Constants.Space, Constants.Aiico, Constants.Order)));
            }

            if (order.IntegrationStatus != IntegrationStatusEnum.Failed)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Order, Constants.Repost));
            }

            order.IntegrationStatus = IntegrationStatusEnum.Pending;
            order.RunBackgroundService = true;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Order, Constants.Reposted));
        }


        [HttpPost]
        [Route("RepostHygeiaOrder")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RepostHygeiaOrder([FromBody] IdRequest<Guid> model)
        {
            OrderModel order = await businessUnitofWork.OrderService.GetOrderAsync(model.Id, null);
            if (order == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Order));
            }

            if (order.Company != CompanyEnum.Hygeia)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, string.Join(Constants.Space, Constants.Hygeia, Constants.Order)));
            }

            if (order.IntegrationStatus != IntegrationStatusEnum.Failed)
            {
                throw new WebsiteException(string.Format(Constants.NotAllowedToPerformActionOnRecord, Constants.Order, Constants.Repost));
            }

            order.IntegrationStatus = IntegrationStatusEnum.Pending;
            order.RunBackgroundService = true;
            order.UpdatedDate = DateTime.Now;
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Order, Constants.Reposted));
        }
    }
}
