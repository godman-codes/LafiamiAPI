using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class NextOfKinResponse
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Address { get; set; }
        public bool UseAsDefault { get; set; }
    }
}
