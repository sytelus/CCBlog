using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Models;

namespace CCBlog.Repository.SqlClient
{
    public class SqlRoles : IRoles
    {
        internal SqlRoles()
        {
            
        }

        #region Implementation of IRoles

        IEnumerable<Role> IRoles.GetRoles()
        {
            throw new NotImplementedException();
        }

        bool IRoles.IsRoleExist(string roleName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Role> IRoles.GetRoles(IEnumerable<int> userIds)
        {
            throw new NotImplementedException();
        }

        IEnumerable<User> IRoles.GetUsers(IEnumerable<string> roleNames)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}