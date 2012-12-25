using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Model.Contracts;

namespace CCBlog.Model.Poco
{
    public partial class Role : IRole
    {
        public const string AdministratorRoleName = "Administrator";
        public const string AuthorRoleName = "Administrator";

        public bool IsAdmin()
        {
            return this.Name == AdministratorRoleName;
        }

        public bool IsAuthor()
        {
            return this.Name == AuthorRoleName;
        }
    }
}