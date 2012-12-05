using System.Collections.Generic;

namespace CCBlog.Repository
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public bool IsAdmin()
        {
            return this.Name == "Administrator";
        }
    }
}