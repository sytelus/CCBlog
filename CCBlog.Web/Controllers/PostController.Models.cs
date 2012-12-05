using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CCBlog.Repository;

namespace CCBlog.Controllers
{
	public partial class PostController
	{
        /// <summary>
        /// View model for Post
        /// </summary>
        public class PostModel
        {
            public PostModel()
            {
                this.AuthorCreateDate = DateTimeOffset.UtcNow;
                this.AuthorModifyDate = this.AuthorCreateDate;
            }

            public PostModel(Post post)
            {
                this.PostId = post.PostId;
                this.Title = post.Title;
                this.Content = post.Content;
                this.AuthorCreateDate = post.AuthorCreateDate;
                this.AuthorModifyDate = post.AuthorModifyDate;
                this.PostTagIdsDelimited = post.PostTagIdsDelimited;
            }

            public int PostId { get; set; }
            [MaxLength(4000)]
            [Required]
            public string Title { get; set; }
            public string Content { get; set; }
            [Required]
            public DateTimeOffset AuthorCreateDate { get; set; }
            [Required]
            public DateTimeOffset AuthorModifyDate { get; set; }
            public string PostTagIdsDelimited { get; set; }
        }
	}
}