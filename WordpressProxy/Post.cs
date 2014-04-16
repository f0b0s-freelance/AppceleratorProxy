using System.Runtime.Serialization;

namespace WordpressProxy
{
    [DataContract]
    public class Post
    {
        [DataMember(Name = "ID")]
        public string Id { get; set; }

        [DataMember(Name = "site_ID")]
        public string SiteId { get; set; }

        [DataMember(Name = "author")]
        public Author Author { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }
        
        [DataMember(Name = "modified")]
        public string Modified { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "URL")]
        public string Url { get; set; }

        [DataMember(Name = "short_URL")]
        public string ShortUrl { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "excerpt")]
        public string Excerpt { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }

        [DataMember(Name = "guid")]
        public string Guid { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "parent")]
        public bool Parent { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "comments_open")]
        public bool CommentsOpen { get; set; }

        [DataMember(Name = "pings_open")]
        public bool PingsOpen { get; set; }

        [DataMember(Name = "comment_count")]
        public string CommentCount { get; set; }

        [DataMember(Name = "like_count")]
        public string LikeCount { get; set; }

        [DataMember(Name = "i_like")]
        public string ILike { get; set; }

        [DataMember(Name = "is_reblogged")]
        public string IsReblogged { get; set; }

        [DataMember(Name = "is_following")]
        public string IsFollowing { get; set; }

        [DataMember(Name = "global_ID")]
        public string GlobalId { get; set; }

        [DataMember(Name = "featured_image")]
        public string FeaturedImage { get; set; }

        [DataMember(Name = "format")]
        public string Format { get; set; }

        [DataMember(Name = "geo")]
        public bool Geo { get; set; }
    }
}
