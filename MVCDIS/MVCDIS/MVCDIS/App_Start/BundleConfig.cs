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

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

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
            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/less").Include(
                       "~/Scripts/less-*"));
            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                        "~/Scripts/jquery.signalR-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/css/bootstrap/bootstrap.min.css", new CssRewriteUrlTransform()).Include("~/Content/css/main.min.css").Include("~/Content/css/animate.min.css").Include("~/Content/site.css").Include("~/Content/css/style-switcher.css").Include("~/Content/css/less/theme.less"));
            bundles.Add(new StyleBundle("~/Content/awesome").Include(
                        "~/Content/font-awesome.min.css"));

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