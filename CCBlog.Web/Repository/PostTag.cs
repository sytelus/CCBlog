using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCBlog.Repository
{
    public class PostTag
    {
        public int PostTagId { get; set; }
        [MaxLength(4000)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Title { get; set; }
        [MaxLength(4000)]
        public string HelpHint { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMenu { get; set; }
        public int? MenuOrder { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}