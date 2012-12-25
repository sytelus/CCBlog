using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBlog.Model.Contracts
{
    public interface ISeries
    {
        int SeriesId { get; }
        string Name { get; set; }
        string Title { get; set; }
        string Abstract { get; set; }
        EntityStatus Status { get; set; }
    }
}
