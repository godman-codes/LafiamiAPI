using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{
    public class NewUserCountPerDayResponnse
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }

    public class BasicUserResponse
    {
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
    }

    public class LiteUserResponse : BasicUserResponse
    {
        public string Id { get; set; }
        public string MiddleName { get; set; }
        public string Picture { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UserResponse : LiteUserResponse
    {
        public string Address { get; set; }
        public SexEnums? Sex { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsBasicProfileCompleted { get; set; }

        public List<BankResponse> BankInformations { get; set; }
        public List<JobResponse> Works { get; set; }
        public List<NextOfKinResponse> NextOfKins { get; set; }
        public List<IdentificationResponse> Identifications { get; set; }
    }



}
