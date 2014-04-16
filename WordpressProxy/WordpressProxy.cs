using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace WordpressProxy
{
    public class WordpressProxy
    {
        HttpClient _httpClient = new HttpClient();

        public async Task Create(string title, string content, string tags, string categories)
        {
            var multipart = new MultipartFormDataContent();

            var result = await _httpClient.PostAsync("https://public-api.wordpress.com/rest/v1/sites/30434183/posts/new/",
                                               multipart).ConfigureAwait(false);
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
        }

        public async Task<Post> GetPostBySlug(string domain, string postSlug)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/slug:{1}", domain, postSlug);
            var result = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(Post));
            return (Post)serializer.ReadObject(result);
        }

        public async Task<Post> GetPostById(string domain, string postId)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", domain, postId);
            var result = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof (Post));
            return (Post)serializer.ReadObject(result);
        }

        public async Task<Post> DeletePost(string domain, string postId, string authKey)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}/delete", domain, postId);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", "Bearer XCmf6l7Sf4");
            var responseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof (Post));

            return (Post)serializer.ReadObject(responseStream);
        }
    }
}
