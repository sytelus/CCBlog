using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CCBlog.Model.Contracts;
using CCBlog.Repository;
using CommonUtils;
using System.Collections.Concurrent;

namespace CCBlog.Infrastructure
{
    public static class AppCache
    {
        public static readonly EntityCache<int, IRole, string> Roles = new EntityCache<int, IRole, string>(GetRoles, r => r.RoleId, r => r.Name);
        public static readonly EntityCache<int, ITag, string> Tags = new EntityCache<int, ITag, string>(GetTags, t => t.TagId, t => t.Name);
        public static readonly EntityCache<int, IUser> AuthorsAndAdmins = new EntityCache<int, IUser>(GetAuthorsAndAdmins, u => u.UserId);
        public static readonly EntityCountsCache PostCounts = new EntityCountsCache();
        public static readonly KeyValueCache<object> ValueCache = new KeyValueCache<object>();

        private static IEnumerable<IRole> GetRoles()
        {
            using (var repository = RepositoryFactory.Get())
            {
                return repository.GetRoles();
            }
        }

        private static IEnumerable<ITag> GetTags()
        {
            using (var repository = RepositoryFactory.Get())
            {
                return repository.GetTags();
            }            
        }

        private static IEnumerable<IUser> GetAuthorsAndAdmins()
        {
            using (var repository = RepositoryFactory.Get())
            {
                return repository.GetUsersInRoles(AppCache.Roles
                    .Where(r => r.IsAdmin() || r.IsAuthor())
                    .Select(r => r.RoleId).ToArray());
            }
        }

    }
}