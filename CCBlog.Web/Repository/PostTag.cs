using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Repository
{
    public class PostTag
    {
        [Key, Column(Order = 0)]
        public int PostId { get; set; }
        [Key, Column(Order = 1)]
        public int TagId { get; set; }

        //Navigation properties exists only because they are required by entity framework, do not use it in code
        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }
}