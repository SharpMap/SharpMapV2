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
using SharpMap.Data.Providers.MsSqlServer2008.Expressions;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public static class DemoMapSetupUtility
    {
        /// <summary>
        /// little util wich just adds one vector layer to the map and assigns it a random theme.
        /// </summary>
        public static void SetupMap(HttpContext context, Map m)
        {
            //setupMsSqlSpatial2008(m);
            setupShapefile(context, m);
        }

        private static void setupShapefile(HttpContext context, Map m)
        {
            GeometryServices geometryServices = new GeometryServices();
            ShapeFileProvider shapeFile = new ShapeFileProvider(context.Server.MapPath("~/App_Data/Shapefiles/BCRoads.shp"),
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

            GeometryLayer geometryLayer = new GeometryLayer("BCRoads", style, provider);
            geometryLayer.Features.IsSpatiallyIndexed = false;
            m.AddLayer(geometryLayer);
            provider.Open();
        }

        private static void setupMsSqlSpatial2008(Map m)
        {
            var layernames = new[]
                                 {
                                     "Capital",
                                     "Cemeteries",
                                     "ConventionExhibition",
                                     "Government",
                                     "Historical",
                                     "Hospitals",
                                     "MajorTowns",
                                     "MidTowns",
                                     "NamedPlaces",
                                     "Airports",
                                     "ParkCity",
                                     "ParkLeisure",
                                     "Retail",
                                     "Shops",
                                     "SmallPlaces",
                                     "SportsLandmark",
                                     "Sports",
                                     "TopTowns",
                                     "Tourist",
                                     "Universities",
                                     "Villages",
                                     "Woodland",
                                     "Motorways",
                                     "ARoads",
                                     "UnclassifiedRoads",
                                     "BRoads",
                                     "Streets",
                                     "Rivers",
                                     "ROIBoundary",
                                     "PrimaryRoads",
                                     "Transportation",
                                     "Ferries",
                                     "AircraftRoads",
                                     "Canals",
                                     "Cultural",
                                     "Medical",
                                     "Lakes",
                                     "IndustrialAreas",
                                     "Education",
                                     "Districts",
                                     "Counties",
                                     "BusinessCommerce",
                                     "CanalAreas",
                                     "RiverAreas",
                                     "TownSprawl",
                                     "ParkMonument",
                                     "Elevation1000To1500m",
                                     "Elevation900m",
                                     "Elevation800m",
                                     "Elevation700m",
                                     "Elevation600m",
                                     "Elevation500m",
                                     "Elevation400m",
                                     "Elevation300m",
                                     "Elevation200m",
                                     "Elevation100m",
                                     "Bays",
                                     "Country",
                                     "Golf",
                                     "Islands",
                                     "Oceans"
                                 };

            //Array.Reverse(layernames);

            var labelprovider = new AppStateMonitoringFeatureProvider(
                new MsSqlServer2008Provider<long>(
                    new GeometryServices()["27700"],
                    ConfigurationManager.ConnectionStrings["db"].ConnectionString,
                    "dbo",
                    "STREETS",
                    "oid",
                    "Geom")
                    {
                        DefaultProviderProperties
                            = new ProviderPropertiesExpression(
                            new ProviderPropertyExpression[]
                                {
                                    new WithNoLockExpression(true),
                                    new ForceIndexExpression(true),
                                    new IndexNamesExpression(new[] {"sidx_Streets_Geom"}),
                                    new MsSqlServer2008ExtentsModeExpression(
                                        SqlServer2008ExtentsMode.UseEnvelopeColumns)
                                })
                    });


            var labellayer = new LabelLayer("streetslabel", labelprovider)
                                 {
                                     CollisionDetector = new LabelCollisionDetection2D(),
                                 };
            labellayer.Enabled = true;
            labellayer.Features.IsSpatiallyIndexed = false;
            labellayer.MultipartGeometryLabelingBehaviour = MultipartGeometryLabelingBehavior.Largest;

            var lblstyle =
                new LabelStyle(new StyleFont(new StyleFontFamily("Arial"), new Size2D(10, 10), StyleFontStyle.Bold),
                               RandomStyle.RandomBrush());
            lblstyle.Enabled = true;

            lblstyle.LabelExpression = new PropertyNameExpression("NAME");
            lblstyle.MaxVisible = 100000;

            labellayer.Style = lblstyle;

            m.AddLayer(labellayer);

            foreach (string lyrname in layernames)
            {
                string tbl = lyrname;
                string col = "Geom";

                var provider = new AppStateMonitoringFeatureProvider(
                    new MsSqlServer2008Provider<long>(
                        new GeometryServices()["27700"],
                        ConfigurationManager.ConnectionStrings["db"].ConnectionString,
                        "dbo",
                        tbl,
                        "oid",
                        col)
                        {
                            DefaultProviderProperties
                                = new ProviderPropertiesExpression(
                                new ProviderPropertyExpression[]
                                    {
                                        new WithNoLockExpression(true),
                                        new ForceIndexExpression(true),
                                        new IndexNamesExpression(new[] {"sidx_" + tbl + "_" + col}),
                                        new MsSqlServer2008ExtentsModeExpression(
                                            SqlServer2008ExtentsMode.UseEnvelopeColumns)
                                    })
                        });

                //string col = "WKB_Geometry";
                //var provider = new AppStateMonitoringFeatureProvider(
                //    new MsSqlSpatialProvider(
                //        new GeometryServices()[27700],
                //        ConfigurationManager.ConnectionStrings["db"].ConnectionString,
                //        "ST",
                //        "dbo",
                //        tbl,
                //        "oid",
                //        col,
                //        true));

                var style = new GeoJsonGeometryStyle();

                switch (tbl)
                {
                    case "ARoads":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(126, 120, 119, 255));
                            var pen = new StylePen(brush, 9);
                            style.EnableOutline = true;
                            style.MaxVisible = 100000;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }


                    case "BRoads":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(126, 120, 119, 255));
                            var pen = new StylePen(brush, 7);
                            style.EnableOutline = true;
                            style.MaxVisible = 100000;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }

                    case "PrimaryRoads":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(126, 120, 119, 255));
                            var pen = new StylePen(brush, 9);
                            style.EnableOutline = true;
                            style.MaxVisible = 100000;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }

                    case "Motorways":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(255, 0, 0, 255));
                            var pen = new StylePen(brush, 12);
                            style.EnableOutline = true;
                            style.MaxVisible = 100000;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }

                    case "UnclassifiedRoads":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(171, 173, 156, 255));
                            var pen = new StylePen(brush, 5);
                            style.EnableOutline = true;
                            style.MaxVisible = 10000;
                            style.Line = pen;
                            style.Fill = brush;
                            break;
                        }

                    case "Rivers":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(255, 255, 0, 255));
                            var pen = new StylePen(brush, 5);
                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = brush;
                            style.MaxVisible = 100000;
                            break;
                        }

                    case "Country":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(0, 0, 0, 255));
                            var pen = new StylePen(brush, 2);
                            StyleBrush trans = new SolidStyleBrush(new StyleColor(255, 255, 255, 0));
                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = trans;

                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation100m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(179, 231, 254, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation200m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(165, 224, 253, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation300m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(115, 211, 252, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation400m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(45, 201, 241, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation500m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(46, 183, 241, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation600m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(45, 136, 239, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation700m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(45, 144, 237, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation800m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(45, 125, 236, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation900m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(44, 107, 236, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Elevation1000to1500m":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(44, 75, 235, 255));
                            var pen = new StylePen(brush, 1);

                            style.EnableOutline = false;
                            style.Outline = pen;
                            style.Fill = brush;
                            style.MinVisible = 100000;
                            break;
                        }

                    case "Counties":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(0, 0, 0, 255));
                            var pen = new StylePen(brush, 2);
                            StyleBrush trans = new SolidStyleBrush(new StyleColor(234, 255, 255, 255));

                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = trans;

                            style.MaxVisible = 100000;
                            break;
                        }

                    case "ROIBoundary":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(0, 0, 0, 255));
                            var pen = new StylePen(brush, 2);
                            StyleBrush trans = new SolidStyleBrush(new StyleColor(255, 255, 255, 0));

                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = trans;

                            break;
                        }

                    case "Oceans":
                        {
                            StyleBrush brush = new SolidStyleBrush(new StyleColor(254, 251, 194, 255));
                            var pen = new StylePen(brush, 2);

                            style.Outline = pen;
                            style.Enabled = true;
                            style.EnableOutline = true;
                            style.Line = pen;
                            style.Fill = brush;

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

                var layer = new GeometryLayer(tbl, style, provider);
                layer.Features.IsSpatiallyIndexed = false;
                layer.AddProperty(AppStateMonitoringFeatureLayerProperties.AppStateMonitor, provider.Monitor);
                m.AddLayer(layer);
            }
        }
    }
}