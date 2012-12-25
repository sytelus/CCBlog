using System;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using CCBlog.Model.Contracts;
using CommonUtils;

namespace CCBlog.Infrastructure
{
    public class AppRoleProvider : RoleProvider
    {
        /*
         * Here userName is int converted to string
         */
        private static int ToUserId(string username)
        {
            return int.Parse(username, CultureInfo.InvariantCulture);
        }

        private IRole GetRoleForUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            var userId = ToUserId(userName);

            var user = AppCache.AuthorsAndAdmins[userId, false];

            if (user == null)
            {
                using (var repo = Repository.RepositoryFactory.Get())
                    user = repo.GetUser(userId);
            }

            if (user == null)
                return null;
            else
                return user.Role;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var role = GetRoleForUser(username);

            if (role == null)
                return false;
            else
                return role.Name == roleName;
        }

        public override string[] GetRolesForUser(string username)
        {
            var role = GetRoleForUser(username);

            if (role == null)
                return new string[]{};
            else
                return new string[] {role.Name};
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            using (var repo = Repository.RepositoryFactory.Get())
            {
                return repo.GetRoles().Any(r => r.Name == roleName);
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var repo = Repository.RepositoryFactory.Get())
            {
                return repo.GetUsersInRoles(new int[] {AppCache.Roles.GetByAlternateKey(roleName).RoleId})
                    .Select(u => u.UserId.ToString())
                    .ToArray();
            }
        }

        public override string[] GetAllRoles()
        {
            using (var repo = Repository.RepositoryFactory.Get())
            {
                return repo.GetRoles().Select(r => r.Name).ToArray();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}