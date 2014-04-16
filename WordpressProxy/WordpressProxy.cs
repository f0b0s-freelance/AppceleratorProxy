using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WordpressProxy
{
    public class WordpressProxy
    {
        readonly HttpClient _httpClient = new HttpClient();

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
            const string token = "X1LL&Lan8M#L@Fhzr77%ZW&8U2yfTBPq4Qa&ObMzdD)#OlGSjpllolevZn(@JI7%"; //GetToken();

            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}/delete", domain, postId);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));
            var responseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof (Post));

            return (Post)serializer.ReadObject(responseStream);
        }

        public async Task<Post> UpdatePost(string domain, PostInfo postInfo, string postId)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", domain, postId);
            return await UpdatePostInner(postInfo, url);
        }

        public async Task<Post> CreatePost(string domain, PostInfo postInfo)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/new", domain);
            return await UpdatePostInner(postInfo, url);
        }

        private async Task<Post> UpdatePostInner(PostInfo postInfo, string url)
        {
            const string token = "X1LL&Lan8M#L@Fhzr77%ZW&8U2yfTBPq4Qa&ObMzdD)#OlGSjpllolevZn(@JI7%"; //GetToken();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));

            var multipart = GetPostDataContent(postInfo);

            requestMessage.Content = multipart;

            var responseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(Post));

            return (Post)serializer.ReadObject(responseStream);
        }

        private static MultipartFormDataContent GetPostDataContent(PostInfo postInfo)
        {
            var multipart = new MultipartFormDataContent();
            var title = GetNameDataContent("title", postInfo.Title);
            multipart.Add(title);

            var content = GetNameDataContent("content", postInfo.Content);
            multipart.Add(content);

            var excerptContent = GetNameDataContent("excerpt", postInfo.Excerpt);
            multipart.Add(excerptContent);

            var statusContent = GetNameDataContent("status", postInfo.PostStatus.ToString().ToLower());
            multipart.Add(statusContent);

            var parentContent = GetNameDataContent("parent", postInfo.Parent);
            multipart.Add(parentContent);

            var tags = GetString(postInfo.Tags);
            var tagsContent = GetNameDataContent("tags", tags);
            multipart.Add(tagsContent);

            var categories = GetString(postInfo.Categories);
            var categoriesContent = GetNameDataContent("categories", categories);
            multipart.Add(categoriesContent);

            var slugContent = GetNameDataContent("slug", postInfo.Slug);
            multipart.Add(slugContent);

            var formatContent = GetNameDataContent("format", postInfo.Format.ToString().ToLower());
            multipart.Add(formatContent);

            if (postInfo.CommentsOpen.HasValue)
            {
                var commentsOpenContent = GetNameDataContent("comments_open", postInfo.CommentsOpen.ToString());
                multipart.Add(commentsOpenContent);
            }

            if (postInfo.PingsOpen.HasValue)
            {
                var pingsOpenContent = GetNameDataContent("pings_open", postInfo.PingsOpen.ToString());
                multipart.Add(pingsOpenContent);
            }

            return multipart;
        }

        private static string GetString(IEnumerable<string> items)
        {
            var itemsString = new StringBuilder();
            foreach (var item in items)
            {
                itemsString.Append(item + ",");
            }

            if (itemsString.Length != 0)
                itemsString.Remove(itemsString.Length - 1, 1);

            return itemsString.ToString();
        }

        private async Task<AuthResult> GetToken()
        {
            const string url = "https://public-api.wordpress.com/oauth2/token";
            var multipart = new MultipartFormDataContent();

            var clientId = GetNameDataContent("client_id", "34711");
            multipart.Add(clientId);

            var clientSecret = GetNameDataContent("client_secret", "REryTBb536tsDFZryLsLE8WinmStTNQShP6B2W8yGXnqMgJZxVA5fTAZB8EEJuVU");
            multipart.Add(clientSecret);

            var code = GetNameDataContent("code", "W5k2NX1wVv");
            multipart.Add(code);

            var redirectUri = GetNameDataContent("redirect_uri", "http://skyfer.com");
            multipart.Add(redirectUri);

            var grantType = GetNameDataContent("grant_type", "authorization_code");
            multipart.Add(grantType);

            var responseMessage = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(AuthResult));

            return (AuthResult)serializer.ReadObject(responseStream);
        }

        private static StringContent GetNameDataContent(string name, string value)
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
