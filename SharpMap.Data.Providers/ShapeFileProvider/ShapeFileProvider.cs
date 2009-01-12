// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Diagnostics;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using SharpMap.Expressions;
using SharpMap.Indexing.RTree;
using SharpMap.Utilities;
using Trace = GeoAPI.Diagnostics.Trace;
using ByteEncoder = GeoAPI.DataStructures.ByteEncoder;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Caster = GeoAPI.DataStructures.Caster;
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif


namespace SharpMap.Data.Providers.ShapeFile
{
    /// <summary>
    /// A data provider for the ESRI ShapeFile spatial data format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The ShapeFile provider is used for accessing ESRI ShapeFiles. 
    /// The ShapeFile should at least contain the [filename].shp 
    /// and the [filename].shx index file. 
    /// If feature-data is to be used a [filename].dbf file should 
    /// also be present.
    /// </para>
    /// <para>
    /// M and Z values in a shapefile are currently ignored by SharpMap.
    /// </para>
    /// </remarks>
    /// <example>
    /// Adding a data source to a layer:
    /// <code lang="C#">
    /// using SharpMap.Layers;
    /// using SharpMap.Data.Providers.ShapeFile;
    /// // [...]
    /// FeatureLayer myLayer = new FeatureLayer("My layer");
    /// myLayer.DataSource = new ShapeFileProvider(@"C:\data\MyShapeData.shp");
    /// </code>
    /// </example>
    public class ShapeFileProvider : FeatureProviderBase, IWritableFeatureProvider<UInt32>
    {
        private const String SharpMapShapeFileIndexFileExtension = ".#index-shp";

        #region IdBounds
        struct IdBounds : IBoundable<IExtents>
        {
            private readonly UInt32 _id;
            private readonly Object _record;

            public IdBounds(UInt32 id, IFeatureDataRecord feature)
            {
                _id = id;
                _record = feature;
            }

            public IdBounds(UInt32 id, IExtents extents)
            {
                _id = id;
                _record = extents;
            }

            public UInt32 Id
            {
                get { return _id; }
            }

            public IFeatureDataRecord Feature
            {
                get { return _record as IFeatureDataRecord; }
            }

            #region IBoundable<IExtents> Members

            public IExtents Bounds
            {
                get { return _record as IExtents ?? Feature.Extents; }
            }

            public Boolean Intersects(IExtents bounds)
            {
                if (bounds == null) throw new ArgumentNullException("bounds");

                return bounds.Intersects(Bounds);
            }

            #endregion
        }
        #endregion

        #region Instance fields
        private Predicate<IFeatureDataRecord> _filterDelegate;
        private Int32? _srid;
        private readonly String _filename;
        private DbaseFile _dbaseFile;
        private FileStream _shapeFileStream;
        private BinaryReader _shapeFileReader;
        private BinaryWriter _shapeFileWriter;
        private readonly Boolean _hasFileBasedSpatialIndex;
        //private Boolean _isOpen;
        private Boolean _isIndexed = true;
        private Boolean _coordsysReadFromFile;
        private ISpatialIndex<IExtents, IdBounds> _spatialIndex;
        private readonly ShapeFileHeader _header;
        private readonly ShapeFileIndex _shapeFileIndex;
        //private ShapeFileDataReader _currentReader;
        private readonly Object _readerSync = new Object();
        private readonly Boolean _hasDbf;
        //private readonly IGeometryFactory _geoFactory;
        private readonly ICoordinateSystemFactory _coordSysFactory;
        //private List<DbaseField> _oidFieldList = new List<DbaseField>();
        private IMathTransform _coordTransform;
        private readonly ICoordinateFactory _coordFactory;

        #endregion

        #region Object construction and disposal

        /// <summary>
        /// Initializes a shapefile data provider.
        /// </summary>
        /// <param name="filename">Path to shapefile (.shp file).</param>
        /// <param name="geoFactory">The geometry factory to use to create geometries.</param>
        /// <remarks>
        /// This constructor creates a <see cref="ShapeFileProvider"/>
        /// with an in-memory spatial index.
        /// </remarks>
        public ShapeFileProvider(String filename, IGeometryFactory geoFactory)
            : this(filename, geoFactory, null, false) { }

        /// <summary>
        /// Initializes a shapefile data provider.
        /// </summary>
        /// <param name="filename">Path to shapefile (.shp file).</param>
        /// <param name="geoFactory">The geometry factory to use to create geometries.</param>
        /// <param name="coordSysFactory">
        /// The coordinate system factory to use to create spatial reference system objects.
        /// </param>
        /// <remarks>
        /// This constructor creates a <see cref="ShapeFileProvider"/>
        /// with an in-memory spatial index.
        /// </remarks>
        public ShapeFileProvider(String filename,
                                 IGeometryFactory geoFactory,
                                 ICoordinateSystemFactory coordSysFactory)
            : this(filename, geoFactory, coordSysFactory, false) { }

        /// <summary>
        /// Initializes a ShapeFile data provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <paramref name="fileBasedIndex"/> is true, the spatial index 
        /// will be read from a local copy. If it doesn't exist,
        /// it will be generated and saved to [filename] + '.sidx'.
        /// </para>
        /// </remarks>
        /// <param name="filename">Path to shapefile (.shp file).</param>
        /// <param name="geoFactory">The geometry factory to use to create geometries.</param>
        /// <param name="coordSysFactory">
        /// The coordinate system factory to use to create spatial reference system objects.
        /// </param>
        /// <param name="fileBasedIndex">True to create a file-based spatial index.</param>
        public ShapeFileProvider(String filename,
                                 IGeometryFactory geoFactory,
                                 ICoordinateSystemFactory coordSysFactory,
                                 Boolean fileBasedIndex)
        {
            _filename = filename;
            IGeometryFactory geoFactoryClone = base.GeometryFactory = geoFactory.Clone();
            OriginalSpatialReference = geoFactoryClone.SpatialReference;
            OriginalSrid = geoFactoryClone.Srid;
            _coordSysFactory = coordSysFactory;
            _coordFactory = geoFactoryClone.CoordinateFactory;

            if (!File.Exists(filename))
            {
                throw new LayerDataLoadException(filename);
            }

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                _header = new ShapeFileHeader(reader, geoFactoryClone);
            }

            _shapeFileIndex = new ShapeFileIndex(this);

            _hasFileBasedSpatialIndex = fileBasedIndex;

            _hasDbf = File.Exists(DbfFilename);

            // Initialize DBF
            if (HasDbf)
            {
                _dbaseFile = new DbaseFile(DbfFilename, geoFactory);
            }
        }

        #region Dispose pattern

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {

                if (_dbaseFile != null)
                {
                    _dbaseFile.Close();
                    _dbaseFile = null;
                }

                if (_shapeFileReader != null)
                {
                    _shapeFileReader.Close();
                    _shapeFileReader = null;
                }

                if (_shapeFileWriter != null)
                {
                    _shapeFileWriter.Close();
                    _shapeFileWriter = null;
                }

                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                    _shapeFileStream = null;
                }

