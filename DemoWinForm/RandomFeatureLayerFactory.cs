using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using DemoWinForm.Properties;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GeoPoint = SharpMap.Geometries.Point;

namespace DemoWinForm
{
    class RandomFeatureLayerFactory : ILayerFactory
    {
        private readonly Random _rnd = new Random();
        private readonly Dictionary<string, Symbol2D> _symbolTable
            = new Dictionary<string, Symbol2D>();
        private readonly Dictionary<string, StyleColor> _colorTable
            = StyleColor.PredefinedColors;
        private readonly List<DataColumn> _attributePool = new List<DataColumn>();
        private readonly StreamReader _givenNamesFemale;
        private readonly StreamReader _familyNames;
        private readonly StreamReader _placeNames;

        public RandomFeatureLayerFactory()
        {
            initializeAttributePool();
            registerSymbols();
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

        private void initializeAttributePool()
        {
            _attributePool.AddRange(new DataColumn[]
                                        {
                                            new DataColumn("Name", typeof (string)),
                                            new DataColumn("Rank", typeof (int)),
                                            new DataColumn("YearsKnown", typeof (float)),
                                            new DataColumn("LastVisited", typeof (DateTime)),
                                        });
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

        private KeyValuePair<string, StyleColor> getColorEntry(int index)
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

                line.Vertices.AddRange(vertexes);

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
