using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Infrastructure
{
    public static class AppCache
    {
        public readonly static EntityCache<int, Repository.Role> Roles = new EntityCache<int, Repository.Role>(r => r.RoleId);
        public readonly static EntityCache<int, Repository.Tag, string> Tags = new EntityCache<int, Repository.Tag, string>(pt => pt.TagId, pt => pt.Name.ToLowerInvariant());
    }
}