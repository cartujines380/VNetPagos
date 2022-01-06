using System.Web.Optimization;

namespace VisaNet.Presentation.VisaNetOn
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //SCRIPTS
            bundles.Add(new ScriptBundle("~/CustomScripts").Include(
                "~/Scripts/_references.js"
                , "~/Scripts/common.js"
                , "~/Scripts/cybs_devicefingerprint.js"
                , "~/Scripts/functions.js"
                ));

            bundles.Add(new ScriptBundle("~/ThirdpartyScripts").Include(
                "~/Scripts/thirdparty/jquery.blockUI.js"
                , "~/Scripts/thirdparty/toastr.js"
                ));


            //STYLES
            bundles.Add(new StyleBundle("~/CustomStyles").Include(
                "~/Content/css/style.css"
                ));

            //ESTE NO FUNCIONA, CUANDO SE OPTIMIZA FONT-AWESOME SE ROMPE
            //bundles.Add(new StyleBundle("~/ThirdpartyStyles")
            //    .Include("~/Content/thirdparty/css/font-awesome.min.css", new CssRewriteUrlTransform())
            //    .Include("~/Content/thirdparty/css/toastr.css")
            //    );
            }

    }
}