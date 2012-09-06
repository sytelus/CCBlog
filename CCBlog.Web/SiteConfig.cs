using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CCBlog
{
    public static class SiteConfig
    {
        public static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["MainDb"].ConnectionString;
    }
}