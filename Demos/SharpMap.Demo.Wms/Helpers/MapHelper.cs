/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

namespace SharpMap.Demo.Wms.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Data.Providers;
    using Data.Providers.ShapeFile;
    using Layers;
    using Styles;
    using Utilities;

    public static class MapHelper
    {
        private static readonly IDictionary<string, GeometryStyle> styles = new Dictionary<string, GeometryStyle>();

        public static void SetupMap(HttpContext context, Map map)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (map == null)
                throw new ArgumentNullException("map");

            const bool indexed = false;

            var services = new GeometryServices();
            var geoFactory = services.DefaultGeometryFactory;
            var csFactory = services.CoordinateSystemFactory;

            var layers = new[] 
            {
                "poly_landmarks",
                "tiger_roads", 
                "poi",
            };
            foreach (var layer in layers)
            {
                var format = String.Format("~/App_Data/nyc/{0}.shp", layer);
                var path = context.Server.MapPath(format);
                var provider = new ShapeFileProvider(path, geoFactory, csFactory, indexed) { IsSpatiallyIndexed = indexed };
                // var provider = new MsSqlServer2008Provider<int>(geoFactory, connstring, "dbo", layer, "uid", "geom");

                if (!styles.ContainsKey(layer))
                {
                    lock (styles)
                    {
                        if (!styles.ContainsKey(layer))
                        {
                            var style = RandomStyle.RandomGeometryStyle();
                            style.IncludeAttributes = true;
                            style.IncludeBBox = true;
                            style.PreProcessGeometries = true;
                            style.CoordinateNumberFormatString = "{0:F}";
                            styles.Add(layer, style);
                        }
                    }
                }

                var item = new GeometryLayer(layer, styles[layer], new AppStateMonitoringFeatureProvider(provider));
                item.Features.IsSpatiallyIndexed = indexed;
                map.AddLayer(item);
                provider.Open(WriteAccess.ReadOnly);
            }
        }
    }
}