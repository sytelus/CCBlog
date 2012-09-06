using System.Collections.Generic;
using CCBlog.Models;

namespace CCBlog.Repository
{
    interface IUsers
    {
        User LoginUser(User user, bool createUserIfNotExists);
    }
}
