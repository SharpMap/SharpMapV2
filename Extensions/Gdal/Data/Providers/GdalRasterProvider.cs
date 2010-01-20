using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.GdalPreview;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Expressions;

using Gdal = OSGeo.GDAL.Gdal;
using GdalDataset = OSGeo.GDAL.Dataset;
using GdalBand = OSGeo.GDAL.Band;
using GdalResampleAlg = OSGeo.GDAL.ResampleAlg;
using GdalAccess = OSGeo.GDAL.Access;
using GdalCPlErr = OSGeo.GDAL.CPLErr;
using GdalColorInterp = OSGeo.GDAL.ColorInterp;
using GdalColorTable = OSGeo.GDAL.ColorTable;
using GdalColorEntry = OSGeo.GDAL.ColorEntry;

namespace SharpMap.Data.Providers
{
    internal unsafe delegate void WritePixel(double x, Int32 bands, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex);

    public enum GdalRasterType
    {
        GrayOrUndefined,
        Palette,
        MultiBand
    }

    internal class GdalBandStatistics
    {
        public readonly Int32 BandNumber;
        public Double Minimum { get; protected set; }
        public Double Maximum { get; protected set; }
        public Double Range { get; protected set; }
        public Double Mean { get; protected set; }
        public Int32[] HistgramVector { get; protected set; }

        internal GdalBandStatistics(Int32 bandNr)
        {
            BandNumber = bandNr;
        }

        internal static GdalBandStatistics GetGdalBandStatistics(Int32 bandNr, GdalBand band, Boolean approximate, Boolean force)
        {
            if (band == null)
                throw new ArgumentNullException("band");

            Double min, max, range, mean;
            band.GetStatistics(approximate ? 1 : 0, force ? 1:0, out min, out max, out range, out mean);
            GdalBandStatistics bs = new GdalBandStatistics(bandNr)
                                        {Minimum = min, Maximum = max, Range = range, Mean = mean};
            return bs;
        }

        public void CalculateHistogram(Double from, Double to, Int32 numBuckets)
        {
            
        }
    }

    public class GdalRasterProvider : ProviderBase, IRasterProvider
    {
        static GdalRasterProvider()
        {
            String path = Environment.GetEnvironmentVariable("Path");
            //NOTE: SET THIS TO SOME APROPTIATE VALUE
            path = @"C:\VENUS\CodePlex\initialgdalintegration\Extensions\Gdal\Gdal1.6.3\apps;C:\VENUS\CodePlex\initialgdalintegration\Extensions\Gdal\Proj4.6.1\src" + path;
            Environment.SetEnvironmentVariable("Path", path);
            Environment.SetEnvironmentVariable("GDAL_DATA", @"C:\VENUS\CodePlex\initialgdalintegration\Extensions\Gdal\Gdal1.6.3\apps;");
            Gdal.AllRegister();
        }

        public static GdalResampleAlg ResampleAlgorithm = GdalResampleAlg.GRA_NearestNeighbour;
        public static Double MaxError = 0.15d;

        private IGeometryFactory _geoFactory;

        private readonly ICoordinateSystemFactory _coordSystemFactory;
        private readonly ICoordinateTransformationFactory _coordTransfromationFactory;

        private readonly String _gdalRasterFilename;
        private GdalRasterType _gdalRasterType;
        private Boolean _isValid;
        private Boolean _disposed;
        private DateTime _lastModified;

        private GdalDataset _gdalDataset;
        private GdalDataset _gdalProjected;
        private Int32[] _gdalBandOrder;
        private Double? _noDataValue;
        public Double? NoDataValue
        {
            get { return _noDataValue; }
            set { _noDataValue = value; }
        }

        private Size2D _imageSize;

        private IExtents _extents;
        private Int32 _lBands;
        //private Int32 _bitDepth = 8;

        private bool _useRotation = true; // use geographic information
        private bool _haveSpot; // use geographic information
        //private GeoTransform _geoTransform;

        private readonly Object _lock = new Object();

        //Preview Class
        private readonly BaseGdalPreview _preview;

        //
        private GdalBandStatistics[] _bandStatistics;

