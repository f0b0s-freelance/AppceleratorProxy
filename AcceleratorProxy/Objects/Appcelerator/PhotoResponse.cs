using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class PhotoResponse
    {
        [DataMember(Name = "photos")]
        public List<PhotoMetaInfo> Photos { get; set; }
    }
}