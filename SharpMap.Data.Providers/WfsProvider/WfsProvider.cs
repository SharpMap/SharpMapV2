using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.WfsProvider.Utility;
using SharpMap.Data.Providers.WfsProvider.Utility.Xml.XPath;
using SharpMap.Expressions;
using SharpMap.Utilities.Wfs;

// WFS provider by Peter Robineau (peter.robineau@gmx.at)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
namespace SharpMap.Data.Providers.WfsProvider
{
    /// <summary>
    /// WFS dataprovider
    /// This provider can be used to obtain data from an OGC Web Feature Service.
    /// It performs the following requests: 'GetCapabilities', 'DescribeFeatureType' and 'GetFeature'.
    /// This class is optimized for performing requests to GeoServer (http://geoserver.org).
    /// Supported geometries are:
    /// - PointPropertyType
    /// - LineStringPropertyType
    /// - PolygonPropertyType
    /// - CurvePropertyType
    /// - SurfacePropertyType
    /// - MultiPointPropertyType
    /// - MultiLineStringPropertyType
    /// - MultiPolygonPropertyType
    /// - MultiCurvePropertyType
    /// - MultiSurfacePropertyType
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///SharpMap.Map demoMap;
    ///
    ///const string getCapabilitiesURI = "http://localhost:8080/geoserver/wfs";
    ///const string serviceURI = "http://localhost:8080/geoserver/wfs";
    ///
    ///demoMap = new SharpMap.Map(new Size(600, 600));
    ///demoMap.MinimumZoom = 0.005;
    ///demoMap.BackColor = Color.White;
    ///
    ///SharpMap.Layers.VectorLayer layer1 = new SharpMap.Layers.VectorLayer("States");
    ///SharpMap.Layers.VectorLayer layer2 = new SharpMap.Layers.VectorLayer("SelectedStatesAndHousholds");
    ///SharpMap.Layers.VectorLayer layer3 = new SharpMap.Layers.VectorLayer("New Jersey");
    ///SharpMap.Layers.VectorLayer layer4 = new SharpMap.Layers.VectorLayer("Roads");
    ///SharpMap.Layers.VectorLayer layer5 = new SharpMap.Layers.VectorLayer("Landmarks");
    ///SharpMap.Layers.VectorLayer layer6 = new SharpMap.Layers.VectorLayer("Poi");
    ///    
    /// // Demo data from Geoserver 1.5.3 and Geoserver 1.6.0 
    ///    
    ///WFS prov1 = new WFS(getCapabilitiesURI, "topp", "states", WFS.WfsVersion.WFS1_0_0);
    ///    
    /// // Bypass 'GetCapabilities' and 'DescribeFeatureType', if you know all necessary metadata.
    ///WfsFeatureTypeInfo featureTypeInfo = new WfsFeatureTypeInfo(serviceURI, "topp", null, "states", "the_geom");
    /// // 'WFS.WfsVersion.WFS1_1_0' supported by Geoserver 1.6.x
    ///WFS prov2 = new SharpMap.Data.Providers.WFS(featureTypeInfo, WFS.WfsVersion.WFS1_1_0);
    /// // Bypass 'GetCapabilities' and 'DescribeFeatureType' again...
    /// // It's possible to specify the geometry type, if 'DescribeFeatureType' does not...(.e.g 'GeometryAssociationType')
    /// // This helps to accelerate the initialization process in case of unprecise geometry information.
    ///WFS prov3 = new WFS(serviceURI, "topp", "http://www.openplans.org/topp", "states", "the_geom", GeometryTypeEnum.MultiSurfacePropertyType, WFS.WfsVersion.WFS1_1_0);
    ///
    /// // Get data-filled FeatureTypeInfo after initialization of dataprovider (useful in Web Applications for caching metadata.
    ///WfsFeatureTypeInfo info = prov1.FeatureTypeInfo;
    ///
    /// // Use cached 'GetCapabilities' response of prov1 (featuretype hosted by same service).
    /// // Compiled XPath expressions are re-used automatically!
    /// // If you use a cached 'GetCapabilities' response make sure the data provider uses the same version of WFS as the one providing the cache!!!
    ///WFS prov4 = new WFS(prov1.GetCapabilitiesCache, "tiger", "tiger_roads", WFS.WfsVersion.WFS1_0_0);
    ///WFS prov5 = new WFS(prov1.GetCapabilitiesCache, "tiger", "poly_landmarks", WFS.WfsVersion.WFS1_0_0);
    ///WFS prov6 = new WFS(prov1.GetCapabilitiesCache, "tiger", "poi", WFS.WfsVersion.WFS1_0_0);
    /// // Clear cache of prov1 - data providers do not have any cache, if they use the one of another data provider  
    ///prov1.GetCapabilitiesCache = null;
    ///
    /// //Filters
    ///IFilter filter1 = new PropertyIsEqualToFilter_FE1_1_0("STATE_NAME", "California");
    ///IFilter filter2 = new PropertyIsEqualToFilter_FE1_1_0("STATE_NAME", "Vermont");
    ///IFilter filter3 = new PropertyIsBetweenFilter_FE1_1_0("HOUSHOLD", "600000", "4000000");
    ///IFilter filter4 = new PropertyIsLikeFilter_FE1_1_0("STATE_NAME", "New*");
    ///
    /// // SelectedStatesAndHousholds: Green
    ///OGCFilterCollection filterCollection1 = new OGCFilterCollection();
    ///filterCollection1.AddFilter(filter1);
    ///filterCollection1.AddFilter(filter2);
    ///OGCFilterCollection filterCollection2 = new OGCFilterCollection();
    ///filterCollection2.AddFilter(filter3);
    ///filterCollection1.AddFilterCollection(filterCollection2);
    ///filterCollection1.Junctor = OGCFilterCollection.JunctorEnum.Or;
    ///prov2.OGCFilter = filterCollection1;
    ///
    /// // Like-Filter('New*'): Bisque
    ///prov3.OGCFilter = filter4;
    ///
    /// // Layer Style
    ///layer1.Style.Fill = new SolidBrush(Color.IndianRed);    // States
    ///layer2.Style.Fill = new SolidBrush(Color.Green); // SelectedStatesAndHousholds
    ///layer3.Style.Fill = new SolidBrush(Color.Bisque); // e.g. New York, New Jersey,...
    ///layer5.Style.Fill = new SolidBrush(Color.LightBlue);
    ///
    /// // Labels
    /// // Labels are collected when parsing the geometry. So there's just one 'GetFeature' call necessary.
    /// // Otherwise (when calling twice for retrieving labels) there may be an inconsistent read...
    /// // If a label property is set, the quick geometry option is automatically set to 'false'.
    ///prov3.Label = "STATE_NAME";
    ///SharpMap.Layers.LabelLayer layLabel = new SharpMap.Layers.LabelLayer("labels");
    ///layLabel.DataSource = prov3;
    ///layLabel.Enabled = true;
    ///layLabel.LabelColumn = prov3.Label;
    ///layLabel.Style = new SharpMap.Styles.LabelStyle();
    ///layLabel.Style.CollisionDetection = false;
    ///layLabel.Style.CollisionBuffer = new SizeF(5, 5);
    ///layLabel.Style.ForeColor = Color.Black;
    ///layLabel.Style.Font = new Font(FontFamily.GenericSerif, 10);
    ///layLabel.MaxVisible = 90;
    ///layLabel.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center;
    /// // Options 
    /// // Defaults: MultiGeometries: true, QuickGeometries: false, GetFeatureGETRequest: false
    /// // Render with validation...
    ///prov1.QuickGeometries = false;
    /// // Important when connecting to an UMN MapServer
    ///prov1.GetFeatureGETRequest = true;
    /// // Ignore multi-geometries...
    ///prov1.MultiGeometries = false;
    ///
    /// // Quick geometries
    /// // We need this option for prov2 since we have not passed a featuretype namespace
    ///prov2.QuickGeometries = true;
    ///prov4.QuickGeometries = true;
    ///prov5.QuickGeometries = true;
    ///prov6.QuickGeometries = true;
    ///
    ///layer1.DataSource = prov1;
    ///layer2.DataSource = prov2;
    ///layer3.DataSource = prov3;
    ///layer4.DataSource = prov4;
    ///layer5.DataSource = prov5;
    ///layer6.DataSource = prov6;
    ///
    ///demoMap.Layers.Add(layer1);
    ///demoMap.Layers.Add(layer2);
    ///demoMap.Layers.Add(layer3);
    ///demoMap.Layers.Add(layer4);
    ///demoMap.Layers.Add(layer5);
    ///demoMap.Layers.Add(layer6);
    ///demoMap.Layers.Add(layLabel);
    ///
    ///demoMap.Center = new SharpMap.Geometries.Point(-74.0, 40.7);
    ///demoMap.Zoom = 10;
    /// // Alternatively zoom closer
    /// // demoMap.Zoom = 0.2;
    /// // Render map
    ///this.mapImage1.Image = demoMap.GetMap();
    /// </code> 
    ///</example>
    public class WfsProvider : ProviderBase
    {

