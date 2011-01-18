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

using System;
using System.Web;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Layers;
using SharpMap.Utilities;

namespace SharpMap.Demo.Wms.Helpers
{
    public static class MapHelper
    {
        public static void SetupMap(HttpContext context, Map map)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (map == null) 
                throw new ArgumentNullException("map");
                        
            var geometryServices = new GeometryServices();
            var gfactory = geometryServices.DefaultGeometryFactory;
            var csfactory = geometryServices.CoordinateSystemFactory;

            var layers = new[] { "poly_landmarks", "tiger_roads", "poi" };
            foreach (var layer in layers)
            {
                var format = String.Format("~/App_Data/nyc/{0}.shp", layer);
                var path = context.Server.MapPath(format);                
                var shapeFile = new ShapeFileProvider(path, gfactory, csfactory, false) { IsSpatiallyIndexed = true };
                var provider = shapeFile; // new AppStateMonitoringFeatureProvider(shapeFile);

                var style = RandomStyle.RandomGeometryStyle();
                style.IncludeAttributes = false;
                style.IncludeBBox = true;
                style.PreProcessGeometries = false;
                style.CoordinateNumberFormatString = "{0:F}";

                var item = new GeometryLayer(layer, style, provider);
                item.Features.IsSpatiallyIndexed = true;
                map.AddLayer(item);
                provider.Open();
            }
        }
    }
}