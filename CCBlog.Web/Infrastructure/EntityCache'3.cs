using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CCBlog.Infrastructure
{
    public class EntityCache<TKey, TEntity, TAlternativeKey> : EntityCache<TKey, TEntity> 
        where TAlternativeKey : class 
        where TEntity:class 
    {
        private readonly Func<TEntity, TAlternativeKey> getAlternateKey;
        private Lazy<Dictionary<TAlternativeKey, TEntity>> alternateIndex = null;

        public EntityCache(Func<IEnumerable<TEntity>> getEntities, Func<TEntity, TKey> getKey, Func<TEntity, TAlternativeKey> getAlternateKey)
            : base(getEntities, getKey)
        {
            this.getAlternateKey = getAlternateKey;
        }

        public override void Refresh()
        {
            base.Refresh();
            alternateIndex = new Lazy<Dictionary<TAlternativeKey, TEntity>>(this.GetAlternateEntityIndex, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private Dictionary<TAlternativeKey, TEntity> GetAlternateEntityIndex()
        {
            return base.EntityIndex.Value.Values.ToDictionary(e => this.getAlternateKey(e), e => e);
        }

        protected Lazy<Dictionary<TAlternativeKey, TEntity>> AlternateIndex
        {
            get
            {
                if (this.alternateIndex == null)
                    this.Refresh();

                return alternateIndex;
            }
        }

        public TEntity GetByAlternateKey(TAlternativeKey alternateKey, bool refreshIfNotFound = true)
        {
            var entity = this.AlternateIndex.Value[alternateKey];
            if (entity == null && refreshIfNotFound)
            {
                Refresh();
                entity = this.AlternateIndex.Value[alternateKey];

                if (entity == null)
                    throw new ArgumentOutOfRangeException("alternateKey", string.Format("Entity not found for the specified alternateKey {0}", alternateKey.ToString()));
            }

            return entity;
        }
    }
}