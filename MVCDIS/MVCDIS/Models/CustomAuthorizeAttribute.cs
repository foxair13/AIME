using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace MVCDIS.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public static bool CheckRoles(string actionselector)
        {
            if (CheckLicense.Check())
            {

                bool flag = false;
                string[] words = GetRoles(actionselector).Split(',');
                foreach (var item in System.Web.Security.Roles.GetRolesForUser(WebSecurity.CurrentUserName))
                {
                    var results = from t in words where t.Trim() == item.Trim() select t;

                    if (results.Count() == 0)
                    {
                        flag = false;
                        break;

                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            else 
            {
                return false; 
            }
        }

        private string selector = "";
        public CustomAuthorizeAttribute(string actionselector)
        {
            this.selector = actionselector;
         
        }

        private static string GetRoles(string actionselector)
        {

            MembershipContext db = new MembershipContext();
            List<ActionByRole> list = (from k in db._ActionByRole
                                       where k.ActionList.Action == actionselector
                                       select k).ToList<ActionByRole>();
            String str = "";
            int i = 0;
            foreach (ActionByRole item in list)
            {
                if (i==0)
                {
                    str += item.Roles.RoleName ;
                }
                else
                {
                    str += string.Concat(", ", item.Roles.RoleName); ;
                }
                i++;
              
            }

            return str;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


           

                if (CheckRoles(this.selector))
                {
                    return true;
                }
                else
                {
                    return false;
                }
         
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}