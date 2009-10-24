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
using System.Configuration;
using System.Web;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public static class DemoMapSetupUtility
    {
        /// <summary>
        /// little util wich just adds one vector layer to the map and assigns it a random theme.
        /// </summary>
        public static void SetupMap(HttpContext context, Map m)
        {
            setupMsSqlSpatial(m);
            //setupShapefile(context, m);
        }

        private static void setupShapefile(HttpContext context, Map m)
        {
            GeometryServices geometryServices = new GeometryServices();

            string[] layernames = new[]
                                      {
                                          "Countries",
                                          "Rivers"/*,
                                          "Cities"*/
                                      };

            foreach (string s in layernames)
            {
                ShapeFileProvider shapeFile =
               new ShapeFileProvider(context.Server.MapPath(string.Format("~/App_Data/Shapefiles/{0}.shp", s)),
                                     geometryServices.DefaultGeometryFactory,
                                     geometryServices.CoordinateSystemFactory, false);
                shapeFile.IsSpatiallyIndexed = false;

                AppStateMonitoringFeatureProvider provider = new AppStateMonitoringFeatureProvider(shapeFile);

                GeoJsonGeometryStyle style = RandomStyle.RandomGeometryStyle();
                /* include GeoJson styles */
                style.IncludeAttributes = false;
                style.IncludeBBox = true;
                style.PreProcessGeometries = false;
                style.CoordinateNumberFormatString = "{0:F}";

                GeometryLayer geometryLayer = new GeometryLayer(s, style, provider);
                geometryLayer.Features.IsSpatiallyIndexed = false;
                m.AddLayer(geometryLayer);
                provider.Open();
            }

        }

        private static void setupMsSqlSpatial(Map m)
        {
            string[] layernames = new[]
                                      {"Rivers",
                                          "Countries"
                                          /*,
                                          "Cities"*/
                                      };

            string sridstr = SridMap.DefaultInstance.Process(4326, "");

            foreach (string lyrname in layernames)
            {
                string tbl = lyrname;


                AppStateMonitoringFeatureProvider provider = new AppStateMonitoringFeatureProvider(
                    new MsSqlSpatialProvider(
                        new GeometryServices()[sridstr],
                        ConfigurationManager.ConnectionStrings["db"].ConnectionString,
                        "st",
                        "dbo",
                        tbl,
                        "oid",
                        "Geometry")
                        {
                            DefaultProviderProperties
                                = new ProviderPropertiesExpression(
                                new ProviderPropertyExpression[]
                                    {
                                        new WithNoLockExpression(true)
                                    })
                        });


                GeoJsonGeometryStyle style = new GeoJsonGeometryStyle();

                switch (tbl)
                {
                    case "Rivers":
                        {
                            StyleBrush brush = new SolidStyleBrush(StyleColor.Blue);
                            StylePen pen = new StylePen(brush, 1);
                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }

                    case "Countries":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(0, 0, 0, 255));
                            StylePen pen = new StylePen(brush, 2);
                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = new SolidStyleBrush(StyleColor.Green);

                            break;
                        }

                    default:
                        {
                            style = RandomStyle.RandomGeometryStyle();
                            style.MaxVisible = 100000;
                            break;
                        }
                }


                /* include GeoJson styles */
                style.IncludeAttributes = false;
                style.IncludeBBox = true;
                style.PreProcessGeometries = false;
                style.CoordinateNumberFormatString = "{0:F}";
                GeometryLayer layer = new GeometryLayer(tbl, style, provider);
                layer.Features.IsSpatiallyIndexed = false;
                layer.AddProperty(AppStateMonitoringFeatureLayerProperties.AppStateMonitor, provider.Monitor);
                m.AddLayer(layer);
            }
        }
    }
}