using System.Web.Optimization;

namespace VisaNet.Presentation.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //bundles.Add(new StyleBundle("~/Content/style").Include(
            //    "~/Content/*.css"
            //    ));

            bundles.Add(new StyleBundle("~/Content/css/style").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/font-awesome.css",
                "~/Content/css/jquery.pnotify.default.css",
                "~/Content/css/visa.css",
                "~/Content/css/datepicker.css",
                "~/Content/css/jquery-ui.css",
                "~/Content/css/bootstrap-select.css.map",
                "~/Content/css/bootstrap-select.css"));

            bundles.Add(new StyleBundle("~/Content/font/font-awesome/css/style").Include(
                "~/Content/font/font-awesome/css/*.css"
                ));

            //bundles.Add(new StyleBundle("~/Content/font/font-awesome/fonts/style").Include(
            //    "~/Content/font/font-awesome/fonts/fontawesome-webfont.eot",
            //    "~/Content/font/font-awesome/fonts/fontawesome-webfont.svg",
            //    "~/Content/font/font-awesome/fonts/fontawesome-webfont.ttf",
            //    "~/Content/font/font-awesome/fonts/fontawesome-webfont.woff",
            //    "~/Content/font/font-awesome/fonts/FontAwesome.otf"));


            bundles.Add(new ScriptBundle("~/Scripts/files").Include("~/Scripts/*.js"));

            bundles.Add(new ScriptBundle("~/Scripts/js/files").Include("~/Scripts/js/*.js"));
        }
    }
}
