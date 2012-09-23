using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCBlog.Models
{
    public class PostTag
    {
        public int PostTagId { get; set; }
        [MaxLength(4000)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Title { get; set; }
        [MaxLength(4000)]
        public string HoverOverHint { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMenu { get; set; }
        public int? MenuOrder { get; set; }

        public virtual Post Posts { get; set; }
    }
}