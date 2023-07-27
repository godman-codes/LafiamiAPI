using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class UserService : IUserService
    {
        protected LafiamiContext DBContext { get; set; }
        private UserManager<UserViewModel> UserManager { get; set; }
        private RoleManager<ApplicationRoleModel> RoleManager { get; set; }
        public UserService(LafiamiContext repositoryContext, UserManager<UserViewModel> userManager, RoleManager<ApplicationRoleModel> roleManager)
        {
            DBContext = repositoryContext;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public IQueryable<UserViewModel> GetQueryable()
        {
            return DBContext.Users.Where(r => !r.IsDeleted).AsQueryable();
        }

        public async Task<List<ObjectId<string>>> GetNewUsersNotificationYetToBeGenerated(int page, int pageSize)
        {
            DateTime _2DaysAgo = DateTime.Now.AddDays(-2);
            var users = await DBContext.Users.Where(r => (r.EmailModel == null))
                .Where(r => r.CreatedDate >= _2DaysAgo)
                .OrderBy(r => r.CreatedDate)
                .Select(r => new
                {
                    r.Id,
                    EmailId = r.EmailModel.Id,
                })
                .Skip(page)
                .Take((page + 1) * pageSize)
                .ToListAsync();



            return users.Select(r => new ObjectId<string>()
            {
                Id = r.Id,
                GenerateEmail = (r.EmailId == Guid.Empty),
            }).ToList();
        }

        public async Task<string> GetUserId(string username)
        {
            return await UserManager.Users.Where(r => r.UserName == username)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetUserFullName(string userId)
        {
            var user = await UserManager.Users.Where(r => r.Id == userId)
                .Select(r => new { r.Surname, r.Firstname })
                .SingleOrDefaultAsync();

            return ((user == null) ? (string.Empty) : (user.Surname + Constants.Space + user.Firstname));
        }


        public async Task<bool> DoesUserHasDateOfBirth(string userId)
        {
            return await UserManager.Users.Where(r => (r.Id == userId) && r.DateOfBirth.HasValue)
                .AnyAsync();
        }


        public async Task<string> CreateUserAccount(UserViewModel newUser, string newpassword, bool IsAdmin = false)
        {

            if (await UserManager.Users.AnyAsync(r => r.UserName == newUser.UserName))
            {
                throw new WebsiteException("Another account has been created with this email. Thank you");
            }

            newUser.MustChangePassword = true;
            newUser.EmailModel = new EmailModel()
            {
                Emailaddresses = newUser.Email,
                EmailType = EmailTypeEnums.NewAccount,
                Status = MessageStatusEnums.Pending,
                Id = Guid.NewGuid(),
            };

            IdentityResult createResult = await UserManager.CreateAsync(newUser, newpassword);
            if (!createResult.Succeeded)
            {
                throw new WebsiteException("Issues creating a new account " + string.Join(',', createResult.Errors.Select(r => WebUtility.HtmlDecode(r.Description)).ToList()).ToString());
            }

            string errormessage = string.Empty;
            if (IsAdmin)
            {
                IdentityResult roleresult = await UserManager.AddToRoleAsync(newUser, SystemRole.Administrator);
                if (!roleresult.Succeeded)
                {
                    errormessage = "Note, we unbale to grant the user, admin priviledge. Thank you";
                }
            }

            return newUser.Id;
        }

        public async Task<UserEmailAndPhone> GetOwnerEmailAndPhoneAsync(string userId)
        {
            return await UserManager.Users.Where(r => r.Id == userId)
                .Select(r => new UserEmailAndPhone()
                {
                    Name = r.Surname + " " + r.Firstname,
                    Email = r.Email,
                    Phone = r.PhoneNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BasicUserResponse> GetBasicUserInformation(Guid? orderId = null, Guid? walletId = null, Guid? paymentId = null)
        {
            IQueryable<UserViewModel> queyable = UserManager.Users.AsQueryable();
            if (orderId.HasValue)
            {
                queyable = queyable.Where(r => r.Orders.Any(t => (t.Id == orderId.Value) && !t.IsDeleted));
            }
            else if (walletId.HasValue)
            {
                queyable = queyable.Where(r => r.Wallet.Id == walletId.Value);
            }
            else if (paymentId.HasValue)
            {
                queyable = queyable.Where(r => r.Orders.Any(t => t.Payments.Any(u => (u.Id == paymentId.Value) && !u.IsDeleted) && !t.IsDeleted));
            }
            else
            {
                return new BasicUserResponse();
            }

            return await queyable.Select(r => new BasicUserResponse()
            {
                Surname = r.Surname,
                Firstname = r.Firstname,
                EmailAddress = r.Email,
                PhoneNumber = r.PhoneNumber
            })
                .FirstOrDefaultAsync();
        }


        public string GenerateReferralCode()
        {
            return string.Join(string.Empty, Constants.Lafiami.ToLower().Trim(), DateTime.Now.ToString(Constants.DateFormatForIdGeneration), RandomGenerator.GetRandomString(3, true)).ToLower();
        }

        public async Task<bool> IsAdminUser(string userId)
        {
            return await DBContext.UserRoles.AnyAsync(r => (r.Role.Name == SystemRole.Administrator) && r.UserId == userId);
        }


        public async Task<bool> DoesUserExist(string emailAddress, string username)
        {
            IQueryable<UserViewModel> querable = DBContext.Users.Where(r => !r.IsDeleted).AsQueryable();
            if (!string.IsNullOrEmpty(emailAddress))
            {
                emailAddress = emailAddress.ToLower();
                querable = querable.Where(r => r.Email == emailAddress);
            }

            if (!string.IsNullOrEmpty(username))
            {
                username = username.ToLower();
                querable = querable.Where(r => r.UserName == username);
            }

            return await querable.AnyAsync();
        }


        public IQueryable<BankInformationModel> GetBankInformationQueryable()
        {
            return DBContext.Banks.Where(r => !r.IsDeleted).AsQueryable();
        }

        public IQueryable<NextOfKinModel> GetNextOfKinQueryable()
        {
            return DBContext.NextOfKins.Where(r => !r.IsDeleted).AsQueryable();
        }

        public IQueryable<IdentificationModel> GetIdentificationQueryable()
        {
            return DBContext.Identifications.Where(r => !r.IsDeleted).AsQueryable();
        }

        public IQueryable<JobModel> GetJobQueryable()
        {
            return DBContext.Jobs.Where(r => !r.IsDeleted).AsQueryable();
        }

        public async Task<JobModel> SaveOrUpdateJob(JobRequest job)
        {
            JobModel dbjob = ((job.Id == Guid.Empty) ? (null) : (await GetJobQueryable().Where(r => r.Id == job.Id).SingleOrDefaultAsync()));

            if (dbjob == null)
            {
                dbjob = job.ToDBModel();
                DBContext.Jobs.Add(dbjob);
            }
            else
            {
                dbjob.EndDate = job.EndDate;
                dbjob.CompanyAddress = job.CompanyAddress;
                dbjob.CompanyName = job.CompanyName;
                dbjob.IsCurrentJob = job.IsCurrentJob;
                dbjob.JobTitle = job.JobTitle;
                dbjob.StartDate = job.StartDate;
            }

            if (job.IsCurrentJob)
            {
                JobModel getExistingDefault = await GetJobQueryable().Where(r => (r.Id != job.Id) && r.IsCurrentJob && r.UserId == job.UserId).SingleOrDefaultAsync();
                if (getExistingDefault != null)
                {
                    getExistingDefault.IsCurrentJob = false;
                }
            }

            return dbjob;
        }

        public async Task<BankInformationModel> SaveOrUpdateBank(BankRequest bank)
        {
            BankInformationModel dbBank = ((bank.Id == Guid.Empty) ? (null) : (await GetBankInformationQueryable().Where(r => r.Id == bank.Id).SingleOrDefaultAsync()));

            if (dbBank == null)
            {
                dbBank = bank.ToDBModel();

                DBContext.Banks.Add(dbBank);
            }
            else
            {
                dbBank.AccountName = bank.AccountName;
                dbBank.AccountNumber = bank.AccountNumber;
                dbBank.BankName = bank.BankName;
                dbBank.UseAsDefault = bank.UseAsDefault;
            }

            if (bank.UseAsDefault)
            {
                BankInformationModel getExistingDefault = await GetBankInformationQueryable().Where(r => (r.Id != bank.Id) && r.UseAsDefault && r.UserId == bank.UserId).SingleOrDefaultAsync();
                if (getExistingDefault != null)
                {
                    getExistingDefault.UseAsDefault = false;
                }
            }

            return dbBank;
        }

        public async Task<NextOfKinModel> SaveOrUpdateNextOfKins(NextOfKinRequest kin)
        {
            NextOfKinModel dbKin = ((kin.Id == Guid.Empty) ? (null) : (await GetNextOfKinQueryable().Where(r => r.Id == kin.Id).SingleOrDefaultAsync()));

            if (dbKin == null)
            {
                dbKin = kin.ToDBModel();
                DBContext.NextOfKins.Add(dbKin);
            }
            else
            {
                dbKin.Address = kin.Address;
                dbKin.Phone = kin.Phone;
                dbKin.Firstname = kin.Firstname;
                dbKin.Relationship = kin.Relationship;
                dbKin.Surname = kin.Surname;
                dbKin.UseAsDefault = kin.UseAsDefault;
            }

            if (kin.UseAsDefault)
            {
                NextOfKinModel getExistingDefault = await GetNextOfKinQueryable().Where(r => (r.Id != kin.Id) && r.UseAsDefault && r.UserId == kin.UserId).SingleOrDefaultAsync();
                if (getExistingDefault != null)
                {
                    getExistingDefault.UseAsDefault = false;
                }
            }

            return dbKin;
        }

        public async Task<IdentificationModel> SaveOrUpdateIdentification(IdentificationRequest identification)
        {
            IdentificationModel dbIdentification = ((identification.Id == Guid.Empty) ? (null) : (await GetIdentificationQueryable().Where(r => r.Id == identification.Id).SingleOrDefaultAsync()));

            if (dbIdentification == null)
            {
                dbIdentification = identification.ToDBModel();
                DBContext.Identifications.Add(dbIdentification);
            }
            else
            {
                dbIdentification.IdUrl = identification.IdUrl;
                dbIdentification.UseAsDefault = identification.UseAsDefault;
            }

            if (identification.UseAsDefault)
            {
                IdentificationModel getExistingDefault = await GetIdentificationQueryable().Where(r => (r.Id != identification.Id) && r.UseAsDefault && r.UserId == identification.UserId).SingleOrDefaultAsync();
                if (getExistingDefault != null)
                {
                    getExistingDefault.UseAsDefault = false;
                }
            }

            return dbIdentification;
        }

    }
}
