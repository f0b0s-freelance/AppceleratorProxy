using System.Runtime.Serialization;

namespace WordpressProxy
{
    [DataContract]
    public class Item
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "ID")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "post_count")]
        public string PostCount { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }

        [DataMember(Name = "meta")]
        public Meta Meta { get; set; }
    }
}