using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class LiteHospitalResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LiteHospitalWithInsurnaceStatusResponse: LiteHospitalResponse
    {
        public bool Status { get; set; }
    }

    public class HospitalResponse: LiteHospitalResponse
    {
        public string Address { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
    }
}
