using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DemoWinForm.Properties;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.SimpleGeometries;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GeoPoint = SharpMap.SimpleGeometries.Point;

namespace DemoWinForm
{
    class RandomFeatureLayerFactory : ILayerFactory
    {
        private class LayerResources
        {
            public String LayerName;
            public VectorStyle Style;
            public Dictionary<DataColumn, StreamReader> AttributeCache;
        }

        private static readonly string[] _availableLayers = new string[]
            {
                "Marshland", "Robot Tracks", "Roads", "Fires",
                "Treasures", "Nice Views", "Fault Lines", "Contamination",
                "Notices", "Radioactive Fuel Rods", "Bases", "Houses", 
                "Measures", "Contacts", "Prospects"
            };

        private static Dictionary<string, LayerResources> _configuredLayers
            = new Dictionary<string, LayerResources>();

        private static List<DataColumn> _attributePool = new List<DataColumn>();
        private static readonly Dictionary<string, StyleColor> _colorTable
            = StyleColor.PredefinedColors;

        private static readonly Random _rnd = new Random();

        private readonly StreamReader _givenNamesFemale;
        private readonly StreamReader _familyNames;
        private readonly StreamReader _placeNames;

        static RandomFeatureLayerFactory()
        {
            initialize();
        }

        public RandomFeatureLayerFactory()
        {
        }

        #region ILayerFactory Members

        public ILayer Create(string layerName, string connectionInfo)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static Stream getStreamFromImage(Image bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream;
        }

        private static void initialize()
        {
            _attributePool.AddRange(new DataColumn[]
                                        {
                                            new DataColumn("Name", typeof (string)),
                                            new DataColumn("Rank", typeof (int)),
                                            new DataColumn("YearsKnown", typeof (float)),
                                            new DataColumn("LastVisited", typeof (DateTime)),
                                            new DataColumn("Criticality", typeof(double)),
                                            new DataColumn("Health", typeof(double)),
                                            new DataColumn("Risk", typeof(double)),
                                            new DataColumn("Urgency", typeof(double)),
                                            new DataColumn("Strength", typeof(int)),
                                            new DataColumn("Priority", typeof(int)),
                                            new DataColumn("Available", typeof(bool)),
                                            new DataColumn("Price", typeof(Decimal)),
                                            new DataColumn("Square Meterage", typeof(double)),
                                            new DataColumn("Expiration Data", typeof(DateTime)),
                                            new DataColumn("Description", typeof(string))
                                        });

            foreach (string layer in _availableLayers)
            {
                LayerResources resources = new LayerResources();
                resources.LayerName = layer;
                resources.Style = generateStyle(layer);
                resources.AttributeCache = generateAttributes(layer);
            }
        }

        private static Dictionary<DataColumn, StreamReader> generateAttributes(string layer)
        {
            Dictionary<DataColumn, StreamReader> attributes = new Dictionary<DataColumn, StreamReader>();

            // Assign some attribute columns
            switch (layer)
            {
                case "Marshland":
                    if (_rnd.NextDouble() > 0.5) attributes.Add(clone(_attributePool[0]), );
                case "Robot Tracks":
                case "Roads":
                case "Fires":
                case "Treasures":
                case "Nice Views":
                case "Fault Lines":
                case "Contamination":
                case "Notices":
                case "Radioactive Fuel Rods":
                case "Bases":
                case "Houses":
                case "Measures":
                case "Contacts":
                case "Prospects":
                default:
                    break;
            }
        }

        private static VectorStyle generateStyle(string layer)
        {
            VectorStyle style = new VectorStyle();
            style.Enabled = true;

            switch (layer)
            {
                case "Marshland":
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
                case "Robot Tracks":
                case "Roads":
                case "Fires":
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
                case "Treasures":
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
                case "Nice Views":
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
                case "Fault Lines":
                case "Contamination":
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
                case "Notices":
                case "Radioactive Fuel Rods":
                case "Bases":
                case "Houses":
                case "Measures":
                case "Contacts":
                case "Prospects":
                default:
                    break;
            }
        }


        private void registerSymbols()
        {
            _symbolTable["Notices"] = new Symbol2D(getStreamFromImage(Resources.Chat),
                                                   new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Radioactive Fuel Rods"] = new Symbol2D(getStreamFromImage(Resources.DATABASE),
                                                                 new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Bases"] = new Symbol2D(getStreamFromImage(Resources.Flag),
                                                 new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Houses"] = new Symbol2D(getStreamFromImage(Resources.Home),
                                                  new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Measures"] = new Symbol2D(getStreamFromImage(Resources.PIE_DIAGRAM),
                                                    new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Contacts"] = new Symbol2D(getStreamFromImage(Resources.Women),
                                                    new Size2D(Resources.Chat.Height, Resources.Chat.Width));
            _symbolTable["Prospects"] = new Symbol2D(getStreamFromImage(Resources.Women_1),
                                                     new Size2D(Resources.Chat.Height, Resources.Chat.Width));
        }