        #region Enumerations

        /// <summary>
        /// This enumeration consists of expressions denoting WFS versions.
        /// </summary>
        public enum WfsVersion
        {
// ReSharper disable InconsistentNaming
            WFS1_0_0,
            WFS1_1_0
// ReSharper restore InconsistentNaming
        } ;

        #endregion

        #region Fields

        private readonly IGeometryFactory _geometryFactory;

        // Info about the featuretype to query obtained from 'GetCapabilites' and 'DescribeFeatureType'

        private readonly WfsGeometryType _geometryType = WfsGeometryType.Unknown;
        private readonly string _getCapabilitiesUri;
        private readonly WfsHttpClientUtility _httpClientUtil = new WfsHttpClientUtility();
        private readonly IWfsTextResources _textResources;

        private readonly WfsVersion _wfsVersion;

        private string _featureType;
        private WfsFeatureTypeInfo _featureTypeInfo;
        private IXPathQueryManager _featureTypeInfoQueryManager;
        private FeatureDataTable _labelInfo;

        private string _nsPrefix;

        // The type of geometry can be specified in case of unprecise information (e.g. 'GeometryAssociationType').
        // It helps to accelerate the rendering process significantly.

        #endregion

        #region Properties

        private bool _getFeatureGETRequest;
        private string _label;
        private bool _multiGeometries = true;
        private IFilter _ogcFilter;
        private bool _quickGeometries;

