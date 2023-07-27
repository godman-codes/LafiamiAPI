using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces
{
    public interface IWebAPI
    {
        Task<R> Get<R>(string endPointAddress, CompanyEnum company);
        Task<R> Post<T, R>(string endPointAddress, T value, CompanyEnum company);
        Task<R> Post<R>(string endPointAddress, CompanyEnum company);
        Task<R> PostFormDataAsJson<R>(string endPointAddress, Dictionary<string, string> values, CompanyEnum company);
        Task<R> PostFormDataAsEncoded<R>(string endPointAddress, Dictionary<string, string> values, CompanyEnum company, string bearerToken);
    }
}
