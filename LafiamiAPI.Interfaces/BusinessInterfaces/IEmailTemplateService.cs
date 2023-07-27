using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Enums;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IEmailTemplateService : IRepositoryBase<EmailTemplateModel, long>
    {
        Task<EmailTemplateResponse> GetEmailTemplateToResponse(EmailTypeEnums emailType);
        Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType);
        Task<EmailTemplateModel> GetEmailTemplate(long id);

    }
}
