using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCDIS.Controllers
{
    public class TreeController : Controller
    {

        CompetenceContext db = new CompetenceContext();
        //
        // GET: /Tree/

        //public ActionResult CreateTree()
        //{

        //    int actid = 13;
        //    int levid = 1;
        //    int startid = 0;
        //    int startattr = 0;
        //    List<Certification> cert = db._Certification.Where(x => x.LevelID == levid).ToList();
        //    List<iasaWorktypesTree> tree = new List<iasaWorktypesTree>();
        //    List<EventsAttributes> ea = new List<EventsAttributes>();
        //    foreach (Certification item in cert)
        //    {
        //        tree = new List<iasaWorktypesTree>();
        //        startid++;
        //        iasaWorktypesTree wt = new iasaWorktypesTree { ActivityId = actid, IsDeleted = false, MinGrade = 0, MaxGrade = 5, sort = startid, ParentId = 0, WorktypeName = item.Competence.Name };
        //        tree.Add(wt);
        //        db.tableIasaWorktypesTree.AddRange(tree);
        //        db.SaveChanges();
        //        List<Attributes> attr = db._Attributes.Where(x => x.CompetenceID == item.CompetenceID).ToList();
        //        tree = new List<iasaWorktypesTree>();
        //        foreach (Attributes att in attr)
        //        {
        //            startattr++;
        //            iasaWorktypesTree at = new iasaWorktypesTree { ActivityId = actid, IsDeleted = false, MinGrade = 0, MaxGrade = 5, sort = startattr, ParentId = wt.WorktypeId, WorktypeName = att.Name, AttributesID = att.ID };

        //            tree.Add(at);
        //        }
        //        db.tableIasaWorktypesTree.AddRange(tree);
        //        db.SaveChanges();
        //    }
        //    List<iasaWorktypesTree> iwt = db.tableIasaWorktypesTree.Where(x => x.AttributesID != null && x.ActivityId==actid).ToList();
        //    foreach (iasaWorktypesTree item in iwt)
        //    {
        //        EventsAttributes eitem = new EventsAttributes();
        //        eitem.ActivityId = item.ActivityId;
        //        eitem.AttributesID = (int)item.AttributesID;
        //        eitem.WorktypeId = item.WorktypeId;
        //        ea.Add(eitem);
        //    }
        //    db._EventsAttributes.AddRange(ea);
        //    db.SaveChanges();

        //    return RedirectToAction("Index", "Tree");
        //}
        [CustomAuthorize("InWork")]
        public int addFirstWorktype(string name, int act)
        {

            int? maxsort = null;
            try
            {
                maxsort = Convert.ToInt32(db.tableIasaWorktypesTree.Where(x => x.ActivityId == act && x.ParentId == 0).Max(x => x.sort)) + 1;
            }
            catch (Exception)
            {
                
              
            }
               
                if (maxsort != null)
                {
                    iasaWorktypesTree record = new iasaWorktypesTree { ParentId = 0, WorktypeName = name, ActivityId = act, IsDeleted = false, sort = (int)maxsort };
                    db.tableIasaWorktypesTree.Add(record);
                    db.SaveChanges();
                    return record.WorktypeId;
                }
                else 
                {
                    iasaWorktypesTree record = new iasaWorktypesTree { ParentId = 0, WorktypeName = name, ActivityId = act, IsDeleted = false, sort = 0 };
                    db.tableIasaWorktypesTree.Add(record);
                    db.SaveChanges();
                    return record.WorktypeId;
                }


               
        }
        [CustomAuthorize("InWork")]
        public ActionResult Index()
        {



            List<SelectListItem> pa = new List<SelectListItem>();

            pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано", Selected = true });

            foreach (iasaActivities item in db.tableIasaActivities)
            {
                pa.Add(new SelectListItem { Value = item.ActivityId.ToString(), Text = item.ActivityName });
            }

            ViewBag.Prof = pa;
            return View();
        }
        [CustomAuthorize("InWork")]
        public JsonResult getRootString(string act)
        {
            int iact = Convert.ToInt32(act);
            List<BranchString> gb = new List<BranchString>();
            if (iact != 0)
            {

                Branch b = new Branch();
                int uid = WebSecurity.GetUserId(User.Identity.Name);
                int activity = 0;
                using (var dbc = new UsersContext())
                {

                    UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                    activity = iact;

                    Clients cl = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                    cl.DefaultActivityId = iact;
                    db.SaveChanges();
                }


                List<iasaWorktypesTree> wt = db.tableIasaWorktypesTree.Where(p => p.ParentId == 0).Where(a => a.ActivityId == activity).Where(d => !d.IsDeleted).OrderBy(s => s.sort).ToList();

                foreach (var item in wt)
                {

                    gb.Add(new BranchString { id = item.WorktypeId, pId = item.ParentId, name = item.WorktypeName, isParent = !b.isLast(item.WorktypeId) });

                }
            }


            return Json(gb, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("InWork")]
        public JsonResult getBranchString(int pid)
        {
            List<BranchString> gb = new List<BranchString>();
            Branch b = new Branch();

            int uid = WebSecurity.GetUserId(User.Identity.Name);

            using (var dbc = new UsersContext())
            {

                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();



            }
            List<iasaWorktypesTree> wt = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid).Where(d => !d.IsDeleted).OrderBy(s => s.sort).ToList();

            foreach (var item in wt)
            {

                gb.Add(new BranchString { id = item.WorktypeId, pId = item.ParentId, name = item.WorktypeName, isParent = !b.isLast(item.WorktypeId) });

            }
            return Json(gb, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [CustomAuthorize("InWork")]
        public string MarkView(iasaWorktypesTree model)
        {

            iasaWorktypesTree wt = db.tableIasaWorktypesTree.Where(w => w.WorktypeId == model.WorktypeId).FirstOrDefault();
            wt.MinGrade = model.MinGrade;
            wt.MaxGrade = model.MaxGrade;
            db.SaveChanges();

            return "Данные успешно сохранены";


        }



        [CustomAuthorize("InWork")]
        public bool removeBranch(int id)
        {
            try
            {
                iasaWorktypesTree wt = db.tableIasaWorktypesTree.Where(w => w.WorktypeId == id).FirstOrDefault();
                wt.IsDeleted = true;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        [CustomAuthorize("InWork")]
        public PartialViewResult MarkView(int id)
        {

            iasaWorktypesTree iwt = db.tableIasaWorktypesTree.Where(x => x.WorktypeId == id).FirstOrDefault();


            return PartialView("_MarkView", iwt);
        }
        [CustomAuthorize("InWork")]
        public bool renameBranch(int id, string newName)
        {
            try
            {
                iasaWorktypesTree wt = db.tableIasaWorktypesTree.Where(w => w.WorktypeId == id).FirstOrDefault();
                wt.WorktypeName = newName;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        [CustomAuthorize("InWork")]
        public int addWorktype(int pid, string name, bool isParent)
        {
            int sort = 1;
            if (isParent)
            {
                sort = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid).Max(s => s.sort) + 1;
            }
            int act = 1;
            iasaWorktypesTree record = new iasaWorktypesTree { ParentId = pid, WorktypeName = name, ActivityId = act, IsDeleted = false, sort = sort, MinGrade = 0, MaxGrade = 0 };
            db.tableIasaWorktypesTree.Add(record);
            db.SaveChanges();
            return record.WorktypeId;
        }
        [CustomAuthorize("InWork")]
        public bool onDrag(int id, int tid, string type)
        {
            int? pid = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == tid).FirstOrDefault().ParentId;
            iasaWorktypesTree record = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == id).FirstOrDefault();
            int sort = db.tableIasaWorktypesTree.Where(i => i.WorktypeId == tid).FirstOrDefault().sort;
            switch (type)
            {
                case "inner":
                    record.ParentId = tid;
                    Branch b = new Branch();
                    record.sort = 1;
                    if (!b.isLast(tid))
                    {
                        record.sort = db.tableIasaWorktypesTree.Where(p => p.ParentId == tid).Max(s => s.sort) + 1;
                    }
                    break;
                case "prev":
                    record.ParentId = pid;
                    var pitems = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid).Where(s => s.sort >= sort);
                    foreach (var item in pitems)
                    {
                        item.sort += 1;
                    }
                    record.sort = sort;
                    break;
                case "next":
                    record.ParentId = pid;
                    var nitems = db.tableIasaWorktypesTree.Where(p => p.ParentId == pid).Where(s => s.sort > sort);
                    foreach (var item in nitems)
                    {
                        item.sort += 1;
                    }
                    record.sort = sort + 1;
                    break;
            }
            db.SaveChanges();
            return true;
        }
    }
}