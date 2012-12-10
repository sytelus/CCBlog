using System;
using System.Collections.Generic;

namespace CCBlog.Repository
{
    public enum EntityChangeType
    {
        New,
        Updated,
        Deleted
    }

    public interface IRepository : IDisposable
    {
        //Read operations
        IEnumerable<T> GetEntities<T>();

        //Write operations
        void RecordEntityChange<T>(T changedEntity, EntityChangeType changeType);
        void RecordEntityChange<T>(T changedEntity, EntityChangeType changeType, string[] updatedPropertyNames);
        void SaveEntityChanges();
    }
}
