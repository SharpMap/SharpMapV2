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
    using Data.Providers.ShapeFile;
    using GeoAPI.CoordinateSystems;
    using GeoAPI.Geometries;
    using Layers;
    using ProjNet.CoordinateSystems;
    using Utilities;

    public static class MapHelper
    {
        public static void SetupMap(HttpContext context, Map map)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (map == null) 
                throw new ArgumentNullException("map");

            var services = new GeometryServices();
            var geoFactory = services.DefaultGeometryFactory;
            var csFactory = services.CoordinateSystemFactory;

            var layers = new[] { "poly_landmarks", "tiger_roads", "poi" };
            foreach (var layer in layers)
            {
                var format = String.Format("~/App_Data/nyc/{0}.shp", layer);
                var path = context.Server.MapPath(format);                
                var shapefile = new ShapeFileProvider(path, geoFactory, csFactory, false) { IsSpatiallyIndexed = false };                
                var provider = shapefile; // new AppStateMonitoringFeatureProvider(shapeFile);                

                var style = RandomStyle.RandomGeometryStyle();
                style.IncludeAttributes = false;
                style.IncludeBBox = true;
                style.PreProcessGeometries = false;
                style.CoordinateNumberFormatString = "{0:F}";

                var item = new GeometryLayer(layer, style, provider);
                item.Features.IsSpatiallyIndexed = false;
                map.AddLayer(item);
                provider.Open();
            }
        }

        private static ICoordinateSystem GetWgs84Projection(ICoordinateSystemFactory csFactory, IGeometryFactory geoFactory)
        {
            if (csFactory == null)
                throw new ArgumentNullException("csFactory");
            if (geoFactory == null)
                throw new ArgumentNullException("geoFactory");

            return csFactory.CreateGeographicCoordinateSystem(CommonGeographicCoordinateSystems.Wgs84);            
        }

        private static ICoordinateSystem GetMercatorProjection(ICoordinateSystemFactory csFactory, IGeometryFactory geoFactory)
        {
            if (csFactory == null)
                throw new ArgumentNullException("csFactory");
            if (geoFactory == null)
                throw new ArgumentNullException("geoFactory");

            var parameters = new List<ProjectionParameter> {
                new ProjectionParameter("semi_major", 6378137),
                new ProjectionParameter("semi_minor", 6378137),
                new ProjectionParameter("latitude_of_origin", 0.0),
                new ProjectionParameter("central_meridian", 0.0),
                new ProjectionParameter("scale_factor", 1.0),
                new ProjectionParameter("false_easting", 0.0),
                new ProjectionParameter("false_northing", 0.0)
            };
            var projection = csFactory.CreateProjection("Mercator", parameters, "mercator_1sp");
            var gcs = csFactory.CreateGeographicCoordinateSystem(
                geoFactory.CreateExtents2D(-2.003750834E7, -2.003750834E7, -2.003750834E7, -2.003750834E7),
                AngularUnit.Degrees,
                HorizontalDatum.Wgs84,
                PrimeMeridian.Greenwich,
                new AxisInfo(AxisOrientation.North, "north"),
                new AxisInfo(AxisOrientation.East, "east"),
                "WGS 84");
            var mercator = csFactory.CreateProjectedCoordinateSystem(                
                gcs,
                projection,
                LinearUnit.Meter,
                new AxisInfo(AxisOrientation.East, "east"),
                new AxisInfo(AxisOrientation.North, "North"),
                "Mercator");
            return mercator;
        }
        
    }
}