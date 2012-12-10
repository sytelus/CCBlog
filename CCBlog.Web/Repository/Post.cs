using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBlog.Repository
{
    public class Post
    {
        //Basic fields
        public int PostId { get; set; }
        [MaxLength(128)][Required]
        public string UrlFriendlyName { get; set; }
        [MaxLength(4000)][Required]
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        public DateTimeOffset ModifyDate { get; set; }
        [Required]
        public DateTimeOffset AuthorCreateDate { get; set; }
        [Required]
        public DateTimeOffset AuthorModifyDate { get; set; }

        //Author fields - these are marked as FK using Fluent APIs for Entity Framework
        public int CreatedByUserId { get; set; }
        public int ModifiedByUserId { get; set; }

        public int RevisionCount { get; set; }

        //Visibility control fields
        public bool IsPrivate { get; set; } //When set, this post is not visible to anyone accept user who created it

        //Multi-part article support fields
        [MaxLength(4000)]
        public string SeriesTitle { get; private set; }
        public int? SeriesPartNumber { get; private set; }

        //Layout config fields
        public int? DisplayOrder { get; private set; }  //Posts are sorted by DisplayOrder DESC, CreateDate ASC
        public string PageLayoutName { get; private set; }  //This to enable adding whole new page (for example JavaScript game). if null then use standard layout.


        //Navigation properties exists only because they are required by entity framework, do not use it in code
        [ForeignKey("CreatedByUserId")]
        public virtual Repository.User CreatedByUser { get; set; }
        [ForeignKey("ModifiedByUserId")]
        public virtual Repository.User ModifiedByUser { get; set; }
        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}