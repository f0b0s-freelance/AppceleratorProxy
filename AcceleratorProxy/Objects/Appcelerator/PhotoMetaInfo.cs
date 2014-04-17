using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class PhotoMetaInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "filename")]
        public string Name { get; set; }

        [DataMember(Name = "size")]
        public string Size { get; set; }

        [DataMember(Name = "collection_name")]
        public string CollectionName { get; set; }

        [DataMember(Name = "processed")]
        public bool Processed { get; set; }

        [DataMember(Name = "created_at")]
        public string Created { get; set; }

        [DataMember(Name = "updated_at")]
        public string Updated { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "urls")]
        public Dictionary<string, object> PhotoUrls { get; set; }
    }
}