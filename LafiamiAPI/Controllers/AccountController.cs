using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
        public const string ControllerName = ControllerConstant.Account;
        private readonly JWTSettings jwtSettings;
        private UserManager<UserViewModel> UserManager { get; set; }
        private RoleManager<ApplicationRoleModel> RoleManager { get; set; }
        private readonly IBusinessUnitofWork businessUnitofWork;

        public AccountController(
            IMemoryCache memoryCache,
            ILogger<AccountController> logger,
            ISystemUnitofWork systemUnitofWork,
            IOptions<JWTSettings> appSettings,
            UserManager<UserViewModel> userManager,
            RoleManager<ApplicationRoleModel> roleManager,
            IBusinessUnitofWork businessUnitofWork
            ) : base(memoryCache, logger, systemUnitofWork)
        {
            jwtSettings = ((appSettings != null) ? (appSettings.Value) : (null));
            UserManager = userManager;
            RoleManager = roleManager;
            this.businessUnitofWork = businessUnitofWork;
        }

        [HttpPost]
        [Route("Authenticate")]
        [ProducesResponseType(typeof(UserTokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Authenticate([FromBody] ValidateRequest auth)
        {
            UserViewModel userTest = await UserManager.Users
                .Where(r => r.UserName == auth.EmailAddress)
                .FirstOrDefaultAsync();


            UserViewModel user = await UserManager.Users
                .Where(r => r.UserName == auth.EmailAddress)
                .Include(r => r.UserRoles)
                   .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync();

            #region User Validation
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Account));
            }

            if (!user.IsActive)
            {
                throw new WebsiteException(string.Format(Constants.ItemCurrentState, Constants.Account, Constants.Suspended));
            }

            #endregion

            if (!await UserManager.CheckPasswordAsync(user, auth.Password))
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Account));
            }


            List<Claim> claims = GenerateClaims(user);
            TokenManagement tokenManagement = new TokenManagement(jwtSettings, claims);

            return Ok(new UserTokenResponse()
            {
                RequireNewPassword = user.MustChangePassword,
                Email = user.Email,
                FirstName = user.Firstname,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Token = tokenManagement.GetJWTToken(),
                HasAdminRole = user.UserRoles.Any(r => r.Role.Name == SystemRole.Administrator),
                ExpiringDate = tokenManagement.Expires(),
                HasBankInformationListed = await systemUnitofWork.UserService.GetBankInformationQueryable().Where(r => r.UseAsDefault && !r.IsDeleted && (r.UserId == user.Id)).AnyAsync(),
                HasIdentification = await systemUnitofWork.UserService.GetIdentificationQueryable().Where(r => r.UseAsDefault && !r.IsDeleted && (r.UserId == user.Id)).AnyAsync(),
                HasJobListed = await systemUnitofWork.UserService.GetJobQueryable().Where(r => r.IsCurrentJob && !r.IsDeleted && (r.UserId == user.Id)).AnyAsync(),
                HasNextofKin = await systemUnitofWork.UserService.GetNextOfKinQueryable().Where(r => r.UseAsDefault && !r.IsDeleted && (r.UserId == user.Id)).AnyAsync(),
                IsBasicProfileCompleted = (user.DateOfBirth.HasValue && !string.IsNullOrEmpty(user.Address) && !string.IsNullOrEmpty(user.Picture))
            });
        }

        private static List<Claim> GenerateClaims(UserViewModel user)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Surname, user.Surname),
                    new Claim(ClaimTypes.GivenName, user.Firstname)
                };


            foreach (ApplicationUserRoleModel role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
            }

            return claims;
        }

        [HttpPost]
        [Route("ResetPassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotRequest model)
        {
            UserViewModel user = await UserManager.FindByNameAsync(model.EmailAddress);
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Account));
            }


            string getToken = await UserManager.GeneratePasswordResetTokenAsync(user);
            string newpassword = PasswordGenerator.GetRandomPassword();

            user.MustChangePassword = true;
            user.LastPasswordChanged = DateTime.Now;
            user.TempPassword = newpassword;


            IdentityResult resetResult = await UserManager.ResetPasswordAsync(user, getToken, newpassword);
            if (!resetResult.Succeeded)
            {
                throw new WebsiteException(string.Format(Constants.UnableTo, Constants.Reset, Constants.Password));
            }

            await businessUnitofWork.EmailService.UserEmail(user.Id, EmailTypeEnums.ResetPassword);
            await businessUnitofWork.SaveAsync();
            return Ok(Constants.ResetInformation);

        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            string userId = GetUserId();

            UserViewModel user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Account));
            }


            if (!Regex.IsMatch(model.Password, @"^(?=.*\d)(?=.*[!@#$%^&*()_+\-\^=[\\\]|{};:<>?,.~])(?=.*[a-z])(?=.*[A-Z]).{8,}$"))
            {
                throw new WebsiteException(Constants.PasswordRequirement);
            }

            if (!Regex.IsMatch(model.Password, @"\d"))
            {
                throw new WebsiteException(Constants.PasswordRequirement);
            }

            if (!Regex.IsMatch(model.Password, @"[!@#$%^&*()_+\-\^=[\\\]|{};:<>?,.~]"))
            {
                throw new WebsiteException(Constants.PasswordRequirement);
            }

            if (!Regex.IsMatch(model.Password, @"[A-Z]"))
            {
                throw new WebsiteException(Constants.PasswordRequirement);
            }

            string getToken = await UserManager.GeneratePasswordResetTokenAsync(user);

            //Confirm this is working 
            user.MustChangePassword = false;
            user.LastPasswordChanged = DateTime.Now;

            IdentityResult changeresult = await UserManager.ResetPasswordAsync(user, getToken, model.Password);
            if (!changeresult.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Change, Constants.Password), changeresult.Errors.Select(r => WebUtility.HtmlDecode(r.Description)).ToList()));
            }

            await businessUnitofWork.EmailService.UserEmail(user.Id, EmailTypeEnums.ChangePassword);
            await businessUnitofWork.SaveAsync();

            return Ok(string.Format(Constants.ActionResponse, Constants.Password, Constants.Changed));
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(NewItemResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            UserViewModel newUser = model.ToNewUserEntity();

            newUser.TempPassword = PasswordGenerator.GetRandomPassword();
            string newUserId = await systemUnitofWork.UserService.CreateUserAccount(newUser, newUser.TempPassword, false);

            ClearCache();
            return Ok(new NewItemResponse<string>(newUserId, string.Format(Constants.ActionResponse, Constants.Account, Constants.Created)));
        }


        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<LiteUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GenerateCacheName(GetMethodName(), keyword ?? string.Empty, page, pageSize);
            List<LiteUserResponse> result = (List<LiteUserResponse>)GetFromCache(cachename);
            if (result == null)
            {
                IQueryable<UserViewModel> queryable = UserManager.Users.Where(r => (!r.IsDeleted)).AsQueryable();
                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower().Trim();
                    queryable = queryable.Where(r => r.Email.Contains(keyword) || r.Surname.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) || r.Firstname.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) || r.MiddleName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }

                result = await queryable.AsNoTracking()
                    .Select(r => new LiteUserResponse()
                    {
                        Id = r.Id,
                        EmailAddress = r.Email,
                        Firstname = r.Firstname,
                        MiddleName = r.MiddleName,
                        Surname = r.Surname,
                        PhoneNumber = r.PhoneNumber,
                        Picture = r.Picture,
                        CreatedDate = r.CreatedDate,
                        IsActive = r.IsActive,
                        IsAdmin = r.UserRoles.Any(t => t.Role.Name == SystemRole.Administrator)
                    })
                    .ToListAsync();

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("GetUserById")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(string id)
        {
            string cachename = GetMethodName() + id;
            UserResponse result = (UserResponse)GetFromCache(cachename);
            if (result == null)
            {
                UserViewModel user = await systemUnitofWork.UserService.GetQueryable().Where(r => (r.Id == id))
                                        .SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Account));
                }

                result = new UserResponse()
                {
                    Id = user.Id,
                    EmailAddress = user.Email,
                    Firstname = user.Firstname,
                    MiddleName = user.MiddleName,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber,
                    Picture = user.Picture,
                    CreatedDate = user.CreatedDate,
                    IsActive = user.IsActive,
                    Address = user.Address,
                    CityId = user.CityId ?? 0,
                    CountryId = user.CountryId ?? 0,
                    Sex = user.Sex,
                    StateId = user.StateId ?? 0,
                    DateOfBirth = user.DateOfBirth,
                    IsAdmin = await UserManager.IsInRoleAsync(user, SystemRole.Administrator),

                    IsBasicProfileCompleted = (user.DateOfBirth.HasValue && !string.IsNullOrEmpty(user.Address) && !string.IsNullOrEmpty(user.Picture)),
                    BankInformations = await systemUnitofWork.UserService.GetBankInformationQueryable().Where(r => r.IsDeleted && (r.UserId == user.Id)).Select(r => new BankResponse()
                    {
                        AccountName = r.AccountName,
                        AccountNumber = r.AccountNumber,
                        BankName = r.BankName,
                        Id = r.Id,
                        UseAsDefault = r.UseAsDefault
                    }).ToListAsync(),
                    Identifications = await systemUnitofWork.UserService.GetIdentificationQueryable().Where(r => r.IsDeleted && (r.UserId == user.Id)).Select(r => new IdentificationResponse()
                    {
                        UseAsDefault = r.UseAsDefault,
                        Id = r.Id,
                        IdUrl = r.IdUrl
                    }).ToListAsync(),
                    NextOfKins = await systemUnitofWork.UserService.GetNextOfKinQueryable().Where(r => r.IsDeleted && (r.UserId == user.Id)).Select(r => new NextOfKinResponse()
                    {
                        Id = r.Id,
                        UseAsDefault = r.UseAsDefault,
                        Address = r.Address,
                        Firstname = r.Firstname,
                        Relationship = r.Relationship,
                        Surname = r.Surname,
                        Phone = r.Phone
                    }).ToListAsync(),
                    Works = await systemUnitofWork.UserService.GetJobQueryable().Where(r => r.IsDeleted && (r.UserId == user.Id)).Select(r => new JobResponse()
                    {
                        CompanyAddress = r.CompanyAddress,
                        CompanyName = r.CompanyName,
                        EndDate = r.EndDate,
                        Id = r.Id,
                        IsCurrentJob = r.IsCurrentJob,
                        JobTitle = r.JobTitle,
                        StartDate = r.StartDate
                    }).ToListAsync()
                };

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMyProfile")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            string userId = GetUserId();
            string cachename = GetMethodName() + userId;
            UserResponse result = (UserResponse)GetFromCache(cachename);
            if (result == null)
            {
                UserViewModel user = await systemUnitofWork.UserService.GetQueryable().Where(r => (r.Id == userId))
                                        .SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Account));
                }

                result = new UserResponse()
                {
                    Id = user.Id,
                    EmailAddress = user.Email,
                    Firstname = user.Firstname,
                    MiddleName = user.MiddleName,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber,
                    Picture = user.Picture,
                    CreatedDate = user.CreatedDate,
                    IsActive = user.IsActive,
                    Address = user.Address,
                    CityId = user.CityId ?? 0,
                    CountryId = user.CountryId ?? 0,
                    Sex = user.Sex,
                    StateId = user.StateId ?? 0,
                    DateOfBirth = user.DateOfBirth,
                    IsAdmin = await UserManager.IsInRoleAsync(user, SystemRole.Administrator),

                    IsBasicProfileCompleted = (user.DateOfBirth.HasValue && !string.IsNullOrEmpty(user.Address) && !string.IsNullOrEmpty(user.Picture)),
                    BankInformations = await systemUnitofWork.UserService.GetBankInformationQueryable().Where(r => !r.IsDeleted && (r.UserId == user.Id)).Select(r => new BankResponse()
                    {
                        AccountName = r.AccountName,
                        AccountNumber = r.AccountNumber,
                        BankName = r.BankName,
                        Id = r.Id,
                        UseAsDefault = r.UseAsDefault
                    }).ToListAsync(),
                    Identifications = await systemUnitofWork.UserService.GetIdentificationQueryable().Where(r => !r.IsDeleted && (r.UserId == user.Id)).Select(r => new IdentificationResponse()
                    {
                        UseAsDefault = r.UseAsDefault,
                        Id = r.Id,
                        IdUrl = r.IdUrl
                    }).ToListAsync(),
                    NextOfKins = await systemUnitofWork.UserService.GetNextOfKinQueryable().Where(r => !r.IsDeleted && (r.UserId == user.Id)).Select(r => new NextOfKinResponse()
                    {
                        Id = r.Id,
                        UseAsDefault = r.UseAsDefault,
                        Address = r.Address,
                        Firstname = r.Firstname,
                        Relationship = r.Relationship,
                        Surname = r.Surname,
                        Phone = r.Phone
                    }).ToListAsync(),
                    Works = await systemUnitofWork.UserService.GetJobQueryable().Where(r => !r.IsDeleted && (r.UserId == user.Id)).Select(r => new JobResponse()
                    {
                        CompanyAddress = r.CompanyAddress,
                        CompanyName = r.CompanyName,
                        EndDate = r.EndDate,
                        Id = r.Id,
                        IsCurrentJob = r.IsCurrentJob,
                        JobTitle = r.JobTitle,
                        StartDate = r.StartDate
                    }).ToListAsync()
                };

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetSexes")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EnumResponse>), StatusCodes.Status200OK)]
        public IActionResult GetSexes()
        {
            return Ok(Enum.GetValues(typeof(SexEnums)).Cast<SexEnums>().Select(r => new
            {
                Id = (byte)r,
                Name = r.DisplayName() ?? r.ToString()
            }).ToList());
        }

        [HttpPost]
        [Route("CreateUserAccount")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUserAccount([FromBody] NewUserRequest model)
        {
            UserViewModel newUser = model.ToNewUserEntity();
            await UpdateUserLocation(model.CityId, newUser);

            string newpassword = PasswordGenerator.GetRandomPassword();
            newUser.TempPassword = newpassword;
            string newUserId = await systemUnitofWork.UserService.CreateUserAccount(newUser, newpassword, model.IsAdmin);


            ClearCache();
            return Ok(new NewItemResponse<string>(newUserId, string.Format(Constants.ActionResponse, Constants.Account, Constants.Created)));
        }


        [HttpPost]
        [Route("AddOrUpdateUserBankInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrUpdateUserBankInformation([FromBody] BankRequest model)
        {
            model.UserId = GetUserId();
            BankInformationModel result = await systemUnitofWork.UserService.SaveOrUpdateBank(model);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Bank, Constants.Saved)));
        }

        [HttpPost]
        [Route("AddOrUpdateUserIdentificationInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrUpdateUserIdentificationInformation([FromBody] IdentificationRequest model)
        {
            model.UserId = GetUserId();
            IdentificationModel result = await systemUnitofWork.UserService.SaveOrUpdateIdentification(model);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Identification, Constants.Saved)));
        }

        [HttpPost]
        [Route("AddOrUpdateUserWorkInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrUpdateUserWorkInformation([FromBody] JobRequest model)
        {
            model.UserId = GetUserId();
            JobModel result = await systemUnitofWork.UserService.SaveOrUpdateJob(model);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Work, Constants.Saved)));
        }

        [HttpPost]
        [Route("AddOrUpdateUserNextOfKinInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrUpdateUserNextOfKinInformation([FromBody] NextOfKinRequest model)
        {
            model.UserId = GetUserId();
            NextOfKinModel result = await systemUnitofWork.UserService.SaveOrUpdateNextOfKins(model);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.NextOfKin, Constants.Saved)));
        }


        [HttpPost]
        [Route("DeleteMyBankInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMyBankInformation([FromBody] BankIdRequest model)
        {
            string userId = GetUserId();
            BankInformationModel result = await systemUnitofWork.UserService.GetBankInformationQueryable().Where(r => (r.Id == model.Id) && !r.IsDeleted && (r.UserId == userId)).SingleOrDefaultAsync();
            if (result == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Bank));
            }

            result.ToDeletedEntity();
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Bank, Constants.Deleted)));
        }

        [HttpPost]
        [Route("DeleteMyIdentificationInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMyIdentificationInformation([FromBody] IdentificationIdRequest model)
        {
            string userId = GetUserId();
            IdentificationModel result = await systemUnitofWork.UserService.GetIdentificationQueryable().Where(r => (r.Id == model.Id) && !r.IsDeleted && (r.UserId == userId)).SingleOrDefaultAsync();
            if (result == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Identification));
            }

            result.ToDeletedEntity();
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Identification, Constants.Deleted)));
        }

        [HttpPost]
        [Route("DeleteMyWorkInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMyWorkInformation([FromBody] JobIdRequest model)
        {
            string userId = GetUserId();
            JobModel result = await systemUnitofWork.UserService.GetJobQueryable().Where(r => (r.Id == model.Id) && !r.IsDeleted && (r.UserId == userId)).SingleOrDefaultAsync();
            if (result == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.Work));
            }

            result.ToDeletedEntity();
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.Work, Constants.Deleted)));
        }

        [HttpPost]
        [Route("DeleteMyNextOfKinInformation")]
        [Authorize]
        [ProducesResponseType(typeof(NewItemResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMyNextOfKinInformation([FromBody] NextOfKinIdRequest model)
        {
            string userId = GetUserId();
            NextOfKinModel result = await systemUnitofWork.UserService.GetNextOfKinQueryable().Where(r => (r.Id == model.Id) && !r.IsDeleted && (r.UserId == userId)).SingleOrDefaultAsync();
            if (result == null)
            {
                throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.NextOfKin));
            }

            result.ToDeletedEntity();
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(new NewItemResponse<Guid>(result.Id, string.Format(Constants.ActionResponse, Constants.NextOfKin, Constants.Deleted)));
        }




        private async Task UpdateUserLocation(int? cityId, UserViewModel newitem)
        {
            if (cityId.HasValue && (cityId > 0))
            {
                CityModel city = await systemUnitofWork.CityService.GetCityWithState(cityId.Value);
                if (city == null)
                {
                    throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.City));
                }

                newitem.CountryId = ((city == null) ? ((int?)null) : (city.State.CountryId));
                newitem.StateId = ((city == null) ? ((int?)null) : (city.StateId));
            }
        }

        [HttpPost]
        [Route("UpdateUserAccount")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAccount([FromBody] ExistingUserRequest model)
        {
            UserViewModel user = await UserManager.Users.Where(r => r.Id == model.Id)
                .FirstOrDefaultAsync();

            if ((user.CityId != model.CityId))
            {
                await UpdateUserLocation(model.CityId, user);
            }

            model.ToUpdateEntity(user);

            IdentityResult updateResult = await UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Update, Constants.Account), updateResult.Errors.Select(r => r.Description).ToList()).ToString());
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Account, Constants.Updated));
        }


        [HttpPost]
        [Route("AddUserToAdminRole")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserToAdminRole([FromBody] UserIdRequest model)
        {
            UserViewModel user = await UserManager.Users.FirstOrDefaultAsync(r => (r.Id == model.Id));
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Account));
            }

            bool exist = await UserManager.IsInRoleAsync(user, SystemRole.Administrator);
            if (!exist)
            {
                IdentityResult roleresult = await UserManager.AddToRoleAsync(user, SystemRole.Administrator);
                if (!roleresult.Succeeded)
                {
                    throw new WebsiteException(string.Format(Constants.UnableTo, Constants.Enable, Constants.AdminAccess));
                }
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.AdminAccess, Constants.Enabled));
        }

        [HttpPost]
        [Route("RemoveUserToAdminRole")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveUserToAdminRole([FromBody] UserIdRequest model)
        {
            UserViewModel user = await UserManager.Users.FirstOrDefaultAsync(r => (r.Id == model.Id));
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Account));
            }

            bool exist = await UserManager.IsInRoleAsync(user, SystemRole.Administrator);
            if (!exist)
            {
                IdentityResult roleresult = await UserManager.RemoveFromRoleAsync(user, SystemRole.Administrator);
                if (!roleresult.Succeeded)
                {
                    throw new WebsiteException(string.Format(Constants.UnableTo, Constants.Remove, Constants.AdminAccess));
                }
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.AdminAccess, Constants.Removed));
        }

        [HttpPost]
        [Route("UpdateMyAccount")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMyAccount([FromBody] MyUserRequest model)
        {
            UserViewModel user = await UserManager.Users.Where(r => r.Id == model.Id)
                .FirstOrDefaultAsync();

            //if (user.CityId != model.CityId)
            //{
            //    CityModel city = await systemUnitofWork.CityService.GetCityWithState(model.CityId.Value);
            //    if ((city == null) && model.CityId.HasValue)
            //    {
            //        throw new WebsiteException(string.Format(Constants.InvalidInformation, Constants.City));
            //    }
            //    user.CountryId = ((city == null) ? ((int?)null) : (city.State.CountryId));
            //    user.StateId = ((city == null) ? ((int?)null) : (city.StateId));
            //}

            model.ToMyUserUpdateEntity(user);

            IdentityResult updateResult = await UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Update, Constants.Account), updateResult.Errors.Select(r => WebUtility.HtmlDecode(r.Description)).ToList()).ToString());
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Account, Constants.Updated));
        }

        [HttpPost]
        [Route("DeleteUserAccount")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAccount([FromBody] string id)
        {
            UserViewModel user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new WebsiteException(Constants.RecordDoesNotExist);

            }

            user.IsActive = false;
            user.IsDeleted = true;
            user.UserName = string.Join(string.Empty, DateTime.Now.ToString(Constants.DateFormatMMMddyyyyhhmm), "_deleted_", user.Email);
            user.Email = string.Join(string.Empty, DateTime.Now.ToString(Constants.DateFormatMMMddyyyyhhmm), "_deleted_", user.Email);
            user.UpdatedDate = DateTime.Now;

            IdentityResult updUser = await UserManager.UpdateAsync(user);
            if (!updUser.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Delete, Constants.Account), updUser.Errors.Select(r => WebUtility.HtmlDecode(r.Description)).ToList()).ToString());
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Account, Constants.Deleted));
        }

        [HttpPost]
        [Route("SuspendUserAccount")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> SuspendUserAccount([FromBody] string id)
        {
            UserViewModel user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Account));
            }

            user.IsActive = false;
            user.UpdatedDate = DateTime.Now;

            IdentityResult updUser = await UserManager.UpdateAsync(user);
            if (!updUser.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Suspend, Constants.Account), updUser.Errors.Select(r => r.Description).ToList()).ToString());
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Account, Constants.Suspended));
        }

        [HttpPost]
        [Route("ActivateUserAccount")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActivateUserAccount([FromBody] string id)
        {
            UserViewModel user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Account));
            }

            user.IsActive = true;
            user.UpdatedDate = DateTime.Now;

            IdentityResult updUser = await UserManager.UpdateAsync(user);
            if (!updUser.Succeeded)
            {
                throw new WebsiteException(string.Join(Constants.Comma, string.Format(Constants.UnableTo, Constants.Activate, Constants.Account), updUser.Errors.Select(r => r.Description).ToList()).ToString());
            }

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Account, Constants.Activated));
        }

    }
}
