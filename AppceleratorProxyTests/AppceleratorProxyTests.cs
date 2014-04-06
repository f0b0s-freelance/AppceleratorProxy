using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using AppceleratorProxy;
using NUnit.Framework;

namespace AppceleratorProxyTests
{
    [TestFixture]
    public class AppceleratorProxyTests
    {
        private const string Key = "djobML01bDhHZenZ4jbS3HV4niGRtSPO";

        [Test]
        public void AuthorizeTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                var result = proxy.Authorize("admin", "admin");
                Debug.WriteLine(result.Result.Result.Status);
            }
        }

        [Test]
        public void CreateFileTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.CreateFile("123.txt", "notes");
                Debug.WriteLine(result.Result.Result.Status);
                Debug.WriteLine(result.Result.Result.Code);
            }
        }

        [Test]
        public void DeleteFile()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.DeleteFile("533ffe0115d8270b632408ac");
                Debug.WriteLine(result.Result.Status);
            }
        }

        [Test]
        public void GetFile()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.GetFile("5341566c891fdf0b73256bdc");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void UpdateFile()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.UpdateFile("updated.txt", "binary", "534156b315d8270b63256fe4");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void ListFiles()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                //var result = proxy.ListFiles("1", "1", "\"user_id\":\"4f0fe764d9ca72833d000002\", \"name\":\"my_file\"", "created_at");
                var result = proxy.ListFiles("1", "10", "\"user_id\":\"4f0fe764d9ca72833d000002\", \"name\":\"my_file\"", "created_at");
                Debug.WriteLine(result.Result);
            }
        }
        
        [Test]
        public void CreatePhotoTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);

                var sizes = new[]
                                {
                                    new PhotoSize
                                        {
                                            Name = "WP8",
                                            Size = "100x100"
                                        },
                                    new PhotoSize
                                        {
                                            Name = "PC",
                                            Size = "200x200"
                                        }
                                };

                var result = proxy.UpdaloadPhoto("1.png", "Photo", sizes);
                Debug.WriteLine(result.Result.Result.Status);
            }
        }

        [Test]
        public void DeletePhotoTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);

                var result = proxy.DeletePhoto("533ff06815d8270b5b23dc16");
                Debug.WriteLine(result.Result.Status);
            }
        }

        [Test]
        public void GetPhotoTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.GetPhoto("533ff96715d8270b5b23ea87");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void UpdatePhotoTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var sizes = new[]
                                {
                                    new PhotoSize
                                        {
                                            Name = "WP8",
                                            Size = "100x100"
                                        },
                                    new PhotoSize
                                        {
                                            Name = "PC",
                                            Size = "200x200"
                                        }
                                };

                var result = proxy.UpdatePhoto("533ff9171316e90b6e23f09f", "2.png", "changed", sizes);
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void ListPhotosTest()
        {
            using (var proxy = new AppceleratorProxy.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.ListPhotos("1", "10", "\"user_id\":\"533efde215d8270b6323064d\"", "created_at");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void SerializationTest()
        {
            var j = new RequestResult
                        {
                            Result = new RequestMetaInfo
                                         {
                                             Code = "asd"
                                         }
                        };
            var memoryStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(RequestResult));
            serializer.WriteObject(memoryStream, j);
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Debug.WriteLine(streamReader.ReadToEnd());
        }
    }
}
