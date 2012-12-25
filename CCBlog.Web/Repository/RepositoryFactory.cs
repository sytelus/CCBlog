using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Repository
{
    public static class RepositoryFactory
    {
        //Caller assumes the responsibility to dispose when done
        public static IRepository Get()
        {
            return new PetaPocoSqlServer.Repository();
        }
    }
}