using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class FileMetaInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "processed")]
        public bool Processed { get; set; }

        [DataMember(Name = "created_at")]
        public string Created { get; set; }

        [DataMember(Name = "updated_at")]
        public string Updated { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}