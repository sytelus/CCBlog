using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Repository.SqlClient
{
    public class SqlRepository : IRepository
    {
        #region Implementation of IRepository

        private readonly IUsers users = new SqlUsers();

        private readonly IRoles roles = new SqlRoles();

        IUsers IRepository.Users
        {
            get { return users; }
        }

        IRoles IRepository.Roles
        {
            get { return roles; }
        }

        #endregion
    }
}