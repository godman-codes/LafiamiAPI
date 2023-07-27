using LafiamiAPI.Utilities.Enums;

namespace LafiamiAPI.Models.Responses
{
    public class EmailTemplateResponse
    {
        public long Id { get; set; }
        public EmailTypeEnums EmailTpye { get; set; }
        public string Template { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
