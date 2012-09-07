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

        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }

        internal string GetfriendlyName()
        {
            return this.FullName ?? this.Nickname ?? this.Email ?? this.ClaimedIdentifier ?? string.Empty;
        }
    }
}