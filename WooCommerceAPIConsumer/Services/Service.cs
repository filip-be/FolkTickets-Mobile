namespace SharpCommerce.Services
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public abstract class Service
    {
        protected readonly WoocommerceApiDriver ApiDriver;

        protected Service(WoocommerceApiDriver apiDriver)
        {
            ApiDriver = apiDriver;
        }

        protected async Task<T> Post<T>(string apiEndpoint, Dictionary<string, string> parameters = null, T toSerialize = default(T))
        {
            var jsonData = JsonConvert.SerializeObject(toSerialize);
            var jsonResult = await ApiDriver.Post(apiEndpoint, parameters, jsonData);
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }

        protected async Task<T> Put<T>(string apiEndpoint, Dictionary<string, string> parameters = null, T toSerialize = default(T))
        {
            var jsonData = JsonConvert.SerializeObject(toSerialize);
            var jsonResult = await ApiDriver.Put(apiEndpoint, parameters, jsonData);
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }

        protected async Task<T> Delete<T>(string apiEndpoint, Dictionary<string, string> parameters = null)
        {
            var jsonResult = await ApiDriver.Delete(apiEndpoint, parameters);
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }

        protected async Task<T> Get<T>(string apiEndpoint, Dictionary<string, string> parameters = null)
        {
            var jsonResult = await ApiDriver.Get(apiEndpoint, parameters);
            return JsonConvert.DeserializeObject<T>(jsonResult);
        }
    }
}
