using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class HygeiaResponse
    {
    }

    public class HygeiaStateResponse
    {
        public int stateid { get; set; }
        public string statename { get; set; }
    }
    public class HygeiaLGAResponse
    {
        public int cityId { get; set; }
        public string cityName { get; set; }
    }
    public class HygeiaRawIdNameResponse
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class HygeiaRegistrationResponse
    {
        public bool success { get; set; }
        public int memberId { get; set; }
        public string legacyCode { get; set; }
        public string dependantId { get; set; }
        public string message { get; set; }
    }

    public class HygeiaTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }

    }
}
