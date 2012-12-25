using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCBlog.Infrastructure;
using CCBlog.Model.Poco;
using CCBlog.Repository;
using CommonUtils;

namespace CCBlog.Controllers
{
    public partial class PostController : AppControllerBase
    {
        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize(Roles = "Author")]
        [HttpGet]
        public ActionResult Add()
        {
            var postEditModel = new PostEditModel()
                {
                    AllTags = AppCache.Tags.ToArray(), 
                    Post = new Post(), 
                    OriginalPostSerialized = null
                };

            return View("Edit", postEditModel);
        }

        [Authorize(Roles = "Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PostEditModel postEditModel)
        {
            if (!ModelState.IsValid)
                return View("Edit", postEditModel);

            this.Repository.AddPost(postEditModel.Post);

            return RedirectToAction("Index", new { Id = postEditModel.Post.PostId });
        }

        [Authorize(Roles = "Author")]
        [HttpGet]
        public ActionResult Edit(int postId)
        {
            var post = this.Repository.GetPost(postId, true);

            if (post == null)
                return HttpNotFound("Post ID '{0}' was not found".FormatEx(postId));

            var postPoco = (Post) post;

            var postEditModel = new PostEditModel()
                {
                    AllTags = AppCache.Tags.ToArray(),
                    Post = postPoco,
                    OriginalPostSerialized = WebUtils.Utils.ToJson(postPoco)
                };

            return View(postEditModel);
        }

        [Authorize(Roles = "Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostEditModel postEditModel)
        {
            if (!ModelState.IsValid)
                return View("Edit", postEditModel);

            var originalPostPoco = WebUtils.Utils.FromJson<Post>(postEditModel.OriginalPostSerialized);

            this.Repository.UpdatePost(originalPostPoco, postEditModel.Post);

            return RedirectToAction("Index", new { Id = postEditModel.Post.PostId });
        }
    }
}
