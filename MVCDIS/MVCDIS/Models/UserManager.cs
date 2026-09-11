using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{

    public class UserManager
    {
        public virtual List<UserProfile> FindUsers(string userNameOrEmail, int pageIndex, int pageSize, string sortDirection, int sortColumnIndex, out int totalRecords)
        {
           
            List<UserProfile> userList = new List<UserProfile>();
            totalRecords = 0;
            List<UserProfile> emailUsers = new List<UserProfile>();
            int recordCount = 0;
            Func<UserProfile, string> orderingFunction = (c => sortColumnIndex == 1 ? c.UserName : sortColumnIndex == 2 ? c.Email : c.UserName);
            using (UsersContext db = new UsersContext())
            {

                if (sortDirection == "asc")
                {
                    emailUsers = db.UserProfiles.Where(m => m.UserName.Contains(userNameOrEmail) || m.Email.Contains(userNameOrEmail)).OrderBy(orderingFunction).ToList<UserProfile>();
                }
                else
                {
                    emailUsers = db.UserProfiles.Where(m => m.UserName.Contains(userNameOrEmail) || m.Email.Contains(userNameOrEmail)).OrderByDescending(orderingFunction).ToList<UserProfile>();
                }


                emailUsers = emailUsers.Skip((pageIndex) * pageSize).Take(pageSize).ToList();
                recordCount = emailUsers.Count();
                totalRecords += recordCount;
                emailUsers.ForEach(u =>
                {
                    if (userList.Count(c => c.UserName == u.UserName) == 0)
                    {
                        userList.Add(u);
                    }
                });
            }

            return userList;
        }

         public void ChangeRole(string rolename, string username, bool? ischecked)
         {
         if (ischecked.HasValue && ischecked.Value)
            {
                System.Web.Security.Roles.AddUserToRole(username, rolename);
            }
            else
            {
                System.Web.Security.Roles.RemoveUserFromRole(username, rolename);
            }

         }
        public virtual List<UserProfile> LoadAllUsers(int pageIndex, int pageSize, string sortDirection, int sortColumnIndex, out int totalRecords)
        {
            List<UserProfile> userList = new List<UserProfile>();
            totalRecords = 0;
            Func<UserProfile, string> orderingFunction = (c => sortColumnIndex == 1 ? c.UserName : sortColumnIndex == 2 ? c.Email : c.UserName);
            int recordCount = 0;
            using (UsersContext db = new UsersContext())
            {
                List<UserProfile> emailUsers = new List<UserProfile>();


                if (sortDirection == "asc")
                {
                    emailUsers = db.UserProfiles.OrderBy(orderingFunction).ToList();
                }
                else
                {
                    emailUsers = db.UserProfiles.OrderByDescending(orderingFunction).ToList();
                }

                recordCount = emailUsers.Count();
                totalRecords += recordCount;
                emailUsers = emailUsers.Skip((pageIndex) * pageSize).Take(pageSize).ToList();

                emailUsers.ForEach(u =>
                {
                    if (userList.Count(c => c.UserName == u.UserName) == 0)
                    {
                        userList.Add(u);
                    }
                });
            }

            return userList;
        }
    }



}