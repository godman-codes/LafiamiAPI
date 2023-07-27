using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Services.SystemServices;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class EmailService : RepositoryBase<EmailModel, Guid>, IEmailService
    {
        public EmailService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task NewOrderEmail(Guid orderId)
        {
            var order = await DBContext.Orders
               .Where(r => r.Id == orderId)
               .Select(r => new
               {
                   Email = r.User.Email,
               })
               .FirstOrDefaultAsync();

            if (order == null)
            {
                return;
            }

            Add(new EmailModel()
            {
                Emailaddresses = order.Email,
                UpdatedDate = DateTime.Now,
                EmailType = EmailTypeEnums.NewOrder,
                Status = MessageStatusEnums.Pending,
                OrderId = orderId
            });
        }

        public async Task UserEmail(string userId, EmailTypeEnums emailType)
        {
            var user = await DBContext.Users
               .Where(r => r.Id == userId)
               .Select(r => new
               {
                   r.Email,
               })
               .FirstOrDefaultAsync();

            if (user == null)
            {
                return;
            }


            Add(new EmailModel()
            {
                Emailaddresses = user.Email,
                EmailType = emailType,
                Status = MessageStatusEnums.Pending,
                NewUserId = ((emailType == EmailTypeEnums.NewAccount) ? (userId) : (null)),
                ChangeOrResetUserId = ((emailType == EmailTypeEnums.NewAccount) ? (null) : (userId)),
                Id = Guid.NewGuid()
            });
        }

        public async Task NewWalletEmail(Guid walletTransactionId)
        {
            var wallet = await DBContext.WalletTransactions
               .Where(r => r.Id == walletTransactionId)
               .Select(r => new
               {
                   r.Wallet.User.Email,
               })
               .FirstOrDefaultAsync();

            if (wallet == null)
            {
                return;
            }

            Add(new EmailModel()
            {
                Emailaddresses = wallet.Email,
                UpdatedDate = DateTime.Now,
                EmailType = EmailTypeEnums.NewWalletTransaction,
                Status = MessageStatusEnums.Pending,
                WalletTransactionId = walletTransactionId
            });
        }

        public void IntegrationIssueEmailNotification(Guid orderId, string emailsCommaSeperated)
        {
            Add(new EmailModel()
            {
                Emailaddresses = emailsCommaSeperated,
                UpdatedDate = DateTime.UtcNow,
                EmailType = EmailTypeEnums.OrderIntegrationIssue,
                Status = MessageStatusEnums.Pending,
                IntegrationOrderId = orderId
            });
        }

        public void HygeiaCompletionEmailNotification(Guid orderId, string emailsCommaSeperated)
        {
            Add(new EmailModel()
            {
                Emailaddresses = emailsCommaSeperated,
                UpdatedDate = DateTime.UtcNow,
                EmailType = EmailTypeEnums.CompleteHygeiaOrderNotification,
                Status = MessageStatusEnums.Pending,
                HygeiaCompletionOrderId = orderId
            });
        }


        public async Task PaymentEmail(Guid paymentId, EmailTypeEnums emailType, string targetEmail = null)
        {
            if (!await DBContext.Payments.AnyAsync(r => r.Id == paymentId))
            {
                return;
            }

            if (string.IsNullOrEmpty(targetEmail))
            {
                targetEmail = await DBContext.Payments
                   .Where(r => r.Id == paymentId)
                   .Select(r => r.Order.User.Email)
                   .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(targetEmail))
                {
                    return;
                }
            }

            Add(new EmailModel()
            {
                Emailaddresses = targetEmail,
                UpdatedDate = DateTime.Now,
                EmailType = emailType,
                Status = MessageStatusEnums.Pending,
                PaymentId = paymentId
            });
        }


        public void CreateEmail(string message, string subject, string email)
        {
            Add(new EmailModel()
            {
                Emailaddresses = email,
                UpdatedDate = DateTime.Now,
                Message = message,
                Status = MessageStatusEnums.Pending,
                Subject = subject,
            });
        }

        public void CreateEmail(string message, string subject, List<string> emails)
        {
            foreach (string email in emails)
            {
                CreateEmail(message, subject, email);
            }
        }

        public async Task<List<EmailModel>> GetPendingEmails(int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            List<EmailModel> pendingEmails = await DBContext.EmailLogs
                                     .Where(r =>
                                         (r.Status == MessageStatusEnums.Pending)
                                     )
                                     .OrderBy(r => r.UpdatedDate)
                                     .Skip(page)
                                    .Take((page + 1) * pageSize)
                                     .ToListAsync();

            return pendingEmails;
        }

        public async Task<List<EmailModel>> GetPendingEmails(LiteEmailSetting liteEmail, int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            List<EmailModel> pendingEmails = await DBContext.EmailLogs
                                     .Where(r =>
                                         (r.Status == MessageStatusEnums.Pending) &&
                                         (
                                            (liteEmail.SendBackLogEmails) ?
                                            (liteEmail.BackLogDate <= r.UpdatedDate) :
                                            (_2DaysAgo <= r.UpdatedDate)
                                         )
                                     )
                                     .OrderBy(r => r.UpdatedDate)
                                     .Skip(page)
                                    .Take((page + 1) * pageSize)
                                     .ToListAsync();

            return pendingEmails;
        }

        public async Task<string> AppendHeaderAndFooterToEmail(string message)
        {
            EmailTemplateService emailTemplateService = new EmailTemplateService(DBContext);
            EmailTemplateModel headerTemplate = await emailTemplateService.GetEmailTemplate(EmailTypeEnums.EmailHeader);
            EmailTemplateModel footerTemplate = await emailTemplateService.GetEmailTemplate(EmailTypeEnums.EmailFooter);

            string finaltemplate = WrapMessageWithHTMLTableROW(headerTemplate.Template) + WrapMessageWithHTMLTableROW(message) + WrapMessageWithHTMLTableROW(footerTemplate.Template);

            return AddHTMLTable(finaltemplate);
        }

        public string AddHTMLTable(string message)
        {
            string HtmlTable = "<div align='center'>";
            HtmlTable += "<table style='background-color:#eeeeee;box-shadow: 0 .5rem 1rem rgba(0,0,0,.15) !important;' bgcolor='#eeeeee' border='0' width='100%' cellspacing='0' cellpadding='0'>";
            HtmlTable += "<tbody>";
            HtmlTable += "<tr>";
            HtmlTable += "<td rowspan='1' colspan='1' align='center' style='line-height:24px;'>";

            HtmlTable += "<table width='600' style='background-color:#ffffff;'bgcolor='#ffffff'border='0'cellspacing='0' cellpadding='0'>";
            HtmlTable += "<tbody>";

            HtmlTable += message;

            HtmlTable += "</tbody>";
            HtmlTable += "</table>";

            HtmlTable += "</td>";
            HtmlTable += "</tr>";
            HtmlTable += "</tbody>";
            HtmlTable += "</table>";
            HtmlTable += "</div>";

            return HtmlTable;
        }

        public string WrapMessageWithHTMLTableROW(string message)
        {
            string HtmlTable = "";

            HtmlTable += "<tr>";
            HtmlTable += "<td rowspan='1' colspan='1' style='padding:20px;" + "'>";
            HtmlTable += message;
            HtmlTable += "</td>";
            HtmlTable += "</tr>";

            return HtmlTable;
        }

        public async Task SendEmail(EmailModel pendingEmail, SMTPSettings sMTP, WebsiteSettings websiteSettings)
        {
            try
            {
                if (sMTP == null)
                {
                    pendingEmail.Status = MessageStatusEnums.Failed;
                    pendingEmail.FailedDate = DateTime.Now;
                    pendingEmail.ResponseMessage = "SMTP not configure yet";
                    return;
                }

                List<string> confirmedEmails = new List<string>();
                List<string> unConfirmedEmails = new List<string>();
                string[] splittedEmails = await SortEmails(pendingEmail, confirmedEmails, unConfirmedEmails);

                if (confirmedEmails.Count() == 0)
                {
                    pendingEmail.Status = MessageStatusEnums.Failed;
                    pendingEmail.FailedDate = DateTime.Now;
                    pendingEmail.ResponseMessage = "Email(s) does not exist or their respective domain(s) has expired";
                    return;
                }

                EmailContent emailContent = await GetEmailContent(pendingEmail, websiteSettings);

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(sMTP.FromEmail),
                    IsBodyHtml = true,
                    Subject = emailContent.Subject,
                    Body = WebUtility.HtmlDecode(emailContent.Message),
                };
                mail.To.Add(string.Join(",", confirmedEmails));

                //var httpClient = new HttpClient(); 
                WebClient webClient = new WebClient();
                foreach (string attachmentUrl in emailContent.AttachmentUrls)
                {
                    byte[] bytes = webClient.DownloadData(attachmentUrl);
                    System.IO.MemoryStream attachmentstream = new MemoryStream(bytes);

                    System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(attachmentstream, DefaultMediaTypes.GetContentType(attachmentUrl));
                    attach.ContentDisposition.FileName = DefaultMediaTypes.GetFilename(attachmentUrl);
                    // Add the file attachment to this email message.
                    mail.Attachments.Add(attach);
                }

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = sMTP.SSLStatus;
                    client.Host = sMTP.HostServer;
                    client.Port = sMTP.HostPort;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(sMTP.FromEmail, sMTP.FromEmailPassword);

                    await client.SendMailAsync(mail);
                }

                if (confirmedEmails.Count() == splittedEmails.Count())
                {
                    pendingEmail.Status = MessageStatusEnums.Sent;
                    pendingEmail.Sentdate = DateTime.Now;
                }
                else
                {
                    pendingEmail.Status = MessageStatusEnums.SentPartially;
                    pendingEmail.ResponseMessage = "Unable to send to these emails: " + string.Join(",", unConfirmedEmails) + ". Either Email(s) does not exist or their respective domain(s) has expired";
                    pendingEmail.Sentdate = DateTime.Now;
                    pendingEmail.FailedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                pendingEmail.Status = MessageStatusEnums.Failed;
                pendingEmail.FailedDate = DateTime.Now;
                pendingEmail.ResponseMessage = ex.Message;
            }
        }

        private static async Task<string[]> SortEmails(EmailModel pendingEmail, List<string> confirmedEmails, List<string> unConfirmedEmails)
        {
            string[] splittedEmails = pendingEmail.Emailaddresses.Split(",");
            DNSCheck dNSCheck = new DNSCheck();
            foreach (string email in splittedEmails)
            {
                if (await dNSCheck.IsEmailValid(email))
                {
                    confirmedEmails.Add(email);
                }
                else
                {
                    unConfirmedEmails.Add(email);
                }
            }

            return splittedEmails;
        }

        public async Task<EmailContent> GetEmailContent(EmailModel pendingEmail, WebsiteSettings websiteSettings)
        {
            EmailContent emailContent = new EmailContent();

            if (pendingEmail.EmailType.HasValue)
            {
                emailContent = await ProcessMessage(pendingEmail, websiteSettings);
            }
            else
            {
                emailContent = new EmailContent()
                {
                    Subject = pendingEmail.Subject,
                    Message = pendingEmail.Message
                };
            }

            emailContent.Message = await AppendHeaderAndFooterToEmail(emailContent.Message);
            return emailContent;
        }

        public async Task<EmailContent> ProcessMessage(EmailModel email, WebsiteSettings websiteSettings)
        {
            EmailContent subjectAndMessage = new EmailContent();
            if (email.EmailType.HasValue)
            {
                switch (email.EmailType.Value)
                {
                    case EmailTypeEnums.NewAccount:
                        subjectAndMessage = await GetUserSubjectAndMessage(email.NewUserId, email.EmailType.Value);
                        break;
                    case EmailTypeEnums.ResetPassword:
                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
                        {
                            subjectAndMessage = await GetUserSubjectAndMessage(email.ChangeOrResetUserId, email.EmailType.Value);
                        }
                        break;
                    case EmailTypeEnums.ChangePassword:
                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
                        {
                            subjectAndMessage = await GetUserSubjectAndMessage(email.ChangeOrResetUserId, email.EmailType.Value);
                        }
                        break;
                    case EmailTypeEnums.NewOrder:
                        subjectAndMessage = await GetNewOrderSubjectAndMessage(email.OrderId.Value, websiteSettings);
                        break;
                    case EmailTypeEnums.NewPayment:
                        if (email.PaymentId.HasValue)
                        {
                            subjectAndMessage = await GetPaymentEmailSubjectAndMessage(email.PaymentId.Value, EmailTypeEnums.NewPayment, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.NewSuccessfulPayment:
                        if (email.PaymentId.HasValue)
                        {
                            subjectAndMessage = await GetPaymentEmailSubjectAndMessage(email.PaymentId.Value, EmailTypeEnums.NewSuccessfulPayment, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.NewFailedPayment:
                        if (email.PaymentId.HasValue)
                        {
                            subjectAndMessage = await GetPaymentEmailSubjectAndMessage(email.PaymentId.Value, EmailTypeEnums.NewFailedPayment, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.PendingPayment:
                        if (email.PaymentId.HasValue)
                        {
                            subjectAndMessage = await GetPaymentEmailSubjectAndMessage(email.PaymentId.Value, EmailTypeEnums.PendingPayment, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.AwaitingPaymentConfirmation:
                        if (email.PaymentId.HasValue)
                        {
                            subjectAndMessage = await GetPaymentEmailSubjectAndMessage(email.PaymentId.Value, EmailTypeEnums.AwaitingPaymentConfirmation, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.NewWalletTransaction:
                        subjectAndMessage = await GetNewWalletSubjectAndMessage(email.WalletTransactionId.Value);
                        break;
                    case EmailTypeEnums.OrderIntegrationIssue:
                        if (email.IntegrationOrderId.HasValue)
                        {
                            subjectAndMessage = await GetIntegrationIssueSubjectAndMessage(email.IntegrationOrderId.Value, websiteSettings);
                        }
                        break;
                    case EmailTypeEnums.CompleteHygeiaOrderNotification:
                        if (email.HygeiaCompletionOrderId.HasValue)
                        {
                            subjectAndMessage = await GetHyqeiaCompletionSubjectAndMessage(email.HygeiaCompletionOrderId.Value, websiteSettings);
                        }
                        break;
                    default:
                        break;
                }
            }

            return subjectAndMessage;
        }


        public async Task<EmailContent> GetUserSubjectAndMessage(string userId, EmailTypeEnums emailType)
        {
            var user = await DBContext.Users
                .Where(r => r.Id == userId)
                .Select(r => new
                {
                    r.Firstname,
                    r.Surname,
                    r.Email,
                    r.PhoneNumber,
                    r.TempPassword
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new EmailContent();
            }

            EmailTemplateService emailTemplateService = new EmailTemplateService(DBContext);
            EmailTemplateModel template = await emailTemplateService.GetEmailTemplate(emailType);

            string message = template.Template
                    .Replace(TagName.FirstName, user.Firstname)
                    .Replace(TagName.Surname, user.Surname)
                    .Replace(TagName.EmailAddress, user.Email)
                    .Replace(TagName.PhoneNumber, user.PhoneNumber)
                    .Replace(TagName.NewPassword, user.TempPassword);
            string subject = template.Subject
                .Replace(TagName.FirstName, user.Firstname)
                .Replace(TagName.Surname, user.Surname)
                .Replace(TagName.EmailAddress, user.Email)
                .Replace(TagName.PhoneNumber, user.PhoneNumber)
                .Replace(TagName.NewPassword, user.TempPassword);

            return new EmailContent()
            {
                Message = message,
                Subject = subject
            };
        }

        private async Task<EmailContent> GetSubjectMessageFromTemplate(EmailTypeEnums emailType)
        {
            EmailTemplateService emailTemplateService = new EmailTemplateService(DBContext);
            EmailTemplateModel template = await emailTemplateService.GetEmailTemplate(emailType);

            EmailContent emailSubjectAndMessage = new EmailContent()
            {
                Subject = template.Subject,
                Message = template.Template
            };
            return emailSubjectAndMessage;
        }

        public async Task<EmailContent> GetNewOrderSubjectAndMessage(Guid orderId, WebsiteSettings websiteSettings)
        {
            OrderService orderService = new OrderService(DBContext);
            var order = await orderService.GetQueryable(r => r.Id == orderId)
               .Select(r => new
               {
                   r.User.Firstname,
                   r.User.Surname,
                   r.User.Email,
                   r.TotalAmount,
                   r.OrderId,
               })
               .FirstOrDefaultAsync();


            if (order == null)
            {
                return new EmailContent();
            }

            string orderIdUrl = string.Join(string.Empty, websiteSettings.WebsiteURL, Constants.DefaultOrderPath, Constants.OrderQueryParameter, order.OrderId);

            EmailContent emailSubjectAndMessage = await GetSubjectMessageFromTemplate(EmailTypeEnums.NewOrder);

            emailSubjectAndMessage.Message = emailSubjectAndMessage.Message
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderId, order.OrderId)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.AmountInWord, order.TotalAmount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + order.TotalAmount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            emailSubjectAndMessage.Subject = emailSubjectAndMessage.Subject
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderId, order.OrderId)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.AmountInWord, order.TotalAmount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + order.TotalAmount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            return emailSubjectAndMessage;
        }

        public async Task<EmailContent> GetPaymentEmailSubjectAndMessage(Guid paymentId, EmailTypeEnums emailType, WebsiteSettings websiteSettings)
        {
            PaymentService paymentService = new PaymentService(DBContext);
            var payment = await paymentService.GetQueryable(r => r.Id == paymentId)
               .Select(r => new
               {
                   r.Order.User.Firstname,
                   r.Order.User.Surname,
                   r.Order.User.Email,
                   r.TransactionId,
                   r.Amount,
                   r.Order.OrderId,
               })
               .FirstOrDefaultAsync();

            if (payment == null)
            {
                return new EmailContent();
            }

            string orderIdUrl = string.Join(string.Empty, websiteSettings.WebsiteURL, Constants.DefaultOrderPath, Constants.OrderQueryParameter, payment.OrderId);
            EmailContent emailSubjectAndMessage = await GetSubjectMessageFromTemplate(emailType);

            emailSubjectAndMessage.Message = emailSubjectAndMessage.Message
                .Replace(TagName.FirstName, payment.Firstname)
                .Replace(TagName.Surname, payment.Surname)
                .Replace(TagName.TransactionId, payment.TransactionId)
                .Replace(TagName.OrderId, payment.OrderId)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.AmountInWord, payment.Amount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + payment.Amount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            emailSubjectAndMessage.Subject = emailSubjectAndMessage.Subject
                .Replace(TagName.FirstName, payment.Firstname)
                .Replace(TagName.Surname, payment.Surname)
                .Replace(TagName.TransactionId, payment.TransactionId)
                .Replace(TagName.OrderId, payment.OrderId)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.AmountInWord, payment.Amount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + payment.Amount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            return emailSubjectAndMessage;
        }

        public async Task<EmailContent> GetNewWalletSubjectAndMessage(Guid walletTransactionId)
        {
            var walletTransaction = await DBContext.WalletTransactions.Where(r => r.Id == walletTransactionId)
               .Select(r => new
               {
                   r.Wallet.User.Firstname,
                   r.Wallet.User.Surname,
                   r.Wallet.User.Email,
                   r.TransactionType,
                   r.Amount,
                   r.Wallet.Balance,
                   r.Wallet.BookBalance,
               })
               .FirstOrDefaultAsync();


            if (walletTransaction == null)
            {
                return new EmailContent();
            }

            EmailContent emailSubjectAndMessage = await GetSubjectMessageFromTemplate(EmailTypeEnums.NewWalletTransaction);

            emailSubjectAndMessage.Message = emailSubjectAndMessage.Message
                .Replace(TagName.FirstName, walletTransaction.Firstname)
                .Replace(TagName.Surname, walletTransaction.Surname)
                .Replace(TagName.TransactionType, walletTransaction.TransactionType.DisplayName())
                .Replace(TagName.AmountInWord, walletTransaction.Amount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + walletTransaction.Amount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)))
                  .Replace(TagName.Balance, (Constants.NairaSymbol + walletTransaction.Balance.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)))
                    .Replace(TagName.BookBalance, (Constants.NairaSymbol + walletTransaction.BookBalance.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            emailSubjectAndMessage.Subject = emailSubjectAndMessage.Subject
                .Replace(TagName.FirstName, walletTransaction.Firstname)
                .Replace(TagName.Surname, walletTransaction.Surname)
                .Replace(TagName.TransactionType, walletTransaction.TransactionType.DisplayName())
                .Replace(TagName.AmountInWord, walletTransaction.Amount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + walletTransaction.Amount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)))
                 .Replace(TagName.Balance, (Constants.NairaSymbol + walletTransaction.Balance.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)))
                    .Replace(TagName.BookBalance, (Constants.NairaSymbol + walletTransaction.BookBalance.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            return emailSubjectAndMessage;
        }

        public async Task<EmailContent> GetIntegrationIssueSubjectAndMessage(Guid orderId, WebsiteSettings websiteSettings)
        {
            OrderService orderService = new OrderService(DBContext);
            var order = await orderService.GetQueryable(r => r.Id == orderId)
                        .Select(r => new
                        {
                            r.IntegrationErrorMessage,
                            r.User.Firstname,
                            r.User.Surname,
                            r.OrderId,
                            r.Company,
                        }).SingleOrDefaultAsync();


            if (order == null)
            {
                return new EmailContent();
            }

            EmailContent emailSubjectAndMessage = await GetSubjectMessageFromTemplate(EmailTypeEnums.OrderIntegrationIssue);
            string orderIdUrl = string.Join(string.Empty, websiteSettings.WebsiteURL, Constants.DefaultOrderPath, Constants.OrderQueryParameter, order.OrderId);


            emailSubjectAndMessage.Message = emailSubjectAndMessage.Message
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.Message, WebUtility.HtmlDecode(order.IntegrationErrorMessage))
                .Replace(TagName.CompanyName, order.Company.DisplayName())
                .Replace(TagName.Action, GetActionName(order.Company));

            emailSubjectAndMessage.Subject = emailSubjectAndMessage.Subject
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.Message, WebUtility.HtmlDecode(order.IntegrationErrorMessage))
                .Replace(TagName.CompanyName, order.Company.DisplayName())
                .Replace(TagName.Action, GetActionName(order.Company));

            return emailSubjectAndMessage;
        }

        public async Task<EmailContent> GetHyqeiaCompletionSubjectAndMessage(Guid orderId, WebsiteSettings websiteSettings)
        {
            OrderService orderService = new OrderService(DBContext);
            var order = await orderService.GetQueryable(r => r.Id == orderId)
                        .Select(r => new
                        {
                            r.User.Firstname,
                            r.User.Surname,
                            r.OrderId,
                            r.TotalAmount,
                            r.HygeiaLegacyCode,
                            r.HygeiaDependantId,
                            r.Company
                        }).SingleOrDefaultAsync();


            if (order == null)
            {
                return new EmailContent();
            }

            EmailContent emailSubjectAndMessage = await GetSubjectMessageFromTemplate(EmailTypeEnums.CompleteHygeiaOrderNotification);
            string orderIdUrl = string.Join(string.Empty, websiteSettings.WebsiteURL, Constants.DefaultOrderPath, Constants.OrderQueryParameter, order.OrderId);


            emailSubjectAndMessage.Message = emailSubjectAndMessage.Message
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderId, order.OrderId)
                .Replace(TagName.EnrolleeNumber, string.Join(Constants.BackSlash, order.HygeiaLegacyCode, order.HygeiaDependantId))
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.CompanyName, order.Company.DisplayName())
                .Replace(TagName.AmountInWord, order.TotalAmount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + order.TotalAmount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            emailSubjectAndMessage.Subject = emailSubjectAndMessage.Subject
                .Replace(TagName.FirstName, order.Firstname)
                .Replace(TagName.Surname, order.Surname)
                .Replace(TagName.OrderId, order.OrderId)
                .Replace(TagName.EnrolleeNumber, string.Join(Constants.BackSlash, order.HygeiaLegacyCode, order.HygeiaDependantId))
                .Replace(TagName.OrderPageURL, orderIdUrl)
                .Replace(TagName.CompanyName, order.Company.DisplayName())
                .Replace(TagName.AmountInWord, order.TotalAmount.CurrencyNumberToWord(Constants.Naira, Constants.NairaUnit))
                .Replace(TagName.Amount, (Constants.NairaSymbol + order.TotalAmount.ToString(Constants.MoneyFormat, System.Globalization.CultureInfo.InvariantCulture)));

            return emailSubjectAndMessage;
        }

        private string GetActionName(CompanyEnum? company)
        {
            string result = string.Empty;
            if (!company.HasValue)
            {
                return result;
            }
            switch (company)
            {
                case CompanyEnum.Lafiami:
                    break;
                case CompanyEnum.Aiico:
                    result = "Posting Hospital Cash";
                    break;
                case CompanyEnum.Hygeia:
                    result = "Posting Registration";
                    break;
                case CompanyEnum.AxaMansand:
                    break;
                case CompanyEnum.RelainceHMO:
                    break;
                default:
                    break;
            }

            return result;
        }


    }
}
