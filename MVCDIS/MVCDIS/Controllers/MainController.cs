using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDIS.Models;
using WebMatrix.WebData;
using System.Web.Security;
using MVCDIS.Filters;

namespace MVCDIS.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        //
        // GET: /SimpleMembershipAdministration/Main/


        [CustomAuthorize("Access")]

        public ActionResult Index()
        {

            return View();
        }

        private MembershipContext db = new MembershipContext();
        [CustomAuthorize("Interface")]

        public ActionResult Action()
        {

            List<MVCDIS.Models.LRoles> roles = new List<MVCDIS.Models.LRoles>();

            foreach (var rolitem in db.MembershipRoles.ToList<MVCDIS.Models.Roles>())
            {
                ActionList list = new ActionList();
                List<ActionList> lista = new List<ActionList>();
                foreach (var item in db._ActionByRole.Where(x => x.RoleID == rolitem.RoleId).ToList())
                {

                    list = (from c in db._ActionList.ToList()
                            where c.ID == item.ActionID
                            select c).SingleOrDefault<ActionList>();
                    lista.Add(list);

                }
                roles.Add(new MVCDIS.Models.LRoles { ActionAttending = lista, RoleId = rolitem.RoleId, RoleName = rolitem.RoleName });

            }


            return View(roles);
        }
        [CustomAuthorize("Interface")]
        public ActionResult ActionEdit(int id)
        {


            List<AdditionalServicesCompetenceModel> Actions = new List<AdditionalServicesCompetenceModel>();
            List<ActionByRole> ActionByRole = db._ActionByRole.Where(x => x.RoleID == id).ToList();



            var LN = db.MembershipRoles.FirstOrDefault(x => x.RoleId == id).RoleName.ToString();


            List<ActionList> lss = db._ActionList.ToList();
            ViewBag.LevelName = LN.ToString();
            //ActionList actitem in db._ActionList.ToList()
            foreach (ActionList fox in db._ActionList.ToList())
            {
                bool flag = false;
                foreach (ActionByRole actroleitem in ActionByRole)
                {

                    if (actroleitem.ActionID == fox.ID)
                    {

                        flag = true;

                        break;
                    }
                    else { flag = false; }


                }

                Actions.Add(new AdditionalServicesCompetenceModel { ID = fox.ID, IsSelected = flag, Name = fox.Action, Description = fox.Description });


            }

            Session["RoleId"] = id;
            return View(Actions);
        }

        [CustomAuthorize("Interface")]


        [HttpPost]
        public ActionResult ActionEdit(List<AdditionalServicesCompetenceModel> item)
        {



            int roleid = Convert.ToInt32(Session["RoleId"].ToString());


            List<ActionByRole> lar = null;
            List<AdditionalServicesCompetenceModel> Selected = item.Where(m => m.IsSelected).ToList<AdditionalServicesCompetenceModel>();
            if (Selected != null && Selected.Any())
            {





                using (MembershipContext db = new MembershipContext())
                {


                    lar = (from k in db._ActionByRole
                           where k.RoleID == roleid
                           select k).ToList<ActionByRole>();

                    foreach (ActionByRole j in lar)
                    {
                        db._ActionByRole.Remove(j);
                    }

                    db.SaveChanges();

                    foreach (AdditionalServicesCompetenceModel k in Selected)
                    {
                        db._ActionByRole.Add(new ActionByRole { ActionID = k.ID, RoleID = roleid });

                    }
                    db.SaveChanges();
                }




            }
            else
            {
                using (MembershipContext db = new MembershipContext())
                {

                    lar = (from k in db._ActionByRole
                           where k.RoleID == roleid
                           select k).ToList<ActionByRole>();

                    foreach (ActionByRole j in lar)
                    {
                        db._ActionByRole.Remove(j);
                    }

                    db.SaveChanges();


                }

            }

            return RedirectToAction("Action");
        }
        [CustomAuthorize("Access")]
        public JsonResult IndexHandler(DataTableModel param)
        {
            string sortDirection = Request["sSortDir_0"]; // 
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);


            CompetenceContext dbc = new CompetenceContext();


            UserManager userManager = new UserManager();

            List<UserProfile> userList = new List<UserProfile>();

            int totalRecords = 0;

            if (param.sSearch != null)
            {
                userList = userManager.FindUsers(param.sSearch, param.iDisplayStart / param.iDisplayLength, param.iDisplayLength, sortDirection, sortColumnIndex, out totalRecords).ToList();
                totalRecords += totalRecords;
            }
            else
            {
                int recordCount = 0;

                var pagedUsers = userManager.LoadAllUsers(param.iDisplayStart / param.iDisplayLength, param.iDisplayLength, sortDirection, sortColumnIndex, out recordCount).ToList();
                pagedUsers.ForEach(u =>
                {
                    if (userList.Count(c => c.UserName == u.UserName) == 0)
                    {
                        userList.Add(u);
                    }
                });
                totalRecords += recordCount;



            }



            var result = (from c in userList
                          select new RegInfo()
                          {
                              UserName = c.UserName,
                              Email = c.Email,
                              Fio = c.ClientsID != null ? dbc._Clients.Where(x => x.ID == c.ClientsID).FirstOrDefault().ClientsInfo.FIO : "",
                              CreateDate = WebSecurity.GetCreateDate(c.UserName).ToShortDateString(),
                              LastPasswordFailureDate = WebSecurity.GetLastPasswordFailureDate(c.UserName).ToShortDateString(),
                              PasswordChangedDate = WebSecurity.GetPasswordChangedDate(c.UserName).ToShortDateString()
                          });




            var jsonObject = new
            {
                sEcho = param.sEcho,
                iTotalRecords = param.iDisplayLength,
                iTotalDisplayRecords = totalRecords,
                aaData = result
            };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize("Access")]
        public PartialViewResult GetRowDetail(string login, string email)
        {
            int i = 0;

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();
            bool[] flag = new bool[ViewBag.Roles.Length];
            foreach (var item in ViewBag.Roles)
            {
                if (System.Web.Security.Roles.IsUserInRole(login, item))
                {
                    flag[i] = true;
                }
                else
                {
                    flag[i] = false;
                }

                i++;
            }

            return PartialView("UserDetails", new UserDetails()
                {
                    UserName = login,
                    Email = email,
                    Flag = flag
                });

        }


        [CustomAuthorize("Access")]
        [InitializeSimpleMembership]
        public ActionResult Users()
        {
            return View();
        }






        [HttpPost]
        [CustomAuthorize("Access")]
        public ActionResult UserToRole(string id, bool? ischecked)
        {
            string[] words = id.Split(' ');
            if (ischecked.HasValue && ischecked.Value)
            {
                System.Web.Security.Roles.AddUserToRole(words[0], words[1]);
            }
            else
            {
                System.Web.Security.Roles.RemoveUserFromRole(words[0], words[1]);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}