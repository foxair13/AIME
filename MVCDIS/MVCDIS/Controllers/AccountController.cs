using Microsoft.Owin.Security;
using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;
using System.Web.Mvc;
using MVCDIS.Filters;
using WebMatrix.WebData;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using DotNetOpenAuth.AspNet;
using System.Transactions;
using Postal;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
namespace MVCDIS.Controllers
{


    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Key(Keys model)
        {
            String akmer = "";
            bool flag = false;
            Keys _key = new Keys();
            TrialMaker t = new TrialMaker("TMTest1", Server.MapPath("~/app_data") + "\\RegFile.reg",
            Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\TMSetp.dbf",
            "",
            5, 10, "675");
            byte[] MyOwnKey = { 97, 5, 3, 5, 84, 21, 7, 63,
            4, 54, 87, 56, 123, 10, 3, 62,
            7, 9, 20, 36, 37, 21, 101, 57};
            t.TripleDESKey = MyOwnKey;
            akmer = t.GetPassword();
            CheckVirtual cv = new CheckVirtual();
            int cvi = cv.isVirtualMachine();
            if (model.UserKey == akmer && cvi == 0)
            {
                t.GetAllResult(true);
                flag = true;
            }
            ViewBag.flag = flag; 
            return PartialView("ShowResult");
           

        }
        [AllowAnonymous]
        public ActionResult Key()
        {
            String MashineCode = "";
            Keys _key = new Keys();
            TrialMaker t = new TrialMaker("TMTest1", Server.MapPath("~/app_data") + "\\RegFile.reg",
            Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\TMSetp.dbf",
            "",
            5, 10, "675");
            byte[] MyOwnKey = { 97, 5, 3, 5, 84, 21, 7, 63,
            4, 54, 87, 56, 123, 10, 3, 62,
            7, 9, 20, 36, 37, 21, 101, 57};
            t.TripleDESKey = MyOwnKey;
            TrialMaker.RunTypes RT = t.test();
            if (RT == TrialMaker.RunTypes.Expired)
            {
                MashineCode = t.GetMashineCode();
            }
            _key.MashineID = MashineCode;
            return View(_key);
        }


        [AllowAnonymous]
        public ActionResult ShowResult(bool flag)
        {
            if (flag)
            {
                ViewBag.Data = "Вы успешно зарегистрировали систему";
            }
            else
            {
                ViewBag.Data = "Серийный номер указан неверно, регистрация не выполнена";
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonRecover(LostPasswordModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {

                using (var context = new UsersContext())
                {
                    var foundUserNames = (from u in context.UserProfiles
                                          where u.Email == model.Email
                                          select u.UserName);
                    if (foundUserNames != null)
                    {
                        try
                        {
                            foreach (var item in foundUserNames)
                            {
                                // Generae password token that will be used in the email link to authenticate user
                                if (WebSecurity.IsConfirmed(item))
                                {
                                    var confirmationToken = WebSecurity.GeneratePasswordResetToken(item);
                                    // Generate the html link sent via email

                                    dynamic email = new Email("ResetEmail");
                                    email.To = model.Email;
                                    email.UserName = item;
                                    email.ConfirmationToken = confirmationToken;
                                    email.Send();
                                }
                            }
                            return Json(new { success = true, redirect = 1, datatext = "На Ваш e-mail отправлено письмо для активации" });

                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "Ошибка отправки : " + e.Message);
                        }
                    }
                    else // Email not found
                    {
                        ModelState.AddModelError("", "Не найден пользователь с таким Email.");
                    }
                }
            }
            return Json(new { errors = GetErrorsFromModelState() });
        }
        [CustomAuthorize("Reports")]
        public bool CheckReport()
        {
            bool flag = true;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                Clients user = new Clients();
                using (var dbc = new UsersContext())
                {
                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                }
                int act_id = (int)user.DefaultActivityId;
                int wrk_id = db.tableIasaUserActivity.Where(x => x.UserId == uid && x.ActivityId == user.DefaultActivityId).FirstOrDefault().DefaultWorkId;
                iasaActivities diasact = db.tableIasaActivities.Where(x => x.ActivityId == act_id).FirstOrDefault();
                Int64 eee = Convert.ToInt64(String.Concat(DateTimeToInt((DateTime)diasact.BeginDate).ToString(), uid.ToString(), act_id.ToString(), wrk_id.ToString()));
                ReportsList RL = db._ReportsList.Where(x => x.GroupId == eee).FirstOrDefault();
                if (RL == null)
                {
                    flag = true;
                }
                else
                    if (RL.Confirmat)
                    {
                        flag = false;
                    }
            }

            return flag;
        }
        public static int DateTimeToInt(DateTime theDate)
        {
            return (int)(theDate.Date - new DateTime(1900, 1, 1)).TotalDays + 2;
        }
        public ActionResult userActivities()
        {
            UserActivities ual = new UserActivities();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                var ua = db.tableIasaUserActivity.Include(x => x.iasaActivities).Where(a => a.UserId == uid).ToList();
                int da = 0;
                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    try
                    {
                        da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    }
                    catch (Exception)
                    {


                    }



                }


