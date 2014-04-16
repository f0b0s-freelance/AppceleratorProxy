using System.Runtime.Serialization;

namespace WordpressProxy
{
    [DataContract]
    public class AuthResult
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "blog_id")]
        public string BlogId { get; set; }

        [DataMember(Name = "blog_url")]
        public string BlogUrl { get; set; }
    }
}