        //Cached Bitmap
        private IExtents _viewPort;
        private Matrix2D _toViewTransform;
        private MemoryStream _cachedImage;
        private Rectangle2D _viewBounds;
        private Rectangle2D _rasterBounds;

        public GdalRasterProvider(String gdalRasterFileName, BaseGdalPreview previewGenerator, IGeometryFactory geometryFactory, ICoordinateSystemFactory coordSystemFactory,
            ICoordinateTransformationFactory coordTransformationFactory)
        {
            _gdalRasterFilename = gdalRasterFileName;

            _geoFactory = geometryFactory;
            _coordSystemFactory = coordSystemFactory;
            _coordTransfromationFactory = coordTransformationFactory;

            _preview = previewGenerator;
            _preview.Initialize(this);
        }

        public GdalRasterType RasterType
        {
            get { return _gdalRasterType; }
        }

        internal Boolean ReadFile()
        {
            //File exists
            if (!File.Exists(_gdalRasterFilename))
            {
                _isValid = false;
                return false;
            }

            //File hasn't changed
            if (File.GetLastWriteTime(_gdalRasterFilename).CompareTo(_lastModified) <= 0 && _gdalDataset != null)
                return _isValid;
            _lastModified = File.GetLastWriteTime(_gdalRasterFilename);

            //Open
            _gdalDataset = Gdal.OpenShared(_gdalRasterFilename, GdalAccess.GA_ReadOnly);
            if (_gdalDataset == null)
            {
                _isValid = false;
                return false;
            }

            //Gdal RasterType
            if (_gdalDataset.RasterCount > 1)
                _gdalRasterType = GdalRasterType.MultiBand;
            else if (_gdalDataset.GetRasterBand(1).GetRasterColorInterpretation() == GdalColorInterp.GCI_PaletteIndex)
                _gdalRasterType = GdalRasterType.Palette;
            else
                _gdalRasterType = GdalRasterType.GrayOrUndefined;

            _bandStatistics = new GdalBandStatistics[_gdalDataset.RasterCount];
            for ( int i = 1; i <= _gdalDataset.RasterCount; i++ )
                _bandStatistics[i-1] = GetBandStatistics(i);

            //
            Double[] geoTrans = new double[6];
            _gdalDataset.GetGeoTransform(geoTrans);
            if ((geoTrans[1] < 0d || geoTrans[2] != 0d || 
                 geoTrans[4] != 0d || geoTrans[5] > 0d ) ||
                _gdalDataset.GetGCPCount() > 0)
            {
                _gdalProjected = Gdal.AutoCreateWarpedVRT(_gdalDataset, "", "",
                    ResampleAlgorithm, MaxError);

                if (_gdalProjected == null)
                    _gdalProjected = _gdalDataset;
                else
                {
                    _gdalProjected.GetGeoTransform(geoTrans);
                }
            }
            else
            {
                _gdalProjected = _gdalDataset;
            }

            GdalBand band = _gdalProjected.GetRasterBand(1);
            if ( band == null )
            {
                Dispose(true);
                return false;
            }

            String srsWkt = _gdalDataset.GetProjection();
            ICoordinateSystem cs = _coordSystemFactory.CreateFromWkt(srsWkt);
            if (cs == null)
            {
                Dispose(true);
                return false;
            }
            OriginalSpatialReference = cs;
            _geoFactory = new GeometryServices()[srsWkt];

            //Extents
            _extents = _geoFactory.CreateExtents2D(
                geoTrans[0],
                geoTrans[3] + _gdalProjected.RasterXSize*geoTrans[4] + _gdalProjected.RasterYSize*geoTrans[5],
                geoTrans[0] + _gdalProjected.RasterXSize*geoTrans[1] + _gdalProjected.RasterYSize*geoTrans[2],
                geoTrans[3]);

            System.Diagnostics.Debug.WriteLine(String.Format("{0}: {1}", ConnectionId, _extents));

            //Rastersize
            _imageSize = new Size2D(_gdalProjected.RasterXSize, _gdalProjected.RasterYSize);

            //No data value
            Int32 isValid;
            Double noDataValue;
            band.GetNoDataValue(out noDataValue, out isValid);
            _noDataValue = isValid!=0 ? new Double?(noDataValue) : null;

            Bands = _gdalProjected.RasterCount;
            _gdalBandOrder = new int[Bands];
            for (int i = 0; i < Bands; i++ ) _gdalBandOrder[i] = i;

            _isValid = true;

            return true;
        }

