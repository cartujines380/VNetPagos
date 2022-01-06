using System.Web.Optimization;

namespace VisaNet.Presentation.Administration
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //SCRIPTS
            bundles.Add(new ScriptBundle("~/Scripts/files").Include(
                "~/Scripts/*.js"
                ));
            bundles.Add(new ScriptBundle("~/Scripts/js/files").Include(
                "~/Scripts/js/*.js"
                ));
            bundles.Add(new ScriptBundle("~/Scripts/CKEditor/scripts").Include(
                "~/Scripts/CKEditor/*.js"
                ));
            bundles.Add(new ScriptBundle("~/Scripts/CKEditor/lang/scripts").Include(
                "~/Scripts/CKEditor/lang/*.js"
                ));
            bundles.Add(new ScriptBundle("~/Scripts/CKEditor/plugins/imageUpload/scripts").Include(
                "~/Scripts/CKEditor/plugins/imageUpload/*.js"
                ));


            //STYLES
            bundles.Add(new StyleBundle("~/Content/css/styles").Include(
                "~/Content/css/*.css"));
            
            bundles.Add(new StyleBundle("~/Content/font-awesome/css/style").
                Include("~/Content/font-awesome-4.7.0/css/*.css").
                IncludeDirectory("~/Content/font-awesome-4.7.0/fonts", "*.eot"));

            bundles.Add(new StyleBundle("~/Scripts/CKEditor/styles").Include(
                "~/Scripts/CKEditor/content.css"));

            bundles.Add(new StyleBundle("~/Scripts/CKEditor/skins/moono/styles").Include(
                "~/Scripts/CKEditor/skins/moono/*.css"));


            //TODO: hay que sacar esto y hacer el ajuste para que agarre las cosas de fontawesome que no agarra
            //Las siguientes lineas son para no perder los estilos al publicar en modo Release
            //            bundles.IgnoreList.Clear();
            //#if DEBUG
            //            BundleTable.EnableOptimizations = false;
            //#else
            //            BundleTable.EnableOptimizations = false;
            //#endif

        }

    }
}