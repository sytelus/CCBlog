using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CCBlog.Models
{
    public class Post
    {
        //Basic fields
        public int PostId { get; set; }
        [MaxLength(256)]
        public string UrlfriendlyName { get; set; }
        [MaxLength(4000)]
        public string Title { get; set; }
        public string Content { get; set; }
        [MaxLength(1024)]
        public string PermanentLink { get; set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset ModifyDate { get; private set; }

        //Author fields
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedBy { get; set; }
        [ForeignKey("ModifiedByUserId")]
        public virtual User ModifiedBy { get; set; }
        public int RevisionCount { get; private set; }

        //Visibility control fields
        public bool IsPrivate { get; private set; } //When set, this post is not visible to anyone accept user who created it
        public DateTimeOffset? VisibleAfter { get; private set; }    //Post becomes visible after this time provided that it's not private (ex, happy birthday posts)

        //Multi-part article support fields
        [MaxLength(256)]
        public string MultiPartSeriesUrlFriendlyName { get; private set; }
        public int? MultiPartSeriesPartNumber { get; private set; }

        //Layout config fields
        public int? DisplayOrder { get; private set; }  //Posts are sorted by DisplayOrder DESC, CreateDate ASC
        public string PageLayoutName { get; private set; }  //This to enable adding whole new page (for example JavaScript game). if null then use standard layout.

        public string PostTagIds { get; set; }
    }
}