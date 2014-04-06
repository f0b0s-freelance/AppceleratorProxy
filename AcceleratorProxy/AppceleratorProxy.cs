using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace AppceleratorProxy
{
    public class AppceleratorProxy : IDisposable
    {
        private bool _disposed;
        private readonly string _appKey;
        private readonly HttpClient _httpClient;

        public AppceleratorProxy(string appKey)
        {
            _appKey = appKey;
            _httpClient = new HttpClient();
        }

        public Task<RequestResult> Authorize(string login, string password)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/users/login.json?key={0}", _appKey);
            var stringContent = new StringContent(string.Format("login={0}&password={1}", login, password));
            stringContent.Headers.ContentType = null;
            return ReadPost<RequestResult>(url, stringContent);
        }

        public Task<RequestResult> CreateFile(string path, string remoteName)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent(remoteName);
                multipart.Add(nameContent);

                var fileContent = GetFileDataContent(path, file, "text/plain");
                multipart.Add(fileContent);

                return ReadPost<RequestResult>(url, multipart);
            }
        }

        public Task<FileResult> GetFile(string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/show.json?key={0}&file_id={1}", _appKey, fileId);
            return ReadObject<FileResult>(url);
        }

        public Task<FileResult> ListFiles(string pageNumber, string perPage, string predicat, string order)
        {
            var url =
                string.Format(
                    "https://api.cloud.appcelerator.com/v1/files/query.json?key={0}&page={1}&per_page={2}&where={{{3}}}&order={4}",
                    _appKey, pageNumber, perPage, Uri.EscapeDataString(predicat), order);

            return ReadObject<FileResult>(url);
        }

        public Task<RequestMetaInfo> DeleteFile(string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/delete.json?key={0}&file_id={1}", _appKey, fileId);
            return ReadObject<RequestMetaInfo>(url);
        }

        public Task<FileResult> UpdateFile(string path, string remoteName, string fileId)
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

                return ReadPost<FileResult>(url, multipart);
            }
        }

        public Task<RequestResult> UpdaloadPhoto(string path, string remoteName, IEnumerable<PhotoSize> sizes)
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

                return ReadPost<RequestResult>(url, multipart);
            }
        }

        public Task<RequestMetaInfo> DeletePhoto(string photoId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/delete.json?key={0}&photo_id={1}", _appKey, photoId);
            return ReadObject<RequestMetaInfo>(url);
        }

        public Task<PhotoResult> GetPhoto(string photoId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/show.json?key={0}&photo_id={1}", _appKey, photoId);
            return ReadObject<PhotoResult>(url);
        }

        public Task<PhotoResult> UpdatePhoto(string photoId, string path, string remoteName, IEnumerable<PhotoSize> sizes)
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

                return ReadPost<PhotoResult>(url, multipart);
            }
        }

        public Task<PhotoResult> ListPhotos(string pageNumber, string perPage, string predicat, string order)
        {
            var url =
                string.Format(
                    "https://api.cloud.appcelerator.com/v1/photos/query.json?key={0}&page={1}&per_page={2}&where={{{3}}}&order={4}",
                    _appKey, pageNumber, perPage, Uri.EscapeDataString(predicat), order);

            return ReadObject<PhotoResult>(url);
        }

        private async Task<T> ReadObject<T>(string url)
        {
            var responseTask = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(responseTask);
        }

        private async Task<T> ReadPost<T>(string url, HttpContent content)
        {
            var responseMessage = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(responseStream);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                _disposed = true;
            }
        }

        ~AppceleratorProxy()
        {
            Dispose(false);
        }
    }
}
