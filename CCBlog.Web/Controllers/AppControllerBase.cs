using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCBlog.Repository;

namespace CCBlog.Controllers
{
    public class AppControllerBase : Controller
    {
        private IRepository repository;

        public IRepository Repository
        {
            get
            {
                if (this.repository == null)
                    this.repository = RepositoryFactory.Get();

                return this.repository;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.repository != null)
                repository.Dispose();

            base.Dispose(disposing);
        }
    }
}