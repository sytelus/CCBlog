using CCBlog.Infrastructure;
using CCBlog.Model.Contracts;

namespace CCBlog.Model.Poco
{
    public partial class User : IUser
    {
        public IRole Role { get; set; }

        public string GetfriendlyName()
        {
            var friendlyName = this.FullName ?? this.Nickname ?? this.Email ?? this.ClaimedIdentifier ?? string.Empty;
            if (this.Role != null)
            {
                if (this.Role.IsAdmin())
                    friendlyName += " [Admin]";
            }

            return friendlyName;
        }
    }
}