using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCDIS.Models;

namespace MVCDIS.Controllers
{
    public class CompetenceController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Competence/
        [CustomAuthorize("Competence")]
        public ActionResult Index()
        {
            var _competence = db._Competence.Include(c => c.Levels);
            return View(_competence.ToList());
        }

        public PartialViewResult GetRowDetail(int id, string pid, string jid, string lid)
        {
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            List<Attributes> _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == id).ToList<Attributes>();
            List<AttributesEdit> _attr = new List<AttributesEdit>();

            foreach (Attributes item in _attributes)
            {
                var z = db._JobTitleAt.Where(x => x.CompetenceID == id && x.JobTitleID == jidi && x.AttributesID == item.ID && x.LevelID == lidi).Select(x => x.Value).SingleOrDefault();
                _attr.Add(new AttributesEdit { ID = item.ID, Name = item.Name, Value = z.ToString() });
            }
            var model = new AttributesEditViewModel();
            model.AttributesEdits = _attr;
            ViewBag.ID = id;
            ViewBag.LevelID = lidi;
            ViewBag.pid = pidi;
            ViewBag.jid = jidi;
            return PartialView("CompetenceDetails", model);

        }

        public PartialViewResult PostAjax(string id, string jid, string lid, Dictionary<string, string> postData)
        {

            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            int idi = Convert.ToInt32(id);
            var select = db._JobTitleAt.Where(c => c.JobTitleID == jidi && c.LevelID == lidi && c.CompetenceID == idi);
            db._JobTitleAt.RemoveRange(select);
            db.SaveChanges();


            foreach (KeyValuePair<string, string> item in postData)
            {


                double val = Math.Round(Convert.ToDouble(item.Value), 2);
                int key = Convert.ToInt32(item.Key);
                db._JobTitleAt.Add(new JobTitleAt { CompetenceID = idi, LevelID = lidi, Value = val, JobTitleID = jidi, AttributesID = key });
            }

            db.SaveChanges();
            ViewBag.Message = "Данные успешно сохранены";
            return PartialView("_PostAjax");


        }
        [HttpGet]
        public ActionResult Priority(String IDS, String ShifrS, String NameS, String pid, String jid)
        {

            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<Competence> _competence = new List<Competence>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int ijid = Convert.ToInt32(jid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();

            int id = Convert.ToInt32(IDS);
            List<Certification> cert = db._Certification.ToList();
            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == id
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            foreach (Competence i in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == i.ID && x.JobTitleID == ijid && x.LevelID == id).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = i.ID, Name = i.Name, Shifr = i.Shifr, Value = z.ToString() });


            }
            ViewBag.LevelName = NameS;
            ViewBag.LevelLev = ShifrS;
            ViewBag.LevelID = IDS;
            var model = new CompetenceEditViewModel();
            model.CompetenceEdits = _come;
            return View(model);
        }
        [HttpGet]
        public ActionResult PriorityGet(CompetenceEditViewModel model, String IDS, String JID)
        {
            int levid = Convert.ToInt32(IDS);
            int jobid = Convert.ToInt32(JID);
            var select = db._JobTitleCo.Where(c => c.JobTitleID == jobid && c.LevelID == levid);
            db._JobTitleCo.RemoveRange(select);
            db.SaveChanges();


            foreach (CompetenceEdit item in model.CompetenceEdits)
            {
                double z = Math.Round(Convert.ToDouble(item.Value), 2);
                db._JobTitleCo.Add(new JobTitleCo { CompetenceID = item.ID, LevelID = levid, Value = z, JobTitleID = jobid });
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Competence/Details/5
        [CustomAuthorize("Competence")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competence competence = db._Competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            return View(competence);
        }

        // GET: /Competence/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            ViewBag.LevelID = new SelectList(db._Levels, "ID", "Lev");
            return View();
        }

        // POST: /Competence/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LevelID,Shifr,Name")] Competence competence)
        {
            if (ModelState.IsValid)
            {
                db._Competence.Add(competence);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LevelID = new SelectList(db._Levels, "ID", "Lev", competence.LevelID);
            return View(competence);
        }

        // GET: /Competence/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competence competence = db._Competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db._Levels, "ID", "Lev", competence.LevelID);
            return View(competence);
        }

        // POST: /Competence/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LevelID,Shifr,Name")] Competence competence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(competence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LevelID = new SelectList(db._Levels, "ID", "Lev", competence.LevelID);
            return View(competence);
        }

        // GET: /Competence/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competence competence = db._Competence.Find(id);
            if (competence == null)
            {
                return HttpNotFound();
            }
            return View(competence);
        }

        // POST: /Competence/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Competence competence = db._Competence.Find(id);
            db._Competence.Remove(competence);
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
