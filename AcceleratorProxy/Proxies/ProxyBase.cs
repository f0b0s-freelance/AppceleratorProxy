using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace AppceleratorProxy.Proxies
{
    public class ProxyBase
    {
        public ProxyBase()
        {
            HttpClient = new HttpClient();
        }

        protected readonly HttpClient HttpClient;

        protected async Task<T> ReadObject<T>(string url, DataContractJsonSerializerSettings settings = null)
        {
            var responseTask = await HttpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = settings == null
                                 ? new DataContractJsonSerializer(typeof (T))
                                 : new DataContractJsonSerializer(typeof (T), settings);
            return (T)serializer.ReadObject(responseTask);
        }

        protected async Task<T> ReadPost<T>(string url, HttpContent content, DataContractJsonSerializerSettings settings = null)
        {
            var responseMessage = await HttpClient.PostAsync(url, content).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = settings == null
                                 ? new DataContractJsonSerializer(typeof(T))
                                 : new DataContractJsonSerializer(typeof(T), settings);

            return (T)serializer.ReadObject(responseStream);
        }

        protected async Task<T> SendRequest<T>(HttpRequestMessage message)
        {
            var responseMessage = await HttpClient.SendAsync(message).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(T));

            return (T)serializer.ReadObject(responseStream);
        }

        protected static StringContent GetNameDataContent(string name, string value)
        {
            var nameContent = new StringContent(value);
            nameContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                         {
                                                             Name = name
                                                         };
            nameContent.Headers.ContentType = null;
            return nameContent;
        }
    }
}