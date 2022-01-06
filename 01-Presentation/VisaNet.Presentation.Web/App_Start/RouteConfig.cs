using System.Web.Mvc;
using System.Web.Routing;

namespace VisaNet.Presentation.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               "Apps",                                           // Route name
               "Apps/{id}",                            // URL with parameters
              new { controller = "AppAdmission", action = "Index" }
              //,new { httpMethod = new HttpMethodConstraint("POST") }// Parameter defaults
          );
            routes.MapRoute(
               "AppsGet",                                           // Route name
               "AppsGet/{id}",                            // URL with parameters
              new { controller = "AppAdmission", action = "Add" },
              new { httpMethod = new HttpMethodConstraint("GET") } // Parameter defaults
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }

        
    }
}
