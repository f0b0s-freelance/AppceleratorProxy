﻿using System;
using System.Collections.Generic;
using AppceleratorProxy.Objects.Wordpress;
using AppceleratorProxy.Proxies;
using NUnit.Framework;

namespace AppceleratorProxyTests
{
    [TestFixture]
    public class WordpressProxyTests
    {
        const string Key = "X1LL&Lan8M#L@Fhzr77%ZW&8U2yfTBPq4Qa&ObMzdD)#OlGSjpllolevZn(@JI7%";
        private WordpressProxy _proxy;

        [SetUp]
        public void SetUp()
        {
            const string clientId = "34711";
            const string clientSecret = "REryTBb536tsDFZryLsLE8WinmStTNQShP6B2W8yGXnqMgJZxVA5fTAZB8EEJuVU";
            const string redirectUri = "http://skyfer.com";
            const string code = "gxE3rbYXnU";
            const string domain = "f0bos.wordpress.com";
            _proxy = new WordpressProxy(clientId, clientSecret, redirectUri, code, domain);
        }
        
        [Test]
        public void GetPostByIdTest()
        {
            var t = _proxy.GetPostById("48");
            var post = t.Result;
            Console.WriteLine(post.Title);
        }

        [Test]
        public void GetPostBySlugTest()
        {
            var t = _proxy.GetPostBySlug("title-for-my-first-post");
            var post = t.Result;
        }

        [Test]
        public void DeletePostTest()
        {
            var t = _proxy.DeletePost("35");
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
            var t = _proxy.CreatePost(postInfo);
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
            var t = _proxy.EditPost(postInfo, "48");
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
            var createdInfoTask = _proxy.CreatePost(postInfo);
            Console.WriteLine(createdInfoTask.Result.Id);
            var createdId = createdInfoTask.Result.Id;

            postInfo.Title += " updated";
            var updatedInfoTask = _proxy.EditPost(postInfo, createdId);
            Console.WriteLine(updatedInfoTask.Result.Id);

            var post = _proxy.GetPostById(createdId);
            Console.WriteLine("Get by Id: " + post.Result.Title);

            post = _proxy.GetPostBySlug(createdInfoTask.Result.Slug);
            Console.WriteLine("Get by Id: " + post.Result.Title + post.Result.Slug);

            //var res = _proxy.DeletePost(createdId);
            //res.Wait();
        }
    }
}
