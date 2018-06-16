using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IShop.Models
{
    public class UserOrManager : AuthorizeAttribute
    {
        public UserOrManager()
        {
            Roles = "user, manager";
        }
    }
}