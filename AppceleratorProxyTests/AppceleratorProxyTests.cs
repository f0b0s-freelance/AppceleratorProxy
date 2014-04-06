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
            var proxy = new AppceleratorProxy.AppceleratorProxy(Key);
            var result = proxy.Authorize("admin", "admin");
            Debug.WriteLine(result.Result.Result.Status);
        }

        [Test]
        public void CreateFileTest()
        {
            var proxy = new AppceleratorProxy.AppceleratorProxy(Key);
            Debug.WriteLine(proxy.Authorize("admin", "admin").Result.Result.Status);
            var result = proxy.UploadFile("123.txt", "teextable");
            Debug.WriteLine(result.Result.Result.Status);
        }

        [Test]
        public void CreatePhotoTest()
        {
            var proxy = new AppceleratorProxy.AppceleratorProxy(Key);
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
