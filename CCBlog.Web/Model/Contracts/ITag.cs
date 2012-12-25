namespace CCBlog.Model.Contracts
{
    public interface ITag
    {
        int TagId { get; }
        string Name { get; set; }
        string Title { get; set; }
        string HelpText { get; set; }
        bool IsVisible { get; set; }
        bool IsNavigation { get; set; }
        int? DisplayOrder { get; set; }
    }
}