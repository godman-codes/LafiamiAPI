using LafiamiAPI.Utilities.Enums;
using System;

namespace LafiamiAPI.Models.Internals
{
    public class LiteEmailSetting
    {
        public EmailProviderEnum EmailProvider { get; set; }
        public DateTime BackLogDate { get; set; }
        public bool SendBackLogEmails { get; set; }
    }
}
