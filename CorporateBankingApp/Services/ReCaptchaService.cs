using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; 

namespace CorporateBankingApp.Services
{
    public class ReCaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReCaptchaService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public static async Task<bool> VerifyReCaptchaV2(string response, string secret)
        {
            using (var client = new HttpClient())
            {
                string url = "https://www.google.com/recaptcha/api/siteverify";
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(response), "response");
                content.Add(new StringContent(secret), "secret");
                var result = await client.PostAsync(url, content);

                if (result.IsSuccessStatusCode)
                {
                    var stringResponse = await result.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(stringResponse);
                    return jsonResponse["success"].Value<bool>();
                }
            }
            return false;
        }

    }

}
