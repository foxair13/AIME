using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{

    public class UserActivities
    {
        public List<iasaActivities> iasaActivities { get; set; }
        public int DefaultActivityId { get; set; }
    }
    public class UserWorks
    {
        public List<iasaWorks> iasaWorks { get; set; }
        public int DefaultWorkId { get; set; }
    }
    public class AllUserActivities
    {
        public Dictionary<iasaActivities, bool> iasaActivities { get; set; }

    }
    public class AllUserWorks
    {
        public Dictionary<iasaWorks, bool> iasaWorks { get; set; }

    }
    public class AllWork
    {
        public Dictionary<iasaWorks, bool> GetWorks(int uid)
        {
            Dictionary<iasaWorks, bool> works = new Dictionary<iasaWorks, bool>();
            using (var db = new CompetenceContext())
            {

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
                //int da = db.tableIasaUserActivity.Where(u => u.UserId == uid).Where(z => z.ActivityId == ua).FirstOrDefault().;

                List<iasaWorks> fw = (from a in db.tableIasaWorks
                                      where !(
                                      from b in db.tableIasaWorkByUserActivity
                                      where b.UserId == uid && b.ActivityId == da
                                      select b.WorkId).Contains(a.WorkId)
                                      select a).ToList();

                //List<iasaWorks> fw = (from a in db.tableIasaWorks
                //                      from b in db.tableIasaWorkByUserActivity
                //                           where !(
                //                           from uw in db.tableIasaUserWork
                //                           where uw.UserId == uid && b.WorkId == a.WorkId
                //                           select uw.WorkId
                //                           ).Contains(a.WorkId) &&  b.ActivityId == da && b.UserId == uid && a.WorkId == b.WorkId
                //                      select a).ToList();
                List<iasaWorks> aw = db.tableIasaWorks.ToList();

             
                foreach (var item in aw)
                {
                    if (fw.Contains(item))
                    {
                        works.Add(item, false);
                    }
                    else
                    {
                        works.Add(item, true);
                    }
                }
            }
            return works;

        }
    }
    public class ReportEdit
    {
        public iasaReports Report { get; set; }
        public List<iasaActivities> userActivities { get; set; }
        public List<iasaWorks> userWorks { get; set; }
        public List<iasaWorktypesTree> worktypes { get; set; }
        public string mode { get; set; }
    }
    public class GetUserWorks
    {
        public List<iasaWorks> GetNotFree(int uid)
        {
               int da = 0;
               using (var db = new CompetenceContext())
               {


                   using (var dbc = new UsersContext())
                   {

                       UserProfile up = dbc.UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                       da = (int)db._Clients.Where(x => x.ID == up.ClientsID).FirstOrDefault().DefaultActivityId;


                   }

                   List<iasaWorks> fw = (from a in db.tableIasaWorks
                                         where (
                                         from b in db.tableIasaWorkByUserActivity
                                         where b.UserId == uid && b.ActivityId == da
                                         select b.WorkId).Contains(a.WorkId)
                                         select a).ToList();
                   return fw;
               }
        }
    }
    public class Preview
    {
        //public List<iasaReports> Worktypes { get; set; }
        // public iasaUser User { get; set; }

        public List<FullReport> Reports { get; set; }
        public Clients User { get; set; }
        public string Activity { get; set; }
        public string Workt { get; set; }
    }

    public class FullReport
    {
        public iasaReports Report { get; set; }
        public string bread { get; set; }
    }
    public class ListPath
    {

        public int WorktypeId { get; set; }
        public string WorktypeName { get; set; }
        public int HierarchyLevel { get; set; }
        public int abc { get; set; }
        public string thePath { get; set; }


    }
    public class IASAModel
    {
        public string sName { get; set; }
        public string fName { get; set; }
        public string mName { get; set; }
        public string department { get; set; }
        public string pos { get; set; }
        public string phone { get; set; }
        public string photo { get; set; }
        public bool check { get; set; }
        public List<iasaReports> records { get; set; }

        //public UserActivities userActivities { get; set; }
        //public UserWorks userWorks { get; set; }
        public List<iasaActivities> userActivities { get; set; }
        public int defaultUserActivity { get; set; }
        public List<iasaWorks> userWorks { get; set; }
        public int defaultUserWork { get; set; }
        public List<iasaWorktypes> worktypes { get; set; }
    }

    public class allActivity
    {
        public Dictionary<iasaActivities, bool> GetActivities(int uid)
        {
            Dictionary<iasaActivities, bool> activities = new Dictionary<iasaActivities, bool>();
            using (var db = new CompetenceContext())
            {


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

               
            

                List<iasaActivities> fa = (from a in db.tableIasaActivities
                                           where !(
                                           from ua in db.tableIasaUserActivity
                                           where ua.UserId == uid
                                           select ua.ActivityId
                                           ).Contains(a.ActivityId)
                                           select a).ToList();
                List<iasaActivities> aa = db.tableIasaActivities.ToList();
             
                foreach (var item in aa)
                {
                    if (fa.Contains(item))
                    {
                        activities.Add(item, false);
                    }
                    else
                    {
                        activities.Add(item, true);
                    }
                }
            }
            return activities;
        }
    }
}