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
    public class TypesController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Types/
        [CustomAuthorize("Types")]
        public ActionResult Index()
        {
            return View(db._TypeAttributes.ToList());
        }

        // GET: /Types/Details/5
        [CustomAuthorize("Types")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAttributes typeattributes = db._TypeAttributes.Find(id);
            if (typeattributes == null)
            {
                return HttpNotFound();
            }
            return View(typeattributes);
        }

        // GET: /Types/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Types/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] TypeAttributes typeattributes)
        {
            if (ModelState.IsValid)
            {
                db._TypeAttributes.Add(typeattributes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typeattributes);
        }

        // GET: /Types/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAttributes typeattributes = db._TypeAttributes.Find(id);
            if (typeattributes == null)
            {
                return HttpNotFound();
            }
            return View(typeattributes);
        }

        // POST: /Types/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] TypeAttributes typeattributes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeattributes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typeattributes);
        }

        // GET: /Types/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAttributes typeattributes = db._TypeAttributes.Find(id);
            if (typeattributes == null)
            {
                return HttpNotFound();
            }
            return View(typeattributes);
        }

        // POST: /Types/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TypeAttributes typeattributes = db._TypeAttributes.Find(id);
            db._TypeAttributes.Remove(typeattributes);
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