        private ILayer generateFeatureLayer()
        {
            VectorStyle style = new VectorStyle();
            string layerName;

            switch (_rnd.Next(3))
            {
                case 0:
                    {
                        KeyValuePair<string, Symbol2D> symbolEntry = getSymbolEntry(_rnd.Next(_symbolTable.Count));
                        style.Symbol = symbolEntry.Value;
                        layerName = symbolEntry.Key;
                    }
                    break;
                case 1:
                    {
                        KeyValuePair<string, StyleColor> colorEntry = getColorEntry(_rnd.Next(_colorTable.Count));
                        style.Line = new StylePen(colorEntry.Value, _rnd.NextDouble() * 3);
                        layerName = String.Format("{0} lines", colorEntry.Key);
                    }
                    break;
                case 2:
                    {
                        KeyValuePair<string, StyleColor> colorEntry = getColorEntry(_rnd.Next(_colorTable.Count));
                        style.Fill = new SolidStyleBrush(colorEntry.Value);
                        layerName = String.Format("{0} squares", colorEntry.Key);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            GeometryLayer layer = new GeometryLayer(layerName, generateFeatureDataSource());
            return layer;
        }

        private IFeatureLayerProvider generateFeatureDataSource()
        {
            List<DataColumn> featureAttributes = new List<DataColumn>(generateAttributeSchema());
            FeatureProvider provider = new FeatureProvider(featureAttributes.ToArray());
            generateFeatures(provider);
            return provider;
        }

        private void generateFeatures(FeatureProvider provider)
        {
            Collection<Geometry> geometry = new Collection<Geometry>();

            switch (_rnd.Next(3))
            {
                case 0:
                    generatePoints(geometry, _rnd);
                    break;
                case 1:
                    generateLines(geometry, _rnd);
                    break;
                case 2:
                    generatePolygons(geometry, _rnd);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<DataColumn> generateAttributeSchema()
        {
            double threshold = 3 / _attributePool.Count;
            foreach (DataColumn column in _attributePool)
            {
                if (_rnd.NextDouble() <= threshold)
                {
                    yield return column;
                }
            }
        }

        private KeyValuePair<string, Symbol2D> getSymbolEntry(int index)
        {
            foreach (KeyValuePair<string, Symbol2D> entry in _symbolTable)
            {
                if (index-- == 0)
                {
                    return entry;
                }
            }

            throw new InvalidOperationException();
        }

        private static KeyValuePair<string, StyleColor> getColorEntry(int index)
        {
            foreach (KeyValuePair<string, StyleColor> entry in _colorTable)
            {
                if (index-- == 0)
                {
                    return entry;
                }
            }

            throw new InvalidOperationException();
        }

        private static void generatePolygons(ICollection<Geometry> geometry, Random rndGen)
        {
            int numPolygons = rndGen.Next(10, 100);
            for (int polyIndex = 0; polyIndex < numPolygons; polyIndex++)
            {
                Polygon polygon = new Polygon();
                Collection<GeoPoint> verticies = new Collection<GeoPoint>();
                GeoPoint upperLeft = new GeoPoint(rndGen.NextDouble() * 1000, rndGen.NextDouble() * 1000);
                double sideLength = rndGen.NextDouble() * 50;

                // Make a square
                verticies.Add(upperLeft);
                verticies.Add(new GeoPoint(upperLeft.X + sideLength, upperLeft.Y));
                verticies.Add(new GeoPoint(upperLeft.X + sideLength, upperLeft.Y - sideLength));
                verticies.Add(new GeoPoint(upperLeft.X, upperLeft.Y - sideLength));
                polygon.ExteriorRing = new LinearRing(verticies);

                geometry.Add(polygon);
            }
        }

        private static void generateLines(Collection<Geometry> geometry, Random rndGen)
        {
            int numLines = rndGen.Next(10, 100);
            for (int lineIndex = 0; lineIndex < numLines; lineIndex++)
            {
                LineString line = new LineString();
                Collection<GeoPoint> vertexes = new Collection<GeoPoint>();

                int numVerticies = rndGen.Next(4, 15);

                GeoPoint lastPoint = new GeoPoint(rndGen.NextDouble() * 1000, rndGen.NextDouble() * 1000);
                vertexes.Add(lastPoint);

                for (int vertexIndex = 0; vertexIndex < numVerticies; vertexIndex++)
                {
                    GeoPoint nextPoint =
                        new GeoPoint(lastPoint.X + rndGen.Next(-50, 50), lastPoint.Y + rndGen.Next(-50, 50));
                    vertexes.Add(nextPoint);
                    lastPoint = nextPoint;
                }

                foreach (GeoPoint point in vertexes)
                {
                    line.Vertices.Add(point);
                }

                geometry.Add(line);
            }
        }

        private static void generatePoints(Collection<Geometry> geometry, Random rndGen)
        {
            int numPoints = rndGen.Next(10, 100);
            for (int pointIndex = 0; pointIndex < numPoints; pointIndex++)
            {
                GeoPoint point = new GeoPoint(rndGen.NextDouble() * 1000, rndGen.NextDouble() * 1000);
                geometry.Add(point);
            }
        }

        internal static void GetLayerNameAndInfo(out string layerName, out string connectionInfo)
        {
            throw new NotImplementedException();
        }
    }
}
