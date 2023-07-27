using Lafiami.Models.Internals;
using LafiamiAPI.Interfaces;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LafiamiAPI.Services
{
    public class WebAPIService : IWebAPI
    {
        private readonly AppSettings _appSettings;
        public WebAPIService(IOptions<AppSettings> config)
        {
            _appSettings = config.Value;
        }

        public async Task<R> Get<R>(string endPointAddress, CompanyEnum company)
        {
            using (HttpClient client = new HttpClient())
            {
                RequiredRequestInformation(client, company);

                using (HttpResponseMessage response = client.GetAsync(endPointAddress).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string incomingResult = await content.ReadAsStringAsync();
                        Console.WriteLine(incomingResult);
                        return DataDeserialized<R>(response, incomingResult);
                    }
                }
            }
        }

        private static R DataDeserialized<R>(HttpResponseMessage response, string incomingResult)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(incomingResult.Trim());
                return JsonSerializer.Deserialize<R>(incomingResult.Trim());
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new WebsiteException(incomingResult);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new WebsiteException(incomingResult);
            }
            else if (response.StatusCode == HttpStatusCode.NotImplemented)
            {
                throw new NotImplementedException(incomingResult);
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException(incomingResult);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException(incomingResult);
            }
            else
            {
                throw new WebsiteException(response.ReasonPhrase);
            }
        }

        private void RequiredRequestInformation(HttpClient client, CompanyEnum company)
        {
            switch (company)
            {
                case CompanyEnum.Lafiami:
                    break;
                case CompanyEnum.Aiico:
                    string decodedAPIKey = WebUtility.HtmlDecode(_appSettings.AiicoAPIKey);
                    client.BaseAddress = new Uri(_appSettings.AiicoAPIAddress);
                    client.DefaultRequestHeaders.Add(CustomHttpHeader.ApiKey, decodedAPIKey);
                    break;
                case CompanyEnum.Hygeia:
                    client.BaseAddress = new Uri(_appSettings.HygeiaAPIAddress);
                    break;
                case CompanyEnum.AxaMansand:
                    break;
                case CompanyEnum.RelainceHMO:
                    break;
                default:
                    break;
            }


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<R> Post<T, R>(string endPointAddress, T value, CompanyEnum company)
        {
            using (HttpClient client = new HttpClient())
            {
                RequiredRequestInformation(client, company);
                string json = JsonSerializer.Serialize(value);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = client.PostAsync(endPointAddress, httpContent).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string incomingResult = await content.ReadAsStringAsync();
                        return DataDeserialized<R>(response, incomingResult);
                    }
                }
            }
        }

        public async Task<R> PostFormDataAsJson<R>(string endPointAddress, Dictionary<string, string> values, CompanyEnum company)
        {
            using (HttpClient client = new HttpClient())
            {
                RequiredRequestInformation(client, company);
                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(values), Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (HttpResponseMessage response = client.PostAsync(endPointAddress, httpContent).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string incomingResult = await content.ReadAsStringAsync();
                        return DataDeserialized<R>(response, incomingResult);
                    }
                }
            }
        }

        public async Task<R> PostFormDataAsEncoded<R>(string endPointAddress, Dictionary<string, string> values, CompanyEnum company, string bearerToken)
        {
            using (HttpClient client = new HttpClient())
            {
                RequiredRequestInformation(client, company);
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    client.DefaultRequestHeaders.Add(CustomHttpHeader.Authorization, "Bearer " + bearerToken);
                }
                FormUrlEncodedContent httpContent = new FormUrlEncodedContent(values);

                using (HttpResponseMessage response = client.PostAsync(endPointAddress, httpContent).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string incomingResult = await content.ReadAsStringAsync();
                        return DataDeserialized<R>(response, incomingResult);
                    }
                }
            }
        }


        public async Task<R> Post<R>(string endPointAddress, CompanyEnum company)
        {
            using (HttpClient client = new HttpClient())
            {
                RequiredRequestInformation(client, company);
                string json = JsonSerializer.Serialize(string.Empty);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = client.PostAsync(endPointAddress, httpContent).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string incomingResult = await content.ReadAsStringAsync();
                        return DataDeserialized<R>(response, incomingResult);
                    }
                }
            }
        }
    }
}
