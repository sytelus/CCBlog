using System.ComponentModel.DataAnnotations;

namespace CCBlog.Repository
{
    public class User
    {
        public int UserId { get; set; }
        [MaxLength(1024)]
        [Required]
        public string ClaimedIdentifier { get; set; }
        [MaxLength(1024)]
        public string FullName { get; set; }
        [MaxLength(1024)]
        public string Email { get; set; }
        [MaxLength(1024)]
        public string Nickname { get; set; }
        public bool IsPostAuthor { get; set; }
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }

        internal string GetfriendlyName()
        {
            var friendlyName = this.FullName ?? this.Nickname ?? this.Email ?? this.ClaimedIdentifier ?? string.Empty;
            if (this.Role != null && this.Role.IsAdmin())
                friendlyName += " [Admin]";

            return friendlyName;
        }
    }
}