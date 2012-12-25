using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CCBlog.Model.Contracts;
using CCBlog.Model.Poco;
using CCBlog.Repository;

namespace CCBlog.Controllers
{
	public partial class PostController
	{
        /// <summary>
        /// View model for Post
        /// </summary>
        public class PostEditModel
        {
            public ITag[] AllTags { get; set; }
            public Post Post { get; set; }
            public string OriginalPostSerialized { get; set; }
        }
	}
}