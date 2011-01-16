using System.Web.Mvc;
using System.Web.Routing;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Demo.Wms
{
	public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Default", "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            var services = new GeometryServices();
            var factory = services.CoordinateSystemFactory;
            var strategy = new SridProj4Strategy(0, factory);
            SridMap.DefaultInstance = new SridMap(new[] { strategy });
    
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }
    }
}