using System.ComponentModel.DataAnnotations;

namespace CCBlog.Repository
{
    public class User
    {
        public int UserId { get; set; }
        [MaxLength(1024)][Required]
        public string ClaimedIdentifier { get; set; }
        [MaxLength(1024)]
        public string FullName { get; set; }
        [MaxLength(1024)]
        public string Email { get; set; }
        [MaxLength(1024)]
        public string Nickname { get; set; }
        public bool IsPostAuthor { get; set; }
        public int? RoleId { get; set; }

        //Navigation properties exists only because they are required by entity framework, do not use it in code
        public virtual Repository.Role Role { get; set; }
    }
}