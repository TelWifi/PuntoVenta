using System.Web;
using System.Web.Optimization;

namespace Franquisia1._1
{
    public class BundleConfig
    {
        // Para obtener más información acerca de Bundling, consulte http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/js/jquery-latest.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery.validate").Include("~/Scripts/js/jquery.validate.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include("~/Scripts/js/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/fact").Include("~/Scripts/js/fact.js"));
            bundles.Add(new ScriptBundle("~/bundles/cons").Include("~/Scripts/js/cons.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-js").Include("~/Scripts/js/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-keyboard").Include("~/Scripts/js/jquery.keyboard.js"));


            
            bundles.Add(new ScriptBundle("~/bundles/aplicacion").Include(
                        "~/Scripts/js/bootstrap.js",
                        "~/Scripts/js/jquery-ui.js",
                        "~/Scripts/js/jquery.keyboard.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/cssapp").Include(
                "~/Content/bootstrap.css",
                "~/Content/estilos.css",
                "~/Content/jquery-ui.css",
                "~/Content/keyboard.css"
                ));
        }
    }
}