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
    using System.Configuration;
    using System.Web;
    using Data.Providers;
    using GeoAPI.CoordinateSystems;
    using GeoAPI.CoordinateSystems.Transformations;
    using GeoAPI.Geometries;
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

            GeometryServices services = new GeometryServices();
            IGeometryFactory geoFactory = services.DefaultGeometryFactory;
            ICoordinateSystemFactory csFactory = services.CoordinateSystemFactory;
            ICoordinateTransformationFactory ctFactory = services.CoordinateTransformationFactory;

            string[] layers = new[] { "poly_landmarks", "tiger_roads",  "poi", };
            foreach (string layer in layers)
            {
                // string format = String.Format("~/App_Data/nyc/{0}.shp", layer);
                // string path = context.Server.MapPath(format);
                // var provider = new ShapeFileProvider(path, geoFactory, csFactory, true) { IsSpatiallyIndexed = true };
                
                //ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["LocalSqlConnectionString"];
                //string connectionString = settings.ConnectionString;
                //MsSqlServer2008Provider<int> provider = new MsSqlServer2008Provider<int>(geoFactory, connectionString, 
                //    "dbo", layer, "UID", "geom") { CoordinateTransformationFactory = ctFactory };

                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["LocalPgisConnectionString"];
                string connectionString = settings.ConnectionString;
                PostGisProvider<int> provider = new PostGisProvider<int>(geoFactory, connectionString,
                    "public", layer, "gid", "the_geom") { CoordinateTransformationFactory = ctFactory };

                if (!styles.ContainsKey(layer))
                {
                    lock (styles)
                    {
                        if (!styles.ContainsKey(layer))
                        {
                            GeoJsonGeometryStyle style = RandomStyle.RandomGeometryStyle();
                            style.IncludeAttributes = true;
                            style.IncludeBBox = true;
                            style.PreProcessGeometries = true;
                            style.CoordinateNumberFormatString = "{0:F}";
                            styles.Add(layer, style);
                        }
                    }
                }

                AppStateMonitoringFeatureProvider monitor = new AppStateMonitoringFeatureProvider(provider);
                GeometryLayer item = new GeometryLayer(layer, styles[layer], monitor);
                item.Features.IsSpatiallyIndexed = true;
                map.AddLayer(item);
                provider.Open(/*WriteAccess.ReadOnly*/);
            }
        }
    }
}