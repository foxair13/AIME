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

namespace MVCDIS.Controllers
{
    public class ClientsController : Controller
    {
        private CompetenceContext db = new CompetenceContext();


        [CustomAuthorize("Grades")]
        public ActionResult Advancedinfo(String Sel)
        {
            int uid = Convert.ToInt32(Sel);
            List<DisciplineEdit> de = new List<DisciplineEdit>();
            UserProfile up = new UserProfile();




            Images img = new Images();


            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.ClientsID == uid).FirstOrDefault();
                try
                {
                    img = db._Images.Where(x => x.ID == up.ImagesID).FirstOrDefault();
                }
                catch (Exception)
                {


                }



                if (img.Picture == null)
                {



                    img = db._Images.Where(x => x.ID == 1).SingleOrDefault();



                }



                using (var dbc = new CompetenceContext())
                {
                    Clients client = dbc._Clients.Where(x => x.ID == uid).FirstOrDefault();
                    ViewBag.FIO = client.ClientsInfo.FIO;
                    ViewBag.email = client.ClientsInfo.Email;
                    ViewBag.Job = client.JobTitle.Name;
                    ViewBag.Step = client.iasaStepen.StepenName;

                    ViewBag.Zvan = client.iasaZvanie.ZvanieName;
                }




            }













            string imgstring = Convert.ToBase64String(img.Picture);
            imgstring = String.Format("data:{0};base64,{1}", img.mime, imgstring);
            ViewBag.img = imgstring;


