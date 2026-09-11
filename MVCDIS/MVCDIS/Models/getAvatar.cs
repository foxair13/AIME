using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace MVCDIS.Models
{
    public class getAvatar
    {
        public string GetImage(int userId)
        {
            Images img = new Images();
            UserProfile up = new UserProfile();
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == userId).SingleOrDefault();
                img = db._Images.Where(x => x.ID == up.ImagesID).SingleOrDefault();
                if (img == null)
                {
                    img = db._Images.Where(x => x.ID == 1).SingleOrDefault();
                    string imgstring = Convert.ToBase64String(img.Picture);
                    return String.Format("data:{0};base64,{1}", img.mime, imgstring);
                }
                else
                {
                   
                    string imgstring = Convert.ToBase64String(img.Picture);
                    return String.Format("data:{0};base64,{1}", img.mime, imgstring);
                }
            }
        }
        public int  getuid (string uname)
        {
              
              return WebSecurity.GetUserId(uname);
        }
    }
}