using System.Web;
using System.Web.Optimization;
namespace MVCDIS
{
    public class BundleConfig
    {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/notif").Include(
                        "~/Scripts/bootstrap-notify.js",
                        "~/Scripts/bootbox.min.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/tiles").Include(
                      "~/Scripts/tiles.js"));
               bundles.Add(new ScriptBundle("~/bundles/ztree").Include(
                       "~/Scripts/jquery.ztree.all.min.js"));
               bundles.Add(new StyleBundle("~/Content/ztree").Include(
                                      "~/Content/zTreeStyle/zTreeStyle.css"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrapswitch").Include(
                     "~/Scripts/bootstrap-switch.min.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrapswitch").Include(
                      "~/Content/css/bootstrap-switch.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/introLoader").Include(
                                   "~/Scripts/jquery.introLoader.pack.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/date_js").Include(
                //"~/Scripts/bootstrap-datetimepicker.js",
                //"~/Scripts/bootstrap-datetimepicker.ru.js"
                  "~/Scripts/bootstrap-datepicker.min.js",
                  "~/Scripts/locales/bootstrap-datepicker.ru.min.js"
                  ));

         bundles.Add(new ScriptBundle("~/bundles/fusioncharts").Include(
                                   "~/Scripts/fusioncharts.js",
                                   "~/Scripts/fusioncharts.powercharts.js",
                                   "~/Scripts/fusioncharts.theme.fint.js"));

            bundles.Add(new ScriptBundle("~/bundles/canvasjs").Include(
                                   "~/Scripts/jquery.canvasjs.min.js",
                                   "~/Scripts/jcanvasjs.js",
                                   "~/Scripts/excanvas.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                         "~/Scripts/fileinput.min.js",
                           "~/Scripts/locales/ru.js"
                        ));

 bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
             "~/Scripts/jquery.unobtrusive*",
             "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/screenfull").Include(
            "~/Scripts/screenfull.js"));
            bundles.Add(new ScriptBundle("~/bundles/core").Include(
            "~/Scripts/js/core.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/ajaxlogin").Include(
    "~/Scripts/app/ajaxlogin.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
"~/Scripts/js/app.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/lity").Include(
                   "~/Scripts/lity.js"));
            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/less").Include(
                       "~/Scripts/less-*"));
            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                        "~/Scripts/jquery.signalR-*"));
         
            bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
                    "~/Scripts/bootstrap-multiselect.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/css/bootstrap/bootstrap.min.css", new CssRewriteUrlTransform()).Include("~/Content/css/main.min.css").Include("~/Content/css/animate.min.css").Include("~/Content/site.css").Include("~/Content/css/style-switcher.css").Include("~/Content/css/less/theme.less").Include("~/Content/css/lity.less").Include("~/Content/crop.css").Include("~/Content/jquery.Jcrop.css").Include("~/Content/bootstrap-fileinput/css/fileinput.min.css"));
            bundles.Add(new StyleBundle("~/Content/awesome").Include(
                        "~/Content/font-awesome.min.css"));
         
            bundles.Add(new StyleBundle("~/Content/css/multiselect").Include(
                     "~/Content/bootstrap-multiselect.css"));

            bundles.Add(new StyleBundle("~/Content/introLoader").Include(
                        "~/Content/introLoader.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/Scripts/jquery.Jcrop.js",
                      "~/Scripts/script.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                          "~/Content/themes/base/core.css",
                          "~/Content/themes/base/resizable.css",
                          "~/Content/themes/base/selectable.css",
                          "~/Content/themes/base/accordion.css",
                          "~/Content/themes/base/autocomplete.css",
                          "~/Content/themes/base/button.css",
                          "~/Content/themes/base/dialog.css",
                          "~/Content/themes/base/slider.css",
                          "~/Content/themes/base/tabs.css",
                          "~/Content/themes/base/datepicker.css",
                          "~/Content/themes/base/progressbar.css",
                          "~/Content/themes/base/theme.css"
                         
                          ));
            bundles.Add(new StyleBundle("~/Content/LogRegCSS").Include(
             "~/Content/LoginRegistrationForm/css/animate-custom.css",
             "~/Content/LoginRegistrationForm/css/base.css",
             "~/Content/LoginRegistrationForm/css/style.css"));


            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                                    "~/Content/DataTables/dataTables.css"));
 
            
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                                   "~/Scripts/DataTables/jquery.dataTables.min.js"));
  
            



        }
    }
}