// WFS provider by Peter Robineau (peter.robineau@gmx.at)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

using GeoAPI.Geometries;

namespace SharpMap.Data.Providers.WfsProvider
{
    public class WfsFeatureTypeInfo
    {
        #region Fields with Properties

        private WfsBoundingBox _wfsBoundingBox;//= new WfsBoundingBox();
        private string _Cs = ",";
        private string _DecimalDel = ".";
        private string _FeatureTypeNamespace = string.Empty;
        private WfsGeometryInfo _wfsGeometry = new WfsGeometryInfo();
        private string _Name = string.Empty;

        private string _prefix = string.Empty;
        private string _serviceUri = string.Empty;
        private string _srid = "4326";
        private string _ts = " ";

        /// <summary>
        /// Gets or sets the name of the featuretype.
        /// This argument is obligatory for data retrieving.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Gets or sets the prefix of the featuretype and it's nested elements.
        /// This argument is obligatory for data retrieving, if the featuretype is declared with a 
        /// prefix in 'GetCapabilities'.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        /// <summary>
        /// Gets or sets the featuretype namespace.
        /// This argument is obligatory for data retrieving, except when using the quick geometries option.
        /// </summary>
        public string FeatureTypeNamespace
        {
            get { return _FeatureTypeNamespace; }
            set { _FeatureTypeNamespace = value; }
        }

        /// <summary>
        /// Gets the qualified name of the featuretype (with namespace URI).
        /// </summary>
        internal string QualifiedName
        {
            get { return _FeatureTypeNamespace + _Name; }
        }

        /// <summary>
        /// Gets or sets the service URI for WFS 'GetFeature' request.
        /// This argument is obligatory for data retrieving.
        /// </summary>
        public string ServiceURI
        {
            get { return _serviceUri; }
            set { _serviceUri = value; }
        }

        /// <summary>
        /// Gets or sets information about the geometry of the featuretype.
        /// Setting at least the geometry name is obligatory for data retrieving.
        /// </summary>
        public WfsGeometryInfo WfsGeometry
        {
            get { return _wfsGeometry; }
            set { _wfsGeometry = value; }
        }

        /// <summary>
        /// Gets or sets the spatial extent of the featuretype - defined as minimum bounding rectangle. 
        /// </summary>
        public WfsBoundingBox BBox
        {
            get { return _wfsBoundingBox; }
            set { _wfsBoundingBox = value; }
        }

        /// <summary>
        /// Gets or sets the spatial reference ID
        /// </summary>
        public string SRID
        {
            get { return _srid; }
            set { _srid = value; }
        }

        //Coordinates can be included in a single string, but there is no 
        //facility for validating string content. The value of the 'cs' attribute 
        //is the separator for coordinate values, and the value of the 'ts' 
        //attribute gives the tuple separator (a single space by default); the 
        //default values may be changed to reflect local usage.

        /// <summary>
        /// Decimal separator (for gml:coordinates)
        /// </summary>
        public string DecimalDel
        {
            get { return _DecimalDel; }
            set { _DecimalDel = value; }
        }

        /// <summary>
        /// Separator for coordinate values (for gml:coordinates)
        /// </summary>
        public string Cs
        {
            get { return _Cs; }
            set { _Cs = value; }
        }

        /// <summary>
        /// Tuple separator (for gml:coordinates)
        /// </summary>
        public string Ts
        {
            get { return _ts; }
            set { _ts = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsFeatureTypeInfo"/> class.
        /// </summary>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureTypeNamespace">
        /// Use an empty string or 'null', if there is no namespace for the featuretype.
        /// You don't need to know the namespace of the feature type, if you use the quick geometries option.
        /// </param>
        /// <param name="geometryName">
        /// The geometry name is the property of the featuretype from which geometry information can be obtained from.
        /// Usually this property is called something like 'Shape' or 'geom'. It is absolutely necessary to give this parameter. 
        /// </param>
        /// <param name="geometryType">
        /// Specifying the geometry type helps to accelerate the rendering process.   
        /// </param>
        public WfsFeatureTypeInfo(string serviceURI, string nsPrefix, string featureTypeNamespace, string featureType,
                                  string geometryName, WfsGeometryType geometryType)
        {
            _serviceUri = serviceURI;
            _prefix = nsPrefix;
            _FeatureTypeNamespace = string.IsNullOrEmpty(featureTypeNamespace) ? string.Empty : featureTypeNamespace;
            _Name = featureType;
            _wfsGeometry.GeometryName = geometryName;
            _wfsGeometry.GeometryType = geometryType.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsFeatureTypeInfo"/> class.
        /// </summary>
        /// <param name="nsPrefix">
        /// Use an empty string or 'null', if there is no prefix for the featuretype.
        /// </param>
        /// <param name="featureTypeNamespace">
        /// Use an empty string or 'null', if there is no namespace for the featuretype.
        /// You don't need to know the namespace of the feature type, if you use the quick geometries option.
        /// </param>
        /// <param name="geometryName">
        /// The geometry name is the property of the featuretype from which geometry information can be obtained from.
        /// Usually this property is called something like 'Shape' or 'geom'. It is absolutely necessary to give this parameter. 
        /// </param>
        public WfsFeatureTypeInfo(string serviceURI, string nsPrefix, string featureTypeNamespace, string featureType,
                                  string geometryName)
            : this(serviceURI, nsPrefix, featureTypeNamespace, featureType, geometryName, WfsGeometryType.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WfsFeatureTypeInfo"/> class.
        /// </summary>
        public WfsFeatureTypeInfo()
        {
        }

        #endregion

        #region Nested Types

        #region BoundingBox

        /// <summary>
        /// The bounding box defines the spatial extent of a featuretype.
        /// </summary>
        public class WfsBoundingBox 
        {
            public readonly double MaxLat;
            public readonly double MaxLong;
            public readonly double MinLat;
            public readonly double MinLong;

            public WfsBoundingBox(double maxLat, double maxLong, double minLat, double minLong)
            {
                MaxLat = maxLat;
                MinLong = minLong;
                MinLat = minLat;
                MaxLong = maxLong;
            }
        }

        #endregion

        #region GeometryInfo

        /// <summary>
        /// The geometry info comprises the name of the geometry attribute (e.g. 'Shape" or 'geom')
        /// and the type of the featuretype's geometry.
        /// </summary>
        public class WfsGeometryInfo
        {
            public string GeometryName = string.Empty;
            public string GeometryType = string.Empty;
        }

        #endregion

        #endregion
    }
}