using System.Collections.Generic;
using NUnit.Framework;
using WordpressProxy;

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
            var t = proxy.GetPostById("f0bos.wordpress.com", "5");
            var post = t.Result;
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
            var t = proxy.DeletePost("f0bos.wordpress.com", "6", "53ea78455221");
            var deleted = t.Result;
        }

        [Test]
        public void CreatePostTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();
            var postInfo = new PostInfo()
                               {
                                   Content = "<h2>Second Post</h2>",
                                   Title = "<h2>Ha-ha</h2>",
                                   Excerpt = "excep",
                                   PostStatus = PostStatus.Publish,
                                   Parent = "5",
                                   Tags = new List<string>
                                              {
                                              },
                                   Categories = new List<string>
                                                    {
                                                    },
                                   Slug = "it's a slug",
                                   Format = PostFormat.Chat,
                                   CommentsOpen = false,
                                   PingsOpen = false
                               };
            var t = proxy.CreatePost("f0bos.wordpress.com", postInfo);
            var deleted = t.Result;
        }

        [Test]
        public void UpdatePostTest()
        {
            var proxy = new WordpressProxy.WordpressProxy();
            var postInfo = new PostInfo()
            {
                Content = "<h2>Second Post....</h2>",
                Title = "<h2>Ha-ha.....</h2>",
                Excerpt = "excep",
                PostStatus = PostStatus.Publish,
                Parent = "5",
                Tags = new List<string>
                {
                    "123"
                },
                Categories = new List<string>
                {
                    "CATS"
                },
                Slug = "it's a slug",
                Format = PostFormat.Quote,
                CommentsOpen = true,
                PingsOpen = true
            };
            var t = proxy.UpdatePost("f0bos.wordpress.com", postInfo, "30");
            var deleted = t.Result;
        }
    }
}
