using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace CCBlog.Repository.EntityRepository
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        public void AttachAsModified<T>(T entity, bool markAllPropertiesDirty, params string[] modifiedPropertyNames) where T:class
        {
            var entry = this.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.Set<T>().Attach(entity);

                if (modifiedPropertyNames != null && modifiedPropertyNames.Length > 0)
                {
                    foreach (var modifiedPropertyName in modifiedPropertyNames)
                        entry.Property(modifiedPropertyName).IsModified = true;
                }
            }

            if (markAllPropertiesDirty)
                entry.State = EntityState.Modified;
        }

        public bool IsDetached(object entity)
        {
            return this.Entry(entity).State == EntityState.Detached;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}