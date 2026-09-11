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
    public class StepenController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Stepen/

        [CustomAuthorize("Stepen")]
        public ActionResult Index()
        {
            return View(db.tableIasaStepen.ToList());
        }

        // GET: /Stepen/Details/5
        [CustomAuthorize("Stepen")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaStepen iasastepen = db.tableIasaStepen.Find(id);
            if (iasastepen == null)
            {
                return HttpNotFound();
            }
            return View(iasastepen);
        }

        // GET: /Stepen/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Stepen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StepenId,StepenName")] iasaStepen iasastepen)
        {
            if (ModelState.IsValid)
            {
                db.tableIasaStepen.Add(iasastepen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iasastepen);
        }

        // GET: /Stepen/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaStepen iasastepen = db.tableIasaStepen.Find(id);
            if (iasastepen == null)
            {
                return HttpNotFound();
            }
            return View(iasastepen);
        }

        // POST: /Stepen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StepenId,StepenName")] iasaStepen iasastepen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iasastepen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iasastepen);
        }

        // GET: /Stepen/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaStepen iasastepen = db.tableIasaStepen.Find(id);
            if (iasastepen == null)
            {
                return HttpNotFound();
            }
            return View(iasastepen);
        }

        // POST: /Stepen/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            iasaStepen iasastepen = db.tableIasaStepen.Find(id);
            db.tableIasaStepen.Remove(iasastepen);
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
