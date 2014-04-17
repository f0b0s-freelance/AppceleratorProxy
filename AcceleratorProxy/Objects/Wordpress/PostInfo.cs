using System.Collections.Generic;

namespace AppceleratorProxy.Objects.Wordpress
{
    public enum PostFormat
    {
        Standart,
        Aside,
        Chat,
        Gallery,
        Link,
        Image,
        Quote,
        Status,
        Video,
        Audio,
        Undefined
    }

    public class PostInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Slug { get; set; }
        public PostStatus PostStatus { get; set; }
        public string Parent { get; set; } //TODO - doesn't work
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public PostFormat Format { get; set; }
        public bool? CommentsOpen { get; set; }
        public bool? PingsOpen { get; set; }
    }
}