            return PartialView("_Advancedinfo", null);


        }


        [HttpGet]
        [CustomAuthorize("Grades")]
        public ActionResult Mark(String IDCLIENT, String Levid, String pid, String jid)
        {

            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<Competence> _competence = new List<Competence>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int iid = Convert.ToInt32(IDCLIENT);
            int ijid = Convert.ToInt32(jid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();

            int Levidi = Convert.ToInt32(Levid);
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

            ViewBag.ClientId = db._Clients.Where(x => x.JobTitleID == ijid && x.ID == iid).Select(x => x.ClientInfoID).SingleOrDefault().ToString();
            ViewBag.ClienD = iid;
            ViewBag.ClientName = db._Clients.Where(x => x.JobTitleID == ijid && x.ID == iid).Select(x => x.ClientsInfo.FIO).SingleOrDefault().ToString();
            ViewBag.LevelName = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Name).SingleOrDefault().ToString();
            ViewBag.LevelLev = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Lev).SingleOrDefault().ToString(); ;
            ViewBag.LevelID = Levid;
            var model = new CompetenceEditViewModel();
            model.CompetenceEdits = _come;
            return View(model);

        }
        [CustomAuthorize("EditMode")]
        public PartialViewResult PostAjax(string id, string lid, string coid, Dictionary<string, string> postData)
        {


            int lidi = Convert.ToInt32(lid);
            int idi = Convert.ToInt32(id);
            int coidi = Convert.ToInt32(coid);

            var select = from x in db._ClientsCards
                         join m in db._Attributes on x.AttributeID equals m.ID
                         join t in db._Competence on m.CompetenceID equals t.ID
                         where x.LevelID == lidi && x.ClientID == idi && t.ID == coidi
                         select x;

            db._ClientsCards.RemoveRange(select);
            db.SaveChanges();


            foreach (KeyValuePair<string, string> item in postData)
            {
                double val = Math.Round(Convert.ToDouble(item.Value), 2);


                int key = Convert.ToInt32(item.Key);
                db._ClientsCards.Add(new ClientsCards { ClientID = idi, LevelID = lidi, Grade = val, AttributeID = key });
            }

            db.SaveChanges();
            ViewBag.Message = "Данные успешно сохранены";
            return PartialView("_PostAjax");


        }
        [CustomAuthorize("Grades")]
        public PartialViewResult GetRowDetail(int id, string pid, string jid, string lid, string cid)
        {
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            int cidi = Convert.ToInt32(cid);
            List<Attributes> _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == id).ToList<Attributes>();
            List<MarksEdit> _attr = new List<MarksEdit>();

            foreach (Attributes item in _attributes)
            {
                var z = db._ClientsCards.Where(x => x.AttributeID == item.ID && x.LevelID == lidi && x.ClientID == cidi).Select(x => x.Grade).SingleOrDefault();
                _attr.Add(new MarksEdit { ID = item.ID, Name = item.Name, Grade = z });
            }
            var model = new MarksEditViewModel();
            model.MarksEdits = _attr;
            ViewBag.ID = id;
            ViewBag.LevelID = lidi;
            ViewBag.pid = pidi;
            ViewBag.jid = jidi;
            ViewBag.coid = id;
            ViewBag.ClientId = cidi;
            return PartialView("MarksDetails", model);

        }
        [CustomAuthorize("Grades")]
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
            return PartialView("SelectClient", db._Levels.ToList());


        }

        [CustomAuthorize("Grades")]
        public ActionResult ShowPJClient(String levelid, int pidt, int jidt)
        {
            ViewBag.Levid = levelid;
            ViewBag.pid = pidt;
            ViewBag.jid = jidt;
            return PartialView("Client", db._Clients.Where(x => x.JobTitleID == jidt).ToList());


        }
        [CustomAuthorize("Grades")]
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
        [CustomAuthorize("Grades")]
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

        [CustomAuthorize("Contingent")]
        // GET: /Clients/
        public ActionResult Index()
        {
            var _clients = db._Clients.Include(c => c.ClientsInfo).Include(c => c.JobTitle);
            return View(_clients.ToList());
        }

        // GET: /Clients/Details/5
        [CustomAuthorize("Contingent")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clients clients = db._Clients.Find(id);
            if (clients == null)
            {
                return HttpNotFound();
            }
            return View(clients);
        }

        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            ViewBag.ClientInfoID = new SelectList(db._ClientsInfo, "ID", "FIO");
            ViewBag.DefaultActivityId = new SelectList(db.tableIasaActivities, "ActivityId", "ActivityName");
            ViewBag.StepenId = new SelectList(db.tableIasaStepen, "StepenId", "StepenName");
            ViewBag.ZvanieId = new SelectList(db.tableIasaZvanie, "ZvanieId", "ZvanieName");
            ViewBag.JobTitleID = new SelectList(db._JobTitle, "ID", "Name");
            return View();
        }

        // POST: /Clients/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Manager,JobTitleID,ClientInfoID,StepenId,DefaultActivityId,ZvanieId")] Clients clients)
        {
            if (ModelState.IsValid)
            {
                db._Clients.Add(clients);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientInfoID = new SelectList(db._ClientsInfo, "ID", "FIO", clients.ClientInfoID);
            ViewBag.DefaultActivityId = new SelectList(db.tableIasaActivities, "ActivityId", "ActivityName", clients.DefaultActivityId);
            ViewBag.StepenId = new SelectList(db.tableIasaStepen, "StepenId", "StepenName", clients.StepenId);
            ViewBag.ZvanieId = new SelectList(db.tableIasaZvanie, "ZvanieId", "ZvanieName", clients.ZvanieId);
            ViewBag.JobTitleID = new SelectList(db._JobTitle, "ID", "Name", clients.JobTitleID);
            return View(clients);
        }

        // GET: /Clients/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clients clients = db._Clients.Find(id);
            if (clients == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientInfoID = new SelectList(db._ClientsInfo, "ID", "FIO", clients.ClientInfoID);
            ViewBag.DefaultActivityId = new SelectList(db.tableIasaActivities, "ActivityId", "ActivityName", clients.DefaultActivityId);
            ViewBag.StepenId = new SelectList(db.tableIasaStepen, "StepenId", "StepenName", clients.StepenId);
            ViewBag.ZvanieId = new SelectList(db.tableIasaZvanie, "ZvanieId", "ZvanieName", clients.ZvanieId);
            ViewBag.JobTitleID = new SelectList(db._JobTitle, "ID", "Name", clients.JobTitleID);
            return View(clients);
        }

        // POST: /Clients/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Manager,JobTitleID,ClientInfoID,StepenId,DefaultActivityId,ZvanieId")] Clients clients)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clients).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientInfoID = new SelectList(db._ClientsInfo, "ID", "FIO", clients.ClientInfoID);
            ViewBag.DefaultActivityId = new SelectList(db.tableIasaActivities, "ActivityId", "ActivityName", clients.DefaultActivityId);
            ViewBag.StepenId = new SelectList(db.tableIasaStepen, "StepenId", "StepenName", clients.StepenId);
            ViewBag.ZvanieId = new SelectList(db.tableIasaZvanie, "ZvanieId", "ZvanieName", clients.ZvanieId);
            ViewBag.JobTitleID = new SelectList(db._JobTitle, "ID", "Name", clients.JobTitleID);
            return View(clients);
        }
        // GET: /Clients/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clients clients = db._Clients.Find(id);

            if (clients == null)
            {
                return HttpNotFound();
            }
            return View(clients);
        }

        // POST: /Clients/Delete/5

        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clients clients = db._Clients.Find(id);
            var records = from x in db._ValueCompetence
                          where x.ClientID == id
                          select x;
            var records1 = from x in db._CalcValueCompetence
                           where x.ClientID == id
                           select x;
            var records2 = from x in db._ClientsCards
                           where x.ClientID == id
                           select x;
            db._ValueCompetence.RemoveRange(records);
            db._CalcValueCompetence.RemoveRange(records1);
            db._ClientsCards.RemoveRange(records2);
            db._Clients.Remove(clients);
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
