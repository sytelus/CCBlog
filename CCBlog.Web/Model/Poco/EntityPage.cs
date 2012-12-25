using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Model.Contracts;

namespace CCBlog.Model.Poco
{
    public class EntityPage<TEntity> : IEntityPage<TEntity>
    {
        private readonly int requestedEntitiesSkip;
        private readonly int requestedEntitiesTake;
        private readonly int requestedPagesSkip;
        private readonly int actualPageNumber;
        private readonly int totalPages;
        private readonly int totalEntities;
        private readonly IEnumerable<TEntity> entities;

        public EntityPage(int requestedEntitiesSkip, int requestedEntitiesTake, int totalEntities, IEnumerable<TEntity> entities)
        {
            this.requestedEntitiesSkip = requestedEntitiesSkip;
            this.requestedEntitiesTake = requestedEntitiesTake;
            this.requestedPagesSkip = requestedEntitiesSkip/requestedEntitiesTake;

            this.totalEntities = totalEntities;
            this.entities = entities;

            this.totalPages = (totalEntities / requestedEntitiesTake) + 1;

            if (requestedPagesSkip > 0)
                this.actualPageNumber = requestedPagesSkip <= totalPages ? requestedPagesSkip + 1 : totalPages;
            else
                this.actualPageNumber = 1;
        }

        public int RequestedEntitiesSkip
        {
            get { return requestedEntitiesSkip; }
        }

        public int RequestedEntitiesTake
        {
            get { return requestedEntitiesTake; }
        }

        public int RequestedPagesSkip
        {
            get { return requestedPagesSkip; }
        }

        public int ActualPageNumber
        {
            get { return actualPageNumber; }
        }

        public int TotalPages
        {
            get { return totalPages; }
        }

        public int TotalEntities
        {
            get { return totalEntities; }
        }

        public IEnumerable<TEntity> Entities
        {
            get { return entities; }
        }
    }
}