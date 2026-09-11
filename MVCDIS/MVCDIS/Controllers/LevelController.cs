using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCDIS.Models;
using MvcContrib;
namespace MVCDIS.Controllers
{
    public class LevelController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Level/
        [CustomAuthorize("Cvalificate")]
        public ActionResult Index()
        {

            return View(db._Levels.ToList());
        }

        public ActionResult Select(String profid, String jobid)
        {
            int pid = Convert.ToInt32(profid);
            int jid = Convert.ToInt32(jobid);
            var profstr = db._ProfArea.Where(x => x.ID == pid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            return View(db._Levels.ToList());
        }

        // GET: /Level/Details/5
        [CustomAuthorize("Cvalificate")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Levels levels = db._Levels.Find(id);
            if (levels == null)
            {
                return HttpNotFound();
            }
            return View(levels);
        }

        // GET: /Level/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Level/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Lev,Name")] Levels levels)
        {
            if (ModelState.IsValid)
            {
                db._Levels.Add(levels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(levels);
        }

        // GET: /Level/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Levels levels = db._Levels.Find(id);
            if (levels == null)
            {
                return HttpNotFound();
            }
            return View(levels);
        }

        // POST: /Level/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Lev,Name")] Levels levels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(levels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(levels);
        }

        // GET: /Level/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Levels levels = db._Levels.Find(id);
            if (levels == null)
            {
                return HttpNotFound();
            }
            return View(levels);
        }

        // POST: /Level/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Levels levels = db._Levels.Find(id);
            db._Levels.Remove(levels);
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
