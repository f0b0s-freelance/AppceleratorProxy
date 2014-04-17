using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AppceleratorProxy.Objects.Wordpress;

namespace AppceleratorProxy.Proxies
{
    public class WordpressProxy : ProxyBase
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _code;
        readonly HttpClient _httpClient = new HttpClient();
        private string _token;
        private bool _disposed;

        public WordpressProxy(string clientId, string clientSecret, string redirectUri, string code) : base()
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _code = code;
        }
        
        public Task<Post> GetPostBySlug(string domain, string postSlug)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/slug:{1}", domain, postSlug);
            return ReadObject<Post>(url);
        }

        public Task<Post> GetPostById(string domain, string postId)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", domain, postId);
            var serializerSettings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true};
            return ReadObject<Post>(url, serializerSettings);
        }

        public async Task<Post> DeletePost(string domain, string postId)
        {
            var token = await GetToken();
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}/delete", domain, postId);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));
            return await SendRequest<Post>(requestMessage);
        }

        public Task<Post> UpdatePost(string domain, PostInfo postInfo, string postId)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", domain, postId);
            return UpdatePostInner(postInfo, url);
        }

        public Task<Post> CreatePost(string domain, PostInfo postInfo)
        {
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/new", domain);
            return UpdatePostInner(postInfo, url);
        }

        private async Task<Post> UpdatePostInner(PostInfo postInfo, string url)
        {
            var token = await GetToken();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));

            var multipart = GetPostDataContent(postInfo);
            requestMessage.Content = multipart;
            return await SendRequest<Post>(requestMessage);
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

        private async Task<string> GetToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return _token;
            }

            const string url = "https://public-api.wordpress.com/oauth2/token";
            var multipart = new MultipartFormDataContent();

            var clientId = GetNameDataContent("client_id", _clientId);
            multipart.Add(clientId);

            var clientSecret = GetNameDataContent("client_secret", _clientSecret);
            multipart.Add(clientSecret);

            var code = GetNameDataContent("code", _code);
            multipart.Add(code);

            var redirectUri = GetNameDataContent("redirect_uri", _redirectUri);
            multipart.Add(redirectUri);

            var grantType = GetNameDataContent("grant_type", "authorization_code");
            multipart.Add(grantType);

            var authResult = await ReadPost<AuthResult>(url, multipart);
            if (string.IsNullOrEmpty(authResult.AccessToken))
            {
                throw new InvalidOperationException("Can't obtain access token");
            }

            _token = authResult.AccessToken;
            return _token;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                _disposed = true;
            }
        }

        ~WordpressProxy()
        {
            Dispose(false);
        }
    }
}
