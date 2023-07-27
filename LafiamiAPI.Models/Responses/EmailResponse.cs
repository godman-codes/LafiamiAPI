using System;

namespace LafiamiAPI.Models.Responses
{
    public class EmailResponse
    {
        public Guid Id { get; set; }
        public bool IsSent { get; set; }
        public bool IsPending { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string Emailaddresses { get; set; }
        public string Subject { get; set; }
    }
}
