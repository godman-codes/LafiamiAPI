using System;

namespace LafiamiAPI.Models.Responses
{
    public class UserTokenResponse
    {
        public bool RequireNewPassword { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public bool HasAdminRole { get; set; }
        public DateTime ExpiringDate { get; set; }
        public bool HasNextofKin { get; set; }
        public bool HasIdentification { get; set; }
        public bool HasJobListed { get; set; }
        public bool HasBankInformationListed { get; set; }
        public bool IsBasicProfileCompleted { get; set; }
    }
}
