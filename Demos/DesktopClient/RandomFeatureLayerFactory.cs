using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DemoWinForm.Properties;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NPack;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace DemoWinForm
{
    class RandomFeatureLayerFactory : ILayerFactory
    {
        private class LayerResources
        {
            public String LayerName;
            public GeometryStyle Style;
            public Dictionary<DataColumn, StreamReader> AttributeCache;
        }

        private static readonly String[] _availableLayers = new String[]
            {
                "Marshland", "Robot Tracks", "Roads", "Fires",
                "Treasures", "Nice Views", "Fault Lines", "Contamination",
                "Notices", "Radioactive Fuel Rods", "Bases", "Houses", 
                "Measures", "Contacts", "Prospects"
            };

        private static Dictionary<String, LayerResources> _configuredLayers
            = new Dictionary<String, LayerResources>();

        private static List<DataColumn> _attributePool = new List<DataColumn>();
        private static readonly Dictionary<String, StyleColor> _colorTable
            = StyleColor.PredefinedColors;

        private static readonly Random _rnd = new MersenneTwister();

        private readonly StreamReader _givenNamesFemale;
        private readonly StreamReader _familyNames;
        private readonly StreamReader _placeNames;

        private readonly IGeometryFactory _geometryFactory;

        static RandomFeatureLayerFactory()
        {
            initialize();
        }

        public RandomFeatureLayerFactory(IGeometryFactory geometryFactory)
        {
            _geometryFactory = geometryFactory;
        }

        #region ILayerFactory Members

        public ILayer Create(String layerName, String connectionInfo)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal static void GetLayerNameAndInfo(out String layerName, out String connectionInfo)
        {
            throw new NotImplementedException();
        }

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
                                            new DataColumn("Name", typeof (String)),
                                            new DataColumn("Rank", typeof (Int32)),
                                            new DataColumn("YearsKnown", typeof (Single)),
                                            new DataColumn("LastVisited", typeof (DateTime)),
                                            new DataColumn("Criticality", typeof(Double)),
                                            new DataColumn("Health", typeof(Double)),
                                            new DataColumn("Risk", typeof(Double)),
                                            new DataColumn("Urgency", typeof(Double)),
                                            new DataColumn("Strength", typeof(Int32)),
                                            new DataColumn("Priority", typeof(Int32)),
                                            new DataColumn("Available", typeof(bool)),
                                            new DataColumn("Price", typeof(Decimal)),
                                            new DataColumn("Square Meterage", typeof(Double)),
                                            new DataColumn("Expiration Date", typeof(DateTime)),
                                            new DataColumn("Description", typeof(String))
                                        });

            foreach (String layer in _availableLayers)
            {
                LayerResources resources = new LayerResources();
                resources.LayerName = layer;
                resources.Style = generateStyle(layer);
                resources.AttributeCache = generateAttributes(layer);
            }
        }

        private static Dictionary<DataColumn, StreamReader> generateAttributes(String layer)
        {
            Dictionary<DataColumn, StreamReader> attributes = new Dictionary<DataColumn, StreamReader>();

            // Assign some attribute columns
            switch (layer)
            {
                case "Marshland":
                    if (_rnd.NextDouble() > 0.5) attributes.Add(clone(_attributePool[0]));
                    break;
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

            return attributes;
        }

        private static GeometryStyle generateStyle(String layer)
        {
            GeometryStyle style = new GeometryStyle();
            style.Enabled = true;

            switch (layer)
            {
                case "Marshland":
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
                    style.Fill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.HighlightFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.SelectFill = new SolidStyleBrush(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value);
                    style.Outline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.HighlightOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    style.SelectOutline = new StylePen(getColorEntry(_rnd.Next(0, _colorTable.Count)).Value, _rnd.NextDouble() * 6);
                    break;
            }

            return style;
        }


        private void registerSymbols()
        {
            _symbolTable["Notices"] = new Symbol2D(getStreamFromImage(Resources.Chat),
                                                   new Size2D(Resources.Chat.Height, 
                                                              Resources.Chat.Width));
            _symbolTable["Radioactive Fuel Rods"] = new Symbol2D(getStreamFromImage(Resources.DATABASE),
                                                                 new Size2D(Resources.DATABASE.Height, 
                                                                            Resources.DATABASE.Width));
            _symbolTable["Bases"] = new Symbol2D(getStreamFromImage(Resources.Flag),
                                                 new Size2D(Resources.Flag.Height, 
                                                            Resources.Flag.Width));
            _symbolTable["Houses"] = new Symbol2D(getStreamFromImage(Resources.Home),
                                                  new Size2D(Resources.Home.Height, 
                                                             Resources.Home.Width));
            _symbolTable["Measures"] = new Symbol2D(getStreamFromImage(Resources.PIE_DIAGRAM),
                                                    new Size2D(Resources.PIE_DIAGRAM.Height, 
                                                               Resources.PIE_DIAGRAM.Width));
            _symbolTable["Contacts"] = new Symbol2D(getStreamFromImage(Resources.Women),
                                                    new Size2D(Resources.Women.Height, 
                                                               Resources.Women.Width));
            _symbolTable["Prospects"] = new Symbol2D(getStreamFromImage(Resources.Women_1),
                                                     new Size2D(Resources.Women_1.Height, 
                                                                Resources.Women_1.Width));
        }

        private ILayer generateFeatureLayer()
        {
            VectorStyle style = new VectorStyle();
            String layerName;

            switch (_rnd.Next(3))
            {
                case 0:
                    {
                        KeyValuePair<String, Symbol2D> symbolEntry = getSymbolEntry(_rnd.Next(_symbolTable.Count));
                        style.Symbol = symbolEntry.Value;
                        layerName = symbolEntry.Key;
                    }
                    break;
                case 1:
                    {
                        KeyValuePair<String, StyleColor> colorEntry = getColorEntry(_rnd.Next(_colorTable.Count));
                        style.Line = new StylePen(colorEntry.Value, _rnd.NextDouble() * 3);
                        layerName = String.Format("{0} lines", colorEntry.Key);
                    }
                    break;
                case 2:
                    {
                        KeyValuePair<String, StyleColor> colorEntry = getColorEntry(_rnd.Next(_colorTable.Count));
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

        private IFeatureProvider generateFeatureDataSource()
        {
            List<DataColumn> featureAttributes = new List<DataColumn>(generateAttributeSchema());
            FeatureProvider provider = new FeatureProvider(_geometryFactory, featureAttributes.ToArray());
            generateFeatures(provider);
            return provider;
        }

        private void generateFeatures(FeatureProvider provider)
        {
            Collection<IGeometry> geometry = new Collection<IGeometry>();

            switch (_rnd.Next(3))
            {
                case 0:
                    generatePoints(_geometryFactory, geometry, _rnd);
                    break;
                case 1:
                    generateLines(_geometryFactory, geometry, _rnd);
                    break;
                case 2:
                    generatePolygons(_geometryFactory, geometry, _rnd);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<DataColumn> generateAttributeSchema()
        {
            Double threshold = 3 / _attributePool.Count;
            foreach (DataColumn column in _attributePool)
            {
                if (_rnd.NextDouble() <= threshold)
                {
                    yield return column;
                }
            }
        }

        private KeyValuePair<String, Symbol2D> getSymbolEntry(Int32 index)
        {
            foreach (KeyValuePair<String, Symbol2D> entry in _symbolTable)
            {
                if (index-- == 0)
                {
                    return entry;
                }
            }

            throw new InvalidOperationException();
        }

        private static KeyValuePair<String, StyleColor> getColorEntry(Int32 index)
        {
            foreach (KeyValuePair<String, StyleColor> entry in _colorTable)
            {
                if (index-- == 0)
                {
                    return entry;
                }
            }

            throw new InvalidOperationException();
        }

        private static void generatePolygons(IGeometryFactory geometryFactory,
                                             ICollection<IGeometry> geometry, 
                                             Random rndGen)
        {
            ICoordinateSequenceFactory coordinateSequenceFactory =
                geometryFactory.CoordinateSequenceFactory;
            ICoordinateFactory coordinateFactory = geometryFactory.CoordinateFactory;
            ICoordinateSequence coords = coordinateSequenceFactory.Create(CoordinateDimensions.Two);

            Int32 polyCount = rndGen.Next(10, 100);

            for (Int32 polyIndex = 0; polyIndex < polyCount; polyIndex++)
            {
                ICoordinate upperLeft = coordinateFactory.Create(rndGen.NextDouble() * 1000,
                                                                  rndGen.NextDouble() * 1000);

                Double sideLength = rndGen.NextDouble() * 50;

                // Make a square
                coords.Add(upperLeft);
                coords.Add(coordinateFactory.Create(upperLeft[Ordinates.X] + sideLength,
                                                    upperLeft[Ordinates.Y]));
                coords.Add(coordinateFactory.Create(upperLeft[Ordinates.X] + sideLength,
                                                    upperLeft[Ordinates.Y] - sideLength));
                coords.Add(coordinateFactory.Create(upperLeft[Ordinates.X],
                                                    upperLeft[Ordinates.Y] - sideLength));

                IPolygon polygon = geometryFactory.CreatePolygon(coords);
                geometry.Add(polygon);
            }
        }

        private static void generateLines(IGeometryFactory geometryFactory,
                                          ICollection<IGeometry> geometry,
                                          Random rndGen)
        {
            ICoordinateSequenceFactory coordinateSequenceFactory =
                geometryFactory.CoordinateSequenceFactory;
            ICoordinateFactory coordinateFactory = geometryFactory.CoordinateFactory;
            ICoordinateSequence coords = coordinateSequenceFactory.Create(CoordinateDimensions.Two);

            Int32 lineCount = rndGen.Next(10, 100);

            for (Int32 lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                Int32 vertexCount = rndGen.Next(4, 15);

                ICoordinate coordinate = coordinateFactory.Create(rndGen.NextDouble() * 1000,
                                                                  rndGen.NextDouble() * 1000);
                coords.Add(coordinate);

                for (Int32 vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                {
                    ICoordinate next = coordinateFactory.Create(coordinate[Ordinates.X] + rndGen.Next(-50, 50), 
                                                                coordinate[Ordinates.Y] + rndGen.Next(-50, 50));
                    coords.Add(next);
                    coordinate = next;
                }

                ILineString line = geometryFactory.CreateLineString(coords);

                geometry.Add(line);
            }
        }

        private static void generatePoints(IGeometryFactory geometryFactory,
                                           ICollection<IGeometry> geometry,
                                           Random rndGen)
        {
            Int32 numPoints = rndGen.Next(10, 100);

            for (Int32 pointIndex = 0; pointIndex < numPoints; pointIndex++)
            {
                IPoint point = geometryFactory.CreatePoint2D(rndGen.NextDouble() * 1000, rndGen.NextDouble() * 1000);
                geometry.Add(point);
            }
        }
    }
}
