using System;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using CommonUtils;

namespace CCBlog.Infrastructure
{
    public class AppRoleProvider : RoleProvider
    {
        /*
         * Here userName is int converted to string
         */
        public static int ToUserID(string username)
        {
            return int.Parse(username, CultureInfo.InvariantCulture);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var repo = Repository.Factory.Get())
            {
                return repo.GetRole(ToUserID(username)).IfNotNull(r => r.Name) == roleName;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var repo = Repository.Factory.Get())
            {
                return repo.GetRole(ToUserID(username)).IfNotNull(r => new string[] {r.Name}, new string[] {});
            }
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
            using (var repo = Repository.Factory.Get())
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
            using (var repo = Repository.Factory.Get())
            {
                return repo.GetUsers(roleName)
                    .IfNotNull(users => users.Select(u => u.UserId.ToStringInvariant())
                                                .ToArray()
                              , new string[] {});
            }
        }

        public override string[] GetAllRoles()
        {
            using (var repo = Repository.Factory.Get())
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