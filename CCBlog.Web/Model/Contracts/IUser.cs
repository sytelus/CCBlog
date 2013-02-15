namespace CCBlog.Model.Contracts
{
    public interface IUser
    {
        int UserId { get; }
        string ClaimedIdentifier { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        string Nickname { get; set; }
        IRole Role { get; set; }
        string GetfriendlyName();
    }
}