                List<iasaActivities> ia = new List<iasaActivities>();
                try
                {
                    foreach (var item in ua)
                    {

                        int ai = item.iasaActivities.ActivityId;
                        string an = item.iasaActivities.ActivityName;
                        ia.Add(new iasaActivities() { ActivityId = ai, ActivityName = an });
                    }
                    ual.iasaActivities = ia;
                    ual.DefaultActivityId = da;
                }
                catch (Exception) { }
            }
            return PartialView("_UserActivities", ual);
        }

        public ActionResult userWorks()
        {
            UserWorks uwl = new UserWorks();
            int da = 0;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    try
                    {
                        da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    }
                    catch (Exception)
                    {


                    }



                }
                int dwi = 0;
                try
                {
                    dwi = db.tableIasaUserActivity.Where(a => a.UserId == uid).Where(b => b.ActivityId == da).FirstOrDefault().DefaultWorkId;
                }
                catch (Exception)
                {
                }
                int dw = 0;
                try
                {
                    dw = db.tableIasaWorks.Where(w => w.WorkId == dwi).FirstOrDefault().WorkId;
                }
                catch (Exception)
                {

                }

                var uw = (from a in db.tableIasaWorks
                          where (
                          from b in db.tableIasaWorkByUserActivity
                          where b.UserId == uid && b.ActivityId == da
                          select b.WorkId).Contains(a.WorkId)
                          select a).ToList();

                //var uw = (from w in db.tableIasaWorks
                //          where
                //          (from wba in db.tableIasaWorkByUserActivity
                //           where wba.ActivityId == da &&
                //           (from uww in db.tableIasaUserWork
                //            where uww.UserId == uid select uww.WorkId).Contains(wba.WorkId)
                //           select wba.WorkId).Contains(w.WorkId)
                //          select w).ToList();


                List<iasaWorks> iw = new List<iasaWorks>();
                try
                {
                    foreach (var item in uw)
                    {

                        int wi = item.WorkId;
                        string wn = item.WorkName;
                        iw.Add(new iasaWorks() { WorkId = wi, WorkName = wn });
                    }
                    uwl.iasaWorks = iw;
                    uwl.DefaultWorkId = dw;
                }
                catch (Exception) { }
            }
            ViewBag.DefaultActivityId = da;
            return PartialView("_UserWorks", uwl);
        }
        [CustomAuthorize("Reports")]
        public ActionResult updateReportAdd()
        {
            IASAModel iasm = new IASAModel();
            int da = 0;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }


                freeActivity ua = new freeActivity();
                List<iasaActivities> ual = new List<iasaActivities>();

                ual = ua.GetNotFree(uid);
                iasm.userActivities = ual;
                iasm.defaultUserActivity = da;
                GetUserWorks uw = new GetUserWorks();
                List<iasaWorks> uwl = new List<iasaWorks>();
                uwl = uw.GetNotFree(uid);
                iasm.userWorks = uwl;
                iasm.defaultUserWork = 0;
                try
                {
                    iasm.defaultUserWork = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).FirstOrDefault().DefaultWorkId;
                }
                catch (Exception)
                {

                }
                iasm.worktypes = db.tableIasaWorktypes.ToList();
            }
            return PartialView("_ReportAdd", iasm);
        }

        [CustomAuthorize("Reports")]
        public ActionResult GetBranch(int pid)
        {
            Nodes nodes = new Nodes();
            int activity = 0;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    activity = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }
                if (pid != 0)
                {
                    int? p = pid;
                    List<iasaWorktypesTree> wl = new List<iasaWorktypesTree>();
                    do
                    {
                        iasaWorktypesTree w = new iasaWorktypesTree();
                        w = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == p && i.IsDeleted != true).FirstOrDefault();
                        wl.Add(w);
                        p = w.ParentId;
                    } while (p != 0);
                    wl.Reverse();
                    nodes.breadcrumbs = wl;
                }

                Branch gb = new Branch();


                List<iasaWorktypesTree> wt = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid && p.IsDeleted != true).Where(a => a.ActivityId == activity).ToList();
                Dictionary<iasaWorktypesTree, bool?> dc = new Dictionary<iasaWorktypesTree, bool?>();
                foreach (var item in wt)
                {
                    dc.Add(item, gb.isLast(item.WorktypeId));

                }
                nodes.lNodes = dc;
            }
            return PartialView("_TreeBranch", nodes);
        }
        [CustomAuthorize("Reports")]
        public ActionResult updateReportsList()
        {
            IASAModel iasm = new IASAModel();
            int da = 0;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }


                int dw = 0;
                try
                {
                    dw = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).FirstOrDefault().DefaultWorkId;
                }
                catch (Exception)
                {

                }
                iasm.records = db.tableIasaReports.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).Where(w => w.WorkId == dw).Where(d => d.Deleted == false).Include(u => u.iasaUser).Include(w => w.iasaWorktypesTree).Include(t => t.iasaWorks).Include(a => a.iasaActivities).OrderBy(d => d.ReportCreate).ToList();
                iasm.records.Reverse();
                iasm.defaultUserActivity = da;
                iasm.defaultUserWork = dw;
            }
            return PartialView("_ReportsList", iasm);
        }



        public bool UpdateActivities(Dictionary<string, bool> acts)
        {
            //AddActivity aa = new AddActivity();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;
                Clients cl = new Clients();
                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    try
                    {
                        da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    }
                    catch (Exception)
                    {


                    }

                    cl = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();

                }
                foreach (var item in acts)
                {
                    //List<iasaUserActivity> result = db.tableIasaUserActivity.Where(r => r.UserId == uid && r => r.ActivityId == Convert.ToInt32(item.Key)).ToList();
                    int key = Convert.ToInt32(item.Key);
                    List<iasaUserActivity> result = (from x in db.tableIasaUserActivity
                                                     where x.UserId == uid &&
                                                     x.ActivityId == key
                                                     select x).ToList();

                    if (result.Count == 0 && item.Value)
                    {
                        db.tableIasaUserActivity.Add(new iasaUserActivity { UserId = uid, ActivityId = Convert.ToInt32(item.Key), DefaultWorkId = 0 });
                    }
                    if (result.Count > 0 && !item.Value)
                    {
                        foreach (var act in result)
                        {
                            if (act.ActivityId == da)
                            {
                                cl.DefaultActivityId = 0;
                            }
                            db.tableIasaUserActivity.Remove(act);
                        }
                    }
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }



        public bool SetDefaultWork(int work)
        {
            Clients user = new Clients();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }

                iasaUserActivity userActivity = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).FirstOrDefault();
                userActivity.DefaultWorkId = work;
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }
        [CustomAuthorize("Reports")]
        public ActionResult GetCurrentBranch(int id)
        {
            Nodes nodes = new Nodes();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int activity = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    activity = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }


                int? pid = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == id).FirstOrDefault().ParentId;
                int? pr = pid;
                List<iasaWorktypesTree> wl = new List<iasaWorktypesTree>();
                do
                {
                    iasaWorktypesTree w = new iasaWorktypesTree();
                    w = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == pr).FirstOrDefault();
                    wl.Add(w);
                    pr = w.ParentId;
                } while (pr != 0);
                wl.Reverse();
                nodes.breadcrumbs = wl;


                Branch gb = new Branch();


                List<iasaWorktypesTree> wt = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid).Where(a => a.ActivityId == activity).ToList();
                Dictionary<iasaWorktypesTree, bool?> dc = new Dictionary<iasaWorktypesTree, bool?>();
                foreach (var item in wt)
                {
                    dc.Add(item, gb.isLast(item.WorktypeId));

                }
                nodes.lNodes = dc;
                nodes.currentId = id;
            }
            return PartialView("_TreeBranchEdit", nodes);
        }



        public ActionResult GetMinimax(int selwrkt)
        {
            using (var db = new CompetenceContext())
            {

                iasaWorktypesTree iwt = db.tableIasaWorktypesTree.Where(x => x.WorktypeId == selwrkt).FirstOrDefault();

                return PartialView("_GetMinimax", iwt);
            }
        }
        [CustomAuthorize("Reports")]
        public ActionResult updateReportInfo(int id)
        {
            ReportEdit report = new ReportEdit();
            using (var db = new CompetenceContext())
            {


                report.Report = db.tableIasaReports.Where(r => r.ReportId == id).Include(u => u.iasaUser).Include(w => w.iasaWorktypesTree).Include(w => w.iasaWorks).Include(a => a.iasaActivities).FirstOrDefault();

            }
            return PartialView("_ReportInfo", report);
        }
        [CustomAuthorize("Reports")]
        public bool removeReport(int id)
        {
            using (var db = new CompetenceContext())
            {
                iasaReports report = db.tableIasaReports.Where(r => r.ReportId == id).FirstOrDefault();
                report.Deleted = true;
                db.SaveChanges();
            }
            return true;
        }
        [CustomAuthorize("Reports")]
        public bool editReport(int grade, int activity, int work, int worktype, string text, string begin, string end, int id = 0)
        {
            ReportEdit report = new ReportEdit();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);


                if (id == 0)
                {

                    db.tableIasaReports.Add(new iasaReports { UserId = uid, WorktypeId = worktype, ReportText = text, ReportBegin = DateTime.Parse(begin), ReportEnd = DateTime.Parse(end), WorkId = work, ActivityId = activity, ReportCreate = DateTime.Now, Grade = grade });
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    iasaReports current = db.tableIasaReports.Where(r => r.ReportId == id).FirstOrDefault();
                    current.ActivityId = activity;
                    current.WorkId = work;
                    current.WorktypeId = worktype;
                    current.ReportText = text;
                    current.Grade = grade;
                    current.ReportBegin = DateTime.Parse(begin);
                    current.ReportEnd = DateTime.Parse(end);
                    db.SaveChanges();
                    return true;
                }
            }

        }

        public PartialViewResult SetDefaultActivity(int activity)
        {
            Clients user = new Clients();
            iasaActivities iasact = new iasaActivities();
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            using (var db = new CompetenceContext())
            {



                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    user = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();


                }


                user.DefaultActivityId = activity;
                try
                {
                    db.SaveChanges();

                }
                catch (Exception)
                {


                }

            }
            using (var db = new CompetenceContext())
            {
                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    user = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                    iasact = user.iasaActivities;

                }
            }

            return PartialView("_UpdAct", iasact);
        }
        [CustomAuthorize("Reports")]
        public ActionResult updateReportEdit(int id)
        {
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }
                ReportEdit report = new ReportEdit();
                report.Report = db.tableIasaReports.Where(r => r.ReportId == id).FirstOrDefault();

                freeActivity ua = new freeActivity();
                List<iasaActivities> ual = new List<iasaActivities>();
                ual = ua.GetNotFree(uid);
                report.userActivities = ual;
                GetUserWorks uw = new GetUserWorks();
                List<iasaWorks> uwl = new List<iasaWorks>();
                uwl = uw.GetNotFree(uid);
                report.userWorks = uwl;
                report.worktypes = db.tableIasaWorktypesTree.ToList();
                return PartialView("_ReportEdit", report);
            }
        }
        public bool UpdateWorks(Dictionary<string, bool> works)
        {
            //AddActivity aa = new AddActivity();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }
                iasaUserActivity dw = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).FirstOrDefault();
                foreach (var item in works)
                {
                    //List<iasaUserActivity> result = db.tableIasaUserActivity.Where(r => r.UserId == uid && r => r.ActivityId == Convert.ToInt32(item.Key)).ToList();
                    int key = Convert.ToInt32(item.Key);

                    List<iasaUserWork> result = (from x in db.tableIasaUserWork
                                                 where x.UserId == uid && x.WorkId == key &&
                                                 (from y in db.tableIasaWorkByUserActivity
                                                  where y.ActivityId == da && y.UserId == uid
                                                  select y.WorkId).Contains(x.WorkId)
                                                 select x).ToList();

                    if (result.Count == 0 && item.Value)
                    {
                        db.tableIasaUserWork.Add(new iasaUserWork { UserId = uid, WorkId = key });
                        db.tableIasaWorkByUserActivity.Add(new iasaWorkByUserActivity { ActivityId = da, WorkId = key, UserId = uid });
                    }
                    if (result.Count > 0 && !item.Value)
                    {
                        foreach (var work in result)
                        {
                            if (work.WorkId == dw.DefaultWorkId)
                            {
                                dw.DefaultWorkId = 0;
                            }
                            iasaWorkByUserActivity range = db.tableIasaWorkByUserActivity.Where(u => u.UserId == uid).Where(u => u.ActivityId == da).Where(w => w.WorkId == work.WorkId).FirstOrDefault();
                            db.tableIasaWorkByUserActivity.Remove(range);
                            db.tableIasaUserWork.Remove(work);

                        }
                    }
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }
        public ActionResult allActivities()
        {
            allActivity aa = new allActivity();
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            Dictionary<iasaActivities, bool> act = aa.GetActivities(uid);
            AllUserActivities ua = new AllUserActivities();
            ua.iasaActivities = act;
            return PartialView("_SelectActivities", ua);
        }

        public ActionResult allWorks()
        {
            allActivity aa = new allActivity();
            AllWork aw = new AllWork();
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            Dictionary<iasaWorks, bool> work = aw.GetWorks(uid);
            AllUserWorks uw = new AllUserWorks();
            uw.iasaWorks = work;
            return PartialView("_SelectWorks", uw);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    return Json(new { success = true, redirect = returnUrl });
                }
                else
                {
                    ModelState.AddModelError("", "Имя пользователя или пароль введены неверно!");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }
        public bool AdvancedM(Boolean Sel)
        {
            using (var db = new UsersContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                up.State = Sel;



                db.SaveChanges();
            }

            return true;


        }


        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/JsonRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult JsonRegister(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {Email=model.Email});
                    //WebSecurity.Login(model.UserName, model.Password, persistCookie: false);
                    //FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);

                    string confirmationToken =
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email }, true);
                    dynamic email = new Email("RegEmail");
                    email.To = model.Email;
                    email.UserName = model.UserName;
                    email.ConfirmationToken = confirmationToken;
                    email.Send();
                    string str = Url.Action("RegisterStepTwo");
                    return Json(new { success = true, redirect = 0, datatext = "На Ваш e-mail отправлено письмо для активации" });
                    //return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string Id)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = Id;
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
                if (resetResponse)
                {
                    return Json(new { success = true, redirect = 2, datatext = "Пароль успешно изменен" });
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так!");
                }
            }
            return Json(new { errors = GetErrorsFromModelState() });
        }

        [AllowAnonymous]
        public ActionResult RegisterConfirmation(string Id)
        {
            if (WebSecurity.ConfirmAccount(Id))
            {
                return RedirectToAction("ConfirmationSuccess", new { _id = Id });
            }
            return RedirectToAction("ConfirmationFailure");
        }

        [AllowAnonymous]
        public ActionResult ConfirmationSuccess(string _id)
        {
            ViewBag.Data = _id;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ConfirmationSuccess(string Id, bool flag)
        {
            if (flag)
            {
                if (WebSecurity.ConfirmAccount(Id))
                {
                    using (var dbm = new MembershipContext())
                    {
                        // Use ConfirmationToken to figure out UserId, then use that to get UserName.
                        int userId = dbm.MembershipProfiles.Single(m => m.ConfirmationToken == Id).UserId;

                        using (var dbu = new UsersContext())
                        {
                            string userName = dbu.UserProfiles.Single(u => u.UserId == userId).UserName;
                            dbu.UserProfiles.Single(u => u.UserId == userId).State = false;
                            dbu.SaveChanges();
                            // Authenticate user.
                            FormsAuthentication.SetAuthCookie(userName, true);
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure()
        {
            return View();
        }
        /// <summary>
        /// Initiate a new todo list for new user
        /// </summary>
        /// <param name="userName"></param>
        private static void InitiateDatabaseForNewUser(string userName)
        {

        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        [HttpPost]
        public ActionResult RemovePhoto()
        {
            using (var db = new UsersContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                Images img = db._Images.Where(x => x.ID == up.ImagesID).SingleOrDefault();
                if (img.ID != 1)
                {
                    db._Images.Remove(img);
                }

                up.ImagesID = 1;
                db.SaveChanges();
            }


            return RedirectToAction("Manage", "Account");
        }
        [CustomAuthorize("Reports")]
        public ActionResult Reports()
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


            return View();
        }
        public ActionResult AddFields(string fio, string email, int jobid, int stepid, int zvanid)
        {
            using (var db = new UsersContext())
            {
                ClientsInfo ci = new ClientsInfo();
                ci.Email = email;
                ci.FIO = fio;
                Clients client = new Clients();
                client.JobTitleID = jobid;








                int uid = WebSecurity.GetUserId(User.Identity.Name);
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID == null)
                {
                    using (var dbc = new CompetenceContext())
                    {


                        dbc._ClientsInfo.Add(ci);
                        dbc.SaveChanges();
                        client.ClientInfoID = ci.ID;
                        client.ZvanieId = zvanid;
                        client.StepenId = stepid;
                        dbc._Clients.Add(client);
                        try
                        {
                            dbc.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            List<string> lstr = new List<string>();
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                lstr.Add("Entity of type" + eve.Entry.Entity.GetType().Name + " in state" + eve.Entry.State + "has the following validation errors:");

                                foreach (var ve in eve.ValidationErrors)
                                {
                                    lstr.Add("- Property:" + ve.PropertyName + " Error:" + ve.ErrorMessage);

                                }
                            }

                        }

                        JobTitle jobtitle = dbc._JobTitle.Where(a => a.ID == jobid).SingleOrDefault();
                        ViewBag.jobtitle = jobtitle.Name;
                        ViewBag.Step = dbc.tableIasaStepen.Where(x => x.StepenId == client.StepenId).Select(x => x.StepenName).FirstOrDefault();

                        ViewBag.Zvan = dbc.tableIasaZvanie.Where(x => x.ZvanieId == client.ZvanieId).Select(x => x.ZvanieName).FirstOrDefault();

                        ProfArea profarea = dbc._ProfArea.Where(a => a.ID == jobtitle.ProfAreaID).SingleOrDefault();
                        ViewBag.ProfArea = profarea.Name;
                        ViewBag.FIO = ci.FIO;
                        ViewBag.email = ci.Email;
                    }
                    up.ClientsID = client.ID;

                    db.SaveChanges();



                }
                else
                {
                    using (var dbc = new CompetenceContext())
                    {
                        int fcid = dbc._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.ClientInfoID).SingleOrDefault();
                        ClientsInfo fci = dbc._ClientsInfo.Where(x => x.ID == fcid).SingleOrDefault();
                        fci.FIO = ci.FIO;
                        fci.Email = ci.Email;

                        dbc.SaveChanges();


                        client.ClientInfoID = fci.ID;
                        Clients fclient = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        fclient.ClientInfoID = fci.ID;
                        fclient.JobTitleID = jobid;
                        fclient.ZvanieId = zvanid;
                        fclient.StepenId = stepid;
                        ViewBag.FIO = fci.FIO;
                        ViewBag.Step = dbc.tableIasaStepen.Where(x => x.StepenId == stepid).Select(x => x.StepenName).FirstOrDefault();
                        ViewBag.Zvan = dbc.tableIasaZvanie.Where(x => x.ZvanieId == zvanid).Select(x => x.ZvanieName).FirstOrDefault();
                        ViewBag.email = fci.Email;
                        dbc.SaveChanges();
                        JobTitle jobtitle = dbc._JobTitle.Where(a => a.ID == jobid).SingleOrDefault();
                        ViewBag.jobtitle = jobtitle.Name;
                        ProfArea profarea = dbc._ProfArea.Where(a => a.ID == jobtitle.ProfAreaID).SingleOrDefault();

                        ViewBag.ProfArea = profarea.Name;







                    }

                }
                //            if (clientid==0)
                //{
                //          up.ClientsID=
                //}


            }
            return PartialView("_AddFields", null);
        }
        public ActionResult GetListDetail(string id)
        {
            int uid = Convert.ToInt32(id);
            int uidu = WebSecurity.GetUserId(User.Identity.Name);
            CompetenceContext ct = new CompetenceContext();
            List<JobTitle> _attributes = ct._JobTitle.Where(a => a.ProfAreaID == uid).ToList<JobTitle>();
            List<SelectListItem> pa = new List<SelectListItem>();
            int jobid = 0;
            using (var db = new UsersContext())
            {
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uidu).SingleOrDefault();

                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        jobid = client.JobTitleID;


                    }

                }

            }
            foreach (JobTitle item in _attributes)
            {
                if (item.ID == jobid)
                {
                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });

                }
                else
                {
                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }

            }
            var model = pa;
            return PartialView("ListDetails", model);
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            if (!CheckLicense.Check())
            {
                WebSecurity.Logout();

           
            }
            List<DisciplineEdit> de = new List<DisciplineEdit>();
            UserProfile up = new UserProfile();

            int uid = WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(uid);
            ViewBag.ReturnUrl = Url.Action("Manage");

            Images img = new Images();
            string email = "";
            string FIO = "";
            bool Manage = false;
            int profid = 0;
            int jobid = 0;

            int stepid = 0;
            int zvanid = 0;
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                img = db._Images.Where(x => x.ID == up.ImagesID).SingleOrDefault();
                email = up.Email;

                if (img == null)
                {



                    img = db._Images.Where(x => x.ID == 1).SingleOrDefault();



                }


                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                        FIO = client.ClientsInfo.FIO;
                        Manage = client.Manager;
                        profid = client.JobTitle.ProfAreaID;
                        jobid = client.JobTitleID;
                        stepid = client.StepenId;
                        zvanid = client.ZvanieId;
                    }

                }

            }
            string imgstring = Convert.ToBase64String(img.Picture);
            imgstring = String.Format("data:{0};base64,{1}", img.mime, imgstring);
            ViewBag.img = imgstring;
            ViewBag.email = email;
            ViewBag.FIO = FIO;
            ViewBag.Manage = Manage;
            using (var db = new CompetenceContext())
            {
                List<SelectListItem> pa = new List<SelectListItem>();
                List<SelectListItem> pa1 = new List<SelectListItem>();
                List<SelectListItem> pa2 = new List<SelectListItem>();
                if (profid == 0)
                {
                    pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано", Selected = true });
                }
                else
                {
                    pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
                }

                foreach (ProfArea item in db._ProfArea.ToList())
                {
                    if (item.ID == profid)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                        ViewBag.ProfArea = item.Name;
                        using (var dbc = new UsersContext())
                        {


                            Clients client = db._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                            JobTitle jobtitle = db._JobTitle.Where(a => a.ProfAreaID == profid && a.ID == jobid).SingleOrDefault();
                            ViewBag.StepenName = client.iasaStepen.StepenName;
                            ViewBag.ZvanieName = client.iasaZvanie.ZvanieName;
                            ViewBag.jobtitle = jobtitle.Name;
                        }



                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, });

                    }


                }
                ViewBag.Prof = pa;

                foreach (iasaStepen item in db.tableIasaStepen.ToList())
                {
                    if (item.StepenId == stepid)
                    {
                        pa1.Add(new SelectListItem { Value = item.StepenId.ToString(), Text = item.StepenName, Selected = true });

                    }
                    else
                    {
                        pa1.Add(new SelectListItem { Value = item.StepenId.ToString(), Text = item.StepenName });

                    }


                }
                ViewBag.Step = pa1;
                foreach (iasaZvanie item in db.tableIasaZvanie.ToList())
                {
                    if (item.ZvanieId == zvanid)
                    {
                        pa2.Add(new SelectListItem { Value = item.ZvanieId.ToString(), Text = item.ZvanieName, Selected = true });

                    }
                    else
                    {
                        pa2.Add(new SelectListItem { Value = item.ZvanieId.ToString(), Text = item.ZvanieName });

                    }


                }
                ViewBag.Zvan = pa2;



                if (up.ClientsID != null)
                {
                    de = (from x in db._Disciplines
                          from m in db._DisciplineClient.Where(t => t.ClientID == up.ClientsID && t.DisciplineID == x.ID).DefaultIfEmpty()

                          select new DisciplineEdit { ID = x.ID, Name = x.Discipline, Mark = m.Mark != null ? m.Mark : 0, Source = x.Source != null ? x.Source : "" }).ToList<DisciplineEdit>();
                }
                else
                {
                    de = (from x in db._Disciplines
                          select new DisciplineEdit { ID = x.ID, Name = x.Discipline, Mark = 0, Source = x.Source != null ? x.Source : "" }).ToList<DisciplineEdit>();
                }

                List<Plans> lp = db._Plans.Where(x => x.ClientID == up.ClientsID && x.JobID == jobid).ToList<Plans>();

                List<Levels> levls = db._Levels.ToList<Levels>();

                List<selgrp> lsgrp = (from x in lp
                                      from y in levls
                                      where x.LevelID == y.ID
                                      select new selgrp { mark = x.mark, max = x.max, min = x.min, Lev = y.Lev, Name = y.Name, ID = x.ID }).ToList<selgrp>();

                ViewBag.plansgroup = lsgrp;



            }

            return View(de);
        }

        public static double ZERO = 1.0e-15;
        public static double INFINITY = 1.0e+30;
        public static int MaxVer = Properties.Ver;
        public static double Eps = 1.0e-10;
        public static int N = MaxVer;
        private double[,] Graph;
        public bool BranchAndBounds(double[,] Matr, int Ver, ref int[] Ans)
        {
            BranchAndBoundManager bab = new BranchAndBoundManager();
            double[,] WMatr = new double[N + 1, N + 1];
            Pbound CurBound = new Pbound();
            Pbound Left = new Pbound();
            Pbound Right = new Pbound();
            Pbound Rec = new Pbound();
            Pbound TmpBound = new Pbound();
            Pbound OldBound = new Pbound();
            for (int i = 1; i <= N; i++)
                for (int j = 1; j <= N; j++)
                {
                    if (Math.Abs(Matr[i, j]) < ZERO)
                    {
                        WMatr[i, j] = 2 * INFINITY;
                    }
                    else WMatr[i, j] = Matr[i, j];
                }
            CurBound.M = WMatr;
            CurBound.Fi = 0.0;
            CurBound.RibCol = 0;


            bab.ReductMatr(CurBound, false, N, ref OldBound);

            while (CurBound.RibCol < N - 1)
            {

                Left = null;
                Right = null;
                bab.NewLevel(CurBound, ref Left, ref Right);
                if (Left.Fi <= Right.Fi)
                {
                    CurBound = Left;
                }
                else
                {

                    CurBound = Right;
                }
            }
            bab.BuildRecord(CurBound);
            Rec = CurBound;

            return bab.BuildPath(Rec, WMatr, Ver, ref Ans);
        }
        public ActionResult GetPlan(List<int> Sel)
        {


            CompetenceContext db = new CompetenceContext();
            int SelectID = Sel[0];
            Plans plns = db._Plans.Where(x => x.ID == SelectID).SingleOrDefault();
            List<Competence> _competence = new List<Competence>();
            List<Certification> cert = db._Certification.ToList();
            List<CompetenceEdit> _come = new List<CompetenceEdit>();

            int iid = plns.ClientID;
            int jid = plns.JobID;
            int Levidi = plns.LevelID;
            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = jid,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = Levidi,
            };
            var ClientParam = new SqlParameter
            {
                ParameterName = "ClientID",
                Value = iid,
            };

            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           orderby c.ID
                           select c).ToList<Competence>();

            foreach (Competence comp in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == comp.ID && x.JobTitleID == jid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = comp.ID, Name = comp.Name, Shifr = comp.Shifr, Value = z.ToString() });


            }
            int i = 0;
            int j = 0;
            int n = _competence.Count();
            int an = 0;
            double sum2 = 0;
            double sum1 = 0;
            double maxsum1 = 0;
            double percent = 0;
            double maxsum2 = 0;

            List<CalcResult> LCR = db.Database.SqlQuery<CalcResult>("exec CalcResult @JobTitleID, @LevelID, @ClientID", JobTitleParam, LevelParam, ClientParam).ToList<CalcResult>();
            List<RANGEVALUES> RVC = null;


            RVC = new List<RANGEVALUES>();
            foreach (Competence comp in _competence)
            {
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == comp.ID).ToList<CalcResult>();
                i = 0;
                j++;
                sum1 = 0;
                an = c.Count();
                maxsum1 = 0;
                foreach (CalcResult itemCalc in c)
                {
                    i++;
                    sum1 += itemCalc.CalcCol;
                    maxsum1 += itemCalc.Value * 5;
                }

                sum2 = sum1;
                maxsum2 = maxsum1;
                percent = Math.Round(sum2 * 100 / maxsum2, 2);
                RVC.Add(new RANGEVALUES { CompetenceID = comp.ID, Shifr = comp.Shifr, Name = comp.Name, _MAX = percent });
            }

            RVC = (from mmm in RVC
                   orderby mmm.CompetenceID
                   select mmm).ToList<RANGEVALUES>();
            RVC = (from x in RVC
                   where x._MAX <= plns.max && x._MAX >= plns.min
                   select x).ToList<RANGEVALUES>();
            an = 0;
            double calc = 0;
            i = 0;
            List<TempObject> TO;
            TO = null;
            TO = new List<TempObject>();

            foreach (RANGEVALUES _rvc in RVC)
            {
                i++;
                double ValueComp = Convert.ToDouble(_come.Where(x => x.ID == _rvc.CompetenceID).Select(x => x.Value).SingleOrDefault());
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == _rvc.CompetenceID).Where(x => x.Grade <= plns.mark).ToList<CalcResult>();
                double calccompet = 0;
                foreach (var calcitem in c)
                {
                    calccompet += calcitem.CalcCol;

                }
                calccompet = calccompet / 100;
                an = c.Count;
                calc = Math.Round(5 - calccompet, 2);

                TO.Add(new TempObject { NN = i, CompetenceID = _rvc.CompetenceID, Value = calc, ValueComp = ValueComp });

            }

            int index = (from x in TO
                         where x.CompetenceID == plns.BeginCompetence
                         select x.NN).FirstOrDefault();
            Properties.Ver = 0;
            Properties.Ver = TO.Count;
            N = Properties.Ver;
            MaxVer = N;
            Graph = new double[N + 1, N + 1];
            string str = "";
            for (int t = 0; t <= TO.Count - 1; t++)
            {
                str = "";
                for (int m = 0; m <= TO.Count - 1; m++)
                {


                    if (t == m)
                    {
                        Graph[t + 1, m + 1] = 0;
                        str = str + "0 ";
                    }
                    else
                    {

                        Graph[t + 1, m + 1] = Math.Round((TO[t].ValueComp / TO[m].ValueComp) * (TO[t].Value + TO[m].Value), 2);








                        str = str + Graph[t + 1, m + 1].ToString() + " ";

                    }

                }
            }
            int[] ShortPath = new int[N + 2];
            BranchAndBounds(Graph, index, ref ShortPath);
            String[] TL = new String[ShortPath.Length];
            String[] TL1 = new String[ShortPath.Length];
            double tsum1 = 0;
            double tsum2 = 0;
            for (int sh = 1; sh < ShortPath.Length - 2; sh++)
            {
                tsum1 += Graph[ShortPath[sh], ShortPath[sh + 1]];
            }

            for (int z = ShortPath.Length - 1; z > 2; z--)
            {
                tsum2 += Graph[ShortPath[z], ShortPath[z - 1]];
            }
            List<Competence> Compet = new List<Competence>();
            int coef = 0;
            if (tsum1 <= tsum2)
            {
                for (int z = 1; z < ShortPath.Length - 1; z++)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }
            else
            {
                for (int z = ShortPath.Length - 1; z > 1; z--)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }



            //for (int z = 1;z<ShortPath.Length; z++)
            //{
            //    coef++;
            //    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
            //    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
            //    TL1[coef] = compid.ToString();
            //    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
            //    Compet.Add(comp);
            //}
            ViewBag.Competence = Compet;
            ViewBag.breadcrumb = TL;
            ViewBag.breadcrumb1 = TL1;
            ViewBag.mark = plns.mark;
            ViewBag.FIO = db._Clients.Where(m => m.JobTitleID == jid && m.ID == iid).Select(m => m.ClientsInfo.FIO).SingleOrDefault().ToString();
            ViewBag.jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.ShortPath = ShortPath;
            ViewBag.competenceid = plns.BeginCompetence;
            ViewBag.levelid = plns.LevelID;
            ViewBag.jidt = plns.JobID;
            ViewBag.clientid = plns.ClientID;
            ViewBag.min = plns.min;
            ViewBag.max = plns.max;
            ViewBag.mark = plns.mark;
            List<DiscipSort> lds = new List<DiscipSort>();
            List<DiscipSort> ldsf = new List<DiscipSort>();
            using (var dbc = new CompetenceContext())
            {
                List<ClientsCards> lcc = dbc._ClientsCards.Include("Attributes").Where(x => x.ClientID == iid && x.LevelID == Levidi).ToList<ClientsCards>();
                List<DisciplineAttributes> lda = dbc._DisciplineAttributes.ToList<DisciplineAttributes>();
                List<DisciplineClient> ldc = dbc._DisciplineClient.Include("Disciplines").Where(x => x.ClientID == iid).ToList<DisciplineClient>();

                List<Listofvar> lac = (from itemx in lcc
                                       from itemy in lda
                                       where itemx.AttributeID == itemy.AttributesID
                                       from itemz in ldc
                                       where itemy.DisciplineID == itemz.DisciplineID && itemz.Mark <= plns.mark
                                       from itemt in LCR
                                       where itemt.CompetenceID == itemy.Attributes.CompetenceID && itemt.AttributesID == itemy.AttributesID
                                       select new Listofvar { DisciplineID = itemy.DisciplineID, Discipline = itemz.Disciplines.Discipline, Mark = (int)itemz.Mark, CompetenceID = itemx.Attributes.CompetenceID, Value = itemt.Value, Time = itemz.Disciplines.Time }).Distinct().OrderBy(x => x.DisciplineID).ToList<Listofvar>();

                double R = 0f;
                double Rmax = 0f;
                double coeff = 0f;
                double ft = 0f;
                foreach (var item in lac)
                {

                    R = (int)item.Mark * (item.Value / 100);
                    Rmax = 5 * (item.Value / 100);

                    coeff = -1 * Math.Log(1 - (R / Rmax)) / item.Time;
                    ft = Rmax - (Rmax - R) * Math.Exp(-1 * coeff * item.Time);
                    Disciplines src = db._Disciplines.Where(x => x.ID == item.DisciplineID).FirstOrDefault();
                    lds.Add(new DiscipSort { ID = item.DisciplineID, name = item.Discipline, Rstart = R, Rend = Rmax, ft = ft, Time = item.Time, Source = src.Source != null ? src.Source : "" });


                }

                lds = lds.OrderBy(x => x.ID).ToList<DiscipSort>();
                var filterdisc = from s in lds
                                 group s by s.ID into g
                                 select new { ID = g.Key, ft = g.Max(s => s.ft) };

                foreach (var item in filterdisc)
                {
                    string name = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.name).FirstOrDefault();
                    double Rstart = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Rstart).FirstOrDefault();
                    double Rend = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Rend).FirstOrDefault();
                    int Time = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Time).FirstOrDefault();
                    String Source = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Source).FirstOrDefault();
                    ldsf.Add(new DiscipSort { ID = item.ID, ft = item.ft, name = name, Rstart = Rstart, Rend = Rend, Time = Time, Source = Source != null ? Source : "" });
                }
            }

            ldsf = ldsf.OrderBy(x => x.ft).ToList();
            ViewBag.ldsf = ldsf;
            return PartialView("UserResultPlan", LCR);


        }

        [HttpPost]
        public ActionResult UploadPhoto(FormCollection form)
        {

            string img64 = form["new-image-base64"];


            string base64 = img64.Substring(img64.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] chartData = Convert.FromBase64String(base64);
            string mime = img64.Substring(5, img64.IndexOf(';') - 5);

            Images img = new Images();
            img.Picture = chartData;
            img.mime = mime;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            using (var db = new UsersContext())
            {
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();

                db._Images.Add(img);
                db.SaveChanges();

                up.ImagesID = img.ID;
                db.SaveChanges();
            }

            return RedirectToAction("Manage");
        }
        //
        // POST: /Account/Manage

        [HttpPost]

        public ActionResult Manage(List<DisciplineEdit> model)
        {



            if (User.Identity.IsAuthenticated)
            {

                UsersContext db = new UsersContext();
                UserManager um = new UserManager();
                MembershipContext ct = new MembershipContext();
                string sqlquery = "select * from webpages_UsersInRoles where UserId= ";

                int? pp = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().ClientsID;
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
                if (pp == null)
                {
                    if (flag != true)
                    {
                        um.ChangeRole("NoneRegClient", User.Identity.Name, true);
                    }

                    return RedirectToAction("Manage", "Account");
                }
                else
                    if (pp != null)
                    {
                        if (flag == true)
                        {
                            um.ChangeRole("NoneRegClient", User.Identity.Name, false);
                            um.ChangeRole("User", User.Identity.Name, true);
                        }

                    }

            }



            UserProfile up = new UserProfile();
            Clients client = new Clients();

            List<Competence> _competence = new List<Competence>();
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(uid);
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
            }
            int profid = 0;
            int jobid = 0;
            List<ClientsCards> _attr = new List<ClientsCards>();
            List<ClientsCards> new_attr = new List<ClientsCards>();
            if (up.ClientsID != null)
            {


                using (var cc = new CompetenceContext())
                {


                    client = cc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                    List<DisciplineClient> DCR = cc._DisciplineClient.Where(x => x.ClientID == up.ClientsID).ToList<DisciplineClient>();
                    List<DisciplineClient> DCA = new List<DisciplineClient>();
                    cc._DisciplineClient.RemoveRange(DCR);
                    cc.SaveChanges();
                    foreach (DisciplineEdit item in model)
                    {
                        DCA.Add(new DisciplineClient { DisciplineID = item.ID, ClientID = up.ClientsID, Mark = item.Mark });
                    }

                    cc._DisciplineClient.AddRange(DCA);
                    cc.SaveChanges();



                    profid = client.JobTitle.ProfAreaID;
                    jobid = client.JobTitleID;
                    var ClientParam = new SqlParameter
                    {
                        ParameterName = "ClientID",
                        Value = (int)up.ClientsID,
                    };
                    var ActParam = new SqlParameter
                    {
                        ParameterName = "ActID",
                        Value = (int)client.DefaultActivityId,
                    };
                    List<object> LCR = cc.Database.SqlQuery<CalcResult>("exec UpdateGrade  @ClientID, @ActID", ClientParam, ActParam).ToList<object>();




                    //List<Certification> cert = cc._Certification.ToList();
                    //foreach (Levels level in cc._Levels)
                    //{
                    //    _competence = (from c in cc._Competence.ToList()
                    //                   where c.LevelID == level.ID
                    //                   from k in cert
                    //                   where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                    //                   select c).ToList<Competence>();

                    //    foreach (Competence competence in _competence)
                    //    {

                    //        List<Attributes> _attributes = cc._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == competence.ID).ToList<Attributes>();
                    //        foreach (Attributes item in _attributes)
                    //        {

                    //            List<DisciplineAttributes> disattr = cc._DisciplineAttributes.Where(x => x.AttributesID == item.ID).ToList<DisciplineAttributes>();
                    //            int n = 0;
                    //            int? sum = 0;
                    //            double average = 0f;
                    //            foreach (DisciplineAttributes disitem in disattr)
                    //            {
                    //                if (disitem.DisciplineID == cc._DisciplineClient.Where(x => x.DisciplineID == disitem.DisciplineID && x.ClientID == up.ClientsID).Select(x => x.DisciplineID).SingleOrDefault())
                    //                {
                    //                    sum += cc._DisciplineClient.Where(x => x.DisciplineID == disitem.DisciplineID && x.ClientID == up.ClientsID).Select(x => x.Mark).SingleOrDefault();
                    //                    n += 1;
                    //                }

                    //            }
                    //            if (n != 0)
                    //            {
                    //                average = Math.Round(Convert.ToDouble(sum / n), 2);
                    //                new_attr.Add(new ClientsCards { AttributeID = item.ID, ClientID = up.ClientsID, LevelID = level.ID, Grade = average });
                    //            }


                    //            ClientsCards clientcard = cc._ClientsCards.Where(x => x.AttributeID == item.ID && x.LevelID == level.ID && x.ClientID == up.ClientsID).SingleOrDefault();
                    //            if (clientcard != null)
                    //            {
                    //                _attr.Add(clientcard);
                    //            }

                    //        }
                    //    }

                    //}
                    //if (_attr.Count != 0)
                    //{
                    //    cc._ClientsCards.RemoveRange(_attr);
                    //    cc.SaveChanges();
                    //}


                    //if (new_attr.Count != 0)
                    //{
                    //    cc._ClientsCards.AddRange(new_attr);
                    //    cc.SaveChanges();
                    //}


                }

            }



            return RedirectToAction("Manage", "Account");


        }



        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        InitiateDatabaseForNewUser(model.UserName);

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Такой пользователь уже существует. Пожалуйста введите другое имя пользователя.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        public PartialViewResult Updwork()
        {

            int defwid = 0;

            List<SelectListItem> pa = new List<SelectListItem>();

            List<iasaWorks> wrks = new List<iasaWorks>();
            Clients user = new Clients();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();


                }

                try
                {
                    defwid = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(a => a.ActivityId == da).FirstOrDefault().DefaultWorkId;
                }
                catch (Exception)
                {


                }


                wrks = db.tableIasaWorks.ToList();


            }

            foreach (iasaWorks item in wrks)
            {
                if (item.WorkId == defwid && defwid != 0)
                {
                    pa.Add(new SelectListItem { Value = item.WorkId.ToString(), Text = item.WorkName, Selected = true });

                }
                else if (defwid != 0)
                {
                    pa.Add(new SelectListItem { Value = item.WorkId.ToString(), Text = item.WorkName });
                }
                else
                {
                    pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
                }

            }
            var model = pa;
            return PartialView("_Updwork", model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
    }
}