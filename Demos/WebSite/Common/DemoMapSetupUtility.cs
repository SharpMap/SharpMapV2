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
using System.Data;
using System.Data.SqlClient;
using System.Web;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Presentation.AspNet.Demo.Common
{
    public static class DemoMapSetupUtility
    {
        private static DataTable _mssqlTables;


        public static DataTable MsSqlSpatialTables
        {
            get
            {
                _mssqlTables = _mssqlTables ?? GetTables();
                return _mssqlTables;
            }
        }

        private static DataTable GetTables()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            using (
                var da =
                    new SqlDataAdapter(
                        "SELECT GC.*, ZIndex FROM  ST.GEOMETRY_COLUMNS GC INNER JOIN LayerStackingOrder lso on GC.id = lso.id order by ZIndex desc",
                        conn)
                )
            {
                conn.Open();

                var dt = new DataTable();
                da.Fill(dt);
                dt.DefaultView.Sort = "ZIndex DESC";
                return dt;
            }
        }


        /// <summary>
        /// little util wich just adds one vector layer to the map and assigns it a random theme.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="m"></param>
        public static void SetupMap(HttpContext context, Map m)
        {
            foreach (DataRowView r in MsSqlSpatialTables.DefaultView)
            {
                string tbl = string.Format("{0}", r["F_TABLE_NAME"]);
                var col = (string)r["F_GEOMETRY_COLUMN"];

                var provider = new AppStateMonitoringFeatureProvider(
                    new MsSqlSpatialProvider(
                        new GeometryServices().DefaultGeometryFactory,
                        ConfigurationManager.ConnectionStrings["db"].ConnectionString,
                        "ST",
                        "dbo",
                        tbl,
                        "oid",
                        col,
                        true
                        ));
                GeoJsonGeometryStyle style = new GeoJsonGeometryStyle(); ;

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
