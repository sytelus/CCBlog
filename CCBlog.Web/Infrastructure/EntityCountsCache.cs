using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils;

namespace CCBlog.Infrastructure
{
    public class EntityCountsCache
    {
        private readonly KeyValueCache<int> counts = new KeyValueCache<int>();

        public int GetOrAddCount(Func<int> getCount, params object[] criteria)
        {
            var key = GetKey(criteria);
            return counts.GetOrAdd(key, (s) => getCount());
        }

        public void Invalidate()
        {
            counts.InvalidateAll();
        }

        private static string GetKey(IEnumerable<object> criteria)
        {
            return criteria.Select(c => c == null ? string.Empty : c.ToString()).ToDelimitedString("\t");
        }
    }
}