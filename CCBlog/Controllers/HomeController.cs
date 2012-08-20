using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonUtils;

namespace CCBlog.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Throw()
        {
            throw new NotImplementedException("Example exception for testing error handling.");
        }

        [HttpGet]
        public ActionResult WhoAmI()
        {
            return Content(User.Identity.Name.AsNullIfEmpty() ?? "Not logged in");
        }

        [HttpGet]
        public ActionResult WhatTimeIsIt()
        {
            return Content(DateTime.Now.ToString());
        }


    }
}
