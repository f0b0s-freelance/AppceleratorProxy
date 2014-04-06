using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace AppceleratorProxy
{
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

        public async Task<FileResult> GetFile(string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/show.json?key={0}&file_id={1}", _appKey, fileId);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(FileResult));
            return (FileResult)serializer.ReadObject(responseTask);
        }

        public async Task<FileResult> ListFiles(string pageNumber, string perPage, string predicat, string order)
        {
            var url =
                string.Format(
                    "https://api.cloud.appcelerator.com/v1/files/query.json?key={0}&page={1}&per_page={2}&where={{{3}}}&order={4}",
                    _appKey, pageNumber, perPage, Uri.EscapeDataString(predicat), order);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(FileResult));
            return (FileResult)serializer.ReadObject(responseTask);
        }

        public async Task<RequestMetaInfo> Delete(string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/delete.json?key={0}&file_id={1}", _appKey, fileId);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(RequestMetaInfo));
            return (RequestMetaInfo)serializer.ReadObject(responseTask);
        }

        public async Task<FileResult> Update(string path, string remoteName, string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent(remoteName);
                multipart.Add(nameContent);

                var fileIdContent = GetNameDataContent(fileId, "fileId");
                multipart.Add(fileIdContent);

                var fileContent = GetFileDataContent(path, file, "text/plain");
                multipart.Add(fileContent);

                var response = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                var ser = new DataContractJsonSerializer(typeof(FileResult));
                return (FileResult)ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
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

                var fileContent = GetFileDataContent(path, file, "image/jpeg");
                multipart.Add(fileContent);

                var response = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                var ser = new DataContractJsonSerializer(typeof(RequestResult));
                return (RequestResult)ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
            }
        }

        public async Task<RequestMetaInfo> DeletePhoto(string photoId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/delete.json?key={0}&photo_id={1}", _appKey, photoId);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(RequestMetaInfo));
            return (RequestMetaInfo)serializer.ReadObject(responseTask);
        }

        public async Task<PhotoResult> GetPhoto(string photoId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/show.json?key={0}&photo_id={1}", _appKey, photoId);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(PhotoResult));
            return (PhotoResult)serializer.ReadObject(responseTask);
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

        public async Task<PhotoResult> UpdatePhoto(string photoId, string path, string remoteName, IEnumerable<PhotoSize> sizes)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/update.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var photoIdContent = GetNameDataContent(photoId, "\"photo_id\"");
                multipart.Add(photoIdContent);

                var nameContent = GetNameDataContent(remoteName);
                multipart.Add(nameContent);

                foreach (var size in sizes)
                {
                    var sizeContent = GetSizeDataContent(size);
                    multipart.Add(sizeContent);
                }

                var fileContent = GetFileDataContent(path, file, "image/jpeg");
                multipart.Add(fileContent);

                var response = await _httpClient.PostAsync(url, multipart).ConfigureAwait(false);
                var ser = new DataContractJsonSerializer(typeof(PhotoResult));
                return (PhotoResult)ser.ReadObject(response.Content.ReadAsStreamAsync().Result);
            }
        }

        public async Task<PhotoResult> ListPhotos(string pageNumber, string perPage, string predicat, string order)
        {
            var url =
                string.Format(
                    "https://api.cloud.appcelerator.com/v1/photos/query.json?key={0}&page={1}&per_page={2}&where={{{3}}}&order={4}",
                    _appKey, pageNumber, perPage, Uri.EscapeDataString(predicat), order);
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(PhotoResult));
            return (PhotoResult)serializer.ReadObject(responseTask);
        }
    }
}
