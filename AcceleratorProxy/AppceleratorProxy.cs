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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appKey">Application key</param>
        public AppceleratorProxy(string appKey)
        {
            _appKey = appKey;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Authorization, should be called before any other method 
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public Task<RequestResult> Authorize(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException("Login can't be null or empty", "login");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password can't be null or empty", "password");
            }

            var url = string.Format("https://api.cloud.appcelerator.com/v1/users/login.json?key={0}", _appKey);
            var stringContent = new StringContent(string.Format("login={0}&password={1}", login, password));
            stringContent.Headers.ContentType = null;
            return ReadPost<RequestResult>(url, stringContent);
        }

        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="path">Path to file that should be uploaded</param>
        /// <param name="remoteName">File name on the site</param>
        /// <returns></returns>
        public Task<FileResult> CreateFile(string path, string remoteName)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException(string.Format("File '{0}' doesn't exists", path));
            }

            if (string.IsNullOrEmpty(remoteName))
            {
                throw new ArgumentException("Remote name can't be null or empty", "remoteName");
            }

            return CreateFileInner(path, remoteName);
        }

        /// <summary>
        /// Returns information associated with the file.
        /// </summary>
        /// <param name="fileId">ID of the file to retrieve information for.</param>
        /// <returns></returns>
        public Task<FileResult> GetFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentException("File id can't be null or empty", "fileId");
            }

            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/show.json?key={0}&file_id={1}", _appKey,
                                    fileId);
            return ReadObject<FileResult>(url);
        }

        /// <summary>
        /// Perform custom query of files with sorting and paginating
        /// </summary>
        /// <param name="pageNumber">Request page number</param>
        /// <param name="perPage">Number of results per page</param>
        /// <param name="predicat">Constraint values for fields. If not specified, query returns all objects.</param>
        /// <param name="order">Sort results by one or more fields. Pass null if you don't want any ordering</param>
        /// <returns></returns>
        public Task<FileResult> ListFiles(string pageNumber, string perPage, string predicat, string order)
        {
            if (string.IsNullOrEmpty(pageNumber))
            {
                throw new ArgumentException("Page number can't be null or empty", "pageNumber");
            }

            if (string.IsNullOrEmpty(perPage))
            {
                throw new ArgumentException("PerPage can't be null or empty", "perPage");
            }

            var escapedData = predicat == null ? null : Uri.EscapeDataString(predicat);

            var url =
                string.Format(
                    "https://api.cloud.appcelerator.com/v1/files/query.json?key={0}&page={1}&per_page={2}&where={{{3}}}&order={4}",
                    _appKey, pageNumber, perPage, escapedData, order);

            return ReadObject<FileResult>(url);
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="fileId">ID of the file to delete.</param>
        /// <returns></returns>
        public Task<RequestMetaInfo> DeleteFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentException("File id can't be null or empty");
            }

            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/delete.json?key={0}&file_id={1}",
                                    _appKey, fileId);
            return ReadObject<RequestMetaInfo>(url);
        }

        /// <summary>
        /// Updates the file object.
        /// </summary>
        /// <param name="path">Path to local file</param>
        /// <param name="remoteName">File name on the site</param>
        /// <param name="fileId">ID of the file to update.</param>
        /// <returns></returns>
        public Task<FileResult> UpdateFile(string path, string remoteName, string fileId)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException(string.Format("File '{0}' doesn't exists", path));
            }

            if (string.IsNullOrEmpty(remoteName))
            {
                throw new ArgumentException("Remote name can't be null or empty", "remoteName");
            }

            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentException("File id can't be null or empty");
            }

            return UpdateFileInner(path, remoteName, fileId);
        }

        public Task<RequestResult> UpdaloadPhoto(string path, string remoteName, IEnumerable<PhotoSize> sizes)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/photos/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent("\"name\"", remoteName);
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
                var photoIdContent = GetNameDataContent("\"photo_id\"", photoId);
                multipart.Add(photoIdContent);

                var nameContent = GetNameDataContent("\"name\"", remoteName);
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

        private async Task<FileResult> CreateFileInner(string path, string remoteName)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/create.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent("\"name\"", remoteName);
                multipart.Add(nameContent);

                var fileContent = GetFileDataContent(path, file, "text/plain");
                multipart.Add(fileContent);

                return await ReadPost<FileResult>(url, multipart).ConfigureAwait(false);
            }
        }

        private async Task<FileResult> UpdateFileInner(string path, string remoteName, string fileId)
        {
            var url = string.Format("https://api.cloud.appcelerator.com/v1/files/update.json?key={0}", _appKey);
            var multipart = new MultipartFormDataContent();

            using (var file = File.Open(path, FileMode.Open))
            {
                var nameContent = GetNameDataContent("\"name\"", remoteName);
                multipart.Add(nameContent);

                var fileIdContent = GetNameDataContent("fileId", fileId);
                multipart.Add(fileIdContent);

                var fileContent = GetFileDataContent(path, file, "text/plain");
                multipart.Add(fileContent);

                return await ReadPost<FileResult>(url, multipart).ConfigureAwait(false);
            }
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
            return GetNameDataContent(string.Format("photo_sizes[{0}]", photoSize.Name), photoSize.Size);
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

        private static StringContent GetNameDataContent(string name, string value)
        {
            var nameContent = new StringContent(value);
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
