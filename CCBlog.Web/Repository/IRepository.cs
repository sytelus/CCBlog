using System;
using System.Collections.Generic;

namespace CCBlog.Repository
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Role> GetRoles();
        bool IsRoleExist(string roleName);
        Role GetRole(int userId);
        IEnumerable<User> GetUsers(string roleName);
        User LoginUser(User user, bool createUserIfNotExists);

        IEnumerable<PostTag> GetTags();
        void SaveTags(IEnumerable<PostTag> postTags);

        void AddPost(Post post);
        void UpdatePost(Post post);
        Post GetPost(int postId);
    }
}
