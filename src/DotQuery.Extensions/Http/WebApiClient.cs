using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Extensions.Http
{
    /// <summary>
    /// A simple wrapper over HttpClient to make easy web api calls
    /// </summary>
    public class WebApiClient
    {
        private readonly HttpClient m_httpClient;
        private readonly string m_siteRoot;

        public WebApiClient(HttpClient httpClient, string siteRoot)
        {
            m_httpClient = httpClient;
            m_siteRoot = siteRoot;
        }

        public async Task<T> GetAsync<T>(string apiPath, string paramKey = null, object paramValue = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(apiPath, paramKey, paramValue));

            var response = await m_httpClient.SendAsync(request);
            return JsonConvert.DeserializeObject<T>(await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task DeleteAsync(string apiPath, string paramKey = null, object paramValue = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, BuildUrl(apiPath, paramKey, paramValue));
            var response = await m_httpClient.DeleteAsync(request.RequestUri);
            response.EnsureSuccessStatusCode();
        }

        public Task<HttpResponseMessage> PostAsync(object body, string apiPath, string paramKey = null, object paramValue = null)
        {
            return m_httpClient.PostAsync(BuildUrl(apiPath, paramKey, paramValue), new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
        }

        public Task<HttpResponseMessage> PutAsync(object body, string apiPath, string paramKey = null, object paramValue = null)
        {
            return m_httpClient.PutAsync(BuildUrl(apiPath, paramKey, paramValue), new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
        }

        public async Task<T> PutAsync<T>(object body, string apiPath, string paramKey = null, object paramValue = null)
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await m_httpClient.PutAsync(BuildUrl(apiPath, paramKey, paramValue), StringContent);
            return JsonConvert.DeserializeObject<T>(await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<T> PostAsync<T>(object body, string apiPath, string paramKey = null, object paramValue = null)
        {
            var StringContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await m_httpClient.PostAsync(BuildUrl(apiPath, paramKey, paramValue), StringContent);
            return JsonConvert.DeserializeObject<T>(await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false));
        }


        private string BuildUrl(string apiPath, string paramKey = null, object paramValue = null)
        {
            if (paramKey != null)
            {
                var valueString = string.Empty;
                if (paramValue != null)
                {
                    valueString = Uri.EscapeDataString(paramValue.ToString());
                }

                return string.Format("{0}{1}?{2}={3}", m_siteRoot, apiPath, paramKey, valueString);
            }
            else return string.Format("{0}{1}", m_siteRoot, apiPath);
        }
    }
}
