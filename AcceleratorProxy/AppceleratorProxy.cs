using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppceleratorProxy
{
    public class PhotoSize
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }

    public class AppceleratorProxy
    {
        private readonly string _appKey;
        private readonly HttpClient _httpClient;

        public AppceleratorProxy(string appKey)
        {
            _appKey = appKey;
            _httpClient = new HttpClient();
        }

        public async Task<RequestResult> Authorize(string login, string password)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/users/login.json?key={0}", _appKey);
            var stringContent = new StringContent(string.Format("login={0}&password={1}", login, password));
            stringContent.Headers.ContentType = null;
            var responseTask = await _httpClient.PostAsync(url, stringContent).ConfigureAwait(false);
            var responseStream = await responseTask.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(RequestResult));
            return (RequestResult)serializer.ReadObject(responseStream);
        }

        public async Task<RequestResult> UploadFile(string path, string remoteName)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent(remoteName);
                multipart.Add(nameContent);

                var fileContent = GetFileDataContent(path, file, "text/plain");
                multipart.Add(fileContent);

                var response = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                var ser = new DataContractJsonSerializer(typeof(RequestResult));
                return (RequestResult)ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
            }
        }
        
        public async Task<RequestResult> UpdaloadPhoto(string path, string remoteName, IEnumerable<PhotoSize> sizes)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent(remoteName);
                multipart.Add(nameContent);

                foreach (var size in sizes)
                {
                    var sizeContent = GetSizeDataContent(size);
                    multipart.Add(sizeContent);
                }

//                var sizeContent = new StringContent("100x100");
//                sizeContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
//                                                             {
//                                                                 Name = "\"photo_sizes[android]\""
//                                                             };
//                sizeContent.Headers.ContentType = null;
//                multipart.Add(sizeContent);

                var fileContent = GetFileDataContent(path, file, "image/jpeg");
                multipart.Add(fileContent);

                var response = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                var ser = new DataContractJsonSerializer(typeof(RequestResult));
                return (RequestResult)ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
            }
        }

        private StringContent GetSizeDataContent(PhotoSize photoSize)
        {
            return GetNameDataContent(photoSize.Size, string.Format("photo_sizes[{0}]", photoSize.Name));
        }

        private static StreamContent GetFileDataContent(string path, FileStream file, string mediaType)
        {
            var fileContent = new StreamContent(file);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                         {
                                                             Name = "file",
                                                             FileName = path,
                                                         };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            return fileContent;
        }

        private static StringContent GetNameDataContent(string remoteName, string name = "\"name\"")
        {
            var nameContent = new StringContent(remoteName);
            nameContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                         {
                                                             Name = name
                                                         };
            nameContent.Headers.ContentType = null;
            return nameContent;
        }
    }
}
