using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Demo.Wms
{
	public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            var defaults = new { controller = "Home", action = "Index", id = UrlParameter.Optional };
            routes.MapRoute("Default", "{controller}/{action}/{id}", defaults);
        }

        protected void Application_Start()
        {
            GeometryServices services = new GeometryServices();
            ICoordinateSystemFactory coordinateSystemFactory = services.CoordinateSystemFactory;
            SridProj4Strategy strategy = new SridProj4Strategy(0, coordinateSystemFactory);    
            SridMap.DefaultInstance = new SridMap(new[] { strategy });
    
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }
    }
}