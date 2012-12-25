using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBlog.Model.Contracts
{
    [Flags]
    public enum EntityStatus :int
    {
        Inactive = 0,
        Active = 1,
        Draft = 2,
        All = 0xFFFFFFF    //One less F then 8 so uint conversion is not needed
    }

    public interface IPost
    {
        int PostId { get; }
        string UrlFriendlyName { get; set; }
        string Title { get; set; }
        string Abstract { get; set; }
        string Body { get; set; }
        DateTimeOffset CreateDate { get; }
        DateTimeOffset ModifyDate { get; }
        DateTimeOffset AuthorCreateDate { get; set; }
        DateTimeOffset AuthorModifyDate { get; set; }
        IUser CreatedByUser { get; set; }
        IUser ModifiedByUser { get; set; }
        EntityStatus Status { get; set; }
        int RevisionCount { get; }
        int? SeriesId { get; set; }
        int? SeriesPart { get; set; }
        int? DisplayOrder { get; set; }
        string PageLayoutName { get; set; }

        ISeries Series { get; set; }
        IList<ITag> Tags { get; set; }
    }
}
