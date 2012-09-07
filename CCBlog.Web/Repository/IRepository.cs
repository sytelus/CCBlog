using System;
using System.Collections.Generic;
using CCBlog.Models;

namespace CCBlog.Repository
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Role> GetRoles();
        bool IsRoleExist(string roleName);
        Role GetRole(int userId);
        IEnumerable<User> GetUsers(string roleName);

        User LoginUser(User user, bool createUserIfNotExists);

    }
}
