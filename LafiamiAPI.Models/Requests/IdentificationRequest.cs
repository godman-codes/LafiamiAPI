using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class IdentificationRequest
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Id URL is required")]
        public string IdUrl { get; set; }
        public bool UseAsDefault { get; set; }
        public string UserId { get; set; }

        public IdentificationModel ToDBModel()
        {
            return new IdentificationModel()
            {
                IdUrl = IdUrl,
                Id = Guid.NewGuid(),
                UseAsDefault = UseAsDefault,
                UserId = UserId
            };
        }
    }

    public class IdentificationIdRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
