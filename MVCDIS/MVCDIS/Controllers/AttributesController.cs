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
    public class AttributesController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /Attributes/
        [CustomAuthorize("Attr")]
        public ActionResult Index()
        {
            var _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes);
            return View(_attributes.ToList());
        }

        // GET: /Attributes/Details/5
        [CustomAuthorize("Attr")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attributes attributes = db._Attributes.Find(id);
            if (attributes == null)
            {
                return HttpNotFound();
            }
            return View(attributes);
        }

        // GET: /Attributes/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            ViewBag.CompetenceID = new SelectList(db._Competence, "ID", "Shifr");
            ViewBag.TypeID = new SelectList(db._TypeAttributes, "ID", "Name");
            return View();
        }

        // POST: /Attributes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CompetenceID,Name,TypeID")] Attributes attributes)
        {
            if (ModelState.IsValid)
            {
                db._Attributes.Add(attributes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompetenceID = new SelectList(db._Competence, "ID", "Shifr", attributes.CompetenceID);
            ViewBag.TypeID = new SelectList(db._TypeAttributes, "ID", "Name", attributes.TypeID);
            return View(attributes);
        }

        // GET: /Attributes/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attributes attributes = db._Attributes.Find(id);
            if (attributes == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompetenceID = new SelectList(db._Competence, "ID", "Shifr", attributes.CompetenceID);
            ViewBag.TypeID = new SelectList(db._TypeAttributes, "ID", "Name", attributes.TypeID);
            return View(attributes);
        }

        // POST: /Attributes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CompetenceID,Name,TypeID")] Attributes attributes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attributes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompetenceID = new SelectList(db._Competence, "ID", "Shifr", attributes.CompetenceID);
            ViewBag.TypeID = new SelectList(db._TypeAttributes, "ID", "Name", attributes.TypeID);
            return View(attributes);
        }

        // GET: /Attributes/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attributes attributes = db._Attributes.Find(id);
            if (attributes == null)
            {
                return HttpNotFound();
            }
            return View(attributes);
        }

        // POST: /Attributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attributes attributes = db._Attributes.Find(id);
            db._Attributes.Remove(attributes);
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
