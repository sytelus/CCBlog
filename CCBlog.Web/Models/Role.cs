using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCBlog.Models
{
    public class Role
    {
        public int? RoleID { get; set; }
        public string Name { get; set; }

        public string Comment { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}