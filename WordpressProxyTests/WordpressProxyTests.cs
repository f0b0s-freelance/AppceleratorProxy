using System;
using System.Collections.Generic;
using NUnit.Framework;
using WordpressProxy;

namespace WordpressProxyTests
{
    [TestFixture]
    public class WordpressProxyTests
    {
        const string Key = "X1LL&Lan8M#L@Fhzr77%ZW&8U2yfTBPq4Qa&ObMzdD)#OlGSjpllolevZn(@JI7%";
        private WordpressProxy.WordpressProxy _proxy;

        [SetUp]
        public void SetUp()
        {
            const string clientId = "34711";
            const string clientSecret = "REryTBb536tsDFZryLsLE8WinmStTNQShP6B2W8yGXnqMgJZxVA5fTAZB8EEJuVU";
            const string redirectUri = "http://skyfer.com";
            const string code = "1ZI4mDc7mJ";
            _proxy = new WordpressProxy.WordpressProxy(clientId, clientSecret, redirectUri, code);
        }
        
        [Test]
        public void GetPostByIdTest()
        {
            var t = _proxy.GetPostById("f0bos.wordpress.com", "5");
            var post = t.Result;
        }

        [Test]
        public void GetPostBySlugTest()
        {
            var t = _proxy.GetPostBySlug("f0bos.wordpress.com", "title-for-my-first-post");
            var post = t.Result;
        }

        [Test]
        public void DeletePostTest()
        {
            var t = _proxy.DeletePost("f0bos.wordpress.com", "35");
            var deleted = t.Result;
        }

        [Test]
        public void CreatePostTest()
        {
            var postInfo = new PostInfo
                               {
                                   Content = "<h2>Four Post</h2>",
                                   Title = "<h2>Four</h2>",
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
            var t = _proxy.CreatePost("f0bos.wordpress.com", postInfo);
            var deleted = t.Result;
        }

        [Test]
        public void UpdatePostTest()
        {
            var postInfo = new PostInfo
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
            var t = _proxy.UpdatePost("f0bos.wordpress.com", postInfo, "30");
            var deleted = t.Result;
        }

        [Test]
        public void IntegrationTest()
        {
            var postInfo = new PostInfo
                               {
                                   Content = "<h2>Integration</h2>",
                                   Title = "<h2>Integration title</h2>",
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
            var createdInfoTask = _proxy.CreatePost("f0bos.wordpress.com", postInfo);
            Console.WriteLine(createdInfoTask.Result.Id);
            var createdId = createdInfoTask.Result.Id;

            postInfo.Title += " updated";
            var updatedInfoTask = _proxy.UpdatePost("f0bos.wordpress.com", postInfo, createdId);
            Console.WriteLine(updatedInfoTask.Result.Id);
        }
    }
}
