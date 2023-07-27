using Lafiami.Models.Internals;
using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Services.BusinessServices;
using LafiamiAPI.Services.SystemServices;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BackgroundServices
{
    public class LafiamiBackgroundService : BackgroundService
    {
        public IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private const int page = 0;
        private const int pageSize = 100;
        private List<string> cacheNames = new List<string>();
        private readonly SMTPSettings sMTPSettings;
        private readonly WebsiteSettings websiteSettings;
        private readonly IOptions<AppSettings> appSettingsConfig;

        public LafiamiBackgroundService(IServiceScopeFactory serviceScopeFactory,
            ILogger<LafiamiBackgroundService> logger, IOptions<SMTPSettings> _config,
            IOptions<WebsiteSettings> _websiteconfig,
            IOptions<AppSettings> _appSettingconfig
            )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            sMTPSettings = _config.Value;
            websiteSettings = _websiteconfig.Value;
            appSettingsConfig = _appSettingconfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            try
            {
                //Do your preparation (e.g. Start code) here
                while (!stopToken.IsCancellationRequested)
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    IServiceProvider services = scope.ServiceProvider;
                    LafiamiContext context = services.GetService<LafiamiContext>();
                    ManageCacheService manageCacheService = new ManageCacheService(context);

                    await NewUserAccountNotifications(context, services, manageCacheService);
                    await NewWalletNotification(context, manageCacheService);
                    await NewOrderNotifications(context, manageCacheService);
                    await PendingPaymentNotification(context, 1, manageCacheService);
                    await PendingPaymentNotification(context, 2, manageCacheService);
                    await PendingPaymentNotification(context, 3, manageCacheService);

                    await UpdateWalletOrderDependencyDetail(context, manageCacheService);

                    await SendEmailsInBatches(context, manageCacheService);
                    await PostHospitalCashScheduleForAiico(context, manageCacheService);
                    await FinalizedPartnerPaymentForAiico(context, manageCacheService);


                    await PostRegistrationForHygeia(context, manageCacheService);

                    //cacheTasks = new List<Task>();
                    cacheNames = new List<string>();
                }
                //Do your cleanup (e.g. Stop code) here
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error Logger Initialzed");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex);

            }
        }

        private void AddToCache(ManageCacheService manageCacheService, string sourceName)
        {
            string cacheName = sourceName;
            if (!cacheNames.Any(r => r == cacheName))
            {
                manageCacheService.AddToManageCache(sourceName).GetAwaiter().GetResult();

                cacheNames.Add(cacheName);
            }
        }

        private async Task<string> GetHygeiaToken()
        {
            WebAPIService webAPI = new WebAPIService(appSettingsConfig);
            Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "username", appSettingsConfig.Value.HygeiaLafimiUsername },
                            { "password", appSettingsConfig.Value.HygeiaLafimiPassword },
                            { "grant_type", appSettingsConfig.Value.HygeiaLafimiGrant }
                        };

            HygeiaTokenResponse result = await webAPI.PostFormDataAsEncoded<HygeiaTokenResponse>("tpi-ms/oauth2/token", values, CompanyEnum.Hygeia, string.Empty);

            return result.access_token;
        }

        private async Task PostRegistrationForHygeia(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                WebAPIService webAPI = new WebAPIService(appSettingsConfig);
                OrderService orderService = new OrderService(context);
                EmailService emailService = new EmailService(context);

                List<OrderModel> pendingOrders = await orderService.GetQueryable(r => r.RunBackgroundService && (r.IntegrationStatus == IntegrationStatusEnum.Pending) && (r.Company == CompanyEnum.Hygeia) && (r.OrderStatus == OrderStatusEnum.Approved))
                    .Include(r => r.Cart)
                    .ThenInclude(r => r.InsurancePlan)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (OrderModel order in pendingOrders)
                {
                    try
                    {
                        UserViewModel user = await context.Users.Where(r => r.Id == order.UserId).SingleOrDefaultAsync();
                        if (!user.DateOfBirth.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Date of Birth"));
                        }
                        if (string.IsNullOrEmpty(user.Picture))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "User Picture"));
                        }

                        string StateId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "StateId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(StateId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "State"));
                        }

                        string MaritalStatusId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "MaritalStatusId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(MaritalStatusId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Marital Status"));
                        }

                        string TitleId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "TitleId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(TitleId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Title"));
                        }
                        string LGAId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "LGAId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(LGAId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "LGA"));
                        }

                        string PrincipalMemberId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "PrincipalMemberId")).Select(r => r.ResultInString).SingleOrDefaultAsync();

                        string CoverageClassId = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "CoverageClassId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(CoverageClassId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Coverage Class"));
                        }

                        string DependantTypeId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "DependantTypeId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(DependantTypeId))
                        {
                            DependantTypeId = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "DependantTypeId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                            if (string.IsNullOrEmpty(DependantTypeId))
                            {
                                throw new WebsiteException(string.Format(Constants.IsRequired, "Dependant Type"));
                            }
                        }

                        string PlanId = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "PlanId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(PlanId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Plan Id"));
                        }
                        
                        //bool? IsDependant = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "IsDependant")).Select(r => r.ResultInBool).SingleOrDefaultAsync();



                        int durationInYear = (order.Cart.QuatityOrder / 12);

                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "Phone", order.ForSomeoneElse?order.PhoneNumber:user.PhoneNumber },
                            { "PlanId", PlanId.ToString() },
                            { "LGAId", LGAId },
                            { "IsDependant", (!string.IsNullOrEmpty(PrincipalMemberId)).ToString() },
                            { "PrincipalMemberId", PrincipalMemberId },
                            { "LastName", order.ForSomeoneElse?order.Surname:user.Surname },
                            { "FirstName", order.ForSomeoneElse?order.Firstname:user.Firstname },
                            { "DOB", order.ForSomeoneElse?order.DateOfBirth.Value.ToString("yyyy-MM-dd"):user.DateOfBirth.Value.ToString("yyyy-MM-dd") },//1998-12-3
                            { "GenderId", ((order.ForSomeoneElse?order.Sex:user.Sex) == SexEnums.Male)?"1":"2" },
                            { "ImagePath", ImageToBase64String((order.ForSomeoneElse?order.Picture:user.Picture), 200, 200) },
                            { "Email", order.ForSomeoneElse?order.EmailAddress:user.Email },
                            { "Address", order.ForSomeoneElse?order.Address:user.Address },
                            { "StateId", StateId },
                            { "MaritalStatusId", MaritalStatusId },
                            { "TitleId", TitleId },
                            { "CoverageClassId", CoverageClassId },
                            { "DependantTypeId", DependantTypeId },
                            { "DataConsent", true.ToString() },
                        };

                        //_logger.LogError(JsonSerializer.Serialize(values));

                        string bearerToken = await GetHygeiaToken();
                        HygeiaRegistrationResponse result = await webAPI.PostFormDataAsEncoded<HygeiaRegistrationResponse>("tpi-ms/api/registration/retail", values, CompanyEnum.Hygeia, bearerToken);


                        order.RunBackgroundService = false;
                        order.IntegrationBackgroundJsonResponse = WebUtility.HtmlEncode(JsonSerializer.Serialize(result) ?? string.Empty);
                        if (result.success)
                        {
                            order.IntegrationStatus = IntegrationStatusEnum.Completed;
                            order.HygeiaMemberId = result.memberId.ToString();
                            order.HygeiaLegacyCode = result.legacyCode;
                            order.HygeiaDependantId = result.dependantId;

                            //Send Email of the Enrollee Id to the Client
                            emailService.HygeiaCompletionEmailNotification(order.Id, (order.ForSomeoneElse?order.EmailAddress:user.Email));
                        }
                        else
                        {
                            order.IntegrationStatus = IntegrationStatusEnum.Failed;
                            order.IntegrationErrorMessage = string.IsNullOrEmpty(result.message)? "Unknown Issue": result.message;

                            emailService.IntegrationIssueEmailNotification(order.Id, appSettingsConfig.Value.AdminEmails);
                            AddToCache(manageCacheService, ControllerConstant.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        order.RunBackgroundService = false;
                        order.IntegrationStatus = IntegrationStatusEnum.Failed;
                        order.IntegrationErrorMessage = WebUtility.HtmlEncode(ex.Message ?? string.Empty);

                        emailService.IntegrationIssueEmailNotification(order.Id, appSettingsConfig.Value.AdminEmails);
                        AddToCache(manageCacheService, ControllerConstant.Email);
                    }
                    orderService.Update(order);
                }

                AddToCache(manageCacheService, ControllerConstant.Order);
                AddToCache(manageCacheService, ControllerConstant.Payment);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task PostHospitalCashScheduleForAiico(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                WebAPIService webAPI = new WebAPIService(appSettingsConfig);
                OrderService orderService = new OrderService(context);
                EmailService emailService = new EmailService(context);

                List<OrderModel> pendingOrders = await orderService.GetQueryable(r => r.RunBackgroundService && (r.IntegrationStatus == IntegrationStatusEnum.Pending) && (r.Company == CompanyEnum.Aiico))
                    .Include(r => r.BankInformation)
                    .Include(r => r.NextOfKin)
                    .Include(r => r.Identification)
                    .Include(r => r.Job)
                    .Include(r => r.Cart)
                    .ThenInclude(r => r.InsurancePlan)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (OrderModel order in pendingOrders)
                {
                    try
                    {
                        UserViewModel user = await context.Users.Where(r => r.Id == order.UserId).SingleOrDefaultAsync();
                        if (!user.DateOfBirth.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Date of Birth"));
                        }

                        BankInformationModel bank = order.ForSomeoneElse ? order.BankInformation : await context.Banks.Where(r => (r.UserId == order.UserId) && r.UseAsDefault).SingleOrDefaultAsync();
                        NextOfKinModel nextofKin = order.ForSomeoneElse ? order.NextOfKin : await context.NextOfKins.Where(r => (r.UserId == order.UserId) && r.UseAsDefault).SingleOrDefaultAsync();
                        IdentificationModel identification = order.ForSomeoneElse ? order.Identification : await context.Identifications.Where(r => (r.UserId == order.UserId) && r.UseAsDefault).SingleOrDefaultAsync();
                        JobModel work = order.ForSomeoneElse ? order.Job : await context.Jobs.Where(r => (r.UserId == order.UserId) && r.IsCurrentJob).SingleOrDefaultAsync();


                        DateTime? effectiveDate = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "effectiveDate")).Select(r => r.ResultInDateTime).SingleOrDefaultAsync();
                        if (!effectiveDate.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Effective Date"));
                        }
                        string Gender = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "Genders")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(Gender))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Gender"));
                        }
                        string Hospital = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "Hospital")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(Hospital))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Hospital"));
                        }
                        string LgaId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "LgaId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(LgaId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "LGA"));
                        }

                        string titleId = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "titleId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(titleId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Title"));
                        }

                        string utilityBillName = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "utilityBillName")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(utilityBillName))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Utility Bill Name"));
                        }
                        string utilityBillUrl = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "utilityBillUrl")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(utilityBillUrl))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Utility Bill"));
                        }

                        DateTime? wef = await context.UserCompanyExtraResults.Where(r => (r.OrderId == order.Id) && (r.CompanyExtra.Name == "wef")).Select(r => r.ResultInDateTime).SingleOrDefaultAsync();
                        if (!wef.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Starting Policy Date"));
                        }

                        string productId = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "productId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(productId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Product Id"));
                        }
                        string subclassSectCovtypeId = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "subclassSectCovtypeId")).Select(r => r.ResultInString).SingleOrDefaultAsync();
                        if (string.IsNullOrEmpty(subclassSectCovtypeId))
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "Sub Class Cover Type"));
                        }
                        decimal? sumAssured = await context.PlanCompanyExtraResults.Where(r => (r.InsurancePlanId == order.Cart.InsurancePlanId) && (r.CompanyExtra.Name == "sumAssured")).Select(r => r.ResultInDecimal).SingleOrDefaultAsync();
                        if (!sumAssured.HasValue)
                        {
                            throw new WebsiteException(string.Format(Constants.IsRequired, "sumAssured"));
                        }

                        int durationInYear = (order.Cart.QuatityOrder / 12);
                        DateTime wet = wef.Value.AddMonths(order.Cart.QuatityOrder);

                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "utilityBillName", utilityBillName },
                            { "utilityBillUrl", utilityBillUrl },

                            { "accountNumber", bank.AccountNumber },
                            { "address", order.ForSomeoneElse?order.Address:user.Address },
                            { "bankName", bank.BankName },
                            { "dob", order.ForSomeoneElse?order.DateOfBirth.Value.ToString("u"):user.DateOfBirth.Value.ToString("u") },
                            { "effectiveDate", effectiveDate.Value.ToString("u") },
                            { "email", order.ForSomeoneElse?order.EmailAddress:user.Email },
                            { "Genders",Gender },
                            { "Hospital", Hospital },
                            { "identificationName",string.Join(Constants.Space, order.ForSomeoneElse?order.Surname:user.Surname, order.ForSomeoneElse?order.Firstname:user.Firstname, order.ForSomeoneElse?order.MiddleName:user.MiddleName) },
                            { "identificationUrl", identification.IdUrl },
                            { "LgaId", LgaId },
                            { "nextOfKinAddress", nextofKin.Address },
                            { "nextOfKinName", string.Join(Constants.Space, nextofKin.Surname, nextofKin.Firstname) },
                            { "nextOfKinPhone", nextofKin.Phone },
                            { "nextOfKinRelationship", nextofKin.Relationship },
                            { "Occupation", work.JobTitle },
                            { "otherNames", order.ForSomeoneElse?order.Firstname:user.Firstname },
                            { "phoneNumber", order.ForSomeoneElse?order.PhoneNumber:user.PhoneNumber },
                            { "SurName", order.ForSomeoneElse?order.Surname:user.Surname },
                            { "titleId", titleId },
                            { "wef", wef.Value.ToString("u") },
                            { "wet", wet.ToString("u") },
                            { "productId", productId },
                            { "subclassSectCovtypeId", subclassSectCovtypeId },
                            { "sumAssured", decimal.Multiply(sumAssured.Value, durationInYear).ToString() },
                            { "durationOfCover", durationInYear.ToString() },
                            { "paymentFrequency", "Yearly" },
                        };

                        HospitalScheduleResponse result = await webAPI.PostFormDataAsJson<HospitalScheduleResponse>("HospitalCashProductService/PostHospitalCashSchedule", values, CompanyEnum.Aiico);

                        order.RunBackgroundService = false;
                        order.IntegrationBackgroundJsonResponse = WebUtility.HtmlEncode(JsonSerializer.Serialize(result) ?? string.Empty);
                        if (result.success)
                        {
                            decimal.TryParse(result.result.premium, out decimal premium);

                            order.IntegrationStatus = IntegrationStatusEnum.Completed;
                            order.AiicoPremiumAmount = premium;
                            order.AiicoTransactionRef = result.result.transactionRef;
                        }
                        else
                        {
                            order.IntegrationStatus = IntegrationStatusEnum.Failed;
                            order.IntegrationErrorMessage = WebUtility.HtmlEncode(result.error ?? string.Empty);

                            emailService.IntegrationIssueEmailNotification(order.Id, appSettingsConfig.Value.AdminEmails);
                            AddToCache(manageCacheService, ControllerConstant.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        order.RunBackgroundService = false;
                        order.IntegrationStatus = IntegrationStatusEnum.Failed;
                        order.IntegrationErrorMessage = WebUtility.HtmlEncode(ex.Message ?? string.Empty);

                        emailService.IntegrationIssueEmailNotification(order.Id, appSettingsConfig.Value.AdminEmails);
                        AddToCache(manageCacheService, ControllerConstant.Email);
                    }
                    orderService.Update(order);
                }

                AddToCache(manageCacheService, ControllerConstant.Order);
                AddToCache(manageCacheService, ControllerConstant.Payment);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private string ImageToBase64String(string imageUrl, int width, int height)
        {
            if (string.IsNullOrEmpty(imageUrl)) { return string.Empty; }
            WebClient webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(imageUrl);

            //for rezising the image
            using MemoryStream ms = new MemoryStream(imageBytes);
            byte[] resizeImageBytes = ImageToByteArray(ResizeImage(ms, width, height));

            string base64String = Convert.ToBase64String(resizeImageBytes);
            return base64String;
        }

        private Image ResizeImage(Stream stream, int finalWidth, int finalHeight)
        {
            using Image img = Image.FromStream(stream);
            int originalWidth = img.Size.Width;
            int originalHeight = img.Size.Height;

            //how many units are there to make the original length
            float hRatio = (float)((finalHeight == 0) ? (float.MaxValue) : (originalHeight / (float)finalHeight));
            float wRatio = (float)((finalWidth == 0) ? (float.MaxValue) : (originalWidth / (float)finalWidth));

            //get the shorter side
            float ratio = Math.Min(hRatio, wRatio);

            if (finalHeight == 0)
            {
                finalHeight = Convert.ToInt32(originalHeight / ratio);
            }

            if (finalWidth == 0)
            {
                finalWidth = Convert.ToInt32(originalWidth / ratio);
            }

            int hScale = Convert.ToInt32(finalHeight * ratio);
            int wScale = Convert.ToInt32(finalWidth * ratio);

            //start cropping from the center
            int startX = (originalWidth - wScale) / 2;
            int startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            Rectangle sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            Rectangle destinationRectangle = new Rectangle(0, 0, finalWidth, finalHeight);

            //fill-in the whole bitmap
            Bitmap res = new Bitmap(finalWidth, finalHeight);
            using (Graphics graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(img, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }
            return res;
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private async Task FinalizedPartnerPaymentForAiico(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                WebAPIService webAPI = new WebAPIService(appSettingsConfig);
                PaymentService paymentService = new PaymentService(context);
                List<PaymentModel> pendingPayments = await paymentService.GetQueryable(r => r.RunBackgroundService && (r.IntegrationStatus == IntegrationStatusEnum.Pending) && (r.Order.Company == CompanyEnum.Aiico) && (r.PaymentStatus == PaymentStatusEnum.Paid) && (r.Order.IntegrationStatus == IntegrationStatusEnum.Completed))
                    .Include(r => r.Order.BankInformation)
                    .Include(r => r.Order)
                    .ThenInclude(r => r.Cart)
                    .ThenInclude(r => r.InsurancePlan)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (PaymentModel payment in pendingPayments)
                {
                    try
                    {
                        BankInformationModel bank = payment.Order.ForSomeoneElse ? payment.Order.BankInformation : await context.Banks.Where(r => (r.UserId == payment.Order.UserId) && r.UseAsDefault).SingleOrDefaultAsync();

                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "accountNumber", bank.AccountNumber },
                            { "amountPaid", payment.Order.AiicoPremiumAmount.ToString() },
                            { "partnerRef", "lafiami" },
                            { "paymentRef", payment.Id.ToString() },
                           { "transactionRef", payment.Order.AiicoTransactionRef },
                        };

                        FinalizeAiicoPartnerPaymentResponse result = await webAPI.PostFormDataAsJson<FinalizeAiicoPartnerPaymentResponse>("PartnerService/FinalizePartnerPayment", values, CompanyEnum.Aiico);

                        payment.RunBackgroundService = false;
                        payment.PartnerPaymentJsonResponse = WebUtility.HtmlEncode(JsonSerializer.Serialize(result) ?? string.Empty);
                        if (result.success)
                        {
                            payment.IntegrationStatus = IntegrationStatusEnum.Completed;
                        }
                        else
                        {
                            payment.IntegrationStatus = IntegrationStatusEnum.Failed;
                            payment.IntegrationErrorMessage = WebUtility.HtmlEncode(result.error ?? string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        payment.RunBackgroundService = false;
                        payment.IntegrationStatus = IntegrationStatusEnum.Failed;
                        payment.IntegrationErrorMessage = WebUtility.HtmlEncode(ex.Message ?? string.Empty);

                    }
                    paymentService.Update(payment);
                }

                AddToCache(manageCacheService, ControllerConstant.Order);
                AddToCache(manageCacheService, ControllerConstant.Payment);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task UpdateWalletOrderDependencyDetail(LafiamiContext context, ManageCacheService manageCacheService)
        {

            try
            {
                WalletService walletService = new WalletService(context);
                List<WalletTransactionModel> pendingCompletedTransactions = await walletService.GetCompletedOrderBookCreditTransactions(page, pageSize);

                foreach (WalletTransactionModel pendingCompletedTransaction in pendingCompletedTransactions)
                {
                    walletService.FinalizedBookCredit(pendingCompletedTransaction, TransactionStatusEnum.Completed);
                }

                List<WalletTransactionModel> pendingCancelledTransactions = await walletService.GetCancelledOrderBookCreditTransactions(page, pageSize);

                foreach (WalletTransactionModel pendingCancelledTransaction in pendingCancelledTransactions)
                {
                    walletService.FinalizedBookCredit(pendingCancelledTransaction, TransactionStatusEnum.Cancelled);
                }

                AddToCache(manageCacheService, ControllerConstant.Wallet);
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        private async Task SendEmailsInBatches(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                EmailService emailService = new EmailService(context);
                List<EmailModel> pendingEmails = await emailService.GetPendingEmails(page, pageSize);

                await SendEmails(manageCacheService, emailService, pendingEmails);
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error Logger Initialzed");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex);

                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task SendEmails(ManageCacheService manageCacheService, EmailService emailService, List<EmailModel> pendingEmails)
        {
            foreach (EmailModel pendingEmail in pendingEmails)
            {
                await emailService.SendEmail(pendingEmail, sMTPSettings, websiteSettings);
                AddToCache(manageCacheService, ControllerConstant.Email);
            }
        }

        private async Task PendingPaymentNotification(LafiamiContext context, int daysFromSentDate, ManageCacheService manageCacheService)
        {

            try
            {
                PaymentService paymentService = new PaymentService(context);
                EmailService emailService = new EmailService(context);
                OrderService orderService = new OrderService(context);
                List<Guid> paymentIds = await paymentService.GetPendingPayments(daysFromSentDate, page, pageSize);

                if (paymentIds.Count() > 0)
                {
                    foreach (Guid paymentId in paymentIds)
                    {
                        await emailService.PaymentEmail(paymentId, EmailTypeEnums.PendingPayment);
                        List<OrderItemsResponse> orderItems = await orderService.GetFullOrderItems(null, paymentId);
                        AddToCache(manageCacheService, ControllerConstant.Email);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task NewWalletNotification(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                WalletService walletService = new WalletService(context);
                EmailService emailService = new EmailService(context);

                List<ObjectId<Guid>> walletTransactions = await walletService.GetRecentWalletYetToBeNotified(page, pageSize);
                if (walletTransactions.Count() > 0)
                {
                    foreach (ObjectId<Guid> wallet in walletTransactions)
                    {
                        if (wallet.GenerateEmail)
                        {
                            await emailService.NewWalletEmail(wallet.Id);
                            AddToCache(manageCacheService, ControllerConstant.Email);
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }


        }

        private async Task NewOrderNotifications(LafiamiContext context, ManageCacheService manageCacheService)
        {
            try
            {
                EmailService emailService = new EmailService(context);
                OrderService orderService = new OrderService(context);

                List<Guid> orders = orderService.GetNewOrdersNotificationYetToBeGenerated(page, pageSize).GetAwaiter().GetResult();

                if (orders.Count() > 0)
                {
                    foreach (Guid orderid in orders)
                    {
                        await emailService.NewOrderEmail(orderid);
                        AddToCache(manageCacheService, ControllerConstant.Email);
                    }

                    await context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task NewUserAccountNotifications(LafiamiContext context, IServiceProvider services, ManageCacheService manageCacheService)
        {
            try
            {
                EmailService emailService = new EmailService(context);
                UserManager<UserViewModel> userManager = services.GetService<UserManager<UserViewModel>>();
                RoleManager<ApplicationRoleModel> roleManager = services.GetService<RoleManager<ApplicationRoleModel>>();

                UserService userService = new UserService(context, userManager, roleManager);

                List<ObjectId<string>> users = userService.GetNewUsersNotificationYetToBeGenerated(page, pageSize).GetAwaiter().GetResult();
                if (users.Count() > 0)
                {
                    foreach (ObjectId<string> user in users)
                    {
                        if (user.GenerateEmail)
                        {
                            await emailService.UserEmail(user.Id, EmailTypeEnums.NewAccount);
                            AddToCache(manageCacheService, ControllerConstant.Email);
                        }
                    }

                    await context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

    }
}