        private GdalBandStatistics GetBandStatistics(Int32 bandNr)
        {
            return GdalBandStatistics.GetGdalBandStatistics(bandNr, _gdalDataset.GetRasterBand(bandNr), true, true);
        }

        public GdalDataset Dataset
        {
            get
            {
                return !_disposed ? _gdalProjected : null;
            }
        }

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
        }

        private void HandlePropertyChanged(Object sender, PropertyChangedEventArgs eventArgs)
        {
            if ( eventArgs.PropertyName == "CoordinateTransformation" ||
                 eventArgs.PropertyName == "OriginalSpatialReference")
                OnSpatialReferenceChanged();
        }

        ~GdalRasterProvider()
        {
            Dispose(true);
        }

        private static Int32 GdalProgressCallback(Double p1, IntPtr p2, IntPtr p3)
        {
            return -1;
        }

        public void OnSpatialReferenceChanged()
        {
            String sourceSrsWkt = OriginalSpatialReference.Wkt;
            String targetSrsWkt = SpatialReference.Wkt;

            if (sourceSrsWkt != targetSrsWkt)
            {
                String file = String.Format(@"{0}\{1}_{2}", Path.GetTempPath(), targetSrsWkt.GetHashCode(),
                                            Path.GetFileName(_gdalRasterFilename));

                if (File.Exists(file))
                {
                    _gdalProjected = Gdal.OpenShared(file, GdalAccess.GA_ReadOnly);

                }
                else
                {
                    //NOTE: NEEDS TO BE TESTED!!
                    GdalCPlErr err = Gdal.ReprojectImage(_gdalDataset, _gdalProjected,
                                                     sourceSrsWkt, targetSrsWkt, ResampleAlgorithm, 10000d, MaxError,
                                                     GdalProgressCallback, "");
                    if (err == 0)
                    {
                        //ToDo safe projected raster so we don't need to reproject every time!
                    }
                }
            }
            else
            {
                _gdalProjected = _gdalDataset;
            }

            _imageSize = new Size2D(_gdalProjected.RasterXSize, _gdalProjected.RasterYSize);
            _extents = GetExtents();

        }

        /// <summary>
        /// Gets the number of bands
        /// </summary>
        public int Bands
        {
            get { return _lBands; }
            private set { _lBands = value; }
        }

        public Size2D Size
        {
            get { return _imageSize; }
        }

        internal GdalBandStatistics[] BandStatistics
        {
            get { return _bandStatistics; }
            private set { _bandStatistics = value; }
        }

        #region Overrides of ProviderBase

