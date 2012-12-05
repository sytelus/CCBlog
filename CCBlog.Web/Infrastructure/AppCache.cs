using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Repository;

namespace CCBlog.Infrastructure
{
    public static class AppCache
    {
        public static PostTagCache PostTagCache
        {
            get
            {
                var cache = HttpContext.Current.Application["PostTagCache"] as PostTagCache;
                if (cache == null)
                {
                    cache = new PostTagCache();
                    using(var repo = Factory.Get())
                    {
                        cache.Load(repo);    
                    }

                    HttpContext.Current.Application.Add("PostTagCache", cache);
                }

                return cache;
            }
        }
    }
}