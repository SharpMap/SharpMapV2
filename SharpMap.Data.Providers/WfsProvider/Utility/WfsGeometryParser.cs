// WFS provider by Peter Robineau (peter.robineau@gmx.at)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.WfsProvider.Utility.Xml;

namespace SharpMap.Data.Providers.WfsProvider.Utility
{
    /// <summary>
    /// This class is the base class for geometry production.
    /// It provides some parsing routines for XML compliant to GML2/GML3.
    /// </summary>
    internal abstract class WfsGeometryParser : IDisposable
    {
        #region Fields

        protected const string GmlNameSpace = "http://www.opengis.net/gml";

        protected readonly IGeometryFactory GeometryFactory;
        private readonly ICoordinateSequenceFactory _coordSequenceFactory;
        private readonly ICoordinateFactory _coordFactory;

        private readonly NumberFormatInfo _formatInfo = new NumberFormatInfo();
        private readonly WfsHttpClientUtility _httpClientUtil;
        private readonly List<IPathNode> _pathNodes = new List<IPathNode>();
        protected AlternativePathNodesCollection CoordinatesNode;
        private string _cs;
        protected IPathNode FeatureNode;
        protected XmlReader FeatureReader;
        protected WfsFeatureTypeInfo FeatureTypeInfo;
        protected XmlReader GeomReader;

        protected Collection<IGeometry> Geoms = new Collection<IGeometry>();

        protected FeatureDataTable LabelInfo;
        protected IPathNode LabelNode;
        protected AlternativePathNodesCollection ServiceExceptionNode;
        private string _ts;
        protected XmlReader XmlReader;

        #endregion

        #region Constructors

        /// <summary>
        /// Protected constructor for the abstract class.
        /// </summary>
        /// <param name="geometryFactory">The factory to create the feature geometries</param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        protected WfsGeometryParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                  FeatureDataTable labelInfo)
        {
            GeometryFactory = geometryFactory;
            _coordFactory = geometryFactory.CoordinateFactory;
            _coordSequenceFactory = geometryFactory.CoordinateSequenceFactory;

            FeatureTypeInfo = featureTypeInfo;
            _httpClientUtil = httpClientUtil;
            CreateReader(httpClientUtil);

            try
            {
                if (labelInfo != null)
                {
                    LabelInfo = labelInfo;
                    LabelNode = new PathNode(FeatureTypeInfo.FeatureTypeNamespace, LabelInfo.Columns[0].ColumnName,
                                              (NameTable) XmlReader.NameTable);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while initializing the label path node!");
                throw ex;
            }

            InitializePathNodes();
            InitializeSeparators();
        }

        /// <summary>
        /// Protected constructor for the abstract class.
        /// </summary>
        /// <param name="geometryFactory">The factory to create the feature geometries</param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        protected WfsGeometryParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
        {
            GeometryFactory = geometryFactory;
            _coordFactory = geometryFactory.CoordinateFactory;
            _coordSequenceFactory = geometryFactory.CoordinateSequenceFactory;

            FeatureTypeInfo = featureTypeInfo;
            XmlReader = xmlReader;
            InitializePathNodes();
            InitializeSeparators();
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// Abstract method - overwritten by derived classes for producing instances
        /// derived from <see cref="IGeometry"/>.
        /// </summary>
        internal abstract Collection<IGeometry> CreateGeometries();

        /// <summary>
        /// This method parses quickly without paying attention to
        /// context validation, polygon boundaries and multi-geometries.
        /// This accelerates the geometry parsing process, 
        /// but in scarce cases can lead to errors. 
        /// </summary>
        /// <param name="geometryType">The geometry type (Point, LineString, Polygon, MultiPoint, MultiCurve, 
        /// MultiLineString (deprecated), MultiSurface, MultiPolygon (deprecated)</param>
        /// <returns>The created geometries</returns>
        internal virtual Collection<IGeometry> CreateQuickGeometries(string geometryType)
        {
            // Ignore multi-geometries
            if (geometryType.Equals("MultiPointPropertyType")) geometryType = "PointPropertyType";
            else if (geometryType.Equals("MultiLineStringPropertyType")) geometryType = "LineStringPropertyType";
            else if (geometryType.Equals("MultiPolygonPropertyType")) geometryType = "PolygonPropertyType";
            else if (geometryType.Equals("MultiCurvePropertyType")) geometryType = "CurvePropertyType";
            else if (geometryType.Equals("MultiSurfacePropertyType")) geometryType = "SurfacePropertyType";

            while (XmlReader.Read())
            {
                if (CoordinatesNode.Matches(XmlReader))
                {
                    try
                    {
                        switch (geometryType)
                        {
                            case "PointPropertyType":
                                Geoms.Add(GeometryFactory.CreatePoint(ParseCoordinates(XmlReader.ReadSubtree())[0]));
                                break;
                            case "LineStringPropertyType":
                            case "CurvePropertyType":
                                Geoms.Add(GeometryFactory.CreateLineString(ParseCoordinates(XmlReader.ReadSubtree())));
                                break;
                            case "PolygonPropertyType":
                            case "SurfacePropertyType":
                                ILinearRing exteriorRing = GeometryFactory.CreateLinearRing(ParseCoordinates(XmlReader.ReadSubtree()));
                                Geoms.Add(GeometryFactory.CreatePolygon(exteriorRing));
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("An exception occured while parsing a " + geometryType + " geometry: " +
                                         ex.Message);
                        throw ex;
                    }
                    continue;
                }

                if (ServiceExceptionNode.Matches(XmlReader))
                {
                    string serviceException = XmlReader.ReadInnerXml();
                    Trace.TraceError("A service exception occured: " + serviceException);
                    throw new Exception("A service exception occured: " + serviceException);
                }
            }

            return Geoms;
        }

        #endregion

        #region Protected Member

        /// <summary>
        /// This method parses a coordinates or posList(from 'GetFeature' response). 
        /// </summary>
        /// <param name="reader">An XmlReader instance at the position of the coordinates to read</param>
        /// <returns>A point collection (the collected coordinates)</returns>
        protected ICoordinateSequence ParseCoordinates(XmlReader reader)
        {
            if (!reader.Read()) return null;

            string name = reader.LocalName;
            string coordinateString = reader.ReadElementString();
            ICoordinateSequence vertices = _coordSequenceFactory.Create(CoordinateDimensions.Two);
            int i = 0;

            string[] coordinateValues = name.Equals("coordinates") ? coordinateString.Split(_cs[0], _ts[0]) : coordinateString.Split(' ');

            int length = coordinateValues.Length;

            while (i < length - 1)
            {
                double c1 = Convert.ToDouble(coordinateValues[i++], _formatInfo);
                double c2 = Convert.ToDouble(coordinateValues[i++], _formatInfo);

                vertices.Add(name.Equals("coordinates") ? _coordFactory.Create(c1, c2) : _coordFactory.Create(c2, c1));
            }

            return vertices;
        }

        /// <summary>
        /// This method retrieves an XmlReader within a specified context.
        /// </summary>
        /// <param name="reader">An XmlReader instance that is the origin of a created sub-reader</param>
        /// <param name="labelValue">A string array for recording a found label value. Pass 'null' to ignore searching for label values</param>
        /// <param name="pathNodes">A list of <see cref="PathNodeDepr"/> instances defining the context of the retrieved reader</param>
        /// <returns>A sub-reader of the XmlReader given as argument</returns>
        protected XmlReader GetSubReaderOf(XmlReader reader, string[] labelValue, params IPathNode[] pathNodes)
        {
            _pathNodes.Clear();
            _pathNodes.AddRange(pathNodes);
            return GetSubReaderOf(reader, labelValue, _pathNodes);
        }

        /// <summary>
        /// This method retrieves an XmlReader within a specified context.
        /// Moreover it collects label values before or after a geometry could be found.
        /// </summary>
        /// <param name="reader">An XmlReader instance that is the origin of a created sub-reader</param>
        /// <param name="labelValue">A string array for recording a found label value. Pass 'null' to ignore searching for label values</param>
        /// <param name="pathNodes">A list of <see cref="PathNodeDepr"/> instances defining the context of the retrieved reader</param>
        /// <returns>A sub-reader of the XmlReader given as argument</returns>
        protected XmlReader GetSubReaderOf(XmlReader reader, string[] labelValue, List<IPathNode> pathNodes)
        {
            string errorMessage = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (pathNodes[0].Matches(reader))
                    {
                        pathNodes.RemoveAt(0);

                        if (pathNodes.Count > 0)
                            return GetSubReaderOf(reader.ReadSubtree(), null, pathNodes);

                        return reader.ReadSubtree();
                    }

                    if (labelValue != null)
                        if (LabelNode != null)
                            if (LabelNode.Matches(reader))
                                labelValue[0] = reader.ReadElementString();


                    if (ServiceExceptionNode.Matches(reader))
                    {
                        errorMessage = reader.ReadInnerXml();
                        Trace.TraceError("A service exception occured: " + errorMessage);
                        throw new Exception("A service exception occured: " + errorMessage);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This method adds a label to the collection.
        /// </summary>
        protected void AddLabel(string labelValue, IGeometry geom)
        {
            if (LabelInfo == null || geom == null || string.IsNullOrEmpty(labelValue)) return;

            try
            {
                FeatureDataRow row = LabelInfo.NewRow();
                row[0] = labelValue;
                row.Geometry = geom;
                LabelInfo.AddRow(row);
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while adding a label to the collection!");
                throw ex;
            }
        }

        #endregion

        #region Private Member

        /// <summary>
        /// This method initializes the XmlReader member.
        /// </summary>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        private void CreateReader(WfsHttpClientUtility httpClientUtil)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreComments = true;
            xmlReaderSettings.IgnoreProcessingInstructions = true;
            xmlReaderSettings.IgnoreWhitespace = true;
            xmlReaderSettings.ProhibitDtd = true;
            XmlReader = XmlReader.Create(httpClientUtil.GetDataStream(), xmlReaderSettings);
        }

        /// <summary>
        /// This method initializes path nodes needed by the derived classes.
        /// </summary>
        private void InitializePathNodes()
        {
            IPathNode coordinatesNode = new PathNode("http://www.opengis.net/gml", "coordinates",
                                                     (NameTable) XmlReader.NameTable);
            IPathNode posListNode = new PathNode("http://www.opengis.net/gml", "posList",
                                                 (NameTable) XmlReader.NameTable);
            IPathNode ogcServiceExceptionNode = new PathNode("http://www.opengis.net/ogc", "ServiceException",
                                                             (NameTable) XmlReader.NameTable);
            IPathNode serviceExceptionNode = new PathNode("", "ServiceException", (NameTable) XmlReader.NameTable);
                //ServiceExceptions without ogc prefix are returned by deegree. PDD.
            IPathNode exceptionTextNode = new PathNode("http://www.opengis.net/ows", "ExceptionText",
                                                       (NameTable) XmlReader.NameTable);
            CoordinatesNode = new AlternativePathNodesCollection(coordinatesNode, posListNode);
            ServiceExceptionNode = new AlternativePathNodesCollection(ogcServiceExceptionNode, exceptionTextNode,
                                                                       serviceExceptionNode);
            FeatureNode = new PathNode(FeatureTypeInfo.FeatureTypeNamespace, FeatureTypeInfo.Name,
                                        (NameTable) XmlReader.NameTable);
        }

        /// <summary>
        /// This method initializes separator variables for parsing coordinates.
        /// From GML specification: Coordinates can be included in a single string, but there is no 
        /// facility for validating string content. The value of the 'cs' attribute 
        /// is the separator for coordinate values, and the value of the 'ts' 
        /// attribute gives the tuple separator (a single space by default); the 
        /// default values may be changed to reflect local usage.
        /// </summary>
        private void InitializeSeparators()
        {
            string decimalDel = string.IsNullOrEmpty(FeatureTypeInfo.DecimalDel) ? ":" : FeatureTypeInfo.DecimalDel;
            _cs = string.IsNullOrEmpty(FeatureTypeInfo.Cs) ? "," : FeatureTypeInfo.Cs;
            _ts = string.IsNullOrEmpty(FeatureTypeInfo.Ts) ? " " : FeatureTypeInfo.Ts;
            _formatInfo.NumberDecimalSeparator = decimalDel;
        }

        #endregion

        #region IDisposable Member

        /// <summary>
        /// This method closes the XmlReader member and the used <see cref="WfsHttpClientUtility"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (XmlReader != null)
                XmlReader.Close();
            if (_httpClientUtil != null)
                _httpClientUtil.Close();
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="GeoAPI.Geometries.IPoint"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsPointParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsPointParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsPointParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                              FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsPointParser"/> class.
        /// This constructor shall just be called from the MultiPoint factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsPointParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
            FeatureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoAPI.Geometries.IPoint"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode pointNode = new PathNode(GmlNameSpace, "Point", (NameTable) XmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    bool geomFound = false;
                    while ((GeomReader = GetSubReaderOf(FeatureReader, labelValue, pointNode, CoordinatesNode)) !=
                           null)
                    {
                        Geoms.Add(GeometryFactory.CreatePoint(ParseCoordinates(GeomReader)[0]));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a point geometry string: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="GeoAPI.Geometries.ILineString"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsLineStringParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsLineStringParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsLineStringParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                   FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsLineStringParser"/> class.
        /// This constructor shall just be called from the MultiLineString factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsLineStringParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
            FeatureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoAPI.Geometries.ILineString"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode lineStringNode = new PathNode(GmlNameSpace, "LineString", (NameTable) XmlReader.NameTable);
            string[] labelValue = new string[1];
            bool geomFound = false;

            try
            {
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    while (
                        (GeomReader = GetSubReaderOf(FeatureReader, labelValue, lineStringNode, CoordinatesNode)) !=
                        null)
                    {
                        Geoms.Add(GeometryFactory.CreateLineString(ParseCoordinates(GeomReader)));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                    geomFound = false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a line geometry string: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="GeoAPI.Geometries.IPolygon"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsPolygonParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsPolygonParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsPolygonParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsPolygonParser"/> class.
        /// This constructor shall just be called from the MultiPolygon factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsPolygonParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
            FeatureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoAPI.Geometries.IPolygon"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode polygonNode = new PathNode(GmlNameSpace, "Polygon", (NameTable) XmlReader.NameTable);
            IPathNode outerBoundaryNode = new PathNode(GmlNameSpace, "outerBoundaryIs", (NameTable) XmlReader.NameTable);
            IPathNode exteriorNode = new PathNode(GmlNameSpace, "exterior", (NameTable) XmlReader.NameTable);
            IPathNode outerBoundaryNodeAlt = new AlternativePathNodesCollection(outerBoundaryNode, exteriorNode);
            IPathNode innerBoundaryNode = new PathNode(GmlNameSpace, "innerBoundaryIs", (NameTable) XmlReader.NameTable);
            IPathNode interiorNode = new PathNode(GmlNameSpace, "interior", (NameTable) XmlReader.NameTable);
            IPathNode innerBoundaryNodeAlt = new AlternativePathNodesCollection(innerBoundaryNode, interiorNode);
            IPathNode linearRingNode = new PathNode(GmlNameSpace, "LinearRing", (NameTable) XmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    bool geomFound = false;
                    while ((GeomReader = GetSubReaderOf(FeatureReader, labelValue, polygonNode)) != null)
                    {
                        ILinearRing exteriorRing = null;
                        XmlReader outerBoundaryReader;
                        if (
                            (outerBoundaryReader =
                             GetSubReaderOf(GeomReader, null, outerBoundaryNodeAlt, linearRingNode, CoordinatesNode)) !=
                            null)
                            exteriorRing = GeometryFactory.CreateLinearRing(ParseCoordinates(outerBoundaryReader));

                        List<ILinearRing> holes = new List<ILinearRing>();
                        XmlReader innerBoundariesReader;
                        while (
                            (innerBoundariesReader =
                             GetSubReaderOf(GeomReader, null, innerBoundaryNodeAlt, linearRingNode, CoordinatesNode)) !=
                            null)
                            holes.Add(GeometryFactory.CreateLinearRing(ParseCoordinates(innerBoundariesReader)));

                        Geoms.Add(GeometryFactory.CreatePolygon(exteriorRing, holes));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a polygon geometry: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="GeoAPI.Geometries.IMultiPoint"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsMultiPointParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiPointParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsMultiPointParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                   FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiPointParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsMultiPointParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoAPI.Geometries.IMultiPoint"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode multiPointNode = new PathNode(GmlNameSpace, "MultiPoint", (NameTable) XmlReader.NameTable);
            IPathNode pointMemberNode = new PathNode(GmlNameSpace, "pointMember", (NameTable) XmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ICoordinateSequence coordinateSequence =
                    GeometryFactory.CoordinateSequenceFactory.Create(CoordinateDimensions.Two);
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    bool geomFound = false;
                    while (
                        (GeomReader = GetSubReaderOf(FeatureReader, labelValue, multiPointNode, pointMemberNode)) !=
                        null)
                    {
                        WfsGeometryParser geomFactory = new WfsPointParser(GeometryFactory, GeomReader, FeatureTypeInfo);
                        foreach (IGeometry geometry in geomFactory.CreateGeometries())
                        {
                            coordinateSequence.Add(geometry.Coordinates);
                        } 

                        Geoms.Add(GeometryFactory.CreateMultiPoint(coordinateSequence));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a multi-point geometry: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        #endregion
    }

    /// <summary>
    /// This class produces objects of type <see cref="GeoAPI.Geometries.IMultiLineString"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsMultiLineStringParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiLineStringParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsMultiLineStringParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                        FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiLineStringParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsMultiLineStringParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoAPI.Geometries.IMultiLineString"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode multiLineStringNode = new PathNode(GmlNameSpace, "MultiLineString", (NameTable) XmlReader.NameTable);
            IPathNode multiCurveNode = new PathNode(GmlNameSpace, "MultiCurve", (NameTable) XmlReader.NameTable);
            IPathNode multiLineStringNodeAlt = new AlternativePathNodesCollection(multiLineStringNode, multiCurveNode);
            IPathNode lineStringMemberNode = new PathNode(GmlNameSpace, "lineStringMember", (NameTable) XmlReader.NameTable);
            IPathNode curveMemberNode = new PathNode(GmlNameSpace, "curveMember", (NameTable) XmlReader.NameTable);
            IPathNode lineStringMemberNodeAlt = new AlternativePathNodesCollection(lineStringMemberNode, curveMemberNode);
            string[] labelValue = new string[1];
            bool geomFound = false;

            try
            {
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    while (
                        (GeomReader =
                         GetSubReaderOf(FeatureReader, labelValue, multiLineStringNodeAlt, lineStringMemberNodeAlt)) !=
                        null)
                    {
                        WfsGeometryParser geomFactory = new WfsLineStringParser(GeometryFactory, GeomReader, FeatureTypeInfo);
                        Collection<IGeometry> lineStrings = geomFactory.CreateGeometries();

                        Geoms.Add(GeometryFactory.CreateMultiLineString(CastToLinestring(lineStrings)));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                    geomFound = false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a multi-lineString geometry: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        private static IEnumerable<ILineString> CastToLinestring(IEnumerable<IGeometry> geometrys)
        {
            foreach (IGeometry geometry in geometrys)
            {
                if (geometry is ILineString) yield return geometry as ILineString;
            }
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="GeoAPI.Geometries.IMultiPolygon"/>.
    /// The base class is <see cref="WfsGeometryParser"/>.
    /// </summary>
    internal class WfsMultiPolygonParser : WfsGeometryParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiPolygonParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="httpClientUtil">A configured <see cref="WfsHttpClientUtility"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsMultiPolygonParser(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil, WfsFeatureTypeInfo featureTypeInfo,
                                     FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsMultiPolygonParser"/> class.
        /// </summary>
        /// <param name="geometryFactory"></param>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal WfsMultiPolygonParser(IGeometryFactory geometryFactory, XmlReader xmlReader, WfsFeatureTypeInfo featureTypeInfo)
            : base(geometryFactory, xmlReader, featureTypeInfo)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="GeoPAI.Geometries.IMultiPolygon"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            IPathNode multiPolygonNode = new PathNode(GmlNameSpace, "MultiPolygon", (NameTable) XmlReader.NameTable);
            IPathNode multiSurfaceNode = new PathNode(GmlNameSpace, "MultiSurface", (NameTable) XmlReader.NameTable);
            IPathNode multiPolygonNodeAlt = new AlternativePathNodesCollection(multiPolygonNode, multiSurfaceNode);
            IPathNode polygonMemberNode = new PathNode(GmlNameSpace, "polygonMember", (NameTable) XmlReader.NameTable);
            IPathNode surfaceMemberNode = new PathNode(GmlNameSpace, "surfaceMember", (NameTable) XmlReader.NameTable);
            IPathNode polygonMemberNodeAlt = new AlternativePathNodesCollection(polygonMemberNode, surfaceMemberNode);
            IPathNode linearRingNode = new PathNode(GmlNameSpace, "LinearRing", (NameTable) XmlReader.NameTable);
            string[] labelValue = new string[1];
            bool geomFound = false;

            try
            {
                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((FeatureReader = GetSubReaderOf(XmlReader, null, FeatureNode)) != null)
                {
                    while (
                        (GeomReader =
                         GetSubReaderOf(FeatureReader, labelValue, multiPolygonNodeAlt, polygonMemberNodeAlt)) != null)
                    {
                        WfsGeometryParser geomFactory = new WfsPolygonParser(GeometryFactory, GeomReader, FeatureTypeInfo);
                        Collection<IGeometry> polygons = geomFactory.CreateGeometries();

                        Geoms.Add(GeometryFactory.CreateMultiPolygon(CastToPolygons(polygons)));
                        geomFound = true;
                    }
                    if (geomFound) AddLabel(labelValue[0], Geoms[Geoms.Count - 1]);
                    geomFound = false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An exception occured while parsing a multi-polygon geometry: " + ex.Message);
                throw ex;
            }

            return Geoms;
        }

        private static IEnumerable<IPolygon> CastToPolygons(Collection<IGeometry> geometrys)
        {
            foreach (IGeometry geometry in geometrys)
            {
                if (geometry is ILineString) yield return geometry as IPolygon;
            }
        }

        #endregion
    }

    /// <summary>
    /// This class must detect the geometry type of the queried layer.
    /// Therefore it works a bit slower than the other factories. Specify the geometry type manually,
    /// if it isn't specified in 'DescribeFeatureType'.
    /// </summary>
    internal class WfsUnspecifiedGeometryParser_WFS1_0_0_GML2 : WfsGeometryParser
    {
        #region Fields

        private readonly WfsHttpClientUtility _HttpClientUtil;
        private readonly bool _QuickGeometries;
        private bool _MultiGeometries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsUnspecifiedGeometryParser_WFS1_0_0_GML2"/> class.
        /// </summary>
        /// <param name="httpClientUtil">A configured <see cref="HttpClientUtil"/> instance for performing web requests</param>
        /// <param name="featureTypeInfo">A <see cref="WfsFeatureTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="multiGeometries">A boolean value specifying whether multi-geometries should be created</param>
        /// <param name="quickGeometries">A boolean value specifying whether the factory should create geometries quickly, but without validation</param>
        /// <param name="labelInfo">A FeatureDataTable for labels</param>
        internal WfsUnspecifiedGeometryParser_WFS1_0_0_GML2(IGeometryFactory geometryFactory, WfsHttpClientUtility httpClientUtil,
                                                          WfsFeatureTypeInfo featureTypeInfo, bool multiGeometries,
                                                          bool quickGeometries, FeatureDataTable labelInfo)
            : base(geometryFactory, httpClientUtil, featureTypeInfo, labelInfo)
        {
            _HttpClientUtil = httpClientUtil;
            _MultiGeometries = multiGeometries;
            _QuickGeometries = quickGeometries;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method detects the geometry type from 'GetFeature' response and uses a geometry factory to create the 
        /// appropriate geometries.
        /// </summary>
        /// <returns></returns>
        internal override Collection<IGeometry> CreateGeometries()
        {
            WfsGeometryParser geomFactory = null;

            string geometryTypeString = string.Empty;
            string serviceException;

            if (_QuickGeometries) _MultiGeometries = false;

            IPathNode pointNode = new PathNode(GmlNameSpace, "Point", (NameTable) XmlReader.NameTable);
            IPathNode lineStringNode = new PathNode(GmlNameSpace, "LineString", (NameTable) XmlReader.NameTable);
            IPathNode polygonNode = new PathNode(GmlNameSpace, "Polygon", (NameTable) XmlReader.NameTable);
            IPathNode multiPointNode = new PathNode(GmlNameSpace, "MultiPoint", (NameTable) XmlReader.NameTable);
            IPathNode multiLineStringNode = new PathNode(GmlNameSpace, "MultiLineString", (NameTable) XmlReader.NameTable);
            IPathNode multiCurveNode = new PathNode(GmlNameSpace, "MultiCurve", (NameTable) XmlReader.NameTable);
            IPathNode multiLineStringNodeAlt = new AlternativePathNodesCollection(multiLineStringNode, multiCurveNode);
            IPathNode multiPolygonNode = new PathNode(GmlNameSpace, "MultiPolygon", (NameTable) XmlReader.NameTable);
            IPathNode multiSurfaceNode = new PathNode(GmlNameSpace, "MultiSurface", (NameTable) XmlReader.NameTable);
            IPathNode multiPolygonNodeAlt = new AlternativePathNodesCollection(multiPolygonNode, multiSurfaceNode);

            while (XmlReader.Read())
            {
                if (XmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_MultiGeometries)
                    {
                        if (multiPointNode.Matches(XmlReader))
                        {
                            geomFactory = new WfsMultiPointParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                            geometryTypeString = "MultiPointPropertyType";
                            break;
                        }
                        if (multiLineStringNodeAlt.Matches(XmlReader))
                        {
                            geomFactory = new WfsMultiLineStringParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                            geometryTypeString = "MultiLineStringPropertyType";
                            break;
                        }
                        if (multiPolygonNodeAlt.Matches(XmlReader))
                        {
                            geomFactory = new WfsMultiPolygonParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                            geometryTypeString = "MultiPolygonPropertyType";
                            break;
                        }
                    }

                    if (pointNode.Matches(XmlReader))
                    {
                        geomFactory = new WfsPointParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                        geometryTypeString = "PointPropertyType";
                        break;
                    }
                    if (lineStringNode.Matches(XmlReader))
                    {
                        geomFactory = new WfsLineStringParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                        geometryTypeString = "LineStringPropertyType";
                        break;
                    }
                    if (polygonNode.Matches(XmlReader))
                    {
                        geomFactory = new WfsPolygonParser(GeometryFactory, _HttpClientUtil, FeatureTypeInfo, LabelInfo);
                        geometryTypeString = "PolygonPropertyType";
                        break;
                    }
                    if (ServiceExceptionNode.Matches(XmlReader))
                    {
                        serviceException = XmlReader.ReadInnerXml();
                        Trace.TraceError("A service exception occured: " + serviceException);
                        throw new Exception("A service exception occured: " + serviceException);
                    }
                }
            }

            FeatureTypeInfo.WfsGeometry.GeometryType = geometryTypeString;

            if (geomFactory != null)
                return _QuickGeometries
                           ? geomFactory.CreateQuickGeometries(geometryTypeString)
                           : geomFactory.CreateGeometries();
            return Geoms;
        }

        #endregion
    }
}