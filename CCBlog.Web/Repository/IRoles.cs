using System.Collections.Generic;
using CCBlog.Models;

namespace CCBlog.Repository
{
    interface IRoles
    {
        IEnumerable<Role> GetRoles();
        bool IsRoleExist(string roleName);
        IEnumerable<Role> GetRoles(IEnumerable<int> userIds);
        IEnumerable<User> GetUsers(IEnumerable<string> roleNames);
    }
}
