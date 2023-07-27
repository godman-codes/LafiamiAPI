using LafiamiAPI.Utilities.Enums;

namespace LafiamiAPI.Utilities.Models
{
    public class SMTPDetail
    {
        public KnownSMTPEnum SMTPEnum { get; set; }
        public int HostPort { get; set; }
        public string HostServer { get; set; }
        public bool SSLStatus { get; set; }
    }
}
