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
    public class ClientsInfoController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /ClientsInfo/

        [CustomAuthorize("Userslist")]
        public ActionResult Index()
        {
            return View(db._ClientsInfo.ToList());
        }

        // GET: /ClientsInfo/Details/5
        [CustomAuthorize("Userslist")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsinfo = db._ClientsInfo.Find(id);
            if (clientsinfo == null)
            {
                return HttpNotFound();
            }
            return View(clientsinfo);
        }

        // GET: /ClientsInfo/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ClientsInfo/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FIO,Email")] ClientsInfo clientsinfo)
        {

            if (ModelState.IsValid)
            {
                db._ClientsInfo.Add(clientsinfo);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientsinfo);
        }

        // GET: /ClientsInfo/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsinfo = db._ClientsInfo.Find(id);


            if (clientsinfo == null)
            {
                return HttpNotFound();
            }

            return View(clientsinfo);
        }

        // POST: /ClientsInfo/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FIO,Email")] ClientsInfo clientsinfo)
        {
            if (ModelState.IsValid)
            {
                UserProfile up = new UserProfile();
                try
                {
                    using (var dbc = new UsersContext())
                    {


                        up = dbc.UserProfiles.Where(x => x.ClientsID == clientsinfo.ID).FirstOrDefault();
                        up.Email = clientsinfo.Email;
                        dbc.SaveChanges();
                    }
                }
                catch (Exception)
                {


                }





                db.Entry(clientsinfo).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientsinfo);
        }

        // GET: /ClientsInfo/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsinfo = db._ClientsInfo.Find(id);
            if (clientsinfo == null)
            {
                return HttpNotFound();
            }
            return View(clientsinfo);
        }

        // POST: /ClientsInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientsInfo clientsinfo = db._ClientsInfo.Find(id);
            db._ClientsInfo.Remove(clientsinfo);
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
