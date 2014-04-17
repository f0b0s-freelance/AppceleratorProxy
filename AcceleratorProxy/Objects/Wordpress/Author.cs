using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Wordpress
{
    [DataContract]
    public class Author
    {
        [DataMember(Name = "ID")]
        public string Id { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "nice_name")]
        public string NiceName { get; set; }

        [DataMember(Name = "URL")]
        public string Url { get; set; }

        [DataMember(Name = "avatar_URL")]
        public string AvatarUrl { get; set; }

        [DataMember(Name = "profile_URL")]
        public string ProfileUrl { get; set; }

        [DataMember(Name = "site_ID")]
        public string SiteId { get; set; }
    }
}