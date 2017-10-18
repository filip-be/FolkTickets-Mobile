namespace SharpCommerce.Web
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class WoocommerceApiDriver
    {
        private readonly WoocommerceApiUrlGenerator urlGenerator;

        internal WoocommerceApiDriver(string storeUrl, string consumerKey, string consumerSecret)
        {
            this.urlGenerator = new WoocommerceApiUrlGenerator(storeUrl, consumerKey, consumerSecret);
        }

        internal async Task<string> Delete(string apiEndpoint, Dictionary<string, string> parameters = null, string jsonData = null)
        {
            return await MakeApiUploadStringCall(HttpMethod.Delete, apiEndpoint, parameters, jsonData);
        }

        internal async Task<string> Post(string apiEndpoint, Dictionary<string, string> parameters = null, string jsonData = null)
        {
            return await MakeApiUploadStringCall(HttpMethod.Post, apiEndpoint, parameters, jsonData);
        }

        internal async Task<string> Put(string apiEndpoint, Dictionary<string, string> parameters = null, string jsonData = null)
        {
            return await MakeApiUploadStringCall(HttpMethod.Put, apiEndpoint, parameters, jsonData);
        }

        internal async Task<string> Get(string apiEndpoint, Dictionary<string, string> parameters = null)
        {
            return await MakeApiDownloadStringCall(apiEndpoint, parameters);
        }

        // Basis for GET
        private async Task<string> MakeApiDownloadStringCall(string apiEndpoint, Dictionary<string, string> parameters = null)
        {
            var url = urlGenerator.GenerateRequestUrl(HttpMethod.Get, apiEndpoint, parameters);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return await client.GetStringAsync(url);
            }
        }
        
        // Basis for PUT, POST, and DELETE
        private async Task<string> MakeApiUploadStringCall(HttpMethod httpMethod, string apiEndpoint, Dictionary<string, string> parameters = null, string jsonData = null)
        {
            var url = this.urlGenerator.GenerateRequestUrl(httpMethod, apiEndpoint, parameters);
            using (var client = new HttpClient())
            {
                using (var content = new StringContent(jsonData ?? string.Empty))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    switch (httpMethod)
                    {
                        case HttpMethod.Post:
                            return await (await client.PostAsync(url, content)).Content.ReadAsStringAsync();
                        case HttpMethod.Put:
                            return await (await client.PutAsync(url, content)).Content.ReadAsStringAsync();
                        case HttpMethod.Delete:
                            return await (await client.DeleteAsync(url)).Content.ReadAsStringAsync();
                        default:
                            throw new ArgumentException("only POST, PUT, and DELETE allowed for this HTTP Method");
                    }
                }
            }
        }
    }
}
