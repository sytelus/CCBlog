using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBlog.Model.Contracts
{
    public interface IEntityPage<out TEntity>
    {
        int RequestedEntitiesSkip { get; }
        int RequestedEntitiesTake { get; }
        int RequestedPagesSkip { get; }
        int ActualPageNumber { get; }
        int TotalPages { get; }
        int TotalEntities { get; }
        IEnumerable<TEntity> Entities { get; }
    }
}
