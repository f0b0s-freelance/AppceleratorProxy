using System.Runtime.Serialization;

namespace AppceleratorProxy
{
    [DataContract]
    public class RequestMetaInfo
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "method_name")]
        public string MethodName { get; set; }

        [DataMember(Name = "sesion_id")]
        public string SessionId { get; set; }
    }
}