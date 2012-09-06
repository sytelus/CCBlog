using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Web;
using CCBlog.Models;
using CommonUtils;

namespace CCBlog.Repository.SqlClient
{
    public class SqlUsers : IUsers
    {
        internal SqlUsers()
        {
        }

        #region Implementation of IUsers

        User IUsers.LoginUser(User user, bool createUserIfNotExists)
        {
            if (!string.IsNullOrEmpty(user.ClaimedIdentifier))
                throw new ArgumentNullException("user.ClaimedIdentifier", "user.ClaimedIdentifier must be set to valid value");

            var resultSets = SqlClientUtils.ExecuteStoredProcedure(SiteConfig.ConnectionString, "[ccb].[LoginUser]",new Dictionary<string, object>()
                {
                    {"@ClaimedIdentifier", user.ClaimedIdentifier},
                    {"@FullName", user.FullName.AsNullIfEmpty()},
                    {"@Email", user.Email.AsNullIfEmpty()},
                    {"@CreateUserIfNotExists", createUserIfNotExists.ToInt()}
                });

            var userTable = resultSets.Tables[0];
            //var userRolesTable = resultSets.Tables[1];
            if (userTable.Rows.Count != 1)
                throw new SecurityException("Cannot login user with ClaimedIdentifier '{0}' because {1} records exists for this user".FormatEx(user.ClaimedIdentifier, userTable.Rows.Count));

            DataRow userRow = userTable.Rows[0];
            var loggeduser = new User()
                                 {
                                     ClaimedIdentifier = (string) userRow["ClaimedIdentifier"],
                                     Comment = (string) userRow["ClaimedIdentifier"],
                                     CreateDate = (DateTimeOffset?) userRow["CreateDate"],
                                     ModifiedDate = (DateTimeOffset?) userRow["ModifiedDate"],
                                     Email = (string) userRow["Email"],
                                     FullName = (string) userRow["FullName"],
                                     LastLoginDate = (DateTimeOffset?) userRow["LastLoginDate"]
                                 };

            return loggeduser;
        }

        #endregion
    }
}