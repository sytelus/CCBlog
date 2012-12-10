using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Infrastructure;

namespace CCBlog.ModelBehaviour
{
    public static class User
    {
        public static string GetfriendlyName(this Repository.User user)
        {
            var friendlyName = user.FullName ?? user.Nickname ?? user.Email ?? user.ClaimedIdentifier ?? string.Empty;
            if (user.RoleId != null)
            {
                var role = AppCache.Roles[user.RoleId.Value];
                if (role.IsAdmin())
                    friendlyName += " [Admin]";
            }

            return friendlyName;
        }
    }
}