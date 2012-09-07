using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CCBlog.Models;

namespace CCBlog.Repository.EntityRepository
{
    public class AppDatabaseCreateIfNotExists : CreateDatabaseIfNotExists<MainDbContext>
    {
        protected override void Seed(MainDbContext context)
        {
            context.Roles.Add(new Role()
                                  {
                                      Name = "Administrator"
                                  });
        } 
    }
}