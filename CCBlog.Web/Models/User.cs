using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace CCBlog.Models
{
    public class User
    {
        public int? UserId { get; set; }
        public string ClaimedIdentifier { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public ICollection<Role> Roles { get; set; }

        public string Comment { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}