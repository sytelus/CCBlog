using CCBlog.Models;

namespace CCBlog.Repository
{
    interface IRepository
    {
        IUsers Users { get; }
        IRoles Roles { get; }
    }
}
