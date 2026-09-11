using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCDIS.Models;
using WebMatrix.WebData;
using System.Text;

namespace MVCDIS.Controllers
{
    public class DisciplineController : Controller
    {

        private CompetenceContext db = new CompetenceContext();

        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult GetBranch(int pid, int coidi, int jidi, int pidi, int lidi, int attridi)
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
                    List<EventsAttributes> evto = new List<EventsAttributes>();
                    bool flag = false;

                    evto = db._EventsAttributes.Where(x => x.AttributesID == attridi && x.ActivityId == activity && x.WorktypeId == item.WorktypeId).ToList();
                    if (evto.Count == 0)
                    {
                        flag = true;
                    }

                    else
                    {

                        flag = false;
                    }

                    if (flag)
                    {
                        dc.Add(item, gb.isLast(item.WorktypeId));

                    }
                    else
                    {

                        dc.Add(item, null);

                    }


                }
                nodes.lNodes = dc;
            }
            return PartialView("_TreeEvents", nodes);
        }


        [CustomAuthorize("ConfigDiscipline")]
        public bool AddEvent(int activity, int worktype, int attr, int id = 0)
        {

            using (var db = new CompetenceContext())
            {



                EventsAttributes LEA = new EventsAttributes();
                EventsAttributes item = new EventsAttributes();
                LEA.AttributesID = attr;
                LEA.WorktypeId = worktype;
                LEA.ActivityId = activity;
                if (id == 0)
                {
                    try
                    {
                        item = db._EventsAttributes.Where(x => x.WorktypeId == LEA.WorktypeId && x.AttributesID == LEA.AttributesID && x.ActivityId == LEA.ActivityId).FirstOrDefault();
                    }
                    catch (Exception)
                    {


                    }

                    if (item == null)
                    {
                        db._EventsAttributes.Add(LEA);
                        db.SaveChanges();
                    }
                }
                else
                {
                    item = db._EventsAttributes.Where(x => x.ID == id).FirstOrDefault();
                    item.ActivityId = LEA.ActivityId;
                    item.AttributesID = LEA.AttributesID;
                    item.WorktypeId = LEA.WorktypeId;

                    db.SaveChanges();
                }






            }
            return true;
        }


        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult updateReportAdd(String coid, String jid, String pid, String LevelID, String attr)
        {

            int coidi = Convert.ToInt32(coid);
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(LevelID);
            int attridi = Convert.ToInt32(attr);
            IASAModel iasm = new IASAModel();
            int da = 0;

            ViewBag.attridi = attridi;
            ViewBag.coidi = coidi;
            ViewBag.pidi = pidi;
            ViewBag.jidi = jidi;
            ViewBag.lidi = lidi;
            ViewBag.attridi = attridi;
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


                iasm.worktypes = db.tableIasaWorktypes.ToList();
            }
            return PartialView("_EventAdd", iasm);

        }
        [CustomAuthorize("ConfigDiscipline")]
        public bool removeReport(int id)
        {
            using (var db = new CompetenceContext())
            {
                EventsAttributes events = db._EventsAttributes.Where(r => r.ID == id).FirstOrDefault();
                try
                {
                    db._EventsAttributes.Remove(events);
                    db.SaveChanges();
                }
                catch (Exception)
                {


                }

            }
            return true;
        }
        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult updateReportsList(String coid, String jid, String pid, String LevelID, String attr)
        {

            List<EventsAttributes> LEA = new List<EventsAttributes>();
            List<EventsAttributesNEW> LEANEW = new List<EventsAttributesNEW>();
            int coidi = Convert.ToInt32(coid);
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(LevelID);
            int attridi = Convert.ToInt32(attr);
            ViewBag.attridi = attridi;

            Attributes _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == coidi && a.ID == attridi).FirstOrDefault();
            ViewBag.__attributes = _attributes;
            LEA = db._EventsAttributes.Where(x => x.AttributesID == attridi).ToList();
            foreach (EventsAttributes item in LEA)
            {
                ListPath lp = db.ListTreePath(item.ActivityId).Where(x => x.WorktypeId == item.WorktypeId).FirstOrDefault();
                LEANEW.Add(new EventsAttributesNEW { ID = item.ID, WorktypeId = item.WorktypeId, ActivityId = item.ActivityId, AttributesID = item.AttributesID, Name = lp.WorktypeName });
            }


            return PartialView("_EventsList", LEANEW);
        }
        [CustomAuthorize("ConfigDiscipline")]

        public ActionResult PJList()
        {
            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int prof = 0;
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        prof = client.JobTitle.ProfAreaID;
                    }
                }

            }
            ViewBag.Manage = Manage;
            List<SelectListItem> pa = new List<SelectListItem>();
            pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
            if (Manage)
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {
                    if (item.ID == prof)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }

            }
            ViewBag.Prof = pa;
            return View(pa);

        }
        [CustomAuthorize("ConfigDiscipline")]
        public PartialViewResult GetSelected(List<int> array, String attr)
        {

            int attrid = Convert.ToInt32(attr);
            List<DisciplineAttributes> disciplinesatr = db._DisciplineAttributes.Where(x => x.AttributesID == attrid).ToList<DisciplineAttributes>();
            foreach (DisciplineAttributes item in disciplinesatr)
            {
                db._DisciplineAttributes.Remove(item);
            }

            db.SaveChanges();
            if (array != null)
            {
                foreach (int item in array)
                {

                    db._DisciplineAttributes.Add(new DisciplineAttributes { AttributesID = attrid, DisciplineID = item });
                }
            }


            db.SaveChanges();
            return PartialView("SaveResultDiscipline", null);
        }
        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult ViewEvents(String coid, String jid, String pid, String LevelID, String attr)
        {
            db = new CompetenceContext();
            int coidi = Convert.ToInt32(coid);
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(LevelID);
            int attridi = Convert.ToInt32(attr);
            ViewBag.coidi = coidi;
            ViewBag.attridi = attridi;
            ViewBag.pidi = pidi;
            ViewBag.lidi = lidi;
            ViewBag.jidi = jidi;
            Attributes _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == coidi && a.ID == attridi).FirstOrDefault();
            ViewBag.__attributes = _attributes;
            return PartialView("EditEventsMethod", null);


        }
        [CustomAuthorize("ConfigDiscipline")]

        public ActionResult ViewDisciplines(String coid, String jid, String pid, String LevelID, String attr)
        {
            db = new CompetenceContext();
            int coidi = Convert.ToInt32(coid);
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(LevelID);
            int attridi = Convert.ToInt32(attr);
            ViewBag.coidi = coidi;
            ViewBag.attridi = attridi;
            ViewBag.pidi = pidi;
            ViewBag.lidi = lidi;
            Attributes _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == coidi && a.ID == attridi).FirstOrDefault();
            List<DisciplineAttributes> DAS = new List<DisciplineAttributes>();
            List<Disciplines> Disc = new List<Disciplines>();
            List<SelectListItem> pa = new List<SelectListItem>();
            Disc = db._Disciplines.ToList<Disciplines>();
            DAS = db._DisciplineAttributes.Where(a => a.AttributesID == attridi).ToList<DisciplineAttributes>();

            foreach (Disciplines item in Disc)
            {
                bool flag = false;

                if (DAS.Where(x => x.DisciplineID == item.ID).Count() != 0)
                {
                    flag = true;

                }
                else { flag = false; }
                SelectListItem selectList = new SelectListItem()
                {
                    Text = item.Discipline,
                    Value = item.ID.ToString(),
                    Selected = flag

                };
                pa.Add(selectList);

            }
            DisciplineViewModel disciplinesViewModel = new DisciplineViewModel()
            {
                Disciplines = pa
            };
            ViewBag.__attributes = _attributes;
            ViewBag.Disc = Disc;

            return PartialView("EditDisciplineMethod", disciplinesViewModel);


        }
        [CustomAuthorize("ConfigDiscipline")]
        public PartialViewResult DisciplineDetail(int id, string pid, string jid, string lid)
        {
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            List<Attributes> _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == id).ToList<Attributes>();
            List<QuizEdit> _attr = new List<QuizEdit>();

            foreach (Attributes item in _attributes)
            {

                _attr.Add(new QuizEdit { ID = item.ID, Name = item.Name });
            }
            var model = new QuizEditViewModel();
            model.QuizEdits = _attr;
            ViewBag.ID = id;
            ViewBag.LevelID = lidi;
            ViewBag.pid = pidi;
            ViewBag.jid = jidi;
            ViewBag.coid = id;
            return PartialView("DisciplineDetail", model);

        }
        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult ManagerDiscipline(String IDS, String pid, String jid)
        {

            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<Competence> _competence = new List<Competence>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int Levidi = Convert.ToInt32(IDS);
            int ijid = Convert.ToInt32(jid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.LevelName = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Name).SingleOrDefault();
            ViewBag.LevelLev = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Lev).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            List<Certification> cert = db._Certification.ToList();
            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            foreach (Competence i in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == i.ID && x.JobTitleID == ijid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = i.ID, Name = i.Name, Shifr = i.Shifr, Value = "0" });


            }

            ViewBag.LevelID = IDS;
            var model = new CompetenceEditViewModel();
            model.CompetenceEdits = _come;
            return View(model);

        }
        [CustomAuthorize("ConfigDiscipline")]
        public PartialViewResult GetListDetail(int id)
        {

            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int job = 0;
            using (var dbm = new UsersContext())
            {
                up = dbm.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        job = client.JobTitleID;
                    }
                }

            }
            List<JobTitle> _attributes = db._JobTitle.Where(a => a.ProfAreaID == id).ToList<JobTitle>();
            List<SelectListItem> pa = new List<SelectListItem>();
            if (Manage)
            {
                foreach (JobTitle item in _attributes)
                {
                    if (item.ID == job)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (JobTitle item in _attributes)
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }
            }
            var model = pa;
            return PartialView("ListDetails", model);




        }


        [CustomAuthorize("ConfigDiscipline")]
        public ActionResult ShowPJList(String targv, String Prof)
        {
            int pid = Convert.ToInt32(Prof);
            int jid = Convert.ToInt32(targv);
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            var profstr = db._ProfArea.Where(x => x.ID == pid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            return PartialView("SelectDiscipline", db._Levels.ToList());
        }
        // GET: /Discipline/
        [CustomAuthorize("Discipline")]
        public ActionResult Index()
        {
            return View(db._Disciplines.ToList());
        }

        // GET: /Discipline/Details/5
        [CustomAuthorize("Discipline")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplines disciplines = db._Disciplines.Find(id);
            if (disciplines == null)
            {
                return HttpNotFound();
            }
            return View(disciplines);
        }

        // GET: /Discipline/Create
        [CustomAuthorize("EditMode")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Discipline/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Discipline,Time,Source")] Disciplines disciplines)
        {
            if (ModelState.IsValid)
            {
                db._Disciplines.Add(disciplines);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(disciplines);
        }

        // GET: /Discipline/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplines disciplines = db._Disciplines.Find(id);
            if (disciplines == null)
            {
                return HttpNotFound();
            }
            return View(disciplines);
        }

        // POST: /Discipline/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Discipline,Time,Source")] Disciplines disciplines)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disciplines).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(disciplines);
        }

        // GET: /Discipline/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Disciplines disciplines = db._Disciplines.Find(id);
            if (disciplines == null)
            {
                return HttpNotFound();
            }
            return View(disciplines);
        }

        // POST: /Discipline/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Disciplines disciplines = db._Disciplines.Find(id);
            db._Disciplines.Remove(disciplines);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
