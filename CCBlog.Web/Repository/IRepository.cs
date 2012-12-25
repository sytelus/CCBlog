using System;
using System.Collections.Generic;
using CCBlog.Model.Contracts;

namespace CCBlog.Repository
{
    public interface IRepository : IDisposable
    {
        //Caching of users/roles and login managemnt
        IEnumerable<IRole> GetRoles();
        IEnumerable<IUser> GetUsersInRoles(int[] roleIDs);
        IUser AddUser(IUser user);
        IUser UpdateUser(IUser user);
        IUser GetUser(string claimedIdentifier);
        IUser GetUser(int userId);

        //Posts
        IEntityPage<IPost> GetPostPage(int? tagId, int? seriesId, EntityStatus status, int skipCount, int takeCount);
        IPost GetPost(int postId, bool forEdit);
        void AddPost(IPost post);
        void UpdatePost(IPost originalPost, IPost updatedPost);

        //Series
        void AddSeries(ISeries series);
        void UpdateSeries(ISeries series);
        ISeries GetSeries(int seriesId);
        IEntityPage<ISeries> GetSeriesPage(EntityStatus status, int skipCount, int takeCount);
            
        //Tags
        IEnumerable<ITag> GetTags();
        void AddTag(ITag tag);
        void UpdateTag(ITag tag);
    }
}
