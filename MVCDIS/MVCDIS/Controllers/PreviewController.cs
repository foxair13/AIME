using Fox.Docx;
using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCDIS.Controllers
{
    public class PreviewController : Controller
    {
        [CustomAuthorize("Unlockreport")]
        public ActionResult Unlockreport(string id)
        {
            Int64 gid = Convert.ToInt64(id);
            using (var db = new CompetenceContext())
            {
                ReportsList rl = db._ReportsList.Where(x => x.GroupId == gid).FirstOrDefault();
                rl.Confirmat = false;
                db.SaveChanges();
            }
            return RedirectToAction("ReportsList", "Preview");
        }

        [CustomAuthorize("ArchReports")]
        public PartialViewResult UpdateGrid()
        {

            List<ReportsList> LRL = new List<ReportsList>();
            Clients user = new Clients();
            bool Manager = false;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;
                UserProfile up = new UserProfile();
                using (var dbc = new UsersContext())
                {

                    up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    if (up.State != null && up.State == true)
                    {
                        Manager = true;
                    }
                    else
                    {
                        Manager = false;
                    }
                }

                if (!Manager)
                {
                    LRL = db._ReportsList.Where(x => x.UserId == uid).ToList();
                }
                else
                {
                    LRL = db._ReportsList.Where(x => x.ActivityId == da).ToList();
                }
            }


            return PartialView("_PartRepL", LRL);
        }
        [CustomAuthorize("Unlockreport")]


        [HttpPost]

        public ActionResult ChangeGrade(FormCollection formCollection)
        {
            using (var db = new CompetenceContext())
            {
                List<iasaIspondArchive> LiIA = new List<iasaIspondArchive>();
                foreach (var item in formCollection)
                {
                    int number;
                    bool result = Int32.TryParse(item.ToString(), out number);
                    if (result)
                    {
                        int id = Convert.ToInt32(item.ToString());
                        int rejectval = Convert.ToInt32(formCollection[item.ToString()]);
                        LiIA.Add(new iasaIspondArchive { archive_id = id, archive_grade = rejectval });
                    }


                }
                Int64 idr = 0;
                foreach (iasaIspondArchive item in LiIA)
                {
                    iasaIspondArchive final = db._iasaIspondArchive.Where(x => x.archive_id == item.archive_id).FirstOrDefault();
                    idr = final.archive_group;
                    final.archive_grade = item.archive_grade;
                    db.SaveChanges();
                }
                ReportsList LR = db._ReportsList.Where(x => x.GroupId == idr).FirstOrDefault();
                LR.Confirmat = true;
                db.SaveChanges();

            }

            return RedirectToAction("ReportsList", "Preview");





        }
        [CustomAuthorize("ArchReports")]
        public PartialViewResult ShowArch(string id)
        {
            Int64 ida = Convert.ToInt64(id);
            Preview preview = new Preview();
            DateTime dBegin = new DateTime();
            DateTime dEnd = new DateTime();
            Clients user = new Clients();
            List<iasaIspondArchive> liasa = new List<iasaIspondArchive>();
            List<FullReport> lfr = new List<FullReport>();
            ReportsList LRL = new ReportsList();





            bool Manager = false;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                liasa = db._iasaIspondArchive.Where(x => x.archive_group == ida).ToList();
                LRL = db._ReportsList.Where(x => x.GroupId == ida).FirstOrDefault();
                using (var dbc = new UsersContext())
                {
                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == LRL.UserId).FirstOrDefault();
                    UserProfile upM = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();

                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    Manager = (bool)db._Clients.Where(u => u.ID == upM.ClientsID).Select(x => x.Manager).FirstOrDefault();
                }
                ViewBag.Manager = Manager;
                liasa = liasa.Where(x => x.archive_user_id == LRL.UserId && x.archive_activity_id == LRL.ActivityId).ToList();
                dBegin = (DateTime)LRL.DateBegin;
                dEnd = (DateTime)LRL.DateEnd;
                ViewBag.Begin = dBegin.ToShortDateString();
                ViewBag.End = dEnd.ToShortDateString();
                foreach (var item in liasa)
                {
                    int? pid = item.iasaWorktypesTree.ParentId;
                    List<iasaWorktypesTree> wl = new List<iasaWorktypesTree>();
                    do
                    {
                        iasaWorktypesTree w = new iasaWorktypesTree();
                        w = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == pid).FirstOrDefault();
                        wl.Add(w);
                        pid = w.ParentId;
                    } while (pid != 0);
                    wl.Reverse();
                    string bread = "";
                    foreach (var br in wl)
                    {
                        bread += br.WorktypeName + " /";
                    }
                    lfr.Add(new FullReport { Report = new iasaReports { iasaWorktypesTree = item.iasaWorktypesTree, ReportId = item.archive_id, ActivityId = item.archive_activity_id, Grade = item.archive_grade, ReportBegin = item.archive_year_begin, ReportCreate = item.archive_year_create, ReportEnd = item.archive_year_end, ReportText = item.archive_comments, UserId = item.archive_user_id, WorkId = item.archive_work_id, WorktypeId = item.archive_worktype_id }, bread = bread });

                }


                preview.Reports = lfr;
                preview.User = user;
                preview.Activity = db.tableIasaActivities.Where(a => a.ActivityId == LRL.ActivityId).FirstOrDefault().ActivityName;
                preview.Workt = db.tableIasaWorks.Where(w => w.WorkId == LRL.WorkId).FirstOrDefault().WorkName;

            }
            ViewBag.GID = ida;
            return PartialView("ReportArch", preview);
        }
        [CustomAuthorize("ArchReports")]
        public PartialViewResult removeArch(string id)
        {
            Int64 idarch = Convert.ToInt64(id);
            List<ReportsList> LRL = new List<ReportsList>();
            Clients user = new Clients();
            bool Manager = false;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                List<iasaIspondArchive> liasa = db._iasaIspondArchive.Where(x => x.archive_group == idarch).ToList();
                List<ReportsList> LRLS = db._ReportsList.Where(x => x.GroupId == idarch).ToList();
                db._ReportsList.RemoveRange(LRLS);
                db._iasaIspondArchive.RemoveRange(liasa);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {


                }

                int da = 0;
                UserProfile up = new UserProfile();
                using (var dbc = new UsersContext())
                {

                    up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    if (up.State != null && up.State == true)
                    {
                        Manager = true;
                    }
                    else
                    {
                        Manager = false;
                    }
                }

                if (!Manager)
                {
                    LRL = db._ReportsList.Where(x => x.UserId == uid).ToList();
                }
                else
                {
                    LRL = db._ReportsList.Where(x => x.ActivityId == da).ToList();
                }
            }


            return PartialView("_PartRepL", LRL);
        }

        [CustomAuthorize("Reports")]
        public ActionResult SaveReport()
        {

            Clients user = new Clients();
            List<iasaReports> lr = new List<iasaReports>();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;
                DateTime dBegin = new DateTime();
                DateTime dEnd = new DateTime();
                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    dBegin = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.BeginDate).FirstOrDefault();
                    dEnd = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.EndDate).FirstOrDefault();
                }

                ViewBag.Begin = dBegin.ToShortDateString();

                ViewBag.End = dEnd.ToShortDateString();
                int act_id = (int)user.DefaultActivityId;
                int wrk_id = db.tableIasaUserActivity.Where(x => x.UserId == uid && x.ActivityId == user.DefaultActivityId).FirstOrDefault().DefaultWorkId;
                iasaActivities diasact = db.tableIasaActivities.Where(x => x.ActivityId == act_id).FirstOrDefault();
                lr = db.tableIasaReports.Where(u => u.UserId == uid).Where(a => a.ActivityId == act_id).Where(w => w.WorkId == wrk_id).Where(b => b.ReportBegin >= dBegin).Where(e => e.ReportEnd <= dEnd).Where(x => !x.Deleted).ToList();
                List<iasaIspondArchive> iia = new List<iasaIspondArchive>();

                Int64 eee = Convert.ToInt64(String.Concat(DateTimeToInt((DateTime)diasact.BeginDate).ToString(), uid.ToString(), act_id.ToString(), wrk_id.ToString()));
                List<iasaIspondArchive> liasa = db._iasaIspondArchive.Where(x => x.archive_group == eee && x.archive_user_id == uid).ToList();
                List<ReportsList> LRL = db._ReportsList.Where(x => x.GroupId == eee && x.UserId == uid).ToList();

                db._ReportsList.RemoveRange(LRL);
                db._iasaIspondArchive.RemoveRange(liasa);
                db.SaveChanges();

                foreach (iasaReports item in lr)
                {
                    iia.Add(new iasaIspondArchive { archive_worktype_id = item.WorktypeId, archive_group = eee, archive_activity_id = item.ActivityId, archive_comments = item.ReportText, archive_grade = (int)item.Grade, archive_user_id = uid, archive_work_id = item.WorkId, archive_year_begin = item.ReportBegin, archive_year_end = item.ReportEnd, archive_year_create = item.ReportCreate, archive_JobId = user.JobTitleID });

                }
                LRL = new List<Models.ReportsList>();
                LRL.Add(new ReportsList { WorkId = lr[0].WorkId, GroupId = eee, Confirmat = false, DateBegin = lr[0].ReportBegin, DateCreate = lr[0].ReportCreate, DateEnd = lr[0].ReportEnd, ActivityId = lr[0].ActivityId, JobId = user.JobTitleID, UserId = uid });
                db._ReportsList.AddRange(LRL);
                db._iasaIspondArchive.AddRange(iia);
                db.SaveChanges();


            }
            return RedirectToAction("ReportsList");
        }
        public static int DateTimeToInt(DateTime theDate)
        {
            return (int)(theDate.Date - new DateTime(1900, 1, 1)).TotalDays + 2;
        }
        [CustomAuthorize("ArchReports")]

        public ActionResult ReportsList()
        {

            List<ReportsList> LRL = new List<ReportsList>();
            Clients user = new Clients();
            bool Manager = false;
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;
                UserProfile up = new UserProfile();
                using (var dbc = new UsersContext())
                {

                    up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    if (up.State != null && up.State == true)
                    {
                        Manager = true;
                    }
                    else
                    {
                        Manager = false;
                    }
                }

                if (!Manager)
                {
                    LRL = db._ReportsList.Where(x => x.UserId == uid).ToList();
                }
                else
                {
                    LRL = db._ReportsList.Where(x => x.ActivityId == da).ToList();
                }
            }
            ViewBag.Manager = Manager;
            return View(LRL);
        }

        //

        // GET: /Preview/
        [CustomAuthorize("Reports")]
        public ActionResult Index()
        {

            Preview preview = new Preview();
            Clients user = new Clients();
            DateTime dBegin = new DateTime();
            DateTime dEnd = new DateTime();
            using (var db = new CompetenceContext())
            {
                int uid = WebSecurity.GetUserId(User.Identity.Name);

                int da = 0;

                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                    user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                    dBegin = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.BeginDate).FirstOrDefault();
                    dEnd = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.EndDate).FirstOrDefault();
                }


                ViewBag.Begin = dBegin.ToShortDateString();

                ViewBag.End = dEnd.ToShortDateString();

                int act_id = (int)user.DefaultActivityId;
                int wrk_id = db.tableIasaUserActivity.Where(x => x.UserId == uid && x.ActivityId == user.DefaultActivityId).FirstOrDefault().DefaultWorkId;

                List<iasaReports> lr = db.tableIasaReports.Where(u => u.UserId == uid).Where(a => a.ActivityId == act_id).Where(w => w.WorkId == wrk_id).Where(b => b.ReportBegin >= dBegin).Where(e => e.ReportEnd <= dEnd).Where(x => !x.Deleted).ToList();
                List<FullReport> lfr = new List<FullReport>();


                foreach (var item in lr)
                {
                    int? pid = item.iasaWorktypesTree.ParentId;
                    List<iasaWorktypesTree> wl = new List<iasaWorktypesTree>();
                    do
                    {
                        iasaWorktypesTree w = new iasaWorktypesTree();
                        w = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == pid).FirstOrDefault();
                        wl.Add(w);
                        pid = w.ParentId;
                    } while (pid != 0);
                    wl.Reverse();
                    string bread = "";
                    foreach (var br in wl)
                    {
                        bread += br.WorktypeName + " /";
                    }
                    lfr.Add(new FullReport { Report = item, bread = bread });
                    //preview.breadcrumbs.Add(bread);

                }


                preview.Reports = lfr;
                preview.User = user;
                preview.Activity = db.tableIasaActivities.Where(a => a.ActivityId == act_id).FirstOrDefault().ActivityName;
                preview.Workt = db.tableIasaWorks.Where(w => w.WorkId == wrk_id).FirstOrDefault().WorkName;
            }
            return View(preview);
        }


        [CustomAuthorize("ArchReports")]

        [HttpGet]
        public FileResult ClearTemplate(string ids)
        {
            Int64 id = Convert.ToInt64(ids);
            Clients user = new Clients();
            CompetenceContext db = new CompetenceContext();
            DateTime dBegin;
            DateTime dEnd;
            ReportsList RL = db._ReportsList.Where(x => x.GroupId == id).FirstOrDefault();
            int uid = RL.UserId;

            int da = 0;

            using (var dbc = new UsersContext())
            {

                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                dBegin = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.BeginDate).FirstOrDefault();
                dEnd = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.EndDate).FirstOrDefault();
            }
            int act_id = RL.ActivityId;
            iasaActivities iact = db.tableIasaActivities.Where(x => x.ActivityId == act_id).FirstOrDefault();
            int wrk_id = RL.WorkId;
            iasaTemplates template = db.tableIasaTemplates.Where(t => t.TemplateActivity == act_id && t.TemplateIsDefault == true).FirstOrDefault();
            DateTime beginDate = dBegin;
            DateTime endDate = dEnd;
            byte[] fileData;

            string fileName = "";

            String wrkstr = db.tableIasaWorks.Where(x => x.WorkId == wrk_id).Select(x => x.WorkName).FirstOrDefault();
            string mappath = Server.MapPath(@"../OutputDocument.docx");

            //Execute query to get actual file name of item.




            fileName = template.TemplateName;

            fileData = (byte[])template.TemplateData.ToArray();
            Stream stream = new MemoryStream(fileData);
            try
            {
                System.IO.File.Delete(mappath);
            }
            catch (Exception)
            {


            }


            using (FileStream fileStream = System.IO.File.Create(mappath, (int)stream.Length))
            {

                // Initialize the bytes array with the stream length and then fill it with data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                // Use write method to write to the file specified above
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);

            }

            List<ListPath> lp = db.ListTreePath(act_id).Where(x => x.HierarchyLevel == 0).ToList<ListPath>();
            Fox.Docx.Content valuesToFill = new Fox.Docx.Content();
            ListContent lc = new ListContent();
            lc.Name = "Projects List";



            valuesToFill.Fields.Add(new FieldContent("fio", user.ClientsInfo.FIO));
            valuesToFill.Fields.Add(new FieldContent("job", user.JobTitle.Name));
            valuesToFill.Fields.Add(new FieldContent("year", dBegin.ToShortDateString()));
            valuesToFill.Fields.Add(new FieldContent("stepzvan", user.iasaStepen.StepenName + ", " + user.iasaZvanie.ZvanieName));
            valuesToFill.Fields.Add(new FieldContent("Works", wrkstr));
            valuesToFill.Fields.Add(new FieldContent("yearb", dBegin.ToShortDateString()));
            valuesToFill.Fields.Add(new FieldContent("yeare", dEnd.ToShortDateString()));

            List<iasaIspondArchive> lr = db._iasaIspondArchive.Where(u => u.archive_group == id).Where(u => u.archive_user_id == uid).Where(a => a.archive_activity_id == act_id).Where(w => w.archive_work_id == wrk_id).Where(b => b.archive_year_begin >= beginDate).Where(e => e.archive_year_end <= endDate).ToList();
            foreach (var item in lp)
            {
                List<ListPath> pp = db.ListTreePath(act_id).Where(x => x.abc == item.abc).ToList<ListPath>();
                TableContent tc = new TableContent();
                tc.Name = "Team members";

                int t = 0;
                for (int i = 1; i <= pp.Count() - 1; i++)
                {
                    int WorktypeId = pp[i].WorktypeId;
                    List<iasaIspondArchive> listarch = lr.Where(x => x.archive_worktype_id == WorktypeId).ToList();
                 
                    string grade = "";
                    string min = "";
                    string max = "";
                    iasaWorktypesTree awt = db.tableIasaWorktypesTree.Where(x => x.WorktypeId == WorktypeId && x.IsDeleted != true).FirstOrDefault();

                    try
                    {
                        min = awt.MinGrade.ToString();
                        max = awt.MaxGrade.ToString();
                        if (min == "")
                        {
                            min = "0";
                        }
                        if (max == "")
                        {
                            max = "5";
                        }
                    }
                    catch (Exception)
                    {
                        min = "0";
                        max = "5";
                    }
                  

                    foreach (iasaIspondArchive iteml in listarch)
                    {
                        string workname = iteml.archive_comments;
                        try
                        {
                            grade = lr.Where(x => x.archive_worktype_id == WorktypeId).FirstOrDefault().archive_grade.ToString();
                        }
                        catch (Exception)
                        {

                            grade = "0";
                        }
                        string range = String.Concat(min + "-" + max);
                        if (workname != null)
                        {
                            t++;
                            tc.AddRow(new FieldContent("Name", pp[i].thePath + ". " + pp[i].WorktypeName), new FieldContent("range", range), new FieldContent("Role", workname), new FieldContent("mark", grade));
                        }

                    }
                }
                if (t != 0)
                {
                    lc.AddItem(new ListItemContent("Project", item.WorktypeName).AddTable(tc));

                 
                }
                else
                {
                    continue;
                }


            }
            valuesToFill.Lists.Add(lc);
            using (var outputDocument = new TemplateProcessor(mappath)
                .SetRemoveContentControls(true))
            {
                try
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                }
                catch (Exception ex)
                {


                }

            }

            byte[] outfiledata = System.IO.File.ReadAllBytes(mappath);
            string contentType = MimeMapping.GetMimeMapping(mappath);

            return File(outfiledata, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [CustomAuthorize("Reports")]
        public FileResult GetTemplates()
        {
            Clients user = new Clients();
            CompetenceContext db = new CompetenceContext();
            DateTime dBegin;
            DateTime dEnd;
            int uid = WebSecurity.GetUserId(User.Identity.Name);

            int da = 0;

            using (var dbc = new UsersContext())
            {

                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();
                dBegin = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.BeginDate).FirstOrDefault();
                dEnd = (DateTime)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.iasaActivities.EndDate).FirstOrDefault();
            }
            int act_id = (int)user.DefaultActivityId;
            iasaActivities iact = db.tableIasaActivities.Where(x => x.ActivityId == act_id).FirstOrDefault();
            int wrk_id = db.tableIasaUserActivity.Where(x => x.UserId == uid && x.ActivityId == user.DefaultActivityId).FirstOrDefault().DefaultWorkId;
            iasaTemplates template = db.tableIasaTemplates.Where(t => t.TemplateActivity == act_id && t.TemplateIsDefault == true).FirstOrDefault();
            DateTime beginDate = dBegin;
            DateTime endDate = dEnd;
            byte[] fileData;

            string fileName = "";
            String wrkstr = db.tableIasaWorks.Where(x => x.WorkId == wrk_id).Select(x => x.WorkName).FirstOrDefault();

            string mappath = Server.MapPath(@"../OutputDocument.docx");

            //Execute query to get actual file name of item.




            fileName = template.TemplateName;

            fileData = (byte[])template.TemplateData.ToArray();
            Stream stream = new MemoryStream(fileData);
            try
            {
                System.IO.File.Delete(mappath);
            }
            catch (Exception)
            {


            }


            using (FileStream fileStream = System.IO.File.Create(mappath, (int)stream.Length))
            {

                // Initialize the bytes array with the stream length and then fill it with data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                // Use write method to write to the file specified above
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);

            }

            List<ListPath> lp = db.ListTreePath(act_id).Where(x => x.HierarchyLevel == 0).ToList<ListPath>();
            Fox.Docx.Content valuesToFill = new Fox.Docx.Content();
            ListContent lc = new ListContent();
            lc.Name = "Projects List";



            valuesToFill.Fields.Add(new FieldContent("fio", user.ClientsInfo.FIO));
            valuesToFill.Fields.Add(new FieldContent("job", user.JobTitle.Name));
            valuesToFill.Fields.Add(new FieldContent("year", dBegin.ToShortDateString()));
            valuesToFill.Fields.Add(new FieldContent("Works", wrkstr));
            valuesToFill.Fields.Add(new FieldContent("stepzvan", user.iasaStepen.StepenName + ", " + user.iasaZvanie.ZvanieName));
            valuesToFill.Fields.Add(new FieldContent("yearb", dBegin.ToShortDateString()));
            valuesToFill.Fields.Add(new FieldContent("yeare", dEnd.ToShortDateString()));

            List<iasaReports> lr = db.tableIasaReports.Where(u => u.UserId == uid).Where(a => a.ActivityId == act_id).Where(w => w.WorkId == wrk_id).Where(b => b.ReportBegin >= beginDate).Where(e => e.ReportEnd <= endDate).Where(x => !x.Deleted).ToList();
            foreach (var item in lp)
            {
                List<ListPath> pp = db.ListTreePath(act_id).Where(x => x.abc == item.abc).ToList<ListPath>();
                TableContent tc = new TableContent();
                tc.Name = "Team members";

                int t = 0;
                for (int i = 1; i <= pp.Count() - 1; i++)
                {
                    int WorktypeId = pp[i].WorktypeId;
                    string workname = lr.Where(x => x.WorktypeId == WorktypeId).Select(x => x.ReportText).FirstOrDefault();
                    string grade = "";
                    string min = "";
                    string max = "";
                    iasaWorktypesTree awt = db.tableIasaWorktypesTree.Where(x => x.WorktypeId == WorktypeId && x.IsDeleted != true).FirstOrDefault();

                    try
                    {
                        min = awt.MinGrade.ToString();
                        max = awt.MaxGrade.ToString();
                        if (min == "")
                        {
                            min = "0";
                        }
                        if (max == "")
                        {
                            max = "5";
                        }
                    }
                    catch (Exception)
                    {
                        min = "0";
                        max = "5";
                    }
                    try
                    {
                        grade = lr.Where(x => x.WorktypeId == WorktypeId).FirstOrDefault().Grade.ToString();
                    }
                    catch (Exception)
                    {

                        grade = "0";
                    }
                    string range = String.Concat(min + "-" + max);
                    if (workname != null)
                    {
                        t++;
                        tc.AddRow(new FieldContent("Name", pp[i].thePath + ". " + pp[i].WorktypeName), new FieldContent("range", range), new FieldContent("Role", workname), new FieldContent("mark", grade));
                    }


                }
                if (t != 0)
                {
                    lc.AddItem(new ListItemContent("Project", item.WorktypeName).AddTable(tc));

                    valuesToFill.Lists.Add(lc);
                }
                else
                {
                    continue;
                }


            }

            using (var outputDocument = new TemplateProcessor(mappath)
                .SetRemoveContentControls(true))
            {
                try
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                }
                catch (Exception ex)
                {


                }

            }

            byte[] outfiledata = System.IO.File.ReadAllBytes(mappath);
            string contentType = MimeMapping.GetMimeMapping(mappath);

            return File(outfiledata, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }









    }
}