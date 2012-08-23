using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBlog.Controllers
{
    public partial class UserController
    {
        public class LoginModel
        {
            public string openid_identifier { get; set; }
            public string ReturnUrl { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}