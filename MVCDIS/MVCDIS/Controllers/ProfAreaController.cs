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
    public class ProfAreaController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        // GET: /ProfArea/

        [CustomAuthorize("Prof")]
        public ActionResult Index()
        {
            return View(db._ProfArea.ToList());
        }

        [CustomAuthorize("Priority")]
        public PartialViewResult GetListDetail(int id)
        {

            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int job = 0;
            using (var dbm = new UsersContext())
            {
                up = dbm.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        job = client.JobTitleID;
                    }
                }

            }
            List<JobTitle> _attributes = db._JobTitle.Where(a => a.ProfAreaID == id).ToList<JobTitle>();
            List<SelectListItem> pa = new List<SelectListItem>();
            if (Manage)
            {
                foreach (JobTitle item in _attributes)
                {
                    if (item.ID == job)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (JobTitle item in _attributes)
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }
            }
            var model = pa;
            return PartialView("ListDetails", model);

        }
        [CustomAuthorize("Priority")]
        public ActionResult PJList()
        {
            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int prof = 0;
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        prof = client.JobTitle.ProfAreaID;
                    }
                }

            }
            ViewBag.Manage = Manage;
            List<SelectListItem> pa = new List<SelectListItem>();
            pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
            if (Manage)
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {
                    if (item.ID == prof)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }
            }
            ViewBag.Prof = pa;
            return View(pa);
        }
        [CustomAuthorize("Priority")]
        public ActionResult ShowPJList(String targv, String Prof)
        {
            int pid = Convert.ToInt32(Prof);
            int jid = Convert.ToInt32(targv);
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            var profstr = db._ProfArea.Where(x => x.ID == pid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            return PartialView("SelectList", db._Levels.ToList());


        }
        [CustomAuthorize("Prof")]
        // GET: /ProfArea/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfArea profarea = db._ProfArea.Find(id);
            if (profarea == null)
            {
                return HttpNotFound();
            }
            return View(profarea);
        }

        // GET: /ProfArea/Create
        [CustomAuthorize("EditMode")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ProfArea/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] ProfArea profarea)
        {
            if (ModelState.IsValid)
            {
                db._ProfArea.Add(profarea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profarea);
        }

        // GET: /ProfArea/Edit/5
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfArea profarea = db._ProfArea.Find(id);
            if (profarea == null)
            {
                return HttpNotFound();
            }
            return View(profarea);
        }

        // POST: /ProfArea/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] ProfArea profarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profarea);
        }

        // GET: /ProfArea/Delete/5
        [CustomAuthorize("EditMode")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfArea profarea = db._ProfArea.Find(id);
            if (profarea == null)
            {
                return HttpNotFound();
            }
            return View(profarea);
        }

        // POST: /ProfArea/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize("EditMode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProfArea profarea = db._ProfArea.Find(id);
            db._ProfArea.Remove(profarea);
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
