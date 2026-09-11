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
    public class WorksController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Works/
        [CustomAuthorize("Works")]
        public ActionResult Index()
        {
            return View(db.tableIasaWorks.ToList());
        }

        // GET: /Works/Details/5
        [CustomAuthorize("Works")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaWorks iasaworks = db.tableIasaWorks.Find(id);
            if (iasaworks == null)
            {
                return HttpNotFound();
            }
            return View(iasaworks);
        }

        // GET: /Works/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Works/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WorkId,WorkName")] iasaWorks iasaworks)
        {
            if (ModelState.IsValid)
            {
                db.tableIasaWorks.Add(iasaworks);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iasaworks);
        }

        // GET: /Works/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaWorks iasaworks = db.tableIasaWorks.Find(id);
            if (iasaworks == null)
            {
                return HttpNotFound();
            }
            return View(iasaworks);
        }

        // POST: /Works/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WorkId,WorkName")] iasaWorks iasaworks)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iasaworks).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iasaworks);
        }

        // GET: /Works/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaWorks iasaworks = db.tableIasaWorks.Find(id);
            if (iasaworks == null)
            {
                return HttpNotFound();
            }
            return View(iasaworks);
        }

        // POST: /Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            iasaWorks iasaworks = db.tableIasaWorks.Find(id);
            db.tableIasaWorks.Remove(iasaworks);
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
