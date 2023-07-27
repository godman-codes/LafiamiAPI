namespace LafiamiAPI.Models.Internals
{
    public class SMTPSettings
    {
        public string FromEmail { get; set; }
        public string FromEmailPassword { get; set; }
        public int HostPort { get; set; }
        public string HostServer { get; set; }
        public bool SSLStatus { get; set; }
    }

}
