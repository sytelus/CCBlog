using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBlog.Models
{
    public class BlogPost
    {
        //Basic fields
        public int BlogPostId { get; set; }
        [MaxLength(256)]
        public string UrlfriendlyName { get; set; }
        [MaxLength(4000)]
        public string Title { get; set; }
        public string Content { get; set; }
        [MaxLength(1024)]
        public string PermanentLink { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset ModifyDate { get; set; }

        //Author fields
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedBy { get; set; }
        [ForeignKey("ModifiedByUserId")]
        public virtual User ModifiedBy { get; set; }
        public int RevisionCount { get; set; }

        //Visibility control fields
        public bool IsPrivate { get; set; } //When set, this post is not visible to anyone accept user who created it
        public DateTimeOffset? VisibleAfter { get; set; }    //Post becomes visible after this time provided that it's not private (ex, happy birthday posts)

        //Multi-part article support fields
        [MaxLength(256)]
        public string MultiPartSeriesUrlFriendlyName { get; set; }
        public int? MultiPartSeriesPartNumber { get; set; }

        //Layout config fields
        public int? DisplayOrder { get; set; }  //Posts are sorted by DisplayOrder DESC, CreateDate ASC
        public string PageLayoutName { get; set; }  //This to enable adding whole new page (for example JavaScript game). if null then use standard layout.

        public virtual BlogPostTag BlogPostTags { get; set; }
    }
}