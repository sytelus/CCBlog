using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.ModelBehaviour
{
    public static class Role
    {
        public static bool IsAdmin(this Repository.Role role)
        {
            return role.Name == "Administrator";
        }
    }
}