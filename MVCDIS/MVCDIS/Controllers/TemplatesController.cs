using MVCDIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCDIS.Controllers
{
    public class TemplatesController : Controller
    {

        CompetenceContext db = new CompetenceContext();

        // GET: Templates








        [HttpGet]
        [CustomAuthorize("Templates")]
        public FileResult GetTemp()
        {
            Clients user = new Clients();
            CompetenceContext db = new CompetenceContext();

            int da = 0;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            using (var dbc = new UsersContext())
            {

                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                user = db._Clients.Where(u => u.ID == up.ClientsID).FirstOrDefault();

            }





            int act_id = (int)user.DefaultActivityId;
            iasaActivities iact = db.tableIasaActivities.Where(x => x.ActivityId == act_id).FirstOrDefault();
            int wrk_id = db.tableIasaUserActivity.Where(x => x.UserId == uid && x.ActivityId == user.DefaultActivityId).FirstOrDefault().DefaultWorkId;
            iasaTemplates template = db.tableIasaTemplates.Where(t => t.TemplateActivity == act_id && t.TemplateIsDefault == true).FirstOrDefault();

            byte[] fileData;

            string fileName = "";


            string mappath = Server.MapPath(@"../OutputDocument.docx");

            //Execute query to get actual file name of item.

            if (template != null)
            {

                fileName = template.TemplateName;

                fileData = (byte[])template.TemplateData.ToArray();
                Stream stream = new MemoryStream(fileData);
                try
                {
                    System.IO.File.Delete(mappath);
                }
                catch (Exception)
                {


                }


                using (FileStream fileStream = System.IO.File.Create(mappath, (int)stream.Length))
                {

                    // Initialize the bytes array with the stream length and then fill it with data
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    // Use write method to write to the file specified above
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);

                }


            }


            byte[] outfiledata = System.IO.File.ReadAllBytes(mappath);
            string contentType = MimeMapping.GetMimeMapping(mappath);

            return File(outfiledata, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        [CustomAuthorize("Templates")]
        public ActionResult Index()
        {
            List<iasaActivities> acts = new List<iasaActivities>();
            freeActivity f = new freeActivity();
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            acts = f.GetNotFree(uid);
            UserActivities ua = new UserActivities();
            ua.iasaActivities = acts;

            int da = 0;
            using (var dbc = new UsersContext())
            {
                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                try
                {
                    da = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                }
                catch (Exception)
                {


                }
            }
            ViewBag.DefAct = da;
            ua.DefaultActivityId = da;
            return View(ua);
        }
        [CustomAuthorize("Templates")]
        public ActionResult userTemplates(int act)
        {
            int acts = 0;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            Clients client = new Clients();
            using (var dbc = new UsersContext())
            {
                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                try
                {
                    client = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                    if (act != 0)
                    {
                        acts = act;
                        ViewBag.act = act;
                        client.DefaultActivityId = act;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.act = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault();
                        acts = (int)db._Clients.Where(x => x.ID == up.ClientsID).Select(x => x.DefaultActivityId).FirstOrDefault(); ;
                    }

                }
                catch (Exception)
                {


                }
            }




            Templates templates = new Templates();
            templates.userTemplates = db.tableIasaTemplates.Where(u => u.UserId == uid).Where(a => a.TemplateActivity == acts).ToList();
            try
            {
                templates.defaultTemplate = templates.userTemplates.Where(d => d.TemplateIsDefault == true).FirstOrDefault().TemplateId;
            }
            catch (Exception)
            {

            }

            //templates.defaultTemplate = db.tableIasaUser.Where(u => u.UserId == uid).FirstOrDefault().DefaultTemplate;
            return PartialView("_UserTemplates", templates);
        }
        [CustomAuthorize("Templates")]
        public bool defaultTemplate(int id)
        {
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            int act = 0;
            Clients client = new Clients();
            using (var dbc = new UsersContext())
            {
                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                try
                {
                    client = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                    act = (int)client.DefaultActivityId;

                }
                catch (Exception)
                {


                }
            }

            List<iasaTemplates> templates = db.tableIasaTemplates.Where(u => u.UserId == uid).Where(a => a.TemplateActivity == act).ToList();
            foreach (var item in templates)
            {
                item.TemplateIsDefault = false;
            }

            iasaTemplates template = templates.Where(i => i.TemplateId == id).FirstOrDefault();


            try
            {
                template.TemplateIsDefault = true;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        [CustomAuthorize("Templates")]
        public bool uploadTemplates()
        {
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            Clients client = new Clients();
            int da = 0;
            using (var dbc = new UsersContext())
            {
                UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                try
                {
                    client = db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault();
                    da = (int)client.DefaultActivityId;

                }
                catch (Exception)
                {


                }
            }
            //Templates template = new Templates();

            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    string fileName = upload.FileName;
                    string fileMime = upload.ContentType;
                    int fileSize = upload.ContentLength;
                    byte[] fileData;
                    using (Stream inputStream = upload.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        fileData = memoryStream.ToArray();
                    }

                    db.tableIasaTemplates.Add(new iasaTemplates { UserId = uid, TemplateData = fileData, TemplateName = fileName, TemplateMime = fileMime, TemplateSize = fileSize, TemplateActivity = da, TemplateIsDefault = false });

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                    }

                }
            }

            return true;
        }
        [CustomAuthorize("Templates")]
        public bool removeTemplate(int id)
        {
            iasaTemplates template = db.tableIasaTemplates.Where(t => t.TemplateId == id).FirstOrDefault();
            db.tableIasaTemplates.Remove(template);
            db.SaveChanges();
            return true;
        }
    }
}