using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
 
   

   
    public class freeActivity
    {
        public List<iasaActivities> GetFree(int uid)
        {
          
            using (var db = new CompetenceContext())
            {


              



                List<iasaActivities> fa = (from a in db.tableIasaActivities
                                           where !(
                                           from ua in db.tableIasaUserActivity
                                           where ua.UserId == uid
                                           select ua.ActivityId
                                           ).Contains(a.ActivityId)
                                           select a).ToList();
                return fa;
            }
           
        }

        public List<iasaActivities> GetNotFree(int uid)
        {
        
          using (var db = new CompetenceContext())
          {


            


              List<iasaActivities> fa = (from a in db.tableIasaActivities
                                         where (
                                         from ua in db.tableIasaUserActivity
                                         where ua.UserId == uid
                                         select ua.ActivityId
                                         ).Contains(a.ActivityId)
                                         select a).ToList();
              return fa;
          }
        }


    }

    

}