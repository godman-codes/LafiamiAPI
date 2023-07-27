using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IUserService
    {
        Task<BasicUserResponse> GetBasicUserInformation(Guid? orderId = null, Guid? walletId = null, Guid? paymentId = null);
        IQueryable<UserViewModel> GetQueryable();
        Task<bool> DoesUserExist(string emailAddress, string username);
        Task<bool> IsAdminUser(string userId);
        string GenerateReferralCode();
        Task<string> GetUserFullName(string userId);
        Task<bool> DoesUserHasDateOfBirth(string userId);
        Task<string> GetUserId(string username);
        Task<UserEmailAndPhone> GetOwnerEmailAndPhoneAsync(string userId);
        Task<string> CreateUserAccount(UserViewModel newUser, string newpassword, bool IsAdmin = false);

        IQueryable<BankInformationModel> GetBankInformationQueryable();
        IQueryable<NextOfKinModel> GetNextOfKinQueryable();
        IQueryable<IdentificationModel> GetIdentificationQueryable();
        IQueryable<JobModel> GetJobQueryable();
        Task<JobModel> SaveOrUpdateJob(JobRequest job);
        Task<BankInformationModel> SaveOrUpdateBank(BankRequest bank);
        Task<NextOfKinModel> SaveOrUpdateNextOfKins(NextOfKinRequest kin);
        Task<IdentificationModel> SaveOrUpdateIdentification(IdentificationRequest identification);
    }
}
