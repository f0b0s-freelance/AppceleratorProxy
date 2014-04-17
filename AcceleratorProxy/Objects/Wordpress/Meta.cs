using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Wordpress
{
    [DataContract]
    public class Meta
    {
        [DataMember(Name = "links")]
        public Dictionary<string, string> Links { get; set; }
    }
}