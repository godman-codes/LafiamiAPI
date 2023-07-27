using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewOrderRequest
    {
        public BankRequest Bank { get; set; }
        public NextOfKinRequest NextOfKin { get; set; }
        public IdentificationRequest Identification { get; set; }
        public JobRequest Work { get; set; }
        public int? HospitalId { get; set; }
        public List<CompanyOrderExtraResultRequest> CompanyExtraResults { get; set; } = new List<CompanyOrderExtraResultRequest>();
        public bool ForSomeoneElse { get; set; }         
        public SomeoneElseInformationRequest SomeoneElse { get; set; } 
    }

    public class SomeoneElseInformationRequest
    {
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Picture { get; set; }
        [Required]
        [Range(1, 2, ErrorMessage ="Out of range")]
        public SexEnums Sex { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }

    public class CompanyOrderExtraResultRequest
    {
        [Required]
        public int CompanyExtraId { get; set; }
        [Required]
        public AnswerTypeEnum AnswerType { get; set; }
        public string ResultInString { get; set; }
        public string ResultInHTML { get; set; }
        public int? ResultInNumber { get; set; }
        public DateTime? ResultInDateTime { get; set; }
        public decimal? ResultInDecimal { get; set; }
        public bool? ResultInBool { get; set; }
    }

    public class CancelOrRejectOrderRequest
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }

}
