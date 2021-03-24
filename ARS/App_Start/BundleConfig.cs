using System.Web;
using System.Web.Optimization;

namespace ARS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/custom/css").Include(
                      "~/Content/custom/bootstrap.min.css",
                      "~/Content/custom/themify-icons.css",
                      "~/Content/custom/font-awesome.css",
                      "~/Content/custom/datepicker.min.css",
                      "~/Content/custom/fullwidth.css",
                      "~/Content/custom/animated-on3step.css",
                      "~/Content/custom/owl.carousel.css",
                      "~/Content/custom/owl.theme.css",
                      "~/Content/custom/on3step-style.css",
                      "~/Content/custom/queries-on3step.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/custom/js").Include(
                "~/Scripts/custom/pluginson3step.js",
                "~/Scripts/custom/bootstrap.min.js",
                "~/Scripts/custom/bootstrap-datepicker.min.js",
                "~/Scripts/custom/sticky.js",
                "~/Scripts/custom/jquery.themepunch.tools.min.js",
                "~/Scripts/custom/jquery.themepunch.revolution.min.js",
                "~/Scripts/custom/on3step.js",
                "~/Scripts/custom/plugin-set.js"
                        ));
        }
    }
}
