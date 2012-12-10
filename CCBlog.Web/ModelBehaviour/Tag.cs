using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Infrastructure;
using CommonUtils;
using CommandTagInfo = System.Collections.Generic.KeyValuePair<CCBlog.ModelBehaviour.CommandTagType, string[]>;

namespace CCBlog.ModelBehaviour
{
    public enum CommandTagType
    {
        PrivatePost,
        PostOrder,
        PostSeries,
        PostLayout
    }

    public static class Tag
    {
        public static void ParseTags(string postTagIdsDelimited, out ICollection<Repository.Tag> tags
            , out ICollection<CommandTagInfo> commandTags)
        {
            tags = new List<Repository.Tag>();
            commandTags = new List<CommandTagInfo>();

            var allTagArguments = (postTagIdsDelimited ?? string.Empty)
                 .Split(Utils.SemiColonDelimiter, StringSplitOptions.RemoveEmptyEntries)
                 .Select(tag => tag.Split(Utils.ColonDelimiter, StringSplitOptions.None)
                                    .Select(tagArgument => tagArgument.Trim()).ToArray());

            foreach (var tagArguments in allTagArguments)
            {
                var tagName = tagArguments[0].ToLowerInvariant();
                switch (tagName)
                {
                    case "_private":
                        ValidateTagArgumentCount(0, tagArguments);
                        commandTags.Add(new CommandTagInfo(CommandTagType.PrivatePost, tagArguments));
                        break;
                    case "_order":
                        ValidateTagArgumentCount(1, tagArguments);
                        commandTags.Add(new CommandTagInfo(CommandTagType.PostOrder, tagArguments));
                        break;
                    case "_series":
                        ValidateTagArgumentCount(2, tagArguments);
                        commandTags.Add(new CommandTagInfo(CommandTagType.PostSeries, tagArguments));
                        break;
                    case "_layout":
                        ValidateTagArgumentCount(1, tagArguments);
                        commandTags.Add(new CommandTagInfo(CommandTagType.PostLayout, tagArguments));
                        break;
                    default:
                        ValidateTagArgumentCount(0, tagArguments);
                        AddNonCommandTag(tagName, tagArguments, tags)
                        break;
                }
            }
        }

        private static void AddNonCommandTag(string tagName, IList<string> tagArguments, ICollection<Repository.Tag> tags)
        {
            if (tagName.StartsWith("_"))
                throw new ArgumentException("Tag command '{0}' is not recognized".FormatEx(tagArguments[0]));

            tags.Add(AppCache.Tags.GetByAlternateKey(tagName));
        }

        private static void ValidateTagArgumentCount(int expectedCount, IList<string> tagArguments)
        {
            if (expectedCount != tagArguments.Count - 1)
                throw new ArgumentException("{0} arguments were expected for tag '{1}' but only '{2}' found. Use ':' to specify expected number of arguments"
                    .FormatEx(expectedCount, tagArguments[0], tagArguments.Count - 1));
        }
    }
}