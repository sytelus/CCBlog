using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CCBlog.Infrastructure;
using CCBlog.Model.Contracts;
using CCBlog.Model.Poco;
using CommonUtils;
using PetaPoco;

namespace CCBlog.Repository.PetaPocoSqlServer
{
    public class Repository : IRepository
    {
        private readonly CCBlogDbContext dbContext = new CCBlogDbContext();

        public IEnumerable<IRole> GetRoles()
        {
            return this.dbContext.Query<Role>("SELECT * FROM [CCBlog].[Role]");
        }

        public IEnumerable<IUser> GetUsersInRoles(int[] roleIDs)
        {
            var sql = PetaPoco.Sql.Builder
                              .Append("SELECT *")
                              .Append("FROM [CCBlog].[User]")
                              .Append("WHERE RoleId IN @RoleIds", new {RoleIds = roleIDs});

            var users = this.dbContext.Query<User>(sql);

            return users.Select(SetupUserAfterLoad);
        }
        
        private static User SetupUserAfterLoad(User user)
        {
            if (user == null)
                return null;

            user.Role = user.RoleId == null ? null : AppCache.Roles[user.RoleId.Value];
            return user;
        }

        private static User SetupUserBeforeSave(IUser user)
        {
            var userPoco = (User) user;
            userPoco.RoleId = user.Role == null ? (int?)null : user.Role.RoleId;
            return userPoco;
        }

        public IUser AddUser(IUser user)
        {
            var userPoco = SetupUserBeforeSave(user);
            this.dbContext.Insert(userPoco);
            return userPoco;
        }

        public IUser UpdateUser(IUser user)
        {
            var userPoco = SetupUserBeforeSave(user);
            this.dbContext.Update(userPoco);
            return userPoco;
        }

        public IUser GetUser(string claimedIdentifier)
        {
            var sql = PetaPoco.Sql.Builder
                              .Append("SELECT *")
                              .Append("FROM [CCBlog].[User]")
                              .Append("WHERE ClaimedIdentifier = @0", claimedIdentifier);
            
            return SetupUserAfterLoad(this.dbContext.SingleOrDefault<User>(sql));
        }

        public IUser GetUser(int userId)
        {
            var sql = PetaPoco.Sql.Builder
                              .Append("SELECT *")
                              .Append("FROM [CCBlog].[User]")
                              .Append("WHERE UserId = @0", userId);

            return SetupUserAfterLoad(this.dbContext.SingleOrDefault<User>(sql));
        }

        public IEntityPage<IPost> GetPostPage(int? tagId, int? seriesId, EntityStatus status, int skipCount, int takeCount)
        {
            //Get total count
            var postCount = AppCache.PostCounts.GetOrAddCount(() => GetPostCounts(tagId, seriesId, status)
                , tagId, seriesId, status);

            //Get posts for the requested page
            var sql = GetPostsPageSql(false, tagId, seriesId, status, skipCount, takeCount);
            var posts = this.dbContext.Query<Post, Series>(sql).Select(p => SetupPostAfterLoad(p, false));

            return new EntityPage<IPost>(skipCount, takeCount, postCount, posts);
        }

        private static IPost SetupPostAfterLoad(Post post, bool forEdit = false)
        {
            if (post == null)
                return null;

            post.CreatedByUser = AppCache.AuthorsAndAdmins[post.CreatedByUserId];
            post.ModifiedByUser = AppCache.AuthorsAndAdmins[post.ModifiedByUserId];

            if (forEdit)
                post.Tags = GetTagsFromCsv(post).ToList();
            else
                post.Tags = GetTagsFromCsv(post).ToArray();

            //Series object is setup by multi-poco mechanism of PetaPoco

            return post;
        }

        private static IEnumerable<ITag> GetTagsFromCsv(Post post)
        {
            return (post.TagIdCsv ?? string.Empty)
                .Split(Utils.CommaDelimiter)
                .Select(t => AppCache.Tags[int.Parse(t, CultureInfo.InvariantCulture)]);
        }

        private int GetPostCounts(int? tagId, int? seriesId, EntityStatus status)
        {
            var sql = GetPostsPageSql(true, tagId, seriesId, status, null, null);
            return (int) this.dbContext.ExecuteScalar<long>(sql);
        }

        private static Sql GetPostsPageSql(bool countOnly, int? tagId, int? seriesId, EntityStatus status, int? skipCount, int? takeCount)
        {
            var sql = PetaPoco.Sql.Builder;

            if (countOnly)
                sql.Append("SELECT COUNT(1) AS Count");
            else
                sql.Append("SELECT p.*, s.SeriesId, s.Title");

            sql.Append("FROM [CCBlog].[Post] AS p");

            if (!countOnly)
            {
                sql.Append("LEFT OUTER JOIN [CCBlog].[Series] AS s");
                sql.Append(" ON p.SeriesId = s.SeriesId");
            }

            //Include tag join if there is tag filter 
            if (tagId != null)
            {
                sql.Append("INNER JOIN [CCBlog].[PostTag] AS pt");
                sql.Append("   ON p.PostId = pt.PostId");
            }

            //Status filter
            if (status == EntityStatus.Active) //Most typical case for performance
                sql.Append("WHERE (p.StatusId = @0)", (int) EntityStatus.Active);
            else
                sql.Append("WHERE (p.StatusId & @0 > 0)", status);

            //Tag filter
            if (tagId != null)
                sql.Append("    AND (pt.TagId = @0)", tagId.Value);

            //Series filter
            if (seriesId != null)
                sql.Append("    AND (p.SeriesId = @0)", seriesId.Value);

            //Order
            if (!countOnly)
                sql.Append("ORDER BY p.DisplayOrder ASC, p.AuthorCreateDate DESC");

            //Paging
            if (skipCount != null)
                sql.Append("OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY", skipCount.Value, takeCount.Value);

            return sql;
        }


        public IPost GetPost(int postId, bool forEdit)
        {
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT p.*, s.SeriesId, s.Title");
            sql.Append("FROM [CCBlog].[Post] AS p");
            sql.Append("LEFT OUTER JOIN [CCBlog].[Series] AS s");
            sql.Append(" ON p.SeriesId = s.SeriesId");
            sql.Append(" WHERE p.PostId = @0", postId);

            return SetupPostAfterLoad(this.dbContext.SingleOrDefault<Post>(sql), forEdit);
        }

        private Post SetupPostBeforeSave(IPost post, IPost originalPost = null)
        {
            var postPoco = (Post) post;

            //Save new childs in post
            AddNewPostChilds(postPoco);

            postPoco.SeriesId = postPoco.Series == null ? (int?)null : postPoco.Series.SeriesId;
            postPoco.CreatedByUserId = postPoco.CreatedByUser.UserId;
            postPoco.ModifiedByUserId = postPoco.ModifiedByUser.UserId;

            postPoco.TagIdCsv = postPoco.Tags.Select(t => t.TagId).ToDelimitedString(",");

            if (originalPost == null)
            {
                //Override non-settable properties
                postPoco.CreateDate = DateTimeOffset.UtcNow;
                postPoco.ModifyDate = postPoco.CreateDate;
                postPoco.RevisionCount = 0;
            }
            else
            {
                postPoco.CreateDate = originalPost.CreateDate;
                postPoco.ModifyDate = DateTimeOffset.UtcNow;
                postPoco.RevisionCount = originalPost.RevisionCount + 1;
                postPoco.PostId = originalPost.PostId;
            }

            return postPoco;
        }

        public void AddSeries(ISeries series)
        {
            var seriesPoco = (Series) series;
            this.dbContext.Insert(seriesPoco);
        }

        public void UpdateSeries(ISeries series)
        {
            var seriesPoco = (Series)series;
            this.dbContext.Update(seriesPoco);
        }

        public ISeries GetSeries(int seriesId)
        {
            return this.dbContext.SingleOrDefault<Series>("SELECT * FROM [CCBlog].[Series] WHERE SeriesId = @0", seriesId);
        }

        public IEntityPage<ISeries> GetSeriesPage(EntityStatus status, int skipCount, int takeCount)
        {
            var seriesCount = (int) AppCache.ValueCache.GetOrAdd("SeriesCount", (s) => GetSeriesCount());

            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT *");
            sql.Append("FROM [CCBlog].[Series]");
            sql.Append("ORDER BY SeriesId DESC");
            sql.Append("OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY", skipCount, takeCount);

            var serieses = this.dbContext.Query<Series>(sql);

            return new EntityPage<ISeries>(skipCount, takeCount, seriesCount, serieses);
        }

        private int GetSeriesCount()
        {
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT COUNT(1) AS Count");
            sql.Append("FROM [CCBlog].[Series]");

            return (int) this.dbContext.ExecuteScalar<long>(sql);
        }

        public void AddPost(IPost post)
        {
            using (var transaction = this.dbContext.GetTransaction())
            {
                var postPoco = SetupPostBeforeSave(post, null);
                this.dbContext.Insert(postPoco);

                //Add relationship
                foreach (var tag in post.Tags.Where(t => t.TagId < 1))
                    this.dbContext.Insert(new PostTag() { PostId = post.PostId, TagId = tag.TagId });

                transaction.Complete();
            }
        }

        private void AddNewPostChilds(IPost post)
        {
            using (var transaction = this.dbContext.GetTransaction())
            {
                //Save series if new
                if (post.Series != null && post.Series.SeriesId < 1)
                    this.AddSeries(post.Series);

                //Save tags if new
                foreach (var tag in post.Tags.Where(t => t.TagId < 1))
                    this.AddTag(tag);

                transaction.Complete();
            }
        }

        public void UpdatePost(IPost originalPost, IPost updatedPost)
        {
            using (var transaction = this.dbContext.GetTransaction())
            {
                //Main update
                var postPoco = SetupPostBeforeSave(updatedPost, originalPost);
                this.dbContext.Update(postPoco);

                //Add relationship
                foreach (var tag in updatedPost.Tags.Except(originalPost.Tags))
                    this.dbContext.Insert(new PostTag() { PostId = postPoco.PostId, TagId = tag.TagId });

                //Remove relationship
                foreach (var tag in originalPost.Tags.Except(updatedPost.Tags))
                    this.dbContext.Delete(new PostTag() { PostId = postPoco.PostId, TagId = tag.TagId });

                transaction.Complete();
            }            
        }

        public IEnumerable<ITag> GetTags()
        {
            return this.dbContext.Query<Tag>("SELECT * FROM [CCBlog].[Tag]");
        }

        public void AddTag(ITag tag)
        {
            var tagPoco = (Tag) tag;
            this.dbContext.Insert(tagPoco);
        }

        public void UpdateTag(ITag tag)
        {
            var tagPoco = (Tag) tag;
            this.dbContext.Update(tagPoco);
        }
 

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
   }
}