using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppceleratorProxy.Objects.Appcelerator
{
    [DataContract]
    public class FileResponse
    {
        [DataMember(Name = "files")]
        public List<FileMetaInfo> Files { get; set; }
    }
}