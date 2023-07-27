using LafiamiAPI.Datas.Models;
using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace LafiamiAPI.Models.Requests
{
    public class NewEmailTemplateRequest
    {
        [Required]
        public EmailTypeEnums EmailTpye { get; set; }
        [Required]
        [DataType(DataType.Html)]
        public string Template { get; set; }
        [Required]
        public string Subject { get; set; }

        public EmailTemplateModel ToEntity()
        {
            return new EmailTemplateModel()
            {
                EmailType = EmailTpye,
                UpdatedDate = DateTime.Now,
                Template = WebUtility.HtmlEncode(Template),
                Subject = Subject,
            };
        }
    }

    public class ExistingEmailTemplateRequest
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Id is required")]
        public long Id { get; set; }
        [Required]
        [DataType(DataType.Html)]
        public string Template { get; set; }
        [Required]
        public string Subject { get; set; }

        public void ToEntity(EmailTemplateModel emailTemplate)
        {
            emailTemplate.Subject = Subject;
            emailTemplate.Template = WebUtility.HtmlEncode(Template);
            emailTemplate.UpdatedDate = DateTime.Now;
        }
    }
}
