using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IEmailService : IRepositoryBase<EmailModel, Guid>
    {
        Task UserEmail(string userId, EmailTypeEnums emailType);
        Task<EmailContent> GetEmailContent(EmailModel pendingEmail, WebsiteSettings websiteSettingss);
        Task<EmailContent> ProcessMessage(EmailModel email, WebsiteSettings websiteSettings);
        void CreateEmail(string message, string subject, string email);
        void CreateEmail(string message, string subject, List<string> emails);

        Task PaymentEmail(Guid paymentId, EmailTypeEnums emailType, string targetEmail = null);
    }
}
