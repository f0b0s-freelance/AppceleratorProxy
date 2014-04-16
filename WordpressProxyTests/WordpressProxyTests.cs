using NUnit.Framework;

namespace WordpressProxyTests
{
    [TestFixture]
    public class WordpressProxyTests
    {
        [Test]
        public void CreateTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();

        }

        [Test]
        public void GetPostByIdTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();
            var t = proxy.GetPostById("f0bos.wordpress.com", "3");
            t.Wait();
        }

        [Test]
        public void GetPostBySlugTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();
            var t = proxy.GetPostBySlug("f0bos.wordpress.com", "title-for-my-first-post");
            var post = t.Result;
        }

        [Test]
        public void DeletePostTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();
            var t = proxy.DeletePost("f0bos.wordpress.com", "3", "53ea78455221");
            var deleted = t.Result;
        }
    }
}
