using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CCBlog.Infrastructure
{
    public class EntityCache<TKey, TEntity> : IEnumerable<TEntity> where TEntity:class 
    {
        private Lazy<Dictionary<TKey, TEntity>> entityIndex = null;
        private Lazy<TEntity[]> toArray = null;
        private readonly Func<TEntity, TKey> getKey;
        private readonly Func<IEnumerable<TEntity>> getEntities = null;
        public EntityCache(Func<IEnumerable<TEntity>> getEntities, Func<TEntity, TKey> getKey)
        {
            this.getKey = getKey;
            this.getEntities = getEntities;
        }

        public virtual void Refresh()
        {
            this.entityIndex = new Lazy<Dictionary<TKey, TEntity>>(this.GetEntityIndex, LazyThreadSafetyMode.ExecutionAndPublication);
            this.toArray = new Lazy<TEntity[]>(() => this.EntityIndex.Value.Values.ToArray(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        protected Lazy<Dictionary<TKey, TEntity>> EntityIndex
        {
            get
            {
                if (this.entityIndex == null)
                    this.Refresh();

                return this.entityIndex;
            }
        }

        private Dictionary<TKey, TEntity> GetEntityIndex()
        {
            var entities = getEntities();
            return entities.ToDictionary(e => getKey(e), e => e);
        }

        public TEntity this[TKey key, bool refreshIfNotFound = true]
        {
            get
            {
                var entity = this.EntityIndex.Value[key];
                if (entity == null && refreshIfNotFound)
                {
                    Refresh();
                    entity = this.EntityIndex.Value[key];

                    if (entity == null)
                        throw new ArgumentOutOfRangeException("key", string.Format("Entity not found for the specified key {0}", key.ToString()));
                }

                return entity;
            }
        }

        public int Count
        {
            get { return this.EntityIndex.Value.Count; }
        }

        public TEntity[] ToArray()
        {
            if (this.toArray == null)   
                this.Refresh();

            return toArray.Value;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.EntityIndex.Value.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}