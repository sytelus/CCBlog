using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Repository
{
    public class Factory
    {
        public static IRepository Get()
        {
            return new EntityRepository.Repository();
        }
    }
}