using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CCBlog.Infrastructure;
using CommonUtils;

namespace CCBlog.Repository
{
    public class Post
    {
        //Basic fields
        public int PostId { get; private set; }
        [MaxLength(128)][Required] public string UrlFriendlyName { get; private set; }
        [MaxLength(4000)][Required] public string Title { get; private set; }
        public string Content { get; private set; }
        [Required] public DateTimeOffset CreateDate { get; private set; }
        [Required] public DateTimeOffset ModifyDate { get; private set; }
        [Required] public DateTimeOffset AuthorCreateDate { get; private set; }
        [Required] public DateTimeOffset AuthorModifyDate { get; private set; }

        //Author fields
        public int CreatedByUserId { get; private set; }
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedBy { get; private set; }
        public int ModifiedByUserId { get; private set; }
        [ForeignKey("ModifiedByUserId")]
        public virtual User ModifiedBy { get; private set; }

        public int RevisionCount { get; private set; }

        //Visibility control fields
        public bool IsPrivate { get; private set; } //When set, this post is not visible to anyone accept user who created it

        //Multi-part article support fields
        [MaxLength(4000)]
        public string SeriesTitle { get; private set; }
        public int? SeriesPartNumber { get; private set; }

        //Layout config fields
        public int? DisplayOrder { get; private set; }  //Posts are sorted by DisplayOrder DESC, CreateDate ASC
        public string PageLayoutName { get; private set; }  //This to enable adding whole new page (for example JavaScript game). if null then use standard layout.

        public string PostTagIdsDelimited { get; private set; }
        //This property mainly exists for EF to establish relationship. We should be using PostTagIds and cached list of tags to avoid round trips
        public virtual ICollection<PostTag> Tags { get; private set; }

        public static Post Create(string title, string content, string postTagIdsDelimited, int userId
            , DateTimeOffset authorCreateDate, DateTimeOffset authorModifyDate
            , PostTagCache tagCache)
        {
            var post = new Post
                           {
                               CreateDate = DateTimeOffset.UtcNow,
                               CreatedByUserId = userId,
                               RevisionCount = 0
                           };

            post.Update(title, content, postTagIdsDelimited, userId, authorCreateDate, authorModifyDate, tagCache);

            return post;
        }

        public void Update(string title, string content, string postTagIdsDelimited, int userId
            , DateTimeOffset authorCreateDate, DateTimeOffset authorModifyDate
            , PostTagCache tagCache)
        {
            this.Title = title;
            this.Content = content;
            this.UrlFriendlyName = title.ToUrlFriendlyValue();
            this.ModifyDate = DateTimeOffset.UtcNow;
            this.AuthorCreateDate = authorCreateDate;
            this.AuthorModifyDate = authorModifyDate;
            this.ModifiedByUserId = userId;
            this.RevisionCount++;
            this.ResetTags(postTagIdsDelimited, tagCache);
        }

        private void ResetTags(string postTagIdsDelimited, PostTagCache tagCache)
        {
            var isPrivate = false;
            var displayOrder = (int?) null;
            var seriesTitle = (string) null;
            var seriesPartNumber =  (int?) null;
            var pageLayoutName = (string)null;
            var postTags = new List<PostTag>();

            var allTagArguments = (postTagIdsDelimited ?? string.Empty)
                 .Split(Utils.SemiColonDelimiter, StringSplitOptions.RemoveEmptyEntries)
                 .Select(tag => tag.Split(Utils.ColonDelimiter, StringSplitOptions.None)
                                    .Select(tagArgument => tagArgument.Trim()).ToArray());

            foreach (var tagArguments in allTagArguments)
            {
                switch (tagArguments[0].ToLowerInvariant())
                {
                    case "_private":
                        ValidateTagArgumentCount(0, tagArguments);
                        isPrivate = true;
                        break;
                    case "_order":
                        ValidateTagArgumentCount(1, tagArguments);
                        displayOrder = tagArguments[1].Length == 0 ?(int?) null : int.Parse(tagArguments[1]);
                        break;
                    case "_series":
                        ValidateTagArgumentCount(2, tagArguments);
                        seriesTitle = tagArguments[1].Length == 0 ? null : tagArguments[1];
                        seriesPartNumber = seriesTitle == null ? (int?) null : int.Parse(tagArguments[2]);
                        break;
                    case "_layout":
                        ValidateTagArgumentCount(1, tagArguments);
                        pageLayoutName = tagArguments[1].Length == 0 ? null : tagArguments[1];
                        break;
                    default:
                        ValidateTagArgumentCount(0, tagArguments);
                        AddNonCommandTag(tagArguments, postTags, tagCache);
                        break;
                }
            }

            this.IsPrivate = isPrivate;
            this.DisplayOrder = displayOrder;
            this.SeriesTitle = seriesTitle;
            this.SeriesPartNumber = seriesPartNumber;
            this.PageLayoutName = pageLayoutName;
            this.Tags = postTags;

            this.PostTagIdsDelimited = postTagIdsDelimited;
        }

        private static void AddNonCommandTag(IList<string> tagArguments, ICollection<PostTag> postTags, PostTagCache tagCache)
        {
            var tagName = tagArguments[0].ToLowerInvariant();

            if (tagName.StartsWith("_"))   
                throw new ArgumentException("Tag command '{0}' is not recognized".FormatEx(tagArguments[0]));

            var tag = tagCache[tagName];
            if (tag == null)
            {
                tag = new PostTag() {Name = tagName};
                tagCache.Add(tag);
            }

            postTags.Add(tag);
        }

        private static void ValidateTagArgumentCount(int expectedCount, IList<string> tagArguments)
        {
            if (expectedCount != tagArguments.Count - 1)
                throw new ArgumentException("{0} arguments were expected for tag '{1}' but only '{2}' found. Use ':' to specify expected number of arguments"
                    .FormatEx(expectedCount, tagArguments[0], tagArguments.Count - 1));
        }
    }
}