using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class PhotoUrl
    {
        public string Name { get; set; }
        
        public string Url { get; set; }
    }
}