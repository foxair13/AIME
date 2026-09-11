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
    public class JobTitleController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /JobTitle/
        [CustomAuthorize("Jobs")]
        public ActionResult Index()
        {
            var _jobtitle = db._JobTitle.Include(j => j.ProfAreas);
            return View(_jobtitle.ToList());
        }

        // GET: /JobTitle/Details/5
        [CustomAuthorize("Jobs")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobTitle jobtitle = db._JobTitle.Find(id);
            if (jobtitle == null)
            {
                return HttpNotFound();
            }
            return View(jobtitle);
        }

        // GET: /JobTitle/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            ViewBag.ProfAreaID = new SelectList(db._ProfArea, "ID", "Name");
            return View();
        }

        // POST: /JobTitle/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProfAreaID,Name")] JobTitle jobtitle)
        {
            if (ModelState.IsValid)
            {
                db._JobTitle.Add(jobtitle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProfAreaID = new SelectList(db._ProfArea, "ID", "Name", jobtitle.ProfAreaID);
            return View(jobtitle);
        }

        // GET: /JobTitle/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobTitle jobtitle = db._JobTitle.Find(id);
            if (jobtitle == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProfAreaID = new SelectList(db._ProfArea, "ID", "Name", jobtitle.ProfAreaID);
            return View(jobtitle);
        }

        // POST: /JobTitle/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProfAreaID,Name")] JobTitle jobtitle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobtitle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProfAreaID = new SelectList(db._ProfArea, "ID", "Name", jobtitle.ProfAreaID);
            return View(jobtitle);
        }

        // GET: /JobTitle/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobTitle jobtitle = db._JobTitle.Find(id);
            if (jobtitle == null)
            {
                return HttpNotFound();
            }
            return View(jobtitle);
        }

        // POST: /JobTitle/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobTitle jobtitle = db._JobTitle.Find(id);
            db._JobTitle.Remove(jobtitle);
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