                if (_spatialIndex != null)
                {
                    _spatialIndex.Dispose();
                    _spatialIndex = null;
                }


            }

            base.Close();
        }

        #endregion

        #endregion

        #region ToString

        /// <summary>
        /// Provides a String representation of the essential ShapeFile info.
        /// </summary>
        /// <returns>A String with the Name, HasDbf, FeatureCount and Extents values.</returns>
        public override String ToString()
        {
            return String.Format("Name: {0}; HasDbf: {1}; " +
                                 "Features: {2}; Extents: {3}",
                                 ConnectionId, HasDbf,
                                 GetFeatureCount(), GetExtents());
        }

        #endregion

        #region Public Methods and Properties (SharpMap ShapeFile API)

        #region Create static methods

        /// <summary>
        /// Creates a new <see cref="ShapeFile"/> instance and .shp and .shx file on disk.
        /// </summary>
        /// <param name="directory">Directory to create the shapefile in.</param>
        /// <param name="layerName">Name of the shapefile.</param>
        /// <param name="type">Type of shape to store in the shapefile.</param>
        /// <returns>A ShapeFile instance.</returns>
        public static ShapeFileProvider Create(String directory, String layerName,
                                               ShapeType type, IGeometryFactory geoFactory)
        {
            return Create(directory, layerName, type, null, geoFactory);
        }

        /// <summary>
        /// Creates a new <see cref="ShapeFile"/> instance and .shp, .shx and, optionally, .dbf file on disk.
        /// </summary>
        /// <remarks>If <paramref name="schema"/> is null, no .dbf file is created.</remarks>
        /// <param name="directory">Directory to create the shapefile in.</param>
        /// <param name="layerName">Name of the shapefile.</param>
        /// <param name="type">Type of shape to store in the shapefile.</param>
        /// <param name="schema">The schema for the attributes DBase file.</param>
        /// <returns>A ShapeFile instance.</returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// 
        /// Thrown if <paramref name="type"/> is <see cref="Providers.ShapeFile.ShapeType.Null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="directory"/> is not a valid path.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="layerName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="layerName"/> has invalid path characters.
        /// </exception>
        public static ShapeFileProvider Create(String directory, String layerName,
                                               ShapeType type, FeatureDataTable schema,
                                               IGeometryFactory geoFactory, ICoordinateSystemFactory coordinateSystemFactory)
        {
            if (type == ShapeType.Null)
            {
                throw new ShapeFileInvalidOperationException(
                    "Cannot create a shapefile with a null geometry type");
            }

            if (String.IsNullOrEmpty(directory) || directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Parameter must be a valid path", "directory");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            return Create(directoryInfo, layerName, type, schema, geoFactory, coordinateSystemFactory);
        }

        public static ShapeFileProvider Create(String directory, String layerName,
                                       ShapeType type, FeatureDataTable schema,
                                       IGeometryFactory geoFactory)
        {
            return Create(directory, layerName, type, schema, geoFactory, null);
        }


        /// <summary>
        /// Creates a new <see cref="ShapeFile"/> instance and .shp, .shx and, optionally, 
        /// .dbf file on disk.
        /// </summary>
        /// <remarks>If <paramref name="model"/> is null, no .dbf file is created.</remarks>
        /// <param name="directory">Directory to create the shapefile in.</param>
        /// <param name="layerName">Name of the shapefile.</param>
        /// <param name="type">Type of shape to store in the shapefile.</param>
        /// <param name="model">The schema for the attributes DBase file.</param>
        /// <returns>A ShapeFile instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="layerName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="layerName"/> has invalid path characters.
        /// </exception>
        public static ShapeFileProvider Create(DirectoryInfo directory, String layerName,
                                               ShapeType type, FeatureDataTable model,
                                               IGeometryFactory geoFactory, ICoordinateSystemFactory coordinateSystemFactory)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            Encoding encoding = Encoding.GetEncoding(culture.TextInfo.OEMCodePage);
            return Create(directory, layerName, type, model, culture, encoding, geoFactory, coordinateSystemFactory);
        }


        public static ShapeFileProvider Create(DirectoryInfo directory, String layerName,
                                              ShapeType type, FeatureDataTable model,
                                              IGeometryFactory geoFactory)
        {
            return Create(directory, layerName, type, model, geoFactory, null);
        }

        /// <summary>
        /// Creates a new <see cref="ShapeFile"/> instance and .shp, .shx and, optionally, 
        /// .dbf file on disk.
        /// </summary>
        /// <remarks>If <paramref name="model"/> is null, no .dbf file is created.</remarks>
        /// <param name="directory">Directory to create the shapefile in.</param>
        /// <param name="layerName">Name of the shapefile.</param>
        /// <param name="type">Type of shape to store in the shapefile.</param>
        /// <param name="model">The schema for the attributes DBase file.</param>
        /// <param name="culture">
        /// The culture info to use to determine default encoding and attribute formatting.
        /// </param>
        /// <param name="encoding">
        /// The encoding to use if different from the <paramref name="culture"/>'s default encoding.
        /// </param>
        /// <returns>A ShapeFile instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="layerName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="layerName"/> has invalid path characters.
        /// </exception>
        public static ShapeFileProvider Create(DirectoryInfo directory, String layerName,
                                               ShapeType type, FeatureDataTable model,
                                               CultureInfo culture, Encoding encoding,
                                               IGeometryFactory geoFactory, ICoordinateSystemFactory coordinateSystemFactory)
        {
            if (String.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException("layerName");
            }

            if (layerName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("Parameter cannot have invalid filename characters", "layerName");
            }

            if (!String.IsNullOrEmpty(Path.GetExtension(layerName)))
            {
                layerName = Path.GetFileNameWithoutExtension(layerName);
            }

            DataTable schemaTable = null;

            if (model != null)
            {
                schemaTable = DbaseSchema.DeriveSchemaTable(model);
            }

            String shapeFile = Path.Combine(directory.FullName, layerName + ".shp");

            using (MemoryStream buffer = new MemoryStream(100))
            {
                using (BinaryWriter writer = new BinaryWriter(buffer))
                {
                    writer.Seek(0, SeekOrigin.Begin);
                    writer.Write(ByteEncoder.GetBigEndian(ShapeFileConstants.HeaderStartCode));
                    writer.Write(new Byte[20]);
                    writer.Write(ByteEncoder.GetBigEndian(ShapeFileConstants.HeaderSizeBytes / 2));
                    writer.Write(ByteEncoder.GetLittleEndian(ShapeFileConstants.VersionCode));
                    writer.Write(ByteEncoder.GetLittleEndian((Int32)type));
                    writer.Write(ByteEncoder.GetLittleEndian(0.0));
                    writer.Write(ByteEncoder.GetLittleEndian(0.0));
                    writer.Write(ByteEncoder.GetLittleEndian(0.0));
                    writer.Write(ByteEncoder.GetLittleEndian(0.0));
                    writer.Write(new Byte[32]); // Z-values and M-values

                    Byte[] header = buffer.ToArray();

                    using (FileStream shape = File.Create(shapeFile))
                    {
                        shape.Write(header, 0, header.Length);
                    }

                    using (FileStream index = File.Create(Path.Combine(directory.FullName, layerName + ".shx")))
                    {
                        index.Write(header, 0, header.Length);
                    }
                }
            }

            if (schemaTable != null)
            {
                String filePath = Path.Combine(directory.FullName, layerName + ".dbf");
                DbaseFile file = DbaseFile.CreateDbaseFile(filePath, schemaTable, culture, encoding, geoFactory);
                file.Close();
            }

            if (geoFactory.SpatialReference != null)
            {
                string filePath = Path.Combine(directory.FullName, layerName + ".prj");
                File.WriteAllText(filePath, geoFactory.SpatialReference.Wkt);
            }

            return new ShapeFileProvider(shapeFile, geoFactory, coordinateSystemFactory);
        }

        #endregion

        #region ShapeFile specific properties

        /// <summary>
        /// Gets the name of the DBase attribute file.
        /// </summary>
        public String DbfFilename
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_filename)),
                                    Path.GetFileNameWithoutExtension(_filename) + ".dbf");
            }
        }

        /// <summary>
        /// Gets or sets the encoding used for parsing strings from the DBase DBF file.
        /// </summary>
        /// <remarks>
        /// The DBase default encoding is <see cref="System.Text.Encoding.UTF8"/>.
        /// </remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if property is read or set and the shapefile is closed. 
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ShapeFileIsInvalidException">
        /// Thrown if set and there is no DBase file with this shapefile.
        /// </exception>
        public Encoding Encoding
        {
            get
            {
                checkOpen();

                return HasDbf
                           ? _dbaseFile.Encoding
                           : Encoding.ASCII;
            }
        }

        /// <summary>
        /// Gets the filename of the shapefile
        /// </summary>
        /// <remarks>If the filename changes, indexes will be rebuilt</remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is executed and the shapefile is open or 
        /// if set and the specified filename already exists.
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        /// <exception cref="ShapeFileIsInvalidException">
        /// Thrown if set and the shapefile cannot be opened after a rename.
        /// </exception>
        public String Filename
        {
            get { return _filename; }
            // set removed after r225
        }

        /// <summary>
        /// Gets or sets a delegate used for filtering records from the shapefile.
        /// </summary>
        public Predicate<IFeatureDataRecord> Filter
        {
            get { return _filterDelegate; }
            set { _filterDelegate = value; }
        }

        /// <summary>
        /// Gets true if the shapefile has an attributes file, false otherwise.
        /// </summary>
        public Boolean HasDbf
        {
            get { return _hasDbf; }
        }

        /// <summary>
        /// The name given to the row identifier in a ShapeFileProvider.
        /// </summary>
        public String IdColumnName
        {
            get { return ShapeFileConstants.IdColumnName; }
        }

        /// <summary>
        /// Gets the record index (.shx file) filename for the given shapefile
        /// </summary>
        public String IndexFilename
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_filename)),
                                    Path.GetFileNameWithoutExtension(_filename) + ".shx");
            }
        }

        /// <summary>
        /// Gets a value indicating if the shapefile is spatially indexed.
        /// </summary>
        public Boolean IsSpatiallyIndexed
        {
            get { return _isIndexed; }
            set
            {
                //throw new NotImplementedException("Allow shapefile provider to be " +
                //                                  "created without an index. [workitem:13025]");
                if (IsOpen)
                {
                    throw new NotSupportedException("Setting 'IsSpatiallyIndexed' only supported before" +
                                                    " the shapefile is opened.");
                }

                _isIndexed = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Providers.ShapeFile.ShapeType">
        /// shape geometry type</see> in this shapefile.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The property isn't set until the first time the data source has been opened,
        /// and will throw an exception if this property has been called since initialization. 
        /// </para>
        /// <para>
        /// All the non-<see cref="SharpMap.Data.Providers.ShapeFile.ShapeType.Null"/> 
        /// shapes in a shapefile are required to be of the same shape type.
        /// </para>
        /// </remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if property is read and the shapefile is closed. 
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        public ShapeType ShapeType
        {
            get
            {
                checkOpen();
                return _header.ShapeType;
            }
        }

        public DbaseHeader DbaseHeader
        {
            get { return _dbaseFile.Header; }
        }

        public Int32 FeatureCount
        {
            get
            {
                return GetFeatureCount();
            }
        }
        #endregion

        #region ShapeFile specific methods

        /// <summary>
        /// Opens the shapefile with optional exclusive access for
        /// faster write performance during bulk updates.
        /// </summary>
        /// <param name="exclusive">
        /// True if exclusive access is desired, false otherwise.
        /// </param>
        public void Open(Boolean exclusive)
        {
            _coordsysReadFromFile = false; // jd setting to false to stop error on second and subsequent open

            if (IsOpen)
            {
                return;
            }

            try
            {
                //enableReading();
                _shapeFileStream = new FileStream(Filename,
                                                  FileMode.OpenOrCreate,
                                                  FileAccess.ReadWrite,
                                                  exclusive ? FileShare.None : FileShare.Read,
                                                  4096,
                                                  FileOptions.None);

                _shapeFileReader = new BinaryReader(_shapeFileStream);
                _shapeFileWriter = new BinaryWriter(_shapeFileStream);

                base.Open();

                // Read projection file
                parseProjection();

                // Load spatial (r-tree) index
                loadSpatialIndex(_hasFileBasedSpatialIndex);

                if (HasDbf)
                {
                    _dbaseFile = new DbaseFile(DbfFilename, GeometryFactory);
                    _dbaseFile.Open(exclusive);
                }
            }
            catch (Exception)
            {
                base.Close();
                throw;
            }
        }

        /// <summary>
        /// Forces a rebuild of the spatial index. 
        /// If the instance of the ShapeFile provider
        /// uses a file-based index the file is rewritten to disk,
        /// otherwise it is kept only in memory.
        /// </summary>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is executed and the shapefile is closed. 
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        public void RebuildSpatialIndex()
        {
            checkOpen();
            //enableReading();

            if (_hasFileBasedSpatialIndex)
            {
                if (File.Exists(_filename + SharpMapShapeFileIndexFileExtension))
                {
                    File.Delete(_filename + SharpMapShapeFileIndexFileExtension);
                }

                _spatialIndex = createSpatialIndexFromFile(_filename);
            }
            else
            {
                _spatialIndex = createSpatialIndex();
            }
        }

        public IFeatureDataReader GetReader()
        {
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(GetExtents());
            return ExecuteFeatureQuery(query, FeatureQueryExecutionOptions.FullFeature);
        }
        #endregion

        #region IProvider Members

        /// <summary>
        /// Gets the connection ID of the data source.
        /// </summary>
        /// <remarks>
        /// The connection ID of a shapefile is its filename.
        /// </remarks>
        public override String ConnectionId
        {
            get { return _filename; }
        }

        /// <summary>
        /// Computes the extents of the data source.
        /// </summary>
        /// <returns>
        /// An <see cref="IExtents"/> instance describing the extents of the entire data source.
        /// </returns>
        public override IExtents GetExtents()
        {
            IExtents extents = _spatialIndex != null
                                   ? _spatialIndex.Bounds
                                   : _header.Extents;

            return CoordinateTransformation != null && CoordinateTransformation.Target != extents.SpatialReference
                       ? CoordinateTransformation.Transform(extents, GeometryFactory)
                       : extents;
        }

        private void initSpatialReference(ICoordinateSystem coordinateSystem)
        {
            //checkOpen();
            if (_coordsysReadFromFile)
            {
                throw new ShapeFileInvalidOperationException("Coordinate system is specified in " +
                                                             "projection file and is read only");
            }

            OriginalSpatialReference = coordinateSystem;
            OriginalSrid = coordinateSystem.AuthorityCode;
            GeometryFactory.SpatialReference = SpatialReference;
            GeometryFactory.Srid = Srid;
        }

        #region Methods

        /// <summary>
        /// Closes the data source
        /// </summary>
        public override void Close()
        {
            (this as IDisposable).Dispose();
        }

        public override Object ExecuteQuery(Expression query)
        {
            FeatureQueryExpression featureQuery = query as FeatureQueryExpression;

            if (featureQuery == null)
            {
                throw new ArgumentException("The query must be a non-null FeatureQueryExpression.");
            }

            return ExecuteFeatureQuery(featureQuery);
        }



        /// <summary>
        /// Opens the data source
        /// </summary>
        public override void Open()
        {
            Open(false);
        }

        #endregion

        #endregion

        #region IFeatureLayerProvider Members

        public override FeatureDataTable CreateNewTable()
        {
            return getNewTable();
        }

        public override IGeometryFactory GeometryFactory
        {
            get { return base.GeometryFactory; }
            set
            {
                // [codekaizen 2008-10-07]
                // Setting this doesn't seem like a good idea... probably need more test cases.
                throw new NotSupportedException("Setting GeometryFactory not well tested, disallowing for now.");
                //_geoFactory = value;
            }
        }

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        public override Int32 GetFeatureCount()
        {
            return _shapeFileIndex.Count;
        }

        /// <summary>
        /// Returns a <see cref="DataTable"/> with rows describing the columns in the schema
        /// for the configured provider. Provides the same result as 
        /// <see cref="IDataReader.GetSchemaTable"/>.
        /// </summary>
        /// <seealso cref="IDataReader.GetSchemaTable"/>
        /// <returns>A DataTable that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            checkOpen();

            DataTable schemaTable = _dbaseFile.GetSchemaTable();

            DataRow oidColumn = schemaTable.NewRow();
            oidColumn[ProviderSchemaHelper.ColumnNameColumn] = "OID";
            oidColumn[ProviderSchemaHelper.ColumnSizeColumn] = 0;
            oidColumn[ProviderSchemaHelper.ColumnOrdinalColumn] = 0;
            oidColumn[ProviderSchemaHelper.NumericPrecisionColumn] = 0;
            oidColumn[ProviderSchemaHelper.NumericScaleColumn] = 0;
            oidColumn[ProviderSchemaHelper.DataTypeColumn] = typeof(UInt32);
            oidColumn[ProviderSchemaHelper.AllowDBNullColumn] = true;
            oidColumn[ProviderSchemaHelper.IsReadOnlyColumn] = false;
            oidColumn[ProviderSchemaHelper.IsUniqueColumn] = true;
            oidColumn[ProviderSchemaHelper.IsRowVersionColumn] = false;
            oidColumn[ProviderSchemaHelper.IsKeyColumn] = true;
            oidColumn[ProviderSchemaHelper.IsAutoIncrementColumn] = false;
            oidColumn[ProviderSchemaHelper.IsLongColumn] = false;
            schemaTable.Rows.InsertAt(oidColumn, 0);

            for (Int32 i = 1; i < schemaTable.Rows.Count; i++)
            {
                schemaTable.Rows[i][ProviderSchemaHelper.ColumnOrdinalColumn] = i;
            }

            return schemaTable;
        }

        /// <summary>
        /// Gets the locale of the data as a CultureInfo.
        /// </summary>
        public override CultureInfo Locale
        {
            get { return _dbaseFile.CultureInfo; }
        }

        /// <summary>
        /// Sets the schema of the given table to match the schema of the shapefile's attributes.
        /// </summary>
        /// <param name="target">Target table to set the schema of.</param>
        public override void SetTableSchema(FeatureDataTable target)
        {
            checkOpen();
            _dbaseFile.SetTableSchema(target, SchemaMergeAction.AddWithKey);
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            if (query == null) throw new ArgumentNullException("query");

            checkOpen();

            lock (_readerSync)
            {
                //if (_currentReader != null)
                //{
                //    throw new ShapeFileInvalidOperationException("Can't open another ShapeFileDataReader " +
                //                                                 "on this ShapeFile, since another reader " +
                //                                                 "is already active.");
                //}

                ShapeFileDataReader reader = new ShapeFileDataReader(this, query, options);
                //reader.Disposed += readerDisposed;
                reader.CoordinateTransformation = CoordinateTransformation;
                return reader;
            }
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return InternalExecuteFeatureQuery(query, FeatureQueryExecutionOptions.FullFeature);
        }

        #endregion

        #region IFeatureLayerProvider<UInt32> Members

        public IExtents GetExtentsByOid(UInt32 oid)
        {
            checkOpen();

            IExtents result;

            if (Filter != null) // Apply filtering
            {
                IFeatureDataRecord fdr = GetFeatureByOid(oid);

                result = fdr != null
                               ? fdr.Extents
                               : null;
            }
            else
            {
                result = readExtents(oid);
            }

            return result;
        }

        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="ProviderBase.IsOpen"/> 
        /// before calling.
        /// </exception>
        public IFeatureDataRecord GetFeatureByOid(UInt32 oid)
        {
            return getFeature(oid, null);
        }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<UInt32> oids)
        {
            //FeatureDataTable<UInt32> table = CreateNewTable() as FeatureDataTable<UInt32>;
            //Assert.IsNotNull(table);
            //table.IsSpatiallyIndexed = false;

            foreach (UInt32 oid in oids)
            {
                yield return getFeature(oid, null);
            }

            //return table;
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="IGeometry"/></returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="ProviderBase.IsOpen"/> 
        /// before calling.
        /// </exception>
        public IGeometry GetGeometryByOid(UInt32 oid)
        {
            checkOpen();

            IGeometry result;

            if (Filter != null) // Apply filtering
            {
                IFeatureDataRecord fdr = GetFeatureByOid(oid);

                result = fdr != null
                               ? fdr.Geometry
                               : null;
            }
            else
            {
                result = readGeometry(oid);
            }

            return result;
        }

        /// <summary>
        /// Returns feature oids which match <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query expression for features.</param>
        /// <returns>
        /// An enumeration of oids (Object ids) which match the given <paramref name="query"/>.
        /// </returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="ProviderBase.IsOpen"/> 
        /// before calling.
        /// </exception>
        public IEnumerable<UInt32> ExecuteOidQuery(SpatialBinaryExpression query)
        {
            // if (query == null) throw new ArgumentNullException("query");

            //jd:modifying so that null query returns all OIDS

            if (Equals(query, null))
                foreach (UInt32 u in getAllOIDs())
                    yield return u;
            else
            {


                SpatialOperation queryOp = query.Op;

                if (queryOp == SpatialOperation.None)
                {
                    yield break;
                }

                checkOpen();

                Boolean isQueryLeft = query.IsSpatialExpressionLeft;
                IEnumerable<IdBounds> keys = _isIndexed
                                                 ? queryIndex(query.SpatialExpression)
                                                 : queryData(query.SpatialExpression);

                IDictionary<UInt32, Object> idsInBounds = null;

                Boolean isDisjoint = queryOp == SpatialOperation.Disjoint;

                if (isDisjoint)
                {
                    idsInBounds = new Dictionary<UInt32, Object>();
                }

                foreach (IdBounds key in keys)
                {
                    UInt32 id = key.Id;

                    if (isMatch(queryOp, isQueryLeft, key, query.SpatialExpression))
                    {
                        yield return id;
                    }

                    if (isDisjoint)
                    {
                        idsInBounds[id] = null;
                    }
                }

                if (isDisjoint)
                {
                    foreach (IdBounds key in getAllKeys())
                    {
                        if (!idsInBounds.ContainsKey(key.Id))
                        {
                            yield return key.Id;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the schema of the given table to match the schema of the shapefile's attributes.
        /// </summary>
        /// <param name="target">Target table to set the schema of.</param>
        public void SetTableSchema(FeatureDataTable<UInt32> target)
        {
            if (String.CompareOrdinal(target.IdColumn.ColumnName, DbaseSchema.OidColumnName) != 0)
            {
                throw new InvalidOperationException(
                    "Object ID column names for this schema and 'target' schema must be identical, " +
                    "including case. For case-insensitive or type-only matching, use " +
                    "SetTableSchema(FeatureDataTable, SchemaMergeAction) with the " +
                    "SchemaMergeAction.CaseInsensitive option and/or SchemaMergeAction.KeyByType " +
                    "option enabled.");
            }

            SetTableSchema(target, SchemaMergeAction.AddWithKey);
        }

        /// <summary>
        /// Sets the schema of the given table to match the schema of the shapefile's attributes.
        /// </summary>
        /// <param name="target">Target table to set the schema of.</param>
        /// <param name="mergeAction">Action or actions to take when schemas don't match.</param>
        public void SetTableSchema(FeatureDataTable<UInt32> target, SchemaMergeAction mergeAction)
        {
            checkOpen();
            _dbaseFile.SetTableSchema(target, mergeAction);
        }

        #endregion

        #region IWritableFeatureLayerProvider<UInt32> Members

        /// <summary>
        /// Adds a feature to the end of a shapefile.
        /// </summary>
        /// <param name="feature">Feature to append.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="feature"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="feature.Geometry"/> is null.
        /// </exception>
        public void Insert(FeatureDataRow<UInt32> feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            if (feature.Geometry == null)
            {
                throw new InvalidOperationException("Cannot insert a feature with a null geometry");
            }

            checkOpen();
            //enableWriting();

            UInt32 id = _shapeFileIndex.GetNextId();
            feature[ShapeFileConstants.IdColumnName] = id;

            _shapeFileIndex.AddFeatureToIndex(feature);

            IExtents featureExtents = feature.Geometry.Extents;

            if (_spatialIndex != null)
            {
                _spatialIndex.Insert(new IdBounds(id, featureExtents));
            }

            Int32 offset = _shapeFileIndex[id].Offset;
            Int32 length = _shapeFileIndex[id].Length;

            _header.FileLengthInWords = _shapeFileIndex.ComputeShapeFileSizeInWords();
            _header.Extents = GeometryFactory.CreateExtents(_header.Extents, featureExtents);

            if (HasDbf)
            {
                _dbaseFile.AddRow(feature);
            }

            writeGeometry(feature.Geometry, id, offset, length);
            _header.WriteHeader(_shapeFileWriter);
            _shapeFileIndex.Save();
        }

        /// <summary>
        /// Adds features to the end of a shapefile.
        /// </summary>
        /// <param name="features">Enumeration of features to append.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/> is null.
        /// </exception>
        public void Insert(IEnumerable<FeatureDataRow<UInt32>> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            checkOpen();
            //enableWriting();

            IExtents allFeaturesExtents = null;

            foreach (FeatureDataRow<UInt32> feature in features)
            {
                IExtents featureExtents = feature.Geometry == null
                                                  ? null
                                                  : feature.Geometry.Extents;

                if (allFeaturesExtents == null)
                {
                    allFeaturesExtents = featureExtents;
                }
                else
                {
                    allFeaturesExtents.ExpandToInclude(featureExtents);
                }

                UInt32 id = _shapeFileIndex.GetNextId();

                _shapeFileIndex.AddFeatureToIndex(feature);

                if (_spatialIndex != null)
                {
                    _spatialIndex.Insert(new IdBounds(id, featureExtents));
                }

                feature[ShapeFileConstants.IdColumnName] = id;

                Int32 offset = _shapeFileIndex[id].Offset;
                Int32 length = _shapeFileIndex[id].Length;

                writeGeometry(feature.Geometry, id, offset, length);

                if (HasDbf)
                {
                    _dbaseFile.AddRow(feature);
                }
            }

            _shapeFileIndex.Save();

            _header.Extents = GeometryFactory.CreateExtents(_header.Extents, allFeaturesExtents);
            _header.FileLengthInWords = _shapeFileIndex.ComputeShapeFileSizeInWords();
            _header.WriteHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Updates a feature in a shapefile by deleting the previous 
        /// version and inserting the updated version.
        /// </summary>
        /// <param name="feature">Feature to update.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="feature"/> is null.
        /// </exception>
        public void Update(FeatureDataRow<UInt32> feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            if (feature.RowState != DataRowState.Modified)
            {
                return;
            }

            checkOpen();
            //enableWriting();

            if (feature.IsGeometryModified)
            {
                Delete(feature);
                Insert(feature);
            }
            else if (HasDbf)
            {
                _dbaseFile.UpdateRow(feature.Id, feature);
            }

            feature.AcceptChanges();
        }

        /// <summary>
        /// Updates a set of features in a shapefile by deleting the previous 
        /// versions and inserting the updated versions.
        /// </summary>
        /// <param name="features">Enumeration of features to update.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/> is null.
        /// </exception>
        public void Update(IEnumerable<FeatureDataRow<UInt32>> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            checkOpen();
            //enableWriting();

            foreach (FeatureDataRow<UInt32> feature in features)
            {
                if (feature.RowState != DataRowState.Modified)
                {
                    continue;
                }

                if (feature.IsGeometryModified)
                {
                    Delete(feature);
                    Insert(feature);
                }
                else if (HasDbf)
                {
                    _dbaseFile.UpdateRow(feature.Id, feature);
                }

                feature.AcceptChanges();
            }
        }

        /// <summary>
        /// Deletes a row from the shapefile by marking it as deleted.
        /// </summary>
        /// <param name="feature">Feature to delete.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="feature"/> is null.
        /// </exception>
        public void Delete(FeatureDataRow<UInt32> feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            if (!_shapeFileIndex.ContainsKey(feature.Id))
            {
                return;
            }

            checkOpen();
            //enableWriting();

            feature.Geometry = null;

            UInt32 id = feature.Id;
            Int32 length = _shapeFileIndex[id].Length;
            Int32 offset = _shapeFileIndex[id].Offset;
            writeGeometry(null, feature.Id, offset, length);
        }

        /// <summary>
        /// Deletes a set of rows from the shapefile by marking them as deleted.
        /// </summary>
        /// <param name="features">Features to delete.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/> is null.
        /// </exception>
        public void Delete(IEnumerable<FeatureDataRow<UInt32>> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            checkOpen();
            //enableWriting();

            foreach (FeatureDataRow<UInt32> feature in features)
            {
                if (!_shapeFileIndex.ContainsKey(feature.Id))
                {
                    continue;
                }

                feature.Geometry = null;

                UInt32 id = feature.Id;
                Int32 length = _shapeFileIndex[id].Length;
                Int32 offset = _shapeFileIndex[id].Offset;
                writeGeometry(null, feature.Id, offset, length);
            }
        }

        ///// <summary>
        ///// Saves features to the shapefile.
        ///// </summary>
        ///// <param name="table">
        ///// A FeatureDataTable containing feature data and geometry.
        ///// </param>
        ///// <exception cref="ShapeFileInvalidOperationException">
        ///// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        ///// </exception>
        //public void Save(FeatureDataTable<UInt32> table)
        //{
        //    if (table == null)
        //    {
        //        throw new ArgumentNullException("table");
        //    }

        //    checkOpen();
        //    enableWriting();

        //    _shapeFileStream.Position = ShapeFileConstants.HeaderSizeBytes;
        //    foreach (FeatureDataRow row in table.Rows)
        //    {
        //        if (row is FeatureDataRow<UInt32>)
        //        {
        //            _tree.Insert(new RTreeIndexEntry<UInt32>((row as FeatureDataRow<UInt32>).Id, row.Geometry.GetBoundingBox()));
        //        }
        //        else
        //        {
        //            _tree.Insert(new RTreeIndexEntry<UInt32>(getNextId(), row.Geometry.GetBoundingBox()));
        //        }

        //        writeFeatureRow(row);
        //    }

        //    writeIndex();
        //    writeHeader(_shapeFileWriter);
        //}

        #endregion

        #endregion

        #region General helper functions

        internal static Int32 ComputeGeometryLengthInWords(IGeometry geometry)
        {
            if (geometry == null)
            {
                throw new NotSupportedException("Writing null shapes not supported in this version.");
            }

            Int32 byteCount;

            if (geometry is IPoint)
            {
                byteCount = 20; // ShapeType integer + 2 doubles at 8 bytes each
            }
            else if (geometry is IMultiPoint)
            {
                byteCount = 4 /* ShapeType Integer */
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 /* NumPoints integer */
                            + 16 * (geometry as IMultiPoint).Count;
            }
            else if (geometry is ILineString)
            {
                byteCount = 4 /* ShapeType Integer */
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 + 4
                    /* NumPoints and NumParts integers */
                            + 4 /* Parts Array 1 integer Int64 */
                            + 16 * (geometry as ILineString).Coordinates.Count;
            }
            else if (geometry is IMultiLineString)
            {
                Int32 pointCount = 0;

                foreach (ILineString line in (geometry as IEnumerable<ILineString>))
                {
                    pointCount += line.Coordinates.Count;
                }

                byteCount = 4 /* ShapeType Integer */
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 + 4
                    /* NumPoints and NumParts integers */
                            + 4 * (geometry as IMultiLineString).Count /* Parts array of integer indexes */
                            + 16 * pointCount;
            }
            else if (geometry is IPolygon) /*jd: Contains Modifications from Lee Keel www.trimble.com to cope with unclosed polygons  */
            {
                Int32 pointCount = (geometry as IPolygon).ExteriorRing.Coordinates.Count;
                ILineString exring = (geometry as IPolygon).ExteriorRing;

                /* need to account for cases where the polygon is not closed. */
                if (exring.Coordinates[0] != exring.Coordinates[exring.Coordinates.Count - 1])
                    pointCount++;

                foreach (ILinearRing ring in (geometry as IPolygon).InteriorRings)
                {
                    pointCount += ring.Coordinates.Count;
                    /* need to account for cases where the polygon is not closed. */
                    if (ring.Coordinates[0] != ring.Coordinates[ring.Coordinates.Count - 1])
                        pointCount++;
                }

                byteCount = 4 /* ShapeType Integer */
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 + 4
                    /* NumPoints and NumParts integers */
                            +
                            4 *
                            ((geometry as IPolygon).InteriorRingsCount + 1
                    /* Parts array of rings: count of interior + 1 for exterior ring */)
                            + 16 * pointCount;
            }
            else if (geometry is IMultiPolygon)/*jd: Contains Modifications from Lee Keel www.trimble.com to cope with unclosed polygons  */
            {
                Int32 pointCount = 0;
                Int32 ringCount = 0;
                IMultiPolygon mp = geometry as IMultiPolygon;
                foreach (IPolygon p in mp as IEnumerable<IPolygon>)
                {
                    pointCount += p.ExteriorRing.PointCount;
                    foreach (ILinearRing ring in p.InteriorRings)
                    {
                        pointCount += ring.PointCount;
                        /* need to account for cases where the polygon is not closed. */
                        if (ring.Coordinates[0] != ring.Coordinates[ring.Coordinates.Count - 1])
                            pointCount++;
                    }

                    ringCount += p.InteriorRingsCount + 1;

                }

                byteCount = 4
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 + 4
                            + 4 * ringCount
                            + 16 * pointCount;
            }
            else
            {
                throw new NotSupportedException("Currently unsupported geometry type.");
            }

            return byteCount / 2; // number of 16-bit words
        }

        private Boolean isMatch(SpatialOperation op,
                                Boolean isQueryLeft,
                                IdBounds idBounds,
                                SpatialExpression spatialExpression)
        {
            GeometryExpression geometryExpression = spatialExpression as GeometryExpression;

            IGeometry candidateGeometry = geometryExpression == null
                                              ? null
                                              : idBounds.Feature == null
                                                    ? GetGeometryByOid(idBounds.Id)
                                                    : idBounds.Feature.Geometry;
            IExtents candidateExtents = geometryExpression == null
                                            ? idBounds.Bounds
                                            : candidateGeometry.Extents;

            if (geometryExpression != null)
            {
                return SpatialBinaryExpression.IsMatch(op, isQueryLeft, candidateGeometry, geometryExpression.Geometry);
            }

            ExtentsExpression extentsExpression = spatialExpression as ExtentsExpression;

            if (extentsExpression != null)
            {
                return SpatialBinaryExpression.IsMatch(op, isQueryLeft, candidateExtents, extentsExpression.Extents);
            }

            return true;
        }

        private void checkOpen()
        {
            if (!IsOpen)
            {
                throw new ShapeFileInvalidOperationException("An attempt was made to access a closed data source.");
            }
        }

        //private static IEnumerable<UInt32> getKeysFromIndexEntries(IEnumerable<RTreeIndexEntry<UInt32>> entries)
        //{
        //    foreach (RTreeIndexEntry<UInt32> entry in entries)
        //    {
        //        yield return entry.Value;
        //    }
        //}

        private IEnumerable<IdBounds> getAllKeys()
        {
            return _isIndexed
                       ? getKeysFromSpatialIndex(_spatialIndex.Bounds)
                       : getKeysFromShapefileIndex(GetExtents());
        }

        private IEnumerable<UInt32> getAllOIDs()
        {
            foreach (IdBounds bounds in getAllKeys())
                yield return bounds.Id;
        }

        private IEnumerable<IdBounds> getKeysFromSpatialIndex(IExtents toIntersect)
        {
            return _spatialIndex.Query(toIntersect);
        }

        private IEnumerable<IdBounds> getKeysFromShapefileIndex(IExtents toIntersect)
        {
            foreach (KeyValuePair<UInt32, ShapeFileIndex.IndexEntry> entry in _shapeFileIndex)
            {
                UInt32 oid = entry.Key;

                IExtents featureExtents = readExtents(oid);

                if (toIntersect.Intersects(featureExtents))
                {
                    yield return new IdBounds(oid, featureExtents);
                }
            }
        }

        //private static IEnumerable<UInt32> getUint32IdsFromObjects(IEnumerable oids)
        //{
        //    foreach (Object oid in oids)
        //    {
        //        yield return (UInt32)oid;
        //    }
        //}

        //private IEnumerable<IFeatureDataRecord> getFeatureRecordsFromIds(IEnumerable<UInt32> ids,
        //                                                                 FeatureDataTable<UInt32> table)
        //{
        //    foreach (UInt32 id in ids)
        //    {
        //        yield return getFeature(id, table);
        //    }
        //}

        private FeatureDataTable<UInt32> getNewTable()
        {
            return HasDbf
                ? _dbaseFile.NewTable
                : FeatureDataTable<UInt32>.CreateEmpty(ShapeFileConstants.IdColumnName, GeometryFactory);
        }

        /// <summary>
        /// Gets a row from the DBase attribute file which has the 
        /// specified <paramref name="oid">Object id</paramref> created from
        /// <paramref name="table"/>.
        /// </summary>
        /// <param name="oid">Object id to lookup.</param>
        /// <param name="table">DataTable with schema matching the feature to retrieve.</param>
        /// <returns>Row corresponding to the Object id.</returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed.
        /// Check <see cref="ProviderBase.IsOpen"/> before calling.
        /// </exception>
        private IFeatureDataRecord getFeature(UInt32 oid, FeatureDataTable<UInt32> table)
        {
            checkOpen();

            IFeatureDataRecord dr;

            if (table != null)
            {
                FeatureDataRow<UInt32> featureRecord = HasDbf
                    // [rp] - should table get passed as the 2nd param here?
                    ? _dbaseFile.GetAttributes(oid, table) as FeatureDataRow<UInt32>
                    : table.NewRow(oid);

                featureRecord.Geometry = readGeometry(oid);
                dr = featureRecord;
            }
            else
            {
                ShapeFileFeatureDataRecord featureRecord = HasDbf
                           ? _dbaseFile.GetAttributes(oid, null) as ShapeFileFeatureDataRecord
                           : ShapeFileFeatureDataRecord.IdOnlyRecord;

                featureRecord.Geometry = readGeometry(oid);
                dr = featureRecord;
            }

            return Filter == null || Filter(dr) ? dr : null;
        }

        //private ShapeFileFeatureDataRecord getOidOnlyFeatureRecord(UInt32 oid)
        //{
        //    ShapeFileFeatureDataRecord record = new ShapeFileFeatureDataRecord(_oidFieldList);
        //    record.SetColumnValue(0, oid);
        //    return record;
        //}

        //private void readerDisposed(Object sender, EventArgs e)
        //{
        //    lock (_readerSync)
        //    {
        //        _currentReader = null;
        //    }
        //}

        //private IEnumerable<IFeatureDataRecord> matchFeatureGeometry(IEnumerable<IdBounds> keys, 
        //                                                             IGeometry query,
        //                                                             Boolean queryIsLeft,
        //                                                             SpatialOperation op)
        //{
        //    foreach (IdBounds key in keys)
        //    {
        //        IFeatureDataRecord candidate = GetFeatureByOid(key.Id);

        //        if (SpatialBinaryExpression.IsMatch(op, queryIsLeft, query, candidate.Geometry))
        //        {
        //            yield return candidate;
        //        }
        //    }
        //}

        private IEnumerable<IdBounds> queryData(SpatialExpression spatialExpression)
        {
            IExtents queryExtents = getExtentsFromSpatialQuery(spatialExpression);

            foreach (KeyValuePair<UInt32, ShapeFileIndex.IndexEntry> entry in _shapeFileIndex)
            {
                UInt32 oid = entry.Key;

                IExtents featureExtents = readExtents(oid);

                if (queryExtents.Intersects(featureExtents))
                {
                    yield return new IdBounds(oid, featureExtents);
                }
            }
        }

        private static IExtents getExtentsFromSpatialQuery(SpatialExpression spatialExpression)
        {
            GeometryExpression geometryExpression = spatialExpression as GeometryExpression;
            ExtentsExpression extentsExpression = spatialExpression as ExtentsExpression;

            Assert.IsTrue(geometryExpression != null || extentsExpression != null);

            IGeometry geometry = geometryExpression != null
                                     ? geometryExpression.Geometry
                                     : null;

            Assert.IsTrue(extentsExpression != null || geometry != null);

            IExtents extents = extentsExpression != null
                                       ? extentsExpression.Extents
                                       : geometry.Extents;

            return extents;
        }

        #endregion

        #region Spatial indexing helper functions

        private IEnumerable<IdBounds> queryIndex(SpatialExpression spatialExpression)
        {
            IExtents extents = getExtentsFromSpatialQuery(spatialExpression);

            return _spatialIndex.Query(extents);
        }

        /// <summary>
        /// Loads a spatial index from a file. If it doesn't exist, one is created and saved
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>QuadTree index</returns>
        private ISpatialIndex<IExtents, IdBounds> createSpatialIndexFromFile(String filename)
        {
            if (File.Exists(filename + SharpMapShapeFileIndexFileExtension))
            {
                throw new NotImplementedException();
                //using(FileStream indexStream =
                //        new FileStream(filename + ".sidx", FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    return DynamicRTree<UInt32>.FromStream(indexStream);
                //}
            }
            else
            {
                ISpatialIndex<IExtents, IdBounds> tree = createSpatialIndex();

                //using (FileStream indexStream =
                //        new FileStream(filename + ".sidx", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                //{
                //    tree.SaveIndex(indexStream);
                //}

                return tree;
            }
        }

        /// <summary>
        /// Generates a spatial index for a specified shape file.
        /// </summary>
        private ISpatialIndex<IExtents, IdBounds> createSpatialIndex()
        {
            // TODO: implement Post-optimization restructure strategy
            IIndexRestructureStrategy<IExtents, IdBounds> restructureStrategy = new NullRestructuringStrategy<IExtents, IdBounds>();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.None, 4.0);
            IItemInsertStrategy<IExtents, IdBounds> insertStrategy = new GuttmanQuadraticInsert<IdBounds>(GeometryFactory);
            INodeSplitStrategy<IExtents, IdBounds> nodeSplitStrategy = new GuttmanQuadraticSplit<IdBounds>(GeometryFactory);
            DynamicRTreeBalanceHeuristic indexHeuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);
            IdleMonitor idleMonitor = null;

            DynamicRTree<IdBounds> index = new SelfOptimizingDynamicSpatialIndex<IdBounds>(
                                                                                GeometryFactory,
                                                                                restructureStrategy,
                                                                                restructureHeuristic,
                                                                                insertStrategy,
                                                                                nodeSplitStrategy,
                                                                                indexHeuristic,
                                                                                idleMonitor);

            UInt32 featureCount = (UInt32)GetFeatureCount();

            for (UInt32 i = 0; i < featureCount; i++)
            {
                IGeometry geom = readGeometry(i + 1); //jd: shapefiles have 1 based index

                if (geom == null || geom.IsEmpty)
                {
                    continue;
                }

                IExtents extents = geom.Extents;

                index.Insert(new IdBounds(i + 1, extents));
            }

            return index;
        }

        private void loadSpatialIndex(Boolean loadFromFile)
        {
            loadSpatialIndex(false, loadFromFile);
        }

        private void loadSpatialIndex(Boolean forceRebuild, Boolean loadFromFile)
        {
            if (!_isIndexed)
            {
                return;
            }

            //Only load the tree if we haven't already loaded it, or if we want to force a rebuild
            if (_spatialIndex == null || forceRebuild)
            {
                if (!loadFromFile)
                {
                    _spatialIndex = createSpatialIndex();
                }
                else
                {
                    _spatialIndex = createSpatialIndexFromFile(_filename);
                }
            }
        }

        #endregion

        #region ProviderBase overrides
        protected override void OnPropertyChanged(PropertyDescriptor property)
        {
            base.OnPropertyChanged(property);

            if (property == CoordinateTransformationProperty)
            {
                GeometryFactory.SpatialReference = SpatialReference;
                _coordTransform = CoordinateTransformation.MathTransform;
                _header.Extents = CoordinateTransformation.Transform(_header.Extents, GeometryFactory);
            }
        }
        #endregion

        #region Geometry reading helper functions
        private ShapeType moveToRecord(UInt32 oid)
        {
            Int32 offset = _shapeFileIndex[oid].AbsoluteByteOffset;
            _shapeFileReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            UInt32 storedOid = ByteEncoder.GetBigEndian(_shapeFileReader.ReadUInt32());

            // Skip content length
            for (Int32 i = 0; i < ShapeFileConstants.ShapeRecordContentLengthByteLength; i++)
            {
                _shapeFileReader.ReadByte();
            }

            ShapeType recordType = (ShapeType)ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            if (oid != storedOid)
            {
                throw new ShapeFileIsInvalidException("Record #" + oid + " is stored with #" + storedOid);
            }

            return recordType;
        }

        private ICoordinate readCoordinate()
        {
            Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());

            ICoordinate coord = _coordFactory.Create(x, y);

            return CoordinateTransformation == null
                       ? coord
                       : CoordinateTransformation.MathTransform.Transform(coord);
        }

        private IExtents readExtents(UInt32 oid)
        {
            ShapeType recordType = moveToRecord(oid);

            // Null geometries encode deleted lines, so OIDs remain consistent
            if (recordType == ShapeType.Null)
            {
                return null;
            }

            if (recordType == ShapeType.Point ||
                recordType == ShapeType.PointM ||
                recordType == ShapeType.PointZ)
            {
                IPoint point = readPoint() as IPoint;
                return point == null ? GeometryFactory.CreateExtents() : point.Extents;
            }

            //Double xMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            //Double yMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            //Double xMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            //Double yMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());

            ICoordinate min = readCoordinate();
            ICoordinate max = readCoordinate();

            return GeometryFactory.CreateExtents(min, max);
        }

        //private ICoordinate readOriginalCoordinate()
        //{
        //    Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
        //    Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());

        //    ICoordinate coord = _coordFactory.Create(x, y);

        //    return coord;
        //}

        //private IExtents readExtents(UInt32 oid)
        //{
        //    return _coordTransform == null
        //                    ? readExtents(oid, true)
        //                    : readExtents(oid, false);
        //}

        //private IExtents readExtents(UInt32 oid, Boolean original)
        //{
        //    ShapeType recordType = moveToRecord(oid);

        //    // Null geometries encode deleted lines, so OIDs remain consistent
        //    if (recordType == ShapeType.Null)
        //    {
        //        return null;
        //    }

        //    if (recordType == ShapeType.Point ||
        //        recordType == ShapeType.PointM ||
        //        recordType == ShapeType.PointZ)
        //    {
        //        IPoint point = readPoint() as IPoint;
        //        return point == null ? GeometryFactory.CreateExtents() : point.Extents;
        //    }

        //    //Double xMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
        //    //Double yMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
        //    //Double xMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
        //    //Double yMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());

        //    ICoordinate min;
        //    ICoordinate max;

        //    if (original)
        //    {
        //        min = readOriginalCoordinate();
        //        max = readOriginalCoordinate();
        //    }
        //    else
        //    {
        //        min = readCoordinate();
        //        max = readCoordinate();
        //    }

        //    return GeometryFactory.CreateExtents(min, max);
        //}

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile.
        /// </summary>
        /// <remarks>
        /// Filtering is not applied to this method.
        /// </remarks>
        /// <param name="oid">Object ID</param>
        /// <returns>
        /// <see cref="GeoAPI.Geometries.IGeometry"/> instance from the 
        /// ShapeFile corresponding to <paramref name="oid"/>.
        /// </returns>
        private IGeometry readGeometry(UInt32 oid)
        {
            ShapeType recordType = moveToRecord(oid);

            // Null geometries encode deleted lines, so OIDs remain consistent
            if (recordType == ShapeType.Null)
            {
                return null;
            }

            IGeometry g;

            switch (ShapeType)
            {
                case ShapeType.Point:
                    g = readPoint();
                    break;
                case ShapeType.PolyLine:
                    g = readPolyLine();
                    break;
                case ShapeType.Polygon:
                    g = readPolygon();
                    break;
                case ShapeType.MultiPoint:
                    g = readMultiPoint();
                    break;
                case ShapeType.PointZ:
                    g = readPointZ();
                    break;
                case ShapeType.PolyLineZ:
                    g = readPolyLineZ();
                    break;
                case ShapeType.PolygonZ:
                    g = readPolygonZ();
                    break;
                case ShapeType.MultiPointZ:
                    g = readMultiPointZ();
                    break;
                case ShapeType.PointM:
                    g = readPointM();
                    break;
                case ShapeType.PolyLineM:
                    g = readPolyLineM();
                    break;
                case ShapeType.PolygonM:
                    g = readPolygonM();
                    break;
                case ShapeType.MultiPointM:
                    g = readMultiPointM();
                    break;
                default:
                    throw new ShapeFileUnsupportedGeometryException("ShapeFile type " +
                                                                    ShapeType +
                                                                    " not supported");
            }

            return g;
        }

        private IGeometry readMultiPointM()
        {
            throw new NotSupportedException("MultiPointM features are not currently supported");
        }

        private IGeometry readPolygonM()
        {
            throw new NotSupportedException("PolygonM features are not currently supported");
        }

        private IGeometry readMultiPointZ()
        {
            throw new NotSupportedException("MultiPointZ features are not currently supported");
        }

        private IGeometry readPolyLineZ()
        {
            throw new NotSupportedException("PolyLineZ features are not currently supported");
        }

        private IGeometry readPolyLineM()
        {
            throw new NotSupportedException("PolyLineM features are not currently supported");
        }

        private IGeometry readPointM()
        {
            throw new NotSupportedException("PointM features are not currently supported");
        }

        private IGeometry readPolygonZ()
        {
            throw new NotSupportedException("PolygonZ features are not currently supported");
        }

        private IGeometry readPointZ()
        {
            throw new NotSupportedException("PointZ features are not currently supported");
        }

        private IGeometry readPoint()
        {
            ICoordinate coord = readCoordinate();
            IPoint point = GeometryFactory.CreatePoint(coord);
            return point;
        }

        private IGeometry readMultiPoint()
        {
            // Skip min/max box
            _shapeFileReader.BaseStream.Seek(ShapeFileConstants.BoundingBoxFieldByteLength, SeekOrigin.Current);

            IMultiPoint geometry = GeometryFactory.CreateMultiPoint();

            // Get the number of points
            Int32 pointCount = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            if (pointCount == 0)
            {
                return null;
            }

            for (Int32 i = 0; i < pointCount; i++)
            {
                ICoordinate coord = readCoordinate();
                IPoint point = GeometryFactory.CreatePoint(coord);
                geometry.Add(point);
            }

            return geometry;
        }

        private void readPolyStructure(out Int32 parts, out Int32 points, out Int32[] segments)
        {
            // Skip min/max box
            _shapeFileReader.BaseStream.Seek(ShapeFileConstants.BoundingBoxFieldByteLength, SeekOrigin.Current);

            // Get number of parts (segments)
            parts = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            // Get number of points
            points = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            segments = new Int32[parts + 1];

            // Read in the segment indexes
            for (Int32 b = 0; b < parts; b++)
            {
                segments[b] = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());
            }

            // Add end point
            segments[parts] = points;
        }

        private void readSegments(int lineId, int[] segments, ICoordinateSequence coordinates)
        {
            for (Int32 i = segments[lineId]; i < segments[lineId + 1]; i++)
            {
                //Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                //Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                //coordinates.Add(_geoFactory.CoordinateFactory.Create(x, y));

                ICoordinate coord = readCoordinate();
                coordinates.Add(coord);
            }
        }

        private IGeometry readPolyLine()
        {
            Int32 parts;
            Int32 points;
            Int32[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
            {
                throw new ShapeFileIsInvalidException("Polyline found with 0 parts.");
            }

            IMultiLineString mline = GeometryFactory.CreateMultiLineString();

            for (Int32 lineId = 0; lineId < parts; lineId++)
            {
                ICoordinateSequence coordinates = GeometryFactory.CoordinateSequenceFactory.Create(CoordinateDimensions.Two);

                readSegments(lineId, segments, coordinates);

                ILineString line = GeometryFactory.CreateLineString(coordinates);

                mline.Add(line);
            }

            if (mline.Count == 1)
            {
                return mline[0];
            }

            return mline;
        }

        private IGeometry readPolygon()
        {
            Int32 parts;
            Int32 points;
            Int32[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
            {
                throw new ShapeFileIsInvalidException("Polygon found with 0 parts.");
            }

            // First read all the rings
            ILinearRing[] rings = new ILinearRing[parts];

            for (Int32 ringId = 0; ringId < parts; ringId++)
            {
                ICoordinateSequence coordinates = GeometryFactory.CoordinateSequenceFactory.Create(CoordinateDimensions.Two);

                readSegments(ringId, segments, coordinates);

                ILinearRing ring = GeometryFactory.CreateLinearRing(coordinates);
                rings[ringId] = ring;
            }

            Int32 polygonCount = 0;

            List<ILinearRing> shells = new List<ILinearRing>();
            List<ILinearRing> holes = new List<ILinearRing>();

            for (Int32 i = 0; i < parts; i++)
            {
                if (!rings[i].IsCcw)
                {
                    polygonCount++;
                    shells.Add(rings[i]);
                }
                else
                {
                    holes.Add(rings[i]);
                }
            }

            Assert.IsNotEquals(polygonCount, 0);

            List<IPolygon> polygons = new List<IPolygon>();

            foreach (ILinearRing ring in shells)
            {
                List<ILinearRing> localHoles = new List<ILinearRing>();

                IPolygon bounds = GeometryFactory.CreatePolygon(ring); //unfortunately we need to build a temp shell to test contains will add processing overhead

                for (int i = holes.Count - 1; i > -1; i--)
                {
                    if (bounds.Contains(holes[i]))
                    {
                        localHoles.Add(holes[i]);
                        holes.RemoveAt(i);
                    }
                }
                polygons.Add(GeometryFactory.CreatePolygon(ring, localHoles));
            }

            Debug.Assert(holes.Count == 0);

            return GeometryFactory.CreateMultiPolygon(polygons);

        }


        #endregion

        #region File parsing helpers

        /// <summary>
        /// Reads and parses the projection if a projection file exists
        /// </summary>
        private void parseProjection()
        {
            String projfile = Path.Combine(Path.GetDirectoryName(Filename),
                                           Path.GetFileNameWithoutExtension(Filename) + ".prj");

            if (File.Exists(projfile))
            {
                if (_coordSysFactory == null)
                {
                    throw new InvalidOperationException("A projection is defined for this shapefile," +
                                                        " but no CoordinateSystemFactory was set.");
                }

                try
                {
                    String wkt = File.ReadAllText(projfile);
                    ICoordinateSystem coordinateSystem = _coordSysFactory.CreateFromWkt(wkt);
                    initSpatialReference(coordinateSystem);
                    _coordsysReadFromFile = true;
                }
                catch (ArgumentException ex)
                {
                    Trace.Warning("Coordinate system file '" + projfile +
                                  "' found, but could not be parsed. " +
                                  "WKT parser returned:" + ex.Message);

                    throw new ShapeFileIsInvalidException("Invalid .prj file", ex);
                }
            }
        }

        #endregion

        #region File writing helper functions

        //private void writeFeatureRow(FeatureDataRow feature)
        //{
        //    UInt32 recordNumber = addIndexEntry(feature);

        //    if (HasDbf)
        //    {
        //        _dbaseWriter.AddRow(feature);
        //    }

        //    writeGeometry(feature.Geometry, recordNumber, _shapeIndex[recordNumber].Length);
        //}


        private void writeGeometry(IGeometry g, UInt32 recordNumber, Int32 recordOffsetInWords, Int32 recordLengthInWords)
        {
            Debug.Assert(recordNumber > 0);

            _shapeFileStream.Position = recordOffsetInWords * 2;

            // Record numbers are 1- based in shapefile
            // recordNumber += 1;

            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordNumber));
            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordLengthInWords));

            if (g == null)
            {
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.Null));
            }

            switch (ShapeType)
            {
                case ShapeType.Point:
                    writePoint(g as IPoint);
                    break;
                case ShapeType.PolyLine:
                    if (g is ILineString)
                    {
                        writeLineString(g as ILineString);
                    }
                    else if (g is IMultiLineString)
                    {
                        writeMultiLineString(g as IMultiLineString);
                    }
                    break;
                //case ShapeType.Polygon:
                //    writePolygon(g as IPolygon);
                //    break;

                case ShapeType.Polygon:
                    if (g is IPolygon)
                        writePolygon(g as IPolygon);
                    else if (g is IMultiPolygon)
                        writeMultiPolygon(g as IMultiPolygon);
                    break;
                case ShapeType.MultiPoint:
                    writeMultiPoint(g as IMultiPoint);
                    break;
                case ShapeType.PointZ:
                case ShapeType.PolyLineZ:
                case ShapeType.PolygonZ:
                case ShapeType.MultiPointZ:
                case ShapeType.PointM:
                case ShapeType.PolyLineM:
                case ShapeType.PolygonM:
                case ShapeType.MultiPointM:
                case ShapeType.MultiPatch:
                case ShapeType.Null:
                default:
                    throw new NotSupportedException(String.Format(
                                                        "Writing geometry type {0} " +
                                                        "is not supported in the " +
                                                        "current version.",
                                                        ShapeType));
            }

            _shapeFileWriter.Flush();
        }

        private void writeCoordinate(Double x, Double y)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(x));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(y));
        }

        private void writePoint(IPoint point)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.Point));
            writeCoordinate(point[Ordinates.X], point[Ordinates.Y]);
        }

        private void writeBoundingBox(IExtents box)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.GetMin(Ordinates.X)));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.GetMin(Ordinates.Y)));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.GetMax(Ordinates.X)));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.GetMax(Ordinates.Y)));
        }

        private void writeMultiPoint(IMultiPoint multiPoint)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.MultiPoint));
            writeBoundingBox(multiPoint.Extents);
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(multiPoint.Count));

            foreach (IPoint point in ((IEnumerable<IPoint>)multiPoint))
            {
                writeCoordinate(point[Ordinates.X], point[Ordinates.Y]);
            }
        }

        private void writePolySegments(IExtents extents, Int32[] parts, IEnumerable points, Int32 pointCount)
        {
            writeBoundingBox(extents);
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(parts.Length));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(pointCount));

            foreach (Int32 partIndex in parts)
            {
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(partIndex));
            }

            foreach (ICoordinate point in points)
            {
                writeCoordinate(point[Ordinates.X], point[Ordinates.Y]);
            }
        }

        private void writeLineString(ILineString lineString)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.PolyLine));

            writePolySegments(lineString.Extents,
                              new Int32[] { 0 },
                              lineString.Coordinates,
                              lineString.Coordinates.Count);
        }

        private void writeMultiLineString(IMultiLineString multiLineString)
        {
            Int32[] parts = new Int32[multiLineString.Count];
            ArrayList allPoints = new ArrayList();

            Int32 currentPartsIndex = 0;

            foreach (ILineString line in (IEnumerable<ILineString>)multiLineString)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange(line.Coordinates);
            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.PolyLine));
            writePolySegments(multiLineString.Extents, parts, allPoints, allPoints.Count);
        }

        /*jd: Contains Modifications from Lee Keel www.trimble.com to cope with unclosed polygons  */
        private void writePolygon(IPolygon polygon)
        {
            Int32[] parts = new Int32[polygon.InteriorRingsCount + 1];
            List<ICoordinate> allPoints = new List<ICoordinate>();
            Int32 currentPartsIndex = 0;
            parts[currentPartsIndex++] = 0;
            allPoints.AddRange((IEnumerable<ICoordinate>)polygon.ExteriorRing.Coordinates);
            /* need to account for cases where the polygon is not closed. */
            if (polygon.ExteriorRing.Coordinates[0] != polygon.ExteriorRing.Coordinates[polygon.ExteriorRing.Coordinates.Count - 1])
                allPoints.Add(polygon.ExteriorRing.Coordinates[0]);

            foreach (ILinearRing ring in polygon.InteriorRings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange((IEnumerable<ICoordinate>)ring.Coordinates);

                /* need to account for cases where the polygon is not closed. */
                if (ring.Coordinates[0] != ring.Coordinates[ring.Coordinates.Count - 1])
                    allPoints.Add(ring.Coordinates[0]);

            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.Polygon));
            writePolySegments(polygon.Extents, parts, allPoints, allPoints.Count);
        }
        /*jd: Contains Modifications from Lee Keel www.trimble.com to cope with unclosed polygons  */
        private void writeMultiPolygon(IMultiPolygon mpoly)
        {
            List<Int32> parts = new List<int>();
            List<ICoordinate> points = new List<ICoordinate>();
            Int32 pointIndex = 0;

            foreach (IPolygon poly in (mpoly as IEnumerable<IPolygon>))
            {
                parts.Add(pointIndex);  /* Add point index for exterior ring */
                pointIndex += poly.ExteriorRing.Coordinates.Count;  /* increment point position by number of points in exterior ring */
                points.AddRange((IEnumerable<ICoordinate>)poly.ExteriorRing.Coordinates);     /* Add points for exterior ring */

                /* need to account for cases where the polygon is not closed. */
                if (poly.ExteriorRing.Coordinates[0] != poly.ExteriorRing.Coordinates[poly.ExteriorRing.Coordinates.Count - 1])
                {
                    //Close ring
                    pointIndex++;
                    points.Add(poly.ExteriorRing.Coordinates[0]);
                }
                foreach (ILinearRing ring in poly.InteriorRings)
                {
                    parts.Add(pointIndex);  /* Add startint point for this interior ring */
                    pointIndex += ring.Coordinates.Count;               /* increment point position by number of points in exterior ring */
                    points.AddRange((IEnumerable<ICoordinate>)ring.Coordinates);     /* Add points for exterior ring */

                    /* need to account for cases where the polygon is not closed. */
                    if (ring.Coordinates[0] != ring.Coordinates[ring.Coordinates.Count - 1])
                    {
                        //Close ring
                        pointIndex++;
                        points.Add(ring.Coordinates[0]);
                    }
                }

            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.Polygon));
            writePolySegments(mpoly.Extents, parts.ToArray(), points, points.Count);

        }

        #endregion

        #region IWritableFeatureProvider Members

        public void Insert(FeatureDataRow feature)
        {
            Insert((FeatureDataRow<UInt32>)feature);
        }

        public void Insert(IEnumerable<FeatureDataRow> features)
        {
            Insert(Caster.Downcast<FeatureDataRow<UInt32>, FeatureDataRow>(features));
        }

        public void Update(FeatureDataRow feature)
        {
            Update((FeatureDataRow<UInt32>)feature);
        }

        public void Update(IEnumerable<FeatureDataRow> features)
        {
            Update(Caster.Downcast<FeatureDataRow<UInt32>, FeatureDataRow>(features));
        }

        public void Delete(FeatureDataRow feature)
        {
            Delete((FeatureDataRow<UInt32>)feature);
        }

        public void Delete(IEnumerable<FeatureDataRow> features)
        {
            Delete(Caster.Downcast<FeatureDataRow<UInt32>, FeatureDataRow>(features));
        }

        #endregion


    }
}
