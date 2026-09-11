using Microsoft.Web.WebPages.OAuth;
using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Microsoft.AspNet.Identity;
namespace MVCDIS.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/
        public ActionResult Index(string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {

                UsersContext db = new UsersContext();
                UserManager um = new UserManager();
                MembershipContext ct = new MembershipContext();
                string sqlquery = "select * from webpages_UsersInRoles where UserId= ";

                int? up = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().ClientsID;
                int? id = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().UserId;
                List<webpages_UsersInRoles> luir = db.Database.SqlQuery<webpages_UsersInRoles>(sqlquery + id.ToString()).ToList();
                bool flag = false;
                foreach (webpages_UsersInRoles item in luir)
                {
                    if (ct.MembershipRoles.Where(x => x.RoleId == item.RoleId).SingleOrDefault().RoleName == "NoneRegClient")
                    {
                        flag = true;
                        break;
                    }
                }
                if (up == null)
                {
                    if (flag != true)
                    {
                        um.ChangeRole("NoneRegClient", User.Identity.Name, true);
                    }

                    return RedirectToAction("Manage", "Account");
                }
                else
                    if (up != null)
                    {
                        if (flag == true)
                        {
                            um.ChangeRole("NoneRegClient", User.Identity.Name, false);
                            um.ChangeRole("User", User.Identity.Name, true);
                        }

                    }

            }
            if (!CheckLicense.Check())
            {
                 WebSecurity.Logout();

           
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult RegClient()
        {


            return View();
        }
        //[Authorize (Roles="Fox")]





    }
}