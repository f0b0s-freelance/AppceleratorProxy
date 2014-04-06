using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppceleratorProxy
{
    [DataContract]
    public class RequestResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }
    }

    [DataContract]
    public class FileResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }

        [DataMember(Name = "response")]
        public FileResponse Response { get; set; }
    }

    [DataContract]
    public class PhotoResult
    {
        [DataMember(Name = "meta")]
        public RequestMetaInfo Result { get; set; }

        [DataMember(Name = "response")]
        public PhotoResponse Response { get; set; }
    }

    [DataContract]
    public class FileResponse
    {
        [DataMember(Name = "files")]
        public List<FileMetaInfo> Files { get; set; }
    }


    [DataContract]
    public class PhotoResponse
    {
        [DataMember(Name = "photos")]
        public List<PhotoMetaInfo> Photos { get; set; }
    }

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

    [DataContract]
    public class PhotoMetaInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "filename")]
        public string Name { get; set; }

        [DataMember(Name = "size")]
        public string Size { get; set; }

        [DataMember(Name = "collection_name")]
        public string CollectionName { get; set; }

        [DataMember(Name = "processed")]
        public bool Processed { get; set; }

        [DataMember(Name = "created_at")]
        public string Created { get; set; }

        [DataMember(Name = "updated_at")]
        public string Updated { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
    
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "created_at")]
        public string Created { get; set; }

        [DataMember(Name = "updated_at")]
        public string Updated { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "admin")]
        public bool IsAdmin { get; set; }
    }
}