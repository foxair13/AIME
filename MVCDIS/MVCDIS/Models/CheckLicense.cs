using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public static class CheckLicense
    {
        public static bool Check()
        {

            bool is_full = false;
            TrialMaker t = new TrialMaker("TMTest1", System.Web.HttpContext.Current.Server.MapPath("~/app_data") + "\\RegFile.reg",
            Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\TMSetp.dbf",
            "",
            5, 10, "675");

            byte[] MyOwnKey = { 97, 5, 3, 5, 84, 21, 7, 63,
            4, 54, 87, 56, 123, 10, 3, 62,
            7, 9, 20, 36, 37, 21, 101, 57};
            t.TripleDESKey = MyOwnKey;

            TrialMaker.RunTypes RT = t.test();

            if (RT != TrialMaker.RunTypes.Expired)
            {
                is_full = true;
            }
            else is_full = false;
            return is_full;
        }
    }
}