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
        private readonly Func<TEntity, TAlternativeKey> alternateKeyGenerator;
        private Lazy<Dictionary<TAlternativeKey, TEntity>> cachedEntities = null;
        public EntityCache(Func<TEntity, TKey> keyGenerator, Func<TEntity, TAlternativeKey> alternateKeyGenerator)
            : base(keyGenerator)
        {
            this.alternateKeyGenerator = alternateKeyGenerator;
        }

        public override void Refresh()
        {
            base.Refresh();
            cachedEntities = new Lazy<Dictionary<TAlternativeKey, TEntity>>(this.GetEntitiesByAlternateKey, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Dictionary<TAlternativeKey, TEntity> GetEntitiesByAlternateKey()
        {
            return base.CachedEntities.Value.Values.ToDictionary(e => this.alternateKeyGenerator(e), e => e);
        }

        public TEntity GetByAlternateKey(TAlternativeKey alternateKey, bool refreshIfNotFound = true)
        {
            var entity = this.cachedEntities.Value[alternateKey];
            if (entity == null && refreshIfNotFound)
            {
                Refresh();
                entity = this.cachedEntities.Value[alternateKey];

                if (entity == null)
                    throw new ArgumentOutOfRangeException("alternateKey", string.Format("Entity not found for the specified alternateKey {0}", alternateKey.ToString()));
            }

            return entity;
        }
    }
}