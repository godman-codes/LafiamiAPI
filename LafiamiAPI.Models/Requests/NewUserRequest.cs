using LafiamiAPI.Datas.Models;
using LafiamiAPI.Utilities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class EmailRequest
    {
        public EmailRequest()
        {
            EmailAddress = ((string.IsNullOrEmpty(EmailAddress)) ? ("") : (EmailAddress.ToLower()));
        }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [EmailAddress(ErrorMessage = "E-mail is not valid")]
        public string EmailAddress { get; set; }
    }

    public class BasicUserRequest : EmailRequest
    {

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(2)]
        public string Surname { get; set; }
        [Required]
        [MinLength(2)]
        public string Firstname { get; set; }
        public string MiddleName { get; set; }

        public UserViewModel ToBasicEntity()
        {
            return new UserViewModel()
            {
                PhoneNumber = PhoneNumber,
                Email = EmailAddress,
                Firstname = Firstname,
                Surname = Surname,
                IsActive = true,
                MiddleName = MiddleName,
                MustChangePassword = false,
                LastPasswordChanged = DateTime.Now,
                UserName = EmailAddress
            };
        }
    }

    public class LiteRegisterRequest : BasicUserRequest
    {
        public string ReferralCode { get; set; }

    }

    public class RegisterRequest : BasicUserRequest
    {
        // public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Picture { get; set; }
        [Required]
        public string Address { get; set; }
        public UserViewModel ToNewUserEntity()
        {
            UserViewModel user = ToBasicEntity();
            user.DateOfBirth = DateOfBirth;
            user.Picture = Picture;
            user.Address = Address;

            return user;
        }
    }

    public class NewUserRequest : BasicUserRequest
    {
        public string Address { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; }
        public SexEnums? Sex { get; set; }
        public int? CityId { get; set; }
        public bool IsAdmin { get; set; }
        public UserViewModel ToNewUserEntity()
        {
            UserViewModel user = ToBasicEntity();
            user.Address = Address;
            user.CityId = CityId;
            user.Picture = Picture;
            user.Sex = Sex;
            user.DateOfBirth = DateOfBirth;

            return user;
        }
    }

    public class ExistingUserRequest : MyUserRequest
    {
        public void ToUpdateEntity(UserViewModel user)
        {
            ToMyUserUpdateEntity(user);
        }
    }

    public class UserIdRequest
    {
        [Required]
        public string Id { get; set; }
    }

    public class MyUserRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public SexEnums? Sex { get; set; }
        public int? CityId { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        public void ToMyUserUpdateEntity(UserViewModel user)
        {
            user.Address = Address;
            user.CityId = (CityId > 0) ? (CityId) : (null);
            user.PhoneNumber = PhoneNumber;
            user.Firstname = Firstname;
            user.Surname = Surname;
            user.MiddleName = MiddleName;
            user.Picture = Picture;
            user.Sex = Sex;
            user.UpdatedDate = DateTime.Now;
            user.DateOfBirth = DateOfBirth;
        }
    }
}
