using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCBlog.Model.Contracts;

namespace CCBlog.Model.Poco
{
    public partial class Post : IPost
    {
        public IUser CreatedByUser { get; set; }
        public IUser ModifiedByUser { get; set; }

        public ISeries Series { get; set; }
        public IList<ITag> Tags { get; set; }
    }
}