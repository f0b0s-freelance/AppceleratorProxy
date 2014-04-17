using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class RequestResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }
    }
}