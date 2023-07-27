namespace LafiamiAPI.Models.Internals
{
    public class JWTSettings
    {
        public string Secret { get; set; }
        public int ExpiringDays { get; set; }
        public string Issurer { get; set; }
        public string TestIssurer { get; set; }
    }
}
