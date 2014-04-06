using System.Runtime.Serialization;

namespace AppceleratorProxy
{
    [DataContract]
    public class RequestResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }
    }
}