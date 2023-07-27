using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class NextOfKinRequest
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Relationship is required")]
        public string Relationship { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }

        public NextOfKinModel ToDBModel() {
            return new NextOfKinModel()
            {
                Address = Address,
                Firstname = Firstname,
                Relationship = Relationship,
                Surname = Surname,
                Id = Guid.NewGuid(),
                UseAsDefault = UseAsDefault,
                UserId = UserId,
                Phone = Phone,
            };
        }
    }
    public class NextOfKinIdRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
