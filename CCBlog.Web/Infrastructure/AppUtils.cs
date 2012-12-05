using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;

namespace CCBlog.Infrastructure
{
    public static class AppUtils
    {
        public static int GetCurrentUserId(bool isRequired)
        {
            var userIdString = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(userIdString))
            {
                var userId = int.Parse(userIdString);
                return userId;
            }
            else
            {
                if (!isRequired)
                    return -1;
                else
                    throw new SecurityException("User is not logged in");
            }
        }
    }
}