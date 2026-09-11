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
    public class ZvanController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Zvan/

        [CustomAuthorize("Zvanie")]
        public ActionResult Index()
        {
            return View(db.tableIasaZvanie.ToList());
        }

        // GET: /Zvan/Details/5
        [CustomAuthorize("Zvanie")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaZvanie iasazvanie = db.tableIasaZvanie.Find(id);
            if (iasazvanie == null)
            {
                return HttpNotFound();
            }
            return View(iasazvanie);
        }

        // GET: /Zvan/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Zvan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ZvanieId,ZvanieName")] iasaZvanie iasazvanie)
        {
            if (ModelState.IsValid)
            {
                db.tableIasaZvanie.Add(iasazvanie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iasazvanie);
        }

        // GET: /Zvan/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaZvanie iasazvanie = db.tableIasaZvanie.Find(id);
            if (iasazvanie == null)
            {
                return HttpNotFound();
            }
            return View(iasazvanie);
        }

        // POST: /Zvan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ZvanieId,ZvanieName")] iasaZvanie iasazvanie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iasazvanie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iasazvanie);
        }

        // GET: /Zvan/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            iasaZvanie iasazvanie = db.tableIasaZvanie.Find(id);
            if (iasazvanie == null)
            {
                return HttpNotFound();
            }
            return View(iasazvanie);
        }

        // POST: /Zvan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            iasaZvanie iasazvanie = db.tableIasaZvanie.Find(id);
            db.tableIasaZvanie.Remove(iasazvanie);
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
