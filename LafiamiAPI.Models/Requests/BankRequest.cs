using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class BankRequest
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Account Number is required")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Bank name is required")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Account Name is required")]
        public string AccountName { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }

        public BankInformationModel ToDBModel()
        {
            return new BankInformationModel()
            {
                AccountName = AccountName,
                AccountNumber = AccountNumber,
                BankName = BankName,
                Id = Guid.NewGuid(),
                UseAsDefault = UseAsDefault,
                UserId = UserId
            };
        }
    }

    public class BankIdRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