        /// <summary>
        /// This cache (obtained from an already instantiated dataprovider that retrieves a featuretype hosted by the same service) 
        /// helps to speed up gathering metadata. It caches the 'GetCapabilities' response. 
        /// </summary>
        internal IXPathQueryManager GetCapabilitiesCache
        {
            get { return _featureTypeInfoQueryManager; }
            set { _featureTypeInfoQueryManager = value; }
        }

        /// <summary>
        /// Gets feature metadata 
        /// </summary>
        public WfsFeatureTypeInfo FeatureTypeInfo
        {
            get { return _featureTypeInfo; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether extracting geometry information 
        /// from 'GetFeature' response shall be done quickly without paying attention to
        /// context validation, polygon boundaries and multi-geometries.
        /// This option accelerates the geometry parsing process, 
        /// but in scarce cases can lead to errors. 
        /// </summary>
        public bool QuickGeometries
        {
            get { return _quickGeometries; }
            set { _quickGeometries = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the 'GetFeature' parser
        /// should ignore multi-geometries (MultiPoint, MultiLineString, MultiCurve, MultiPolygon, MultiSurface). 
        /// By default it does not. Ignoring multi-geometries can lead to a better performance.
        /// </summary>
        public bool MultiGeometries
        {
            get { return _multiGeometries; }
            set { _multiGeometries = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the 'GetFeature' request
        /// should be done with HTTP GET. This option can be important when obtaining
        /// data from a WFS provided by an UMN MapServer.
        /// </summary>
        public bool GetFeatureGETRequest
        {
            get { return _getFeatureGETRequest; }
            set { _getFeatureGETRequest = value; }
        }

        /// <summary>
        /// Gets or sets an OGC Filter.
        /// </summary>
        public IFilter OgcFilter
        {
            get { return _ogcFilter; }
            set { _ogcFilter = value; }
        }

        /// <summary>
        /// Gets or sets the property of the featuretype responsible for labels
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all necessary
        /// parameters to gather metadata from 'GetCapabilities' contract.
        /// </summary>
        /// <param name="getCapabilitiesUri"></param>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureType"></param>
        /// <param name="geometryType">
        /// Specifying the geometry type helps to accelerate the rendering process, 
        /// if the geometry type in 'DescribeFeatureType is unprecise.   
        /// </param>
        /// <param name="wfsVersion"></param>
        public WfsProvider(string getCapabilitiesUri, string nsPrefix, string featureType, WfsGeometryType geometryType,
                   WfsVersion wfsVersion)
        {
            _getCapabilitiesUri = getCapabilitiesUri;

            if (wfsVersion == WfsVersion.WFS1_0_0)
                _textResources = new WFS_1_0_0_TextResources();
            else _textResources = new WFS_1_1_0_TextResources();

            _wfsVersion = wfsVersion;

            if (string.IsNullOrEmpty(nsPrefix))
                ResolveFeatureType(featureType);
            else
            {
                _nsPrefix = nsPrefix;
                _featureType = featureType;
            }

            _geometryType = geometryType;
            GetFeatureTypeInfo();
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all necessary
        /// parameters to gather metadata from 'GetCapabilities' contract.
        /// </summary>
        /// <param name="getCapabilitiesUri"></param>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureType"></param>
        /// <param name="wfsVersion"></param>
        public WfsProvider(string getCapabilitiesUri, string nsPrefix, string featureType, WfsVersion wfsVersion)
            : this(getCapabilitiesUri, nsPrefix, featureType, WfsGeometryType.Unknown, wfsVersion)
        {
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with a 
        /// <see cref="WfsFeatureTypeInfo"/> object, 
        /// so that 'GetCapabilities' and 'DescribeFeatureType' can be bypassed.
        /// </summary>
        public WfsProvider(WfsFeatureTypeInfo featureTypeInfo, WfsVersion wfsVersion)
        {
            _featureTypeInfo = featureTypeInfo;

            if (wfsVersion == WfsVersion.WFS1_0_0)
                _textResources = new WFS_1_0_0_TextResources();
            else _textResources = new WFS_1_1_0_TextResources();

            _wfsVersion = wfsVersion;
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all mandatory
        /// metadata for retrieving a featuretype, so that 'GetCapabilities' and 'DescribeFeatureType' can be bypassed.
        /// </summary>
        /// <param name="serviceURI"></param>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureTypeNamespace">
        /// Use an empty string or 'null', if there is no namespace for the featuretype.
        /// You don't need to know the namespace of the feature type, if you use the quick geometries option.
        /// </param>
        /// <param name="geometryName"></param>
        /// <param name="geometryType">
        /// Specifying the geometry type helps to accelerate the rendering process.   
        /// </param>
        /// <param name="featureType"></param>
        /// <param name="wfsVersion"></param>
        public WfsProvider(string serviceURI, string nsPrefix, string featureTypeNamespace, string featureType,
                   string geometryName, WfsGeometryType geometryType, WfsVersion wfsVersion)
        {
            _featureTypeInfo = new WfsFeatureTypeInfo(serviceURI, nsPrefix, featureTypeNamespace, featureType,
                                                      geometryName, geometryType);

            if (wfsVersion == WfsVersion.WFS1_0_0)
                _textResources = new WFS_1_0_0_TextResources();
            else _textResources = new WFS_1_1_0_TextResources();

            _wfsVersion = wfsVersion;
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all mandatory
        /// metadata for retrieving a featuretype, so that 'GetCapabilities' and 'DescribeFeatureType' can be bypassed.
        /// </summary>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureTypeNamespace">
        /// Use an empty string or 'null', if there is no namespace for the featuretype.
        /// You don't need to know the namespace of the feature type, if you use the quick geometries option.
        /// </param>
        public WfsProvider(string serviceURI, string nsPrefix, string featureTypeNamespace, string featureType,
                   string geometryName, WfsVersion wfsVersion)
            : this(
                serviceURI, nsPrefix, featureTypeNamespace, featureType, geometryName, WfsGeometryType.Unknown,
                wfsVersion)
        {
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all necessary
        /// parameters to gather metadata from 'GetCapabilities' contract.
        /// </summary>
        ///<param name="geometryFactory"></param>
        ///<param name="getCapabilitiesCache">
        /// This cache (obtained from an already instantiated dataprovider that retrieves a featuretype hosted by the same service) 
        /// helps to speed up gathering metadata. It caches the 'GetCapabilities' response. 
        ///</param>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="geometryType">
        /// Specifying the geometry type helps to accelerate the rendering process, 
        /// if the geometry type in 'DescribeFeatureType is unprecise.   
        /// </param>
        ///<param name="wfsVersion"></param>
        public WfsProvider(IGeometryFactory geometryFactory, IXPathQueryManager getCapabilitiesCache, string nsPrefix, string featureType,
                   WfsGeometryType geometryType, WfsVersion wfsVersion)
        {
            _geometryFactory = geometryFactory;
            _featureTypeInfoQueryManager = getCapabilitiesCache;

            if (wfsVersion == WfsVersion.WFS1_0_0)
                _textResources = new WFS_1_0_0_TextResources();
            else _textResources = new WFS_1_1_0_TextResources();

            _wfsVersion = wfsVersion;

            if (string.IsNullOrEmpty(nsPrefix))
                ResolveFeatureType(featureType);
            else
            {
                _nsPrefix = nsPrefix;
                _featureType = featureType;
            }

            _geometryType = geometryType;
            GetFeatureTypeInfo();
        }

        /// <summary>
        /// Use this constructor for initializing this dataprovider with all necessary
        /// parameters to gather metadata from 'GetCapabilities' contract.
        /// </summary>
        /// <param name="getCapabilitiesCache">
        /// This cache (obtained from an already instantiated dataprovider that retrieves a featuretype hosted by the same service) 
        /// helps to speed up gathering metadata. It caches the 'GetCapabilities' response. 
        ///</param>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        ///<param name="featureType"></param>
        public WfsProvider(IGeometryFactory geometryFactory, IXPathQueryManager getCapabilitiesCache, string nsPrefix, string featureType,
                   WfsVersion wfsVersion)
            : this(geometryFactory, getCapabilitiesCache, nsPrefix, featureType, WfsGeometryType.Unknown, wfsVersion)
        {
        }

        #endregion

        #region IProvider Member

        public Collection<IGeometry> GetGeometriesInView(IExtents bbox)
        {
            if (_featureTypeInfo == null) return null;

            Collection<IGeometry> geoms = new Collection<IGeometry>();

            string geometryTypeString = _featureTypeInfo.WfsGeometry.GeometryType;

            WfsGeometryParser geomFactory = null;

            if (!string.IsNullOrEmpty(_label))
            {
                _labelInfo = new FeatureDataTable(_geometryFactory);
                _labelInfo.Columns.Add(_label);
                // Turn off quick geometries, if a label is applied...
                _quickGeometries = false;
            }

            // Configuration for GetFeature request */
            WfsClientHttpConfigurator config = new WfsClientHttpConfigurator(_textResources);
            config.ConfigureForWfsGetFeatureRequest(_httpClientUtil, _featureTypeInfo, _label, bbox, _ogcFilter,
                                                    _getFeatureGETRequest);

            try
            {
                switch (geometryTypeString)
                {
                        /* Primitive geometry elements */

                        // GML2
                    case "PointPropertyType":
                        geomFactory = new WfsPointParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML2
                    case "LineStringPropertyType":
                        geomFactory = new WfsLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML2
                    case "PolygonPropertyType":
                        geomFactory = new WfsPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML3
                    case "CurvePropertyType":
                        geomFactory = new WfsLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML3
                    case "SurfacePropertyType":
                        geomFactory = new WfsPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        /* Aggregate geometry elements */

                        // GML2
                    case "MultiPointPropertyType":
                        if (_multiGeometries)
                            geomFactory = new WfsMultiPointParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        else
                            geomFactory = new WfsPointParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML2
                    case "MultiLineStringPropertyType":
                        if (_multiGeometries)
                            geomFactory = new WfsMultiLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        else
                            geomFactory = new WfsLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML2
                    case "MultiPolygonPropertyType":
                        if (_multiGeometries)
                            geomFactory = new WfsMultiPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        else
                            geomFactory = new WfsPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML3
                    case "MultiCurvePropertyType":
                        if (_multiGeometries)
                            geomFactory = new WfsMultiLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        else
                            geomFactory = new WfsLineStringParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // GML3
                    case "MultiSurfacePropertyType":
                        if (_multiGeometries)
                            geomFactory = new WfsMultiPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        else
                            geomFactory = new WfsPolygonParser(_geometryFactory, _httpClientUtil, _featureTypeInfo, _labelInfo);
                        break;

                        // .e.g. 'gml:GeometryAssociationType' or 'GeometryPropertyType'
                        //It's better to set the geometry type manually, if it is known...
                    default:
                        geomFactory = new WfsUnspecifiedGeometryParser_WFS1_0_0_GML2(_geometryFactory, _httpClientUtil, _featureTypeInfo,
                                                                                   _multiGeometries, _quickGeometries,
                                                                                   _labelInfo);
                        geoms = geomFactory.CreateGeometries();
                        return geoms;
                }

                geoms = _quickGeometries
                            ? geomFactory.CreateQuickGeometries(geometryTypeString)
                            : geomFactory.CreateGeometries();
                geomFactory.Dispose();

                return geoms;
            }
                // Free resources (net connection of geometry factory)
            finally
            {
                geomFactory.Dispose();
            }
        }

        public Collection<uint> GetObjectIDsInView(IExtents bbox)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IGeometry GetGeometryByID(uint oid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExecuteIntersectionQuery(IGeometry geom, FeatureDataSet ds)
        {
            if (_labelInfo == null) return;
            ds.Tables.Add(_labelInfo);
            // Destroy internal reference
            _labelInfo = null;
        }

        public void ExecuteIntersectionQuery(IExtents box, FeatureDataSet ds)
        {
            if (_labelInfo == null) return;
            ds.Tables.Add(_labelInfo);
            // Destroy internal reference
            _labelInfo = null;
        }

        public int GetFeatureCount()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public FeatureDataRow GetFeature(uint RowID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IExtents GetExtents()
        {
            return _geometryFactory.CreateExtents2D(
                                    _featureTypeInfo.BBox.MinLong,
                                   _featureTypeInfo.BBox.MinLat,
                                   _featureTypeInfo.BBox.MaxLong,
                                   _featureTypeInfo.BBox.MaxLat);
        }

        /// <summary>
        /// Gets the service-qualified name of the featuretype.
        /// The service-qualified name enables the differentiation between featuretypes 
        /// from different services with an equal qualified name and therefore can be
        /// regarded as an ID for the featuretype.
        /// </summary>
        public override string ConnectionId
        {
            get { return _featureTypeInfo.ServiceURI + "/" + _featureTypeInfo.QualifiedName; }
        }

        public override void Close()
        {
            base.Close();
            _httpClientUtil.Close();
        }

        public override object ExecuteQuery(Expression query)
        {
            throw new NotImplementedException();
        }

        public int SRID
        {
            get { return Convert.ToInt32(_featureTypeInfo.SRID); }
            set { _featureTypeInfo.SRID = value.ToString(); }
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _featureTypeInfoQueryManager = null;
                    _labelInfo = null;
                    _httpClientUtil.Close();
                }
                IsDisposed = true;
            }
        }

        #endregion

        #region Private Member

        /// <summary>
        /// This method gets metadata about the featuretype to query from 'GetCapabilities' and 'DescribeFeatureType'.
        /// </summary>
        private void GetFeatureTypeInfo()
        {
            try
            {
                _featureTypeInfo = new WfsFeatureTypeInfo();
                WfsClientHttpConfigurator config = new WfsClientHttpConfigurator(_textResources);

                _featureTypeInfo.Prefix = _nsPrefix;
                _featureTypeInfo.Name = _featureType;

                string featureQueryName = string.IsNullOrEmpty(_nsPrefix)
                                              ? _featureType
                                              : _nsPrefix + ":" + _featureType;

                /***************************/
                /* GetCapabilities request  /
                /***************************/

                if (_featureTypeInfoQueryManager == null)
                {
                    /* Initialize IXPathQueryManager with configured HttpClientUtil */
                    _featureTypeInfoQueryManager =
                        new XPathQueryManager_CompiledExpressionsDecorator(new XPathQueryManager());
                    _featureTypeInfoQueryManager.SetDocumentToParse(
                        config.ConfigureForWfsGetCapabilitiesRequest(_httpClientUtil, _getCapabilitiesUri));
                    /* Namespaces for XPath queries */
                    _featureTypeInfoQueryManager.AddNamespace(_textResources.NSWFSPREFIX, _textResources.NSWFS);
                    _featureTypeInfoQueryManager.AddNamespace(_textResources.NSOWSPREFIX, _textResources.NSOWS);
                    _featureTypeInfoQueryManager.AddNamespace(_textResources.NSXLINKPREFIX, _textResources.NSXLINK);
                }

                /* Service URI (for WFS GetFeature request) */
                _featureTypeInfo.ServiceURI = _featureTypeInfoQueryManager.GetValueFromNode
                    (_featureTypeInfoQueryManager.Compile(_textResources.XPATH_GETFEATURERESOURCE));
                /* If no GetFeature URI could be found, try GetCapabilities URI */
                if (_featureTypeInfo.ServiceURI == null) _featureTypeInfo.ServiceURI = _getCapabilitiesUri;
                else if (_featureTypeInfo.ServiceURI.EndsWith("?", StringComparison.Ordinal))
                    _featureTypeInfo.ServiceURI =
                        _featureTypeInfo.ServiceURI.Remove(_featureTypeInfo.ServiceURI.Length - 1);

                /* URI for DescribeFeatureType request */
                string describeFeatureTypeUri = _featureTypeInfoQueryManager.GetValueFromNode
                    (_featureTypeInfoQueryManager.Compile(_textResources.XPATH_DESCRIBEFEATURETYPERESOURCE));
                /* If no DescribeFeatureType URI could be found, try GetCapabilities URI */
                if (describeFeatureTypeUri == null) describeFeatureTypeUri = _getCapabilitiesUri;
                else if (describeFeatureTypeUri.EndsWith("?", StringComparison.Ordinal))
                    describeFeatureTypeUri =
                        describeFeatureTypeUri.Remove(describeFeatureTypeUri.Length - 1);

                /* Spatial reference ID */
                _featureTypeInfo.SRID = _featureTypeInfoQueryManager.GetValueFromNode(
                    _featureTypeInfoQueryManager.Compile(_textResources.XPATH_SRS),
                    new[] {new DictionaryEntry("_param1", featureQueryName)});
                /* If no SRID could be found, try '4326' by default */
                if (_featureTypeInfo.SRID == null) _featureTypeInfo.SRID = "4326";
                else
                    /* Extract number */
                    _featureTypeInfo.SRID = _featureTypeInfo.SRID.Substring(_featureTypeInfo.SRID.LastIndexOf(":") + 1);

                /* Bounding Box */
                IXPathQueryManager bboxQuery = _featureTypeInfoQueryManager.GetXPathQueryManagerInContext(
                    _featureTypeInfoQueryManager.Compile(_textResources.XPATH_BBOX),
                    new[] {new DictionaryEntry("_param1", featureQueryName)});

                if (bboxQuery != null)
                {
                    NumberFormatInfo formatInfo = new NumberFormatInfo();
                    formatInfo.NumberDecimalSeparator = ".";
                    string bboxVal = null;

                    double maxLat = 0d, minLat = 0d;
                    double maxLong = 0d, minLong = 0f;
                    if (_wfsVersion == WfsVersion.WFS1_0_0)
                        minLat =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMINY))) !=
                                null
                                    ? bboxVal
                                    : "0.0", formatInfo);
                    else if (_wfsVersion == WfsVersion.WFS1_1_0)
                        minLat =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMINY))) !=
                                null
                                    ? bboxVal.Substring(bboxVal.IndexOf(' ') + 1)
                                    : "0.0", formatInfo);

                    if (_wfsVersion == WfsVersion.WFS1_0_0)
                        maxLat =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMAXY))) !=
                                null
                                    ? bboxVal
                                    : "0.0", formatInfo);
                    else if (_wfsVersion == WfsVersion.WFS1_1_0)
                        maxLat =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMAXY))) !=
                                null
                                    ? bboxVal.Substring(bboxVal.IndexOf(' ') + 1)
                                    : "0.0", formatInfo);

                    if (_wfsVersion == WfsVersion.WFS1_0_0)
                        minLong =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMINX))) !=
                                null
                                    ? bboxVal
                                    : "0.0", formatInfo);
                    else if (_wfsVersion == WfsVersion.WFS1_1_0)
                        minLong =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMINX))) !=
                                null
                                    ? bboxVal.Substring(0, bboxVal.IndexOf(' ') + 1)
                                    : "0.0", formatInfo);

                    if (_wfsVersion == WfsVersion.WFS1_0_0)
                        maxLong =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMAXX))) !=
                                null
                                    ? bboxVal
                                    : "0.0", formatInfo);
                    else if (_wfsVersion == WfsVersion.WFS1_1_0)
                        maxLong =
                            Convert.ToDouble(
                                (bboxVal =
                                 bboxQuery.GetValueFromNode(bboxQuery.Compile(_textResources.XPATH_BOUNDINGBOXMAXX))) !=
                                null
                                    ? bboxVal.Substring(0, bboxVal.IndexOf(' ') + 1)
                                    : "0.0", formatInfo);

                    _featureTypeInfo.BBox = new WfsFeatureTypeInfo.WfsBoundingBox(maxLat, maxLong, minLat, minLong);
                }

                //Continue with a clone in order to preserve the 'GetCapabilities' response
                IXPathQueryManager describeFeatureTypeQueryManager = _featureTypeInfoQueryManager.Clone();

                /******************************/
                /* DescribeFeatureType request /
                /******************************/

                /* Initialize IXPathQueryManager with configured HttpClientUtil */
                describeFeatureTypeQueryManager.ResetNamespaces();
                describeFeatureTypeQueryManager.SetDocumentToParse(config.ConfigureForWfsDescribeFeatureTypeRequest
                                                                       (_httpClientUtil, describeFeatureTypeUri,
                                                                        featureQueryName));

                /* Namespaces for XPath queries */
                describeFeatureTypeQueryManager.AddNamespace(_textResources.NSSCHEMAPREFIX, _textResources.NSSCHEMA);
                describeFeatureTypeQueryManager.AddNamespace(_textResources.NSGMLPREFIX, _textResources.NSGML);

                /* Get target namespace */
                string targetNs = describeFeatureTypeQueryManager.GetValueFromNode(
                    describeFeatureTypeQueryManager.Compile(_textResources.XPATH_TARGETNS));
                if (targetNs != null)
                    _featureTypeInfo.FeatureTypeNamespace = targetNs;

                /* Get geometry */
                string geomType = _geometryType == WfsGeometryType.Unknown ? null : _geometryType.ToString();
                string geomName = null;
                string geomComplexTypeName = null;

                /* The easiest way to get geometry info, just ask for the 'gml'-prefixed type-attribute... 
                   Simple, but effective in 90% of all cases...this is the standard GeoServer creates.*/
                /* example: <xs:element nillable = "false" name = "the_geom" maxOccurs = "1" type = "gml:MultiPolygonPropertyType" minOccurs = "0" /> */
                /* Try to get context of the geometry element by asking for a 'gml:*' type-attribute */
                IXPathQueryManager geomQuery = describeFeatureTypeQueryManager.GetXPathQueryManagerInContext(
                    describeFeatureTypeQueryManager.Compile(_textResources.XPATH_GEOMETRYELEMENT_BYTYPEATTRIBUTEQUERY));
                if (geomQuery != null)
                {
                    geomName = geomQuery.GetValueFromNode(geomQuery.Compile(_textResources.XPATH_NAMEATTRIBUTEQUERY));

                    /* Just, if not set manually... */
                    if (geomType == null)
                        geomType = geomQuery.GetValueFromNode(geomQuery.Compile(_textResources.XPATH_TYPEATTRIBUTEQUERY));
                }
                else
                {
                    /* Try to get context of a complexType with element ref ='gml:*' - use the global context */
                    /* example:
                    <xs:complexType name="geomType">
                        <xs:sequence>
                            <xs:element ref="gml:polygonProperty" minOccurs="0"/>
                        </xs:sequence>
                    </xs:complexType> */
                    geomQuery = describeFeatureTypeQueryManager.GetXPathQueryManagerInContext(
                        describeFeatureTypeQueryManager.Compile(
                            _textResources.XPATH_GEOMETRYELEMENTCOMPLEXTYPE_BYELEMREFQUERY));
                    if (geomQuery != null)
                    {
                        /* Ask for the name of the complextype - use the local context*/
                        geomComplexTypeName =
                            geomQuery.GetValueFromNode(geomQuery.Compile(_textResources.XPATH_NAMEATTRIBUTEQUERY));

                        if (geomComplexTypeName != null)
                        {
                            /* Ask for the name of an element with a complextype of 'geomComplexType' - use the global context */
                            geomName =
                                describeFeatureTypeQueryManager.GetValueFromNode(
                                    describeFeatureTypeQueryManager.Compile(
                                        _textResources.XPATH_GEOMETRY_ELEMREF_GEOMNAMEQUERY), new[]
                                                                                                  {
                                                                                                      new DictionaryEntry
                                                                                                          ("_param1",
                                                                                                           _featureTypeInfo
                                                                                                               .
                                                                                                               FeatureTypeNamespace)
                                                                                                      ,
                                                                                                      new DictionaryEntry
                                                                                                          ("_param2",
                                                                                                           geomComplexTypeName)
                                                                                                  });
                        }
                        else
                        {
                            /* The geometry element must be an ancestor, if we found an anonymous complextype */
                            /* Ask for the element hosting the anonymous complextype - use the global context */
                            /* example: 
                            <xs:element name ="SHAPE">
                                <xs:complexType>
                            	    <xs:sequence>
                              		    <xs:element ref="gml:lineStringProperty" minOccurs="0"/>
                                  </xs:sequence>
                                </xs:complexType>
                            </xs:element> */
                            geomName =
                                describeFeatureTypeQueryManager.GetValueFromNode(
                                    describeFeatureTypeQueryManager.Compile(
                                        _textResources.XPATH_GEOMETRY_ELEMREF_GEOMNAMEQUERY_ANONYMOUSTYPE));
                        }
                        /* Just, if not set manually... */
                        if (geomType == null)
                        {
                            /* Ask for the 'ref'-attribute - use the local context */
                            if (
                                (geomType =
                                 geomQuery.GetValueFromNode(
                                     geomQuery.Compile(_textResources.XPATH_GEOMETRY_ELEMREF_GMLELEMENTQUERY))) != null)
                            {
                                switch (geomType)
                                {
                                    case "gml:pointProperty":
                                        geomType = "PointPropertyType";
                                        break;
                                    case "gml:lineStringProperty":
                                        geomType = "LineStringPropertyType";
                                        break;
                                    case "gml:curveProperty":
                                        geomType = "CurvePropertyType";
                                        break;
                                    case "gml:polygonProperty":
                                        geomType = "PolygonPropertyType";
                                        break;
                                    case "gml:surfaceProperty":
                                        geomType = "SurfacePropertyType";
                                        break;
                                    case "gml:multiPointProperty":
                                        geomType = "MultiPointPropertyType";
                                        break;
                                    case "gml:multiLineStringProperty":
                                        geomType = "MultiLineStringPropertyType";
                                        break;
                                    case "gml:multiCurveProperty":
                                        geomType = "MultiCurvePropertyType";
                                        break;
                                    case "gml:multiPolygonProperty":
                                        geomType = "MultiPolygonPropertyType";
                                        break;
                                    case "gml:multiSurfaceProperty":
                                        geomType = "MultiSurfacePropertyType";
                                        break;
                                        // e.g. 'gml:_geometryProperty' 
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                if (geomName == null)
                    /* Default value for geometry column = geom */
                    geomName = "geom";

                if (geomType == null)
                    /* Set geomType to an empty string in order to avoid exceptions.
                    The geometry type is not necessary by all means - it can be detected in 'GetFeature' response too.. */
                    geomType = string.Empty;

                /* Remove prefix */
                if (geomType.Contains(":"))
                    geomType = geomType.Substring(geomType.IndexOf(":") + 1);

                WfsFeatureTypeInfo.WfsGeometryInfo geomInfo = new WfsFeatureTypeInfo.WfsGeometryInfo();
                geomInfo.GeometryName = geomName;
                geomInfo.GeometryType = geomType;
                _featureTypeInfo.WfsGeometry = geomInfo;
            }
            finally
            {
                _httpClientUtil.Close();
            }
        }

        private void ResolveFeatureType(string featureType)
        {
            if (featureType.Contains(":"))
            {
                string[] split = featureType.Split(':');
                _nsPrefix = split[0];
                _featureType = split[1];
            }
            else
                _featureType = featureType;
        }

        #endregion

        #region Nested Types

        #region WFSClientHTTPConfigurator

        /// <summary>
        /// This class configures a <see cref="WfsHttpClientUtility"/> class 
        /// for requests to a Web Feature Service.
        /// </summary>
        private class WfsClientHttpConfigurator
        {
            #region Fields

            private readonly IWfsTextResources _wfsTextResources;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WFS.WFSClientHTTPConfigurator"/> class.
            /// An instance of this class can be used to configure a <see cref="SharpMap.Utilities.Wfs.HttpClientUtil"/> object.
            /// </summary>
            /// <param name="wfsTextResources">
            /// An instance implementing <see cref="IWfsTextResources" /> 
            /// for getting version-specific text resources for WFS request configuration.
            ///</param>
            internal WfsClientHttpConfigurator(IWfsTextResources wfsTextResources)
            {
                _wfsTextResources = wfsTextResources;
            }

            #endregion

            #region Internal Member

            /// <summary>
            /// Configures for WFS 'GetCapabilities' request using an instance implementing <see cref="IWfsTextResources"/>.
            /// The <see cref="WfsHttpClientUtility"/> instance is returned for immediate usage. 
            /// </summary>
            internal WfsHttpClientUtility ConfigureForWfsGetCapabilitiesRequest(WfsHttpClientUtility httpClientUtil,
                                                                          string targetUrl)
            {
                httpClientUtil.Reset();
                httpClientUtil.Url = targetUrl + _wfsTextResources.GetCapabilitiesRequest();
                return httpClientUtil;
            }

            /// <summary>
            /// Configures for WFS 'DescribeFeatureType' request using an instance implementing <see cref="IWfsTextResources"/>.
            /// The <see cref="WfsHttpClientUtility"/> instance is returned for immediate usage. 
            /// </summary>
            internal WfsHttpClientUtility ConfigureForWfsDescribeFeatureTypeRequest(WfsHttpClientUtility httpClientUtil,
                                                                              string targetUrl,
                                                                              string featureTypeName)
            {
                httpClientUtil.Reset();
                httpClientUtil.Url = targetUrl + _wfsTextResources.DescribeFeatureTypeRequest(featureTypeName);
                return httpClientUtil;
            }

            /// <summary>
            /// Configures for WFS 'GetFeature' request using an instance implementing <see cref="IWfsTextResources"/>.
            /// The <see cref="WfsHttpClientUtility"/> instance is returned for immediate usage. 
            /// </summary>
            internal WfsHttpClientUtility ConfigureForWfsGetFeatureRequest(WfsHttpClientUtility httpClientUtil,
                                                                     WfsFeatureTypeInfo featureTypeInfo,
                                                                     string labelProperty, IExtents boundingBox,
                                                                     IFilter filter, bool GET)
            {
                httpClientUtil.Reset();
                httpClientUtil.Url = featureTypeInfo.ServiceURI;

                if (GET)
                {
                    /* HTTP-GET */
                    httpClientUtil.Url += _wfsTextResources.GetFeatureGETRequest(featureTypeInfo, boundingBox, filter);
                    return httpClientUtil;
                }

                /* HTTP-POST */
                httpClientUtil.PostData = _wfsTextResources.GetFeaturePOSTRequest(featureTypeInfo, labelProperty,
                                                                                  boundingBox, filter);
                httpClientUtil.AddHeader(HttpRequestHeader.ContentType.ToString(), "text/xml");
                return httpClientUtil;
            }

            #endregion
        }

        #endregion

        #endregion

    }
}
