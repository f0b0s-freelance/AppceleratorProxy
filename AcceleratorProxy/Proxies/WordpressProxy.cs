using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AppceleratorProxy.Infrastructure;
using AppceleratorProxy.Objects.Wordpress;

namespace AppceleratorProxy.Proxies
{
    public class WordpressProxy : ProxyBase
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _code;
        private readonly string _domain;
        private IsolatedStorageRepository<string> isolatedStorageRepository = new IsolatedStorageRepository<string>();  
        private string _token;
        private bool _disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientId">Your Application OAuth Client Id</param>
        /// <param name="clientSecret">Your Application OAuth Client Secret</param>
        /// <param name="redirectUri">Your Application OAuth Redirect url</param>
        /// <param name="code">Core retrieved from OAuth authorization on redirect url</param>
        /// <param name="domain">The site ID, The site domain</param>
        public WordpressProxy(string clientId, string clientSecret, string redirectUri, string code, string domain) : base()
        {
            if(string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("'clientId' can't be empty", "clientId");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("'clientSecret' can't be empty", "clientSecret");
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                throw new ArgumentException("'redirectUri' can't be empty", "redirectUri");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("'code' can't be empty", "code");
            }

            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException("'domain' can't be empty", "domain");
            }

            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _code = code;
            _domain = domain;
        }

        /// <summary>
        /// Return a single Post (by slug)
        /// </summary>
        /// <param name="postSlug">The post slug (a.k.a. sanitized name)</param>
        /// <returns>Post object</returns>
        public Task<Post> GetPostBySlug(string postSlug)
        {
            if (string.IsNullOrEmpty(postSlug))
            {
                throw new ArgumentException("'postSlug' can't be empty", "postSlug");
            }

            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/slug:{1}", _domain, postSlug);
            return ReadObject<Post>(url);
        }

        /// <summary>
        /// Return a single Post (by ID)
        /// </summary>
        /// <param name="postId">The post ID</param>
        /// <returns>Post object</returns>
        public Task<Post> GetPostById(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("'postId' can't be empty", "postId");
            }

            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", _domain, postId);
            var serializerSettings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true};
            return ReadObject<Post>(url, serializerSettings);
        }

        /// <summary>
        /// Delete a Post
        /// </summary>
        /// <param name="postId">The post ID</param>
        /// <returns>Deleted Post object</returns>
        public Task<Post> DeletePost(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("'postId' can't be empty", "postId");
            }

            return DeletePostInner(postId);
        }

        /// <summary>
        /// Edit a Post
        /// </summary>
        /// <param name="postInfo">PostInfo object</param>
        /// <param name="postId">The post ID</param>
        /// <returns>Updated Post object</returns>
        public Task<Post> EditPost(PostInfo postInfo, string postId)
        {
            if (postInfo == null)
            {
                throw new ArgumentNullException("postInfo");
            }

            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("'postId' can't be empty", "postId");
            }

            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}", _domain, postId);
            return EditPostInner(postInfo, url);
        }

        /// <summary>
        /// Create a Post
        /// </summary>
        /// <param name="postInfo">PostInfo object</param>
        /// <returns>Created Post object</returns>
        public Task<Post> CreatePost(PostInfo postInfo)
        {
            if (postInfo == null)
            {
                throw new ArgumentNullException("postInfo");
            }

            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/new", _domain);
            return EditPostInner(postInfo, url);
        }

        private async Task<Post> EditPostInner(PostInfo postInfo, string url)
        {
            var token = await GetToken();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));

            var multipart = GetPostDataContent(postInfo);
            requestMessage.Content = multipart;
            return await SendRequest<Post>(requestMessage);
        }

        private async Task<Post> DeletePostInner(string postId)
        {
            var token = await GetToken();
            var url = string.Format("https://public-api.wordpress.com/rest/v1/sites/{0}/posts/{1}/delete", _domain, postId);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("authorization", string.Format("Bearer {0}", token));
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

            if (isolatedStorageRepository.Exists(_code))
            {
                var key = isolatedStorageRepository.Read(_code);
                _token = key;
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
            isolatedStorageRepository.Write(_code, _token);

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
                    HttpClient.Dispose();
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
