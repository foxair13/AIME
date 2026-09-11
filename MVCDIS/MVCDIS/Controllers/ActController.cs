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
    public class ActController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Act/
        [CustomAuthorize("Actis")]
        public ActionResult Index()
        {
            return View(db.tableIasaActivities.ToList());
        }

        // GET: /Act/Details/5
        [CustomAuthorize("Actis")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaActivities iasaactivities = db.tableIasaActivities.Find(id);
            if (iasaactivities == null)
            {
                return HttpNotFound();
            }
            return View(iasaactivities);
        }

        // GET: /Act/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Act/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityId,ActivityName,BeginDate,EndDate")] iasaActivities iasaactivities)
        {
            if (ModelState.IsValid)
            {
                db.tableIasaActivities.Add(iasaactivities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iasaactivities);
        }

        // GET: /Act/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaActivities iasaactivities = db.tableIasaActivities.Find(id);
            if (iasaactivities == null)
            {
                return HttpNotFound();
            }
            return View(iasaactivities);
        }

        // POST: /Act/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityId,ActivityName,BeginDate,EndDate")] iasaActivities iasaactivities)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iasaactivities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iasaactivities);
        }

        // GET: /Act/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaActivities iasaactivities = db.tableIasaActivities.Find(id);
            if (iasaactivities == null)
            {
                return HttpNotFound();
            }
            return View(iasaactivities);
        }

        // POST: /Act/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            iasaActivities iasaactivities = db.tableIasaActivities.Find(id);
            db.tableIasaActivities.Remove(iasaactivities);
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