        /// <summary>
        /// Disposes the GdalRasterLayer and release the raster file
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (!ReferenceEquals(_gdalProjected, _gdalDataset))
                    {
                        if (_gdalProjected != null)
                        {
                            _gdalProjected.Dispose();
                            _gdalProjected = null;
                        }
                    }
                    if (_gdalDataset != null)
                    {
                        _gdalDataset.Dispose();
                        _gdalDataset = null;
                    }
                }
                _isValid = false;
                _disposed = true;
            }
        }

        public override IExtents GetExtents()
        {
            if (ReadFile())
                return _extents;
            return null;
        }
        
        public override string ConnectionId
        {
            get { return _gdalRasterFilename; }
        }

        public override object ExecuteQuery(Expression query)
        {
            return ExecuteRasterQuery(query as RasterQueryExpression);
        }

        public IEnumerable<IRasterRecord> ExecuteRasterQuery(RasterQueryExpression query)
        {
            if (_disposed)
                yield break;

            if (!ReadFile())
                yield break;

            if (query.SpatialPredicate.SpatialExpression.Extents.Intersects(_extents))
                yield return new GdalRasterRecord(this, query.SpatialPredicate.SpatialExpression.Extents);

        }

        #endregion

        private StyleColor? _transparentColor;
        public StyleColor? TransparentColor
        {
            get { return _transparentColor; }
            set { _transparentColor = value; }
        }


        public Stream GetPreview(IExtents viewPort, Matrix2D toViewTransform, out Rectangle2D viewBounds, out Rectangle2D rasterBounds)
        {

            //Test if our cached image fits
            if (_viewPort != null)
            {
                if (viewPort.Equals(_viewPort) && toViewTransform.Equals(_toViewTransform))
                {
                    viewBounds = _viewBounds;
                    rasterBounds = _rasterBounds;
                    return _cachedImage;
                }
            }

            Bitmap bm = _preview.GetPreview(viewPort, toViewTransform, out viewBounds, out rasterBounds);
            if ( bm == null )
                return null;

            if (TransparentColor.HasValue)
            {
                StyleColor c = TransparentColor.Value;
                switch (bm.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                        bm.MakeTransparent(Color.FromArgb(c.A, c.R, c.G, c.B));
                        break;
                    default:
                        bm.MakeTransparent(Color.FromArgb(0, c.R, c.G, c.B));
                        break;
                }
            }

            if (!ReferenceEquals(_gdalDataset, _gdalProjected))
            {
                StyleColor c = StyleColor.Black;
                switch (bm.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                        bm.MakeTransparent(Color.FromArgb(c.A, c.R, c.G, c.B));
                        break;
                    default:
                        bm.MakeTransparent(Color.FromArgb(0, c.R, c.G, c.B));
                        break;
                }
            }

            //Create image stream
            MemoryStream ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            //cache image and properties
            _viewPort = viewPort;
            _toViewTransform = toViewTransform;
            _cachedImage = ms;
            _viewBounds = viewBounds;
            _rasterBounds = rasterBounds;

            return ms;
        }

        #region Public static functions

        public static String SupportedFileFormats()
        {
            int numDrivers = Gdal.GetDriverCount();
            Boolean jp2Driver = false;
            string[] description = new string[numDrivers];
            string[] extension = new string[numDrivers];

            for (int i = 0; i < numDrivers; i++)
            {
                OSGeo.GDAL.Driver driver = Gdal.GetDriver(i);

                String[] metadata = driver.GetMetadata("");
                foreach (string item in metadata)
                {
                    String[] pair = item.Split('=');
                    if (pair[0] == "DMD_EXTENSION")
                        extension[i] = String.Format("*.{0}", pair[1].ToLower());
                    if (pair[0] == "DMD_LONGNAME")
                        description[i] = pair[1];

                    if (!String.IsNullOrEmpty(extension[i]) && !String.IsNullOrEmpty(description[i]))
                    {
                        if (description[i] == "JPEG2000" && extension[i] == "*.jp2")
                        {
                            if (!jp2Driver)
                            {
                                extension[i] += "*.j2k";
                                jp2Driver = true;
                            }
                            else
                                break;
                        }
                        break;
                    }
                }

                if (description[i] == "In Memory Raster")
                    description[i] = "";

                if (String.IsNullOrEmpty(extension[i]) && !String.IsNullOrEmpty(description[i]))
                {
                    if (description[i].StartsWith("USGSDEM"))
                        extension[i] = "*.dem";
                    else if (description[i].StartsWith("DTED"))
                        extension[i] = "*.dt0";
                    else if (description[i].StartsWith("MrSID"))
                        extension[i] = "*.sid";
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("No extension specified for '{0}'",
                                                                         driver.LongName));
                        extension[i] = "*.*";
                    }
                }
                if ( !String.IsNullOrEmpty(extension[i]))
                    while (extension[i].Contains("/"))
                        extension[i] = extension[i].Replace("/", ";*.");

            }

            String initialFiter = String.Format("All supported Files|{0}", String.Join(";", extension));
            StringBuilder filter = new StringBuilder(initialFiter.Replace("*.*", "").Replace(";;", ""));
            for( int i = 0; i < numDrivers; i++)
                if (!String.IsNullOrEmpty(description[i])) filter.AppendFormat("|{0}|{1}", description[i], extension[i]);

            return filter.ToString();
        }

        #endregion

    }
}
