namespace CCBlog.Model.Contracts
{
    public interface IRole
    {
        int RoleId { get; }
        string Name { get; }

        bool IsAdmin();
        bool IsAuthor();
    }
}