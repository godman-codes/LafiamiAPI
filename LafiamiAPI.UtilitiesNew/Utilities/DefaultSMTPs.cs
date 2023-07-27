using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Models;
using System.Collections.Generic;
using System.Linq;

namespace LafiamiAPI.Utilities.Utilities
{
    public static class DefaultSMTPs
    {
        public static int DefaultPort587 { get { return 587; } }
        public static int DefaultPort465 { get { return 465; } }

        public static SMTPDetail GetSMTP(KnownSMTPEnum sMTPEnum)
        {
            return GetSMTPs().Where(r => r.SMTPEnum == sMTPEnum).FirstOrDefault();
        }

        public static List<SMTPDetail> GetSMTPs()
        {
            return new List<SMTPDetail>()
            {
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Gmail, HostPort=DefaultPort587, HostServer="smtp.gmail.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Gmail_SSL, HostPort=DefaultPort465, HostServer="smtp.gmail.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Outlook, HostPort=DefaultPort587, HostServer="smtp-mail.outlook.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Yahoo, HostPort=DefaultPort587, HostServer="smtp.mail.yahoo.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Yahoo_SSL, HostPort=DefaultPort465, HostServer="smtp.mail.yahoo.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Zoho, HostPort=DefaultPort587, HostServer="smtp.zoho.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Zoho_SSL, HostPort=DefaultPort465, HostServer="smtp.zoho.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.HotMail, HostPort=DefaultPort465, HostServer="smtp.live.com", SSLStatus = true },
                new SMTPDetail(){ SMTPEnum = KnownSMTPEnum.Office365, HostPort=DefaultPort587, HostServer="smtp.office365.com", SSLStatus = true }
            };
        }
    }
}
