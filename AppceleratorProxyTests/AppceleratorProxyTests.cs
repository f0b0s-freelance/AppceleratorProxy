using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using AppceleratorProxy.Objects.Appcelerator;
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
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                var result = proxy.Authorize("admin", "admin");
                Debug.WriteLine(result.Result.Result.Status);
            }
        }

        [Test]
        public void CreateFileTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.CreateFile("123.txt", "notes");
                Debug.WriteLine(result.Result.Result.Status);
                Debug.WriteLine(result.Result.Result.Code);
            }
        }

        [Test]
        public void CreateThroughStreamTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            using (var file = File.Open("q.txt", FileMode.Open))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.CreateFile(file, "ha");
                Debug.WriteLine(result.Result.Result.Status);
                Debug.WriteLine(result.Result.Result.Code);
            }
        }


        [Test]
        public void DeleteFile()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.DeleteFile("53415768891fdf0b73256cf6");
                Debug.WriteLine(result.Result.Result.Status);
                Debug.WriteLine(result.IsCompleted);
            }
        }

        [Test]
        public void GetFile()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.GetFile("5341566c891fdf0b73256bdc");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void UpdateFile()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.UpdateFile("123.txt", "ttt", "534156b315d8270b63256fe4");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void ListFiles()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
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
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
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

                var result = proxy.CreatePhoto("1.jpg", "Karina", sizes);
                Debug.WriteLine(result.Result.Result.Status);
            }
        }

        [Test]
        public void DeletePhotoTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);

                var result = proxy.DeletePhoto("5341658f0f4fb50bae003c15");
                Debug.WriteLine(result.Result.Result.Status);
            }
        }

        [Test]
        public void GetPhotoTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.GetPhoto("53416654ed8cdc0b39003bd3");
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void UpdatePhotoTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var sizes = new[]
                                {
                                    new PhotoSize
                                        {
                                            Name = "WP8",
                                            Size = "100x100"
                                        }
                                };

                //var result = proxy.UpdatePhoto("533ff9171316e90b6e23f09f", "2.png", "changed", sizes);
                var result = proxy.UpdatePhoto("534163ceed8cdc0b2f003aab", "2.png", "ccc", sizes);
                Debug.WriteLine(result.Result);
            }
        }

        [Test]
        public void ListPhotosTest()
        {
            using (var proxy = new AppceleratorProxy.Proxies.AppceleratorProxy(Key))
            {
                Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
                var result = proxy.ListPhotos("1", "10", "\"user_id\":\"533efde215d8270b63230641\"", "created_at");
                //var result = proxy.ListPhotos("1", "10", null, "created_at");
                //var result = proxy.ListPhotos("2", "2", null, null);
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
