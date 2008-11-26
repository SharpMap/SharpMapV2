using System;
using System.Web;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Presentation.AspNet.Demo
{
    public class Global : HttpApplication
    {
        static Global()
        {
            //jd: hopefully a temporary measure
            SridMap.DefaultInstance =
                new SridMap(new[] { new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory) });
        }

        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}