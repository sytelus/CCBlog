using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils;

namespace CCBlog.Repository.EntityRepository
{
    public class Repository : IRepository
    {
        readonly MainDbContext dbContext = new MainDbContext();

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (true)
                dbContext.Dispose();
        }

        #endregion


        #region Implementation of IRepository

        public IEnumerable<Role> GetRoles()
        {
            return dbContext.Roles;
        }

        public bool IsRoleExist(string roleName)
        {
            return dbContext.Roles.Any(r => r.Name == roleName);
        }

        public Role GetRole(int userId)
        {
            return dbContext.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Role)
                .FirstOrDefault();
        }

        public IEnumerable<User> GetUsers(string roleName)
        {
            return dbContext.Roles.Where(r => r.Name == roleName).SelectMany(r => r.Users);
        }

        public User LoginUser(User user, bool createUserIfNotExists)
        {
            if (string.IsNullOrEmpty(user.ClaimedIdentifier))
                throw new ArgumentNullException("user.ClaimedIdentifier", "user.ClaimedIdentifier must be valid");

            //Find this user
            var foundUser = dbContext.Users.FirstOrDefault(u => u.ClaimedIdentifier == user.ClaimedIdentifier);
            if (foundUser != null)
            {
                var updated = false;

                //Check if we have latest info
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    foundUser.FullName = user.FullName;
                    updated = true;
                }

                if (!string.IsNullOrEmpty(user.Email))
                {
                    foundUser.Email = user.Email;
                    updated = true;
                }

                if (updated)
                    dbContext.SaveChanges();

                return foundUser;
            }
            else
            {
                if (createUserIfNotExists)
                {
                    if (user.UserId != 0)
                        throw new ArgumentOutOfRangeException("user.UserId must be 0 if new user needs to be created");

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    return user;
                }
                else return null;
            }
        }

        public IEnumerable<PostTag> GetTags()
        {
            return dbContext.PostTags;
        }

        public void SaveTags(IEnumerable<PostTag> postTags)
        {
            foreach (var postTag in postTags)
                dbContext.PostTags.Add(postTag);

            dbContext.SaveChanges();
        }

        public void AddPost(Post post)
        {
            dbContext.Posts.Add(post);
            AttachExistingTags(post);
            dbContext.SaveChanges();
        }

        private void AttachExistingTags(Post post)
        {
            if (post.Tags != null && post.Tags.Count > 0)
            {
                foreach (var tag in post.Tags)
                {
                    if (tag.PostTagId != 0)
                        dbContext.AttachAsModified(tag, false);
                }
            }
        }

        public void UpdatePost(Post post)
        {
            if (dbContext.IsDetached(post))
                throw new ArgumentException("Post '{0}' cannot be updated because it is not being tracked by database context".FormatEx(post.PostId));
            // don't know if we should allow this
            // dbContext.AttachAsModified(post, true);
            
            AttachExistingTags(post);

            dbContext.SaveChanges();
        }

        public Post GetPost(int postId)
        {
            return dbContext.Posts.Find(postId);
        }

        #endregion
    }
}