using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCBlog.Repository
{
    public class Tag
    {
        public int TagId { get; set; }
        [MaxLength(4000)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Title { get; set; }
        [MaxLength(4000)]
        public string HelpHint { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMenu { get; set; }
        public int? MenuOrder { get; set; }

        //Navigation properties exists only because they are required by entity framework, do not use it in code
        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}