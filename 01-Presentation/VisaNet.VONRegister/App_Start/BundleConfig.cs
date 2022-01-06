using System.Web.Optimization;

namespace VisaNet.VONRegister
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
                ));

            bundles.Add(new ScriptBundle("~/ThirdpartyScripts").Include(
                "~/Scripts/thirdparty/jquery.blockUI.js"
                , "~/Scripts/thirdparty/jquery.validate*"
                , "~/Scripts/thirdparty/modernizr-*"
                , "~/Scripts/thirdparty/respond.min.js"
                , "~/Scripts/thirdparty/toastr.min.js"
                , "~/Scripts/thirdparty/cybs_devicefingerprint.js"
                , "~/Scripts/ClientSideValidations/regexValidation.js"
                ));


            //STYLES
            bundles.Add(new StyleBundle("~/CustomStyles").Include(
                "~/Content/css/Site.css"
                ));

            //ESTE NO FUNCIONA, CUANDO SE OPTIMIZA SE ROMPE
            //bundles.Add(new StyleBundle("~/ThirdpartyStyles")
            //    .Include("~/Content/thirdparty/css/bootstrap-theme.min.css")
            //    .Include("~/Content/thirdparty/css/toastr.css")
            //    );

        }
    }

}