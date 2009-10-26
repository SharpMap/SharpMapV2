using System;
using System.Web;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Utilities;

namespace SharpMap.Presentation.AspNet.Demo.GeoJson
{
    public class GeoJsonRequestConfigFactory : IMapRequestConfigFactory<GeoJsonRequestConfig>
    {
        #region IMapRequestConfigFactory<GeoJsonRequestConfig> Members

        public GeoJsonRequestConfig CreateConfig(HttpContext context)
        {
            GeoJsonRequestConfig config = new GeoJsonRequestConfig();
            string soutputsize = context.Request.QueryString["outputsize"];

            bool useDefaultSize = true;
            if (!string.IsNullOrEmpty(soutputsize))
            {
                string[] parts = soutputsize.Split('x');

                if (parts.Length == 2)
                {
                    int width, height;
                    if (int.TryParse(parts[0], out width) && int.TryParse(parts[1], out height))
                    {
                        config.OutputSize = new Size2D(width, height);
                        useDefaultSize = false;
                    }
                }
            }
            if (useDefaultSize)
                config.OutputSize = new Size2D(400, 400);

            if (context.Request["BBOX"] != null)
            {
                config.RealWorldBounds
                    = Common.Utility.ParseExtents(
                        new GeometryServices().DefaultGeometryFactory,
                        context.Request["BBOX"]);
            }


            //ensure that the differences in the string diverges quickly by reversing it.
            char[] arr = context.Request.Url.PathAndQuery.ToLower().ToCharArray();
            Array.Reverse(arr);

            config.BaseSrid = "EPSG:4326";

            config.CacheKey = new string(arr);

            return config;
        }

        IMapRequestConfig IMapRequestConfigFactory.CreateConfig(HttpContext context)
        {
            return CreateConfig(context);
        }

        #endregion
    }
}