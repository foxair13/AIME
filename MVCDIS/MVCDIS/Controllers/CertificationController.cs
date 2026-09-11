using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCDIS.Controllers
{
    public class CertificationController : Controller
    {
        private CompetenceContext db = new CompetenceContext();

        [CustomAuthorize("Cert")]
        public ActionResult Index()
        {

            List<Levels> levs = new List<Levels>();
            foreach (var item in db._Levels.ToList())
            {

                List<Certification> cert = db._Certification.ToList();
                List<Competence> list = (from c in db._Competence.ToList()
                                         where c.LevelID == item.ID
                                         from k in cert
                                         where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                                         select c).ToList<Competence>();

                levs.Add(new Levels { CompetenceAttending = list, ID = item.ID, Lev = item.Lev, Name = item.Name });
            }



            return View(levs);
        }
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(int id)
        {


            List<AdditionalServicesCompetenceModel> Competences = new List<AdditionalServicesCompetenceModel>();
            List<Certification> cert = db._Certification.ToList();
            List<Competence> allcompetence = (from c in db._Competence.ToList()
                                              where c.LevelID == id
                                              select c).ToList<Competence>();


            List<Competence> certcompetence = (from c in db._Competence.ToList()
                                               where c.LevelID == id
                                               from k in cert
                                               where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                                               select c).ToList<Competence>();
            var LN = db._Levels.FirstOrDefault(x => x.ID == id).Name.ToString();

            var L = db._Levels.FirstOrDefault(x => x.ID == id).Lev.ToString();

            ViewBag.LevelName = LN.ToString();
            ViewBag.Level = L.ToString();
            foreach (Competence compitem in allcompetence)
            {
                bool flag = false;
                foreach (Competence certitem in certcompetence)
                {

                    if (compitem.ID == certitem.ID)
                    {
                        flag = true;
                        break;
                    }
                    else { flag = false; }

                }
                Session["LevelId"] = id;
                Competences.Add(new AdditionalServicesCompetenceModel { ID = compitem.ID, IsSelected = flag, Name = compitem.Name, Shifr = compitem.Shifr });


            }


            return View(Competences);
        }

        [HttpPost]
        [CustomAuthorize("EditMode")]
        public ActionResult Edit(List<AdditionalServicesCompetenceModel> item)
        {
            List<Certification> Certifications = new List<Certification>();


            int levelid = Convert.ToInt32(Session["LevelId"].ToString());

            List<AdditionalServicesCompetenceModel> Selected = item.Where(m => m.IsSelected).ToList<AdditionalServicesCompetenceModel>();
            if (Selected != null && Selected.Any())
            {





                using (CompetenceContext db = new CompetenceContext())
                {


                    Certifications = (from k in db._Certification
                                      where k.LevelID == levelid
                                      select k).ToList<Certification>();

                    foreach (Certification j in Certifications)
                    {
                        db._Certification.Remove(j);
                    }

                    db.SaveChanges();

                    foreach (AdditionalServicesCompetenceModel k in Selected)
                    {
                        db._Certification.Add(new Certification { CompetenceID = k.ID, LevelID = levelid });

                    }
                    db.SaveChanges();
                }




            }
            else
            {
                using (CompetenceContext db = new CompetenceContext())
                {

                    Certifications = (from k in db._Certification
                                      where k.LevelID == levelid
                                      select k).ToList<Certification>();

                    foreach (Certification j in Certifications)
                    {
                        db._Certification.Remove(j);
                    }

                    db.SaveChanges();


                }

            }

            return RedirectToAction("Index");
        }
    }
}