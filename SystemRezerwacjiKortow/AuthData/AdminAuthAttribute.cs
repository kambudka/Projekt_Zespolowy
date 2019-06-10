using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;

namespace SystemRezerwacjiKortow.AuthData
{
    public class AdminAuthAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string CurrentUserEmail = HttpContext.Current.User.Identity.Name.ToString();
            if (SqlUser.GetUserRole(CurrentUserEmail)=="administrator")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}