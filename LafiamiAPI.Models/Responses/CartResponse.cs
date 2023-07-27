using LafiamiAPI.Utilities.Enums;
using System;

namespace LafiamiAPI.Models.Responses
{
    public class CartResponse : LiteCartResponse
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class LiteCartResponse
    {
        public Guid Id { get; set; }
        public CompanyEnum  CompanyId { get; set; }
        public bool HasFixedQuantity { get; set; }
        public long? InsurancePlanId { get; set; }
        public string ItemName { get; set; }
        public string Picture { get; set; }
        public string UserId { get; set; }
        public int QuatityOrder { get; set; }
        public decimal UnitAmount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }
        public bool UseSystemHospital { get; set; }


    }

}
