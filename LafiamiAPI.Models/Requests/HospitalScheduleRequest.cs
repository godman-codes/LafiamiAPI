using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class HospitalScheduleRequest
    {
        public string titleId { get; set; }
        public string SurName { get; set; }
        public string otherNames { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string dob { get; set; }
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public string Occupation { get; set; }
        public string Genders { get; set; }
        public string address { get; set; }
        public string paymentFrequency { get; set; }
        public string durationOfCover { get; set; }
        public string effectiveDate { get; set; }
        public string sumAssured { get; set; }
        public string Hospital { get; set; }
        public string LgaId { get; set; }
        public string identificationName { get; set; }
        public string identificationUrl { get; set; }
        public string nextOfKinName { get; set; }
        public string nextOfKinPhone { get; set; }
        public string nextOfKinRelationship { get; set; }
        public string nextOfKinAddress { get; set; }
        public string subclassSectCovtypeId { get; set; }
        public string productId { get; set; }
        public string wef { get; set; }
        public string wet { get; set; }
    }
}
