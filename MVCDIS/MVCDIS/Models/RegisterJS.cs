using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCDIS.Models
{
    public static class ContentJS
    {
        public static IHtmlString RegisterJS(this System.Web.Mvc.HtmlHelper helper, int scriptLib)
        {


            StringBuilder JSstring = new StringBuilder();
            

            JSstring.Append("function updateReportsList(coid, jid, pid, LevelID, attr) {$('#reportsList_"+scriptLib.ToString()+"').html(\"<div class=\"text-center\"><img src=\"/Content/Images/loader.gif\" ></div>\");$.get('/Discipline/updateReportsList', { coid: coid, jid: jid, pid: pid, LevelID: LevelID, attr: attr },function (viewdata) {$('#reportsList_"+ scriptLib.ToString()+"').html(viewdata);});}");

          


            return MvcHtmlString.Create(JSstring.ToString());
        }
    }
}