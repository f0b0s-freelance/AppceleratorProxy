using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class FileResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }

        [DataMember(Name = "response")]
        public FileResponse Response { get; set; }
    }
}