using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class EmailTemplateService : RepositoryBase<EmailTemplateModel, long>, IEmailTemplateService
    {
        public EmailTemplateService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType)
        {
            EmailTemplateModel emailTemplate = await GetQueryable(r => r.EmailType == emailType).FirstOrDefaultAsync();
            if (emailTemplate == null)
            {
                return new EmailTemplateModel()
                {
                    Template = DefaultTemplates.GetEmailTemplate(emailType),
                    Subject = DefaultTemplates.GetEmailSubject(emailType),
                };
            }


            return emailTemplate;
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(long id)
        {
            return await GetQueryable(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<EmailTemplateResponse> GetEmailTemplateToResponse(EmailTypeEnums emailType)
        {
            EmailTemplateResponse emailTemplate = await GetQueryable(r => r.EmailType == emailType)
                .Select(r => new EmailTemplateResponse()
                {
                    EmailTpye = r.EmailType,
                    Id = r.Id,
                    Template = r.Template,
                    Subject = r.Subject
                })
                .FirstOrDefaultAsync();

            return ((emailTemplate == null) ? (new EmailTemplateResponse()
            {
                EmailTpye = emailType,
                Template = DefaultTemplates.GetEmailTemplate(emailType),
                Subject = DefaultTemplates.GetEmailSubject(emailType),
            }) : (emailTemplate));
        }
    }
}
