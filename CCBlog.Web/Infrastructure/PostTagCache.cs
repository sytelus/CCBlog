using System;
using System.Collections.Generic;
using CCBlog.Repository;
using CommonUtils;
using System.Linq;

namespace CCBlog.Infrastructure
{
    public class PostTagCache
    {
        private readonly Dictionary<string, PostTag> existingTags = new Dictionary<string, PostTag>();
        private readonly Dictionary<string, PostTag> newTags = new Dictionary<string, PostTag>();
        public void Load(IRepository repository)
        {
            existingTags.Clear();
            foreach (var tag in repository.GetTags())
                existingTags.Add(tag.Name, tag);

        }

        public PostTag this[string tagName]
        {
            get { return existingTags.GetValueOrDefault(tagName, () => newTags.GetValueOrDefault(tagName)); }
        }

        public void Add(PostTag postTag)
        {
            if (this[postTag.Name] == null)
                newTags.Add(postTag.Name, postTag);
            else
                throw new ArgumentException("The tag '{0}' already exists".FormatEx(postTag.Name));
        }

        public void Save(IRepository repository)
        {
            if (newTags.Count > 0)
            {
                repository.SaveTags(newTags.Values);
                existingTags.AddRange(newTags);
                newTags.Clear();
            }
        }
    }
}