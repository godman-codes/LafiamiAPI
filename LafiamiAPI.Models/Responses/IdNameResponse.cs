using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Responses
{
    public class IdNameResponse<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
    }

    public class AiicoIdNameResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class HygeiaIdNameResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    

    public class AiicoBasicResponse
    {
        public List<AiicoBasicResultResponse> result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }

    public class AiicoBasicResultResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string prefix { get; set; }
        public bool reqUW { get; set; }
        public bool renewable { get; set; }
        public string wef { get; set; }
        public string wet { get; set; }
        public string imageUrl { get; set; }
        public string description { get; set; }
        public string productCategory { get; set; }
        public bool isThirdParty { get; set; }
        public bool isActive { get; set; }
    }

    public class AiicoBasicCoverResponse
    {
        public List<SubCoverTypeResponse> result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }

    public class SubCoverTypeResponse
    {
        public CoverTypeResponse subClassCoverTypes { get; set; }
        public List<BenefitResponse> benefits { get; set; }
    }

    public class BenefitResponse
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class CoverTypeResponse
    {
        public string id { get; set; }
        public string coverTypeName { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string subClassName { get; set; }
        public string description { get; set; }
        public string benefit { get; set; }
        public decimal rate { get; set; }
        
        public bool @fixed { get; set; }
        public string sectionType { get; set; }
    }
}
