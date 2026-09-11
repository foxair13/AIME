using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDIS.Models;
using System.IO;
using System.Drawing;
namespace MVCDIS.Controllers
{
    public class MQuizController : Controller
    {
        private CompetenceContext db = new CompetenceContext();
        //
        // GET: /MQuiz/
        public ActionResult PJList()
        {
            List<SelectListItem> pa = new List<SelectListItem>();
            pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
            foreach (ProfArea item in db._ProfArea.ToList())
            {
                pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
            }
            ViewBag.Prof = pa;
            return View(pa);

        }



        public ActionResult CreateQuiz(String coid, String jid, String pid, String LevelID, String attr)
        {
            db = new CompetenceContext();
            int coidi = Convert.ToInt32(coid);
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(LevelID);
            int attridi = Convert.ToInt32(attr);
            ViewBag.coidi = coidi;
            ViewBag.attridi = attridi;
            ViewBag.pidi = pidi;
            ViewBag.lidi = lidi;
            Attributes _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == coidi && a.ID == attridi).SingleOrDefault();
            return PartialView("CreateQuizMethod", _attributes);


        }
        public PartialViewResult MquizDetail(int id, string pid, string jid, string lid)
        {
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            List<Attributes> _attributes = db._Attributes.Include(a => a.Competence).Include(a => a.TypeAttributes).Where(a => a.CompetenceID == id).ToList<Attributes>();
            List<QuizEdit> _attr = new List<QuizEdit>();

            foreach (Attributes item in _attributes)
            {

                _attr.Add(new QuizEdit { ID = item.ID, Name = item.Name });
            }
            var model = new QuizEditViewModel();
            model.QuizEdits = _attr;
            ViewBag.ID = id;
            ViewBag.LevelID = lidi;
            ViewBag.pid = pidi;
            ViewBag.jid = jidi;
            ViewBag.coid = id;
            return PartialView("MquizDetail", model);

        }


        public PartialViewResult GetListDetail(int id)
        {

            List<JobTitle> _attributes = db._JobTitle.Where(a => a.ProfAreaID == id).ToList<JobTitle>();
            List<SelectListItem> pa = new List<SelectListItem>();
            foreach (JobTitle item in _attributes)
            {
                pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
            }
            var model = pa;
            return PartialView("ListDetails", model);




        }

        public byte[] Content { get; set; }
        [HttpPost]
        public JsonResult Upload()
        {
            String Questid = Request.Headers["Questid"];
            String Quest = Request.Headers["Quest"];
            String QuestT = Request.Headers["QuestT"];
            String TypeQ = Request.Headers["TypeQ"];
            String Coidi = Request.Headers["Coidi"];
            String Attridi = Request.Headers["Attridi"];
            String pidi = Request.Headers["pidi"];
            String lidi = Request.Headers["lidi"];
            ImagesManager im = new ImagesManager();
            string[] Answers = Request.Form.GetValues("Answers");
            string[] AnswersC = Request.Form.GetValues("AnswersC");
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file


                //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = String.Concat(Request.Files.AllKeys[i], Path.GetExtension(file.FileName));
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                Image image = Image.FromStream(fileContent);
                Content = im.imageToByteArray(image);
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
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
            return PartialView("SelectQuiz", db._Levels.ToList());
        }


        public ActionResult ManagerQuiz(String IDS, String pid, String jid)
        {

            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<Competence> _competence = new List<Competence>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int Levidi = Convert.ToInt32(IDS);
            int ijid = Convert.ToInt32(jid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.LevelName = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Name).SingleOrDefault();
            ViewBag.LevelLev = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Lev).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            List<Certification> cert = db._Certification.ToList();
            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            foreach (Competence i in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == i.ID && x.JobTitleID == ijid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = i.ID, Name = i.Name, Shifr = i.Shifr, Value = "0" });


            }

            ViewBag.LevelID = IDS;
            var model = new CompetenceEditViewModel();
            model.CompetenceEdits = _come;
            return View(model);

        }



    }
}