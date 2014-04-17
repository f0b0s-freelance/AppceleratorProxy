using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class PhotoResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }

        [DataMember(Name = "response")]
        public PhotoResponse Response { get; set; }
    }
}