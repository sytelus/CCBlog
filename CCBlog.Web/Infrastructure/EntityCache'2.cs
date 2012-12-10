using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CCBlog.Infrastructure
{
    public class EntityCache<TKey, TEntity> : IEnumerable<KeyValuePair<TKey, TEntity>> where TEntity:class 
    {
        private Lazy<Dictionary<TKey, TEntity>> cachedEntities = null;
        private readonly Func<TEntity, TKey> keyGenerator;
        public EntityCache(Func<TEntity, TKey> keyGenerator)
        {
            this.keyGenerator = keyGenerator;
            Refresh();
        }

        public virtual void Refresh()
        {
            this.cachedEntities = new Lazy<Dictionary<TKey, TEntity>>(this.GetEntities, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        protected Lazy<Dictionary<TKey, TEntity>> CachedEntities
        {
            get { return this.cachedEntities; }
        }

        private Dictionary<TKey, TEntity> GetEntities()
        {
            using (var repository = Repository.Factory.Get())
            {
                return repository.GetEntities<TEntity>().ToDictionary(e => this.keyGenerator(e), e => e);
            }
        }

        public TEntity this[TKey key, bool refreshIfNotFound = true]
        {
            get
            {
                var entity = this.cachedEntities.Value[key];
                if (entity == null && refreshIfNotFound)
                {
                    Refresh();
                    entity = this.cachedEntities.Value[key];

                    if (entity == null)
                        throw new ArgumentOutOfRangeException("key", string.Format("Entity not found for the specified key {0}", key.ToString()));
                }

                return entity;
            }
        }

        public int Count
        {
            get { return this.cachedEntities.Value.Count; }
        }

        public IEnumerator<KeyValuePair<TKey, TEntity>> GetEnumerator()
        {
            return this.cachedEntities.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}