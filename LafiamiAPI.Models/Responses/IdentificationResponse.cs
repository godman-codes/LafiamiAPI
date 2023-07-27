using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class IdentificationResponse
    {
        public Guid Id { get; set; } 
        public string IdUrl { get; set; }
        public bool UseAsDefault { get; set; }
    }
}
