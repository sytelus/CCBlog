using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCBlog.Infrastructure;
using CCBlog.Repository;
using CommonUtils;

namespace CCBlog.Controllers
{
    public partial class PostController : AppControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Author")]
        [HttpGet]
        public ActionResult Add()
        {
            var postModel = new PostModel();

            return View("Edit", postModel);
        }

        [Authorize(Roles = "Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PostModel postModel)
        {
            if (!ModelState.IsValid)
                return View("Edit", postModel);

            var post = Post.Create(postModel.Title, postModel.Content, postModel.PostTagIdsDelimited, AppUtils.GetCurrentUserId(true)
                                   , postModel.AuthorCreateDate, postModel.AuthorModifyDate, AppCache.PostTagCache);

            this.Repository.AddPost(post);

            return RedirectToAction("Index", new {Id = post.PostId});
        }

        [Authorize(Roles = "Author")]
        [HttpGet]
        public ActionResult Edit(int postId)
        {
            var post = this.Repository.GetPost(postId);

            if (post == null)
                return HttpNotFound("Post ID '{0}' was not found".FormatEx(postId));

            var postModel = new PostModel(post);

            return View(postModel);
        }

        [Authorize(Roles = "Author")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostModel postModel)
        {
            if (!ModelState.IsValid)
                return View("Edit", postModel);

            var post = this.Repository.GetPost(postModel.PostId);

            if (post == null)
                return HttpNotFound("Post ID '{0}' was not found".FormatEx(postModel.PostId));

            post.Update(postModel.Title, postModel.Content, postModel.PostTagIdsDelimited,
                        AppUtils.GetCurrentUserId(true), postModel.AuthorCreateDate, postModel.AuthorModifyDate,
                        AppCache.PostTagCache);

            this.Repository.UpdatePost(post);

            return RedirectToAction("Index", new { Id = post.PostId });
        }
    }
}
