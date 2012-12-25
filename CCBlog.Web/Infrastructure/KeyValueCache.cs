using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils;

namespace CCBlog.Infrastructure
{
    public class KeyValueCache<T>
    {
        private ConcurrentDictionary<string, T> values = new ConcurrentDictionary<string, T>();

        public T GetOrAdd(string key, Func<string, T> getValue)
        {
            return values.GetOrAdd(key, getValue);
        }

        public bool InvalidateValue(string key)
        {
            T value;
            return values.TryRemove(key, out value);
        }

        public void InvalidateAll()
        {
            values = new ConcurrentDictionary<string, T>();
        }
    }
}