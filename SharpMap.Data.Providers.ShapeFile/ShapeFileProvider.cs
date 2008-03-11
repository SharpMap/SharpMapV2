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
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GeoAPI.IO.WellKnownText;
using GeoAPI.Utilities;
using SharpMap.Indexing.RTree;
using SharpMap.Expressions;
using SharpMap.Utilities;
using ByteEncoder = SharpMap.Utilities.ByteEncoder;

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
    /// Adding a datasource to a layer:
    /// <code lang="C#">
    /// using SharpMap.Layers;
    /// using SharpMap.Data.Providers.ShapeFile;
    /// // [...]
    /// FeatureLayer myLayer = new FeatureLayer("My layer");
    /// myLayer.DataSource = new ShapeFile(@"C:\data\MyShapeData.shp");
    /// </code>
    /// </example>
    public class ShapeFileProvider : IWritableFeatureLayerProvider<UInt32>
    {
        //#region FilterMethod

        ///// <summary>
        ///// A delegate to a filter method for feature data.
        ///// </summary>
        ///// <remarks>
        ///// The FilterMethod delegate is used for applying a method that filters data from the dataset.
        ///// The method should return 'true' if the feature should be included and false if not.
        ///// <para>See the <see cref="FilterDelegate"/> property for more info</para>
        ///// </remarks>
        ///// <seealso cref="FilterDelegate"/>
        ///// <param name="dr"><see cref="FeatureDataRow"/> to test on</param>
        ///// <returns>true if this feature should be included, false if it should be filtered</returns>
        //public delegate Boolean FilterMethod(FeatureDataRow dr);

        //#endregion

        #region IdBounds
        struct IdBounds :IBoundable<IExtents>
        {
            private UInt32 _id;
            private IExtents _extents;

            public IdBounds(UInt32 id, IExtents extents)
            {
                _id = id;
                _extents = extents;
            }

            public UInt32 Id
            {
                get { return _id; }
            }

            #region IBoundable<IExtents> Members

            public IExtents Bounds
            {
                get { return _extents; }
            }

            public Boolean Intersects(IExtents bounds)
            {
                if (bounds == null)
                {
                    throw new ArgumentNullException("bounds");
                }

                return bounds.Intersects(_extents);
            }

            #endregion
        }
        #endregion

        #region Instance fields
        private Predicate<FeatureDataRow> _filterDelegate;
        private Int32? _srid;
        private readonly String _filename;
        private DbaseFile _dbaseFile;
        private FileStream _shapeFileStream;
        private BinaryReader _shapeFileReader;
        private BinaryWriter _shapeFileWriter;
        private readonly Boolean _hasFileBasedSpatialIndex;
        private Boolean _isOpen;
        private readonly Boolean _isIndexed = true;
        private Boolean _coordsysReadFromFile;
        private ICoordinateSystem _coordinateSystem;
        private Boolean _disposed;
        private ISpatialIndex<IExtents, IdBounds> _spatialIndex;
        private readonly ShapeFileHeader _header;
        private readonly ShapeFileIndex _shapeFileIndex;
        private ShapeFileDataReader _currentReader;
        private readonly Object _readerSync = new Object();
        private readonly Boolean _hasDbf;
        private IGeometryFactory _geoFactory;
        private ICoordinateSystemFactory _coordSysFactory;
        #endregion

        #region Object construction and disposal

        /// <summary>
        /// Initializes a ShapeFile data provider without a file-based spatial index.
        /// </summary>
        /// <param name="filename">Path to shapefile (.shp file).</param>
        public ShapeFileProvider(String filename, IGeometryFactory geoFactory)
            : this(filename, geoFactory, false) { }

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
        /// <param name="fileBasedIndex">True to create a file-based spatial index.</param>
        public ShapeFileProvider(String filename, IGeometryFactory geoFactory, Boolean fileBasedIndex)
        {
            _filename = filename;
            _geoFactory = geoFactory;

            if (!File.Exists(filename))
            {
                throw new LayerDataLoadException(filename);
            }

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                _header = new ShapeFileHeader(reader, _geoFactory);
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

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~ShapeFileProvider()
        {
            dispose(false);
        }

        #region Dispose pattern

        /// <summary>
        /// Disposes the object
        /// </summary>
        void IDisposable.Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        private void dispose(Boolean disposing)
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

            _isOpen = false;
        }

        protected internal Boolean IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
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
            return String.Format("[ShapeFile] Name: {0}; HasDbf: {1}; Features: {2}; Extents: {3}",
                                 ConnectionId, HasDbf, GetFeatureCount(), GetExtents());
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
        public static ShapeFileProvider Create(String directory, String layerName, ShapeType type, IGeometryFactory geoFactory)
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
        public static ShapeFileProvider Create(String directory, String layerName, ShapeType type,
                                               FeatureDataTable schema, IGeometryFactory geoFactory)
        {
            if (type == ShapeType.Null)
            {
                throw new ShapeFileInvalidOperationException("Cannot create a shapefile with a null geometry type");
            }

            if (String.IsNullOrEmpty(directory) || directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Parameter must be a valid path", "directory");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            return Create(directoryInfo, layerName, type, schema, geoFactory);
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
                                               ShapeType type, FeatureDataTable model, IGeometryFactory geoFactory)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            Encoding encoding = Encoding.GetEncoding(culture.TextInfo.OEMCodePage);
            return Create(directory, layerName, type, model, culture, encoding, geoFactory);
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
        public static ShapeFileProvider Create(DirectoryInfo directory, String layerName, ShapeType type,
                                               FeatureDataTable model, CultureInfo culture, Encoding encoding,
                                               IGeometryFactory geoFactory)
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

            return new ShapeFileProvider(shapeFile, geoFactory);
        }

        #endregion

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
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ShapeFileIsInvalidException">
        /// Thrown if set and there is no DBase file with this shapefile.
        /// </exception>
        public Encoding Encoding
        {
            get
            {
                checkOpen();

                if (!HasDbf)
                {
                    return Encoding.ASCII;
                }

                return _dbaseFile.Encoding;
            }
        }

        /// <summary>
        /// Gets the filename of the shapefile
        /// </summary>
        /// <remarks>If the filename changes, indexes will be rebuilt</remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is executed and the shapefile is open or 
        /// if set and the specified filename already exists.
        /// Check <see cref="IsOpen"/> before calling.
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
        /// Filter Delegate Method for limiting the datasource
        /// </summary>
        /// <remarks>
        /// <example>
        /// Using an anonymous method for filtering all features where the NAME column starts with S:
        /// <code lang="C#">
        /// myShapeDataSource.FilterDelegate = new FilterMethod(delegate(FeatureDataRow row) 
        ///		{ return (!row["NAME"].ToString().StartsWith("S")); });
        /// </code>
        /// </example>
        /// <example>
        /// Declaring a delegate method for filtering (multi)polygon-features whose area is larger than 5.
        /// <code>
        /// using GeoAPI.Geometries;
        /// [...]
        /// myShapeDataSource.FilterDelegate = CountryFilter;
        /// [...]
        /// public static Boolean CountryFilter(FeatureDataRow row)
        /// {
        ///		if(row.Geometry is Polygon)
        ///		{
        ///			return (row.Geometry as Polygon).Area > 5;
        ///		}
        ///		if (row.Geometry is MultiPolygon)
        ///		{
        ///			return (row.Geometry as MultiPolygon).Area > 5;
        ///		}
        ///		else 
        ///		{
        ///			return true;
        ///		}
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public Predicate<FeatureDataRow> Filter
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

        public Boolean IsSpatiallyIndexed
        {
            get { return _isIndexed; }
            set { throw new NotImplementedException("Allow shapefile provider to be created without an index. [workitem:13025]"); }
        }

        /// <summary>
        /// Forces a rebuild of the spatial index. 
        /// If the instance of the ShapeFile provider
        /// uses a file-based index the file is rewritten to disk,
        /// otherwise it is kept only in memory.
        /// </summary>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is executed and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void RebuildSpatialIndex()
        {
            checkOpen();
            //enableReading();

            if (_hasFileBasedSpatialIndex)
            {
                if (File.Exists(_filename + ".sidx"))
                {
                    File.Delete(_filename + ".sidx");
                }

                _spatialIndex = createSpatialIndexFromFile(_filename);
            }
            else
            {
                _spatialIndex = createSpatialIndex();
            }
        }

        /// <summary>
        /// Gets the <see cref="Providers.ShapeFile.ShapeType">
        /// shape geometry type</see> in this shapefile.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The property isn't set until the first time the datasource has been opened,
        /// and will throw an exception if this property has been called since initialization. 
        /// </para>
        /// <para>
        /// All the non-<see cref="SharpMap.Data.Providers.ShapeFile.ShapeType.Null"/> 
        /// shapes in a shapefile are required to be of the same shape type.
        /// </para>
        /// </remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if property is read and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public ShapeType ShapeType
        {
            get
            {
                checkOpen();
                return _header.ShapeType;
            }
        }

        #region ILayerProvider Members

        /// <summary>
        /// Gets the connection ID of the datasource.
        /// </summary>
        /// <remarks>
        /// The connection ID of a shapefile is its filename.
        /// </remarks>
        public String ConnectionId
        {
            get { return _filename; }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Computes the extents of the data source.
        /// </summary>
        /// <returns>
        /// A BoundingBox instance describing the extents of the entire data source.
        /// </returns>
        public IExtents GetExtents()
        {
            if (_spatialIndex != null)
            {
                return _spatialIndex.Bounds;
            }

            return _header.Extents;
        }

        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>		
        public Boolean IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Gets or sets the coordinate system of the ShapeFile. 
        /// </summary>
        /// <remarks>
        /// If a shapefile has a corresponding [filename].prj file containing a Well-Known Text 
        /// description of the coordinate system this will automatically be read.
        /// If this is not the case, the coordinate system will default to null.
        /// </remarks>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if property is set and the coordinate system is read from file.
        /// </exception>
        public ICoordinateSystem SpatialReference
        {
            get { return _coordinateSystem; }
            set
            {
                //checkOpen();
                if (_coordsysReadFromFile)
                {
                    throw new ShapeFileInvalidOperationException(
                        "Coordinate system is specified in projection file and is read only");
                }

                _coordinateSystem = value;
            }
        }

        /// <summary>
        /// Gets or sets the spatial reference ID.
        /// </summary>
        public Int32? Srid
        {
            get { return _srid; }
            set { _srid = value; }
        }

        #region Methods

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            (this as IDisposable).Dispose();
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            Open(false);
        }

        #endregion

        /// <summary>
        /// Opens the shapefile with optional exclusive access for
        /// faster write performance during bulk updates.
        /// </summary>
        /// <param name="exclusive">
        /// True if exclusive access is desired, false otherwise.
        /// </param>
        public void Open(Boolean exclusive)
        {
            if (!_isOpen)
            {
                try
                {
                    //enableReading();
                    _shapeFileStream = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                                      exclusive ? FileShare.None : FileShare.Read, 4096,
                                                      FileOptions.None);

                    _shapeFileReader = new BinaryReader(_shapeFileStream);
                    _shapeFileWriter = new BinaryWriter(_shapeFileStream);

                    _isOpen = true;

                    // Read projection file
                    parseProjection();

                    // Load spatial (r-tree) index
                    loadSpatialIndex(_hasFileBasedSpatialIndex);

                    if (HasDbf)
                    {
                        _dbaseFile = new DbaseFile(DbfFilename, _geoFactory);
                        _dbaseFile.Open(exclusive);
                    }
                }
                catch (Exception)
                {
                    _isOpen = false;
                    throw;
                }
            }
        }

        #endregion

        #region IFeatureLayerProvider Members

        public IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet,
                                                     AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table,
                                                     AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet,
                                                          AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet,
                                                          QueryExecutionOptions options, AsyncCallback callback,
                                                          object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table,
                                                          AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table,
                                                          QueryExecutionOptions options, AsyncCallback callback,
                                                          object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetFeatures(IEnumerable oids, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public FeatureDataTable CreateNewTable()
        {
            return getNewTable();
        }

        public IEnumerable<IFeatureDataRecord> EndGetFeatures(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public void EndExecuteFeatureQuery(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureSpatialExpression query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataSet"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Feature spatial query to execute.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet)
        {
            FeatureDataTable<UInt32> dt = HasDbf
                                            ? _dbaseFile.NewTable
                                            : FeatureDataTable<UInt32>.CreateEmpty(ShapeFileConstants.IdColumnName, _geoFactory);

            dt.TableName = Path.GetFileNameWithoutExtension(Filename);

            ExecuteFeatureQuery(query, dt);

            dataSet.Tables.Add(dt);
        }

        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataTable"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table)
        {
            checkOpen();

            IExtents extents = query.QueryRegion.Extents;

            IEnumerable<UInt32> oidList;

            // Get candidates by intersecting the spatial index tree 
            // or enumerating all rows
            if (IsSpatiallyIndexed)
            {
                IEnumerable<IdBounds> keys = _spatialIndex.Query(extents);
                oidList = intersectFeatureGeometry(keys, query.QueryRegion);
            }
            else
            {
                throw new NotImplementedException("Allow shapefile provider to be created without an index. [workitem:13025]");
            }

            FeatureDataTable<UInt32> result = getNewTable();

            IGeometry extentGeometry = extents.ToGeometry();

            foreach (UInt32 oid in oidList)
            {
                FeatureDataRow<UInt32> fdr = getFeature(oid, result);

                if (fdr.Geometry != null)
                {
                    // TODO: Beta 2 - replace following line with:  if(fdr.Geometry.Intersects(bbox)). Currently always true.
                    if (fdr.Geometry.Intersects(extentGeometry))
                    {
                        if (Filter == null || Filter(fdr))
                        {
                            result.AddRow(fdr);
                        }
                    }
                }
            }

            table.Merge(result, true);
        }

        public IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns geometries whose bounding box intersects <paramref name="bounds"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Please note that this method doesn't guarantee that the geometries returned actually 
        /// intersect <paramref name="bounds"/>, but only that their bounding box intersects <paramref name="bounds"/>.
        /// </para>
        /// <para>
        /// This method is much faster than the QueryFeatures method, because intersection tests
        /// are performed on objects simplifed by their BoundingBox, and using the spatial index.
        /// </para>
        /// </remarks>
        /// <param name="bounds">
        /// <see cref="IExtents"/> which determines the view.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing the <see cref="IGeometry"/> objects
        /// which are at least partially contained within the given <paramref name="bounds"/>.
        /// </returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the 
        /// shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IExtents bounds)
        {
            checkOpen();
            //enableReading();

            foreach (UInt32 oid in GetIntersectingObjectIds(bounds))
            {
                IGeometry g = GetGeometryById(oid);

                if (!ReferenceEquals(g, null))
                {
                    yield return g;
                }
            }
        }

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds)
        {
            return ExecuteIntersectionQuery(bounds, QueryExecutionOptions.FullFeature);
        }

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a value other than <see cref="QueryExecutionOptions.FullFeature"/> 
        /// is supplied for <paramref name="options"/>.
        /// </exception>
        /// <remarks>
        /// Only <see cref="QueryExecutionOptions.FullFeature"/> is a supported value for <paramref name="options"/>.
        /// </remarks>
        public IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds, QueryExecutionOptions options)
        {
            checkOpen();

            lock (_readerSync)
            {
                if (_currentReader != null)
                {
                    throw new ShapeFileInvalidOperationException(
                        "Can't open another ShapeFileDataReader on this ShapeFile, since another reader is already active.");
                }

                //enableReading();
                _currentReader = new ShapeFileDataReader(this, bounds, options);
                _currentReader.Disposed += readerDisposed;
                return _currentReader;
            }
        }

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents"/> which determines the view.</param>
        /// <param name="dataSet">The <see cref="FeatureDataSet"/> to fill 
        /// with features within the <paramref name="bounds">view</paramref>.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet)
        {
            ExecuteIntersectionQuery(bounds, dataSet, QueryExecutionOptions.FullFeature);
        }

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents"/> which determines the view.</param>
        /// <param name="dataSet">
        /// The <see cref="FeatureDataSet"/> to fill 
        /// with features within the <paramref name="bounds"/>.
        /// </param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a value other than <see cref="QueryExecutionOptions.FullFeature"/> 
        /// is supplied for <paramref name="options"/>.
        /// </exception>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, QueryExecutionOptions options)
        {
            checkOpen();

            // TODO: search the dataset for the table and merge results.
            FeatureDataTable<UInt32> dt = getNewTable();

            ExecuteIntersectionQuery(bounds, dt, options);

            dataSet.Tables.Add(dt);
        }

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table)
        {
            ExecuteIntersectionQuery(bounds, table, QueryExecutionOptions.FullFeature);
        }

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when a value other than <see cref="QueryExecutionOptions.FullFeature"/> 
        /// is supplied for <paramref name="options"/>.
        /// </exception>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options)
        {
            checkOpen();

            if (options != QueryExecutionOptions.FullFeature)
            {
                throw new ArgumentException("Only QueryExecutionOptions.All is supported.", "options");
            }

            if (!(table is FeatureDataTable<UInt32>))
            {
                throw new ArgumentException("Parameter 'table' must be of type FeatureDataTable<UInt32>.");
            }

            FeatureDataTable<UInt32> keyedTable = table as FeatureDataTable<UInt32>;

            SetTableSchema(keyedTable);

            IEnumerable<UInt32> objectsInQuery = GetIntersectingObjectIds(bounds);

            keyedTable.Merge(getFeatureRecordsFromIds(objectsInQuery, keyedTable), GeometryFactory);
        }

        public IGeometryFactory GeometryFactory
        {
            get
            {
                return _geoFactory;
            }
            set
            {
                _geoFactory = value;
            }
        }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable oids)
        {
            return GetFeatures(getUint32IdsFromObjects(oids));
        }

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        public Int32 GetFeatureCount()
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
        public DataTable GetSchemaTable()
        {
            //enableReading();
            return _dbaseFile.GetSchemaTable();
        }

        /// <summary>
        /// Gets the locale of the data as a CultureInfo.
        /// </summary>
        public CultureInfo Locale
        {
            get { return _dbaseFile.CultureInfo; }
        }

        /// <summary>
        /// Sets the schema of the given table to match the schema of the shapefile's attributes.
        /// </summary>
        /// <param name="target">Target table to set the schema of.</param>
        public void SetTableSchema(FeatureDataTable target)
        {
            checkOpen();
            _dbaseFile.SetTableSchema(target, SchemaMergeAction.Add | SchemaMergeAction.Key);
        }

        #endregion

        #region IFeatureLayerProvider<UInt32> Members

        /// <summary>
        /// Gets a feature row from the datasource with the specified id.
        /// </summary>
        /// <param name="oid">Id of the feautre to return.</param>
        /// <returns>
        /// The feature corresponding to <paramref name="oid" />, or null if no feature is found.
        /// </returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IFeatureDataRecord GetFeature(UInt32 oid)
        {
            return getFeature(oid, null);
        }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<UInt32> oids)
        {
            FeatureDataTable<UInt32> table = CreateNewTable() as FeatureDataTable<UInt32>;
            Debug.Assert(table != null);
            table.IsSpatiallyIndexed = false;

            foreach (UInt32 oid in oids)
            {
                table.AddRow(getFeature(oid, table));
            }

            return table;
        }

        /// <summary>
        /// Returns the geometry corresponding to the object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="IGeometry"/></returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IGeometry GetGeometryById(UInt32 oid)
        {
            checkOpen();

            if (Filter != null) //Apply filtering
            {
                IFeatureDataRecord fdr = GetFeature(oid);

                if (fdr != null)
                {
                    return fdr.Geometry;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return readGeometry(oid);
            }
        }

        /// <summary>
        /// Returns geometry object IDs whose bounding box intersects <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">Bounds which to search for objects in.</param>
        /// <returns>
        /// An enumeration of object ids which have geometries whose 
        /// bounding box is intersected by <paramref name="bounds"/>.
        /// </returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IEnumerable<UInt32> GetIntersectingObjectIds(IExtents bounds)
        {
            checkOpen();

            IEnumerable<IdBounds> keys = _spatialIndex.Query(bounds);

            foreach (UInt32 id in intersectFeatureGeometry(keys, bounds.ToGeometry()))
            {
                yield return id;
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
                    "Object ID column names for this schema and 'target' schema must be identical, including case. " +
                    "For case-insensitive or type-only matching, use SetTableSchema(FeatureDataTable, SchemaMergeAction) " +
                    "with the SchemaMergeAction.CaseInsensitive option and/or SchemaMergeAction.KeyByType option enabled.");
            }

            SetTableSchema(target, SchemaMergeAction.Add | SchemaMergeAction.Key);
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
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
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
            _header.Extents = _geoFactory.CreateExtents(_header.Extents, featureExtents);

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

            _header.Extents = _geoFactory.CreateExtents(_header.Extents, allFeaturesExtents);
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
            else if (geometry is IPolygon)
            {
                Int32 pointCount = (geometry as IPolygon).ExteriorRing.Coordinates.Count;

                foreach (ILinearRing ring in (geometry as IPolygon).InteriorRings)
                {
                    pointCount += ring.Coordinates.Count;
                }

                byteCount = 4 /* ShapeType Integer */
                            + ShapeFileConstants.BoundingBoxFieldByteLength + 4 + 4
                    /* NumPoints and NumParts integers */
                            +
                            4 *
                            ((geometry as IPolygon).InteriorRings.Count + 1
                    /* Parts array of rings: count of interior + 1 for exterior ring */)
                            + 16 * pointCount;
            }
            else
            {
                throw new NotSupportedException("Currently unsupported geometry type.");
            }

            return byteCount / 2; // number of 16-bit words
        }

        private void checkOpen()
        {
            if (!IsOpen)
            {
                throw new ShapeFileInvalidOperationException("An attempt was made to access a closed datasource.");
            }
        }

        //private static IEnumerable<UInt32> getKeysFromIndexEntries(IEnumerable<RTreeIndexEntry<UInt32>> entries)
        //{
        //    foreach (RTreeIndexEntry<UInt32> entry in entries)
        //    {
        //        yield return entry.Value;
        //    }
        //}

        private static IEnumerable<UInt32> getUint32IdsFromObjects(IEnumerable oids)
        {
            foreach (object oid in oids)
            {
                yield return (UInt32)oid;
            }
        }

        private IEnumerable<IFeatureDataRecord> getFeatureRecordsFromIds(IEnumerable<UInt32> ids,
                                                                         FeatureDataTable<UInt32> table)
        {
            foreach (UInt32 id in ids)
            {
                yield return getFeature(id, table);
            }
        }

        private FeatureDataTable<UInt32> getNewTable()
        {
            if (HasDbf)
            {
                return _dbaseFile.NewTable;
            }
            else
            {
                return FeatureDataTable<UInt32>.CreateEmpty(ShapeFileConstants.IdColumnName, _geoFactory);
            }
        }

        /// <summary>
        /// Gets a row from the DBase attribute file which has the 
        /// specified <paramref name="oid">object id</paramref> created from
        /// <paramref name="table"/>.
        /// </summary>
        /// <param name="oid">Object id to lookup.</param>
        /// <param name="table">DataTable with schema matching the feature to retrieve.</param>
        /// <returns>Row corresponding to the object id.</returns>
        /// <exception cref="ShapeFileInvalidOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        private FeatureDataRow<UInt32> getFeature(UInt32 oid, FeatureDataTable<UInt32> table)
        {
            checkOpen();

            if (table == null)
            {
                if (!HasDbf)
                {
                    table = FeatureDataTable<UInt32>.CreateEmpty(ShapeFileConstants.IdColumnName, _geoFactory);
                }
                else
                {
                    table = _dbaseFile.NewTable;
                }
            }

            FeatureDataRow<UInt32> dr = HasDbf ? _dbaseFile.GetAttributes(oid, table) : table.NewRow(oid);
            dr.Geometry = readGeometry(oid);

            if (Filter == null || Filter(dr))
            {
                return dr;
            }
            else
            {
                return null;
            }
        }

        //private void enableReading()
        //{
        //    if (_shapeFileReader == null || !_shapeFileStream.CanRead)
        //    {
        //        if (_shapeFileStream != null)
        //        {
        //            _shapeFileStream.Close();
        //        }

        //        if (_exclusiveMode)
        //        {
        //            _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        //        }
        //        else
        //        {
        //            _shapeFileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        }

        //        _shapeFileReader = new BinaryReader(_shapeFileStream);
        //    }

        //    if (HasDbf)
        //    {
        //        if (_dbaseFile == null)
        //        {
        //            _dbaseFile = new DbaseFile(DbfFilename);
        //        }

        //        if (!_dbaseFile.IsOpen)
        //        {
        //            _dbaseFile.Open();
        //        }
        //    }
        //}

        //private void enableWriting()
        //{
        //    if (_currentReader != null)
        //    {
        //        throw new ShapeFileInvalidOperationException(
        //            "Can't write to shapefile, since a ShapeFileDataReader is actively reading it.");
        //    }

        //    if (_shapeFileWriter == null || !_shapeFileStream.CanWrite)
        //    {
        //        if (_shapeFileStream != null)
        //        {
        //            _shapeFileStream.Close();
        //        }

        //        if (_exclusiveMode)
        //        {
        //            _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        //        }
        //        else
        //        {
        //            _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        //        }

        //        _shapeFileWriter = new BinaryWriter(_shapeFileStream);
        //    }

        //    if (HasDbf)
        //    {
        //        if (_dbaseFile == null)
        //        {
        //            _dbaseFile = new DbaseFile(DbfFilename);
        //        }

        //        if (!_dbaseFile.IsOpen)
        //        {
        //            _dbaseFile.Open();
        //        }
        //    }
        //}

        private void readerDisposed(object sender, EventArgs e)
        {
            lock (_readerSync)
            {
                _currentReader = null;
            }
        }

        private IEnumerable<UInt32> intersectFeatureGeometry(IEnumerable<IdBounds> keys, IGeometry query)
        {
            foreach (IdBounds key in keys)
            {
                IGeometry candidate = GetGeometryById(key.Id);

                if (candidate.Intersects(query))
                {
                    yield return key.Id;
                }
            }
        }

        #endregion

        #region Spatial indexing helper functions

        /// <summary>
        /// Loads a spatial index from a file. If it doesn't exist, one is created and saved
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>QuadTree index</returns>
        private ISpatialIndex<IExtents, IdBounds> createSpatialIndexFromFile(String filename)
        {
            if (File.Exists(filename + ".sidx"))
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
            Func<UInt32, IExtents> bounder = delegate(UInt32 item)
            {
                return GetGeometryById(item).Extents;
            };

            // TODO: implement Post-optimization restructure strategy
            IIndexRestructureStrategy<IExtents, IdBounds> restructureStrategy = new NullRestructuringStrategy<IExtents, IdBounds>();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.None, 4.0);
            IItemInsertStrategy<IExtents, IdBounds> insertStrategy = new GuttmanQuadraticInsert<IdBounds>(_geoFactory);
            INodeSplitStrategy<IExtents, IdBounds> nodeSplitStrategy = new GuttmanQuadraticSplit<IdBounds>(_geoFactory);
            DynamicRTreeBalanceHeuristic indexHeuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue); 
            IdleMonitor idleMonitor = null;

            DynamicRTree<IdBounds> index = new SelfOptimizingDynamicSpatialIndex<IdBounds>(
                                                                                _geoFactory,
                                                                                restructureStrategy,
                                                                                restructureHeuristic, 
                                                                                insertStrategy,
                                                                                nodeSplitStrategy,
                                                                                indexHeuristic,
                                                                                idleMonitor);

            for (UInt32 i = 0; i < (UInt32)GetFeatureCount(); i++)
            {
                IGeometry geom = readGeometry(i);

                if (geom == null)
                {
                    continue;
                }

                IExtents extents = geom.Extents;

                if (!Double.IsNaN(extents.GetMin(Ordinates.X)) && !Double.IsNaN(extents.GetMax(Ordinates.X)) &&
                    !Double.IsNaN(extents.GetMin(Ordinates.Y)) && !Double.IsNaN(extents.GetMax(Ordinates.Y)))
                {
                    index.Insert(new IdBounds(i, extents));
                }
            }

            return index;
        }

        private void loadSpatialIndex(Boolean loadFromFile)
        {
            loadSpatialIndex(false, loadFromFile);
        }

        private void loadSpatialIndex(Boolean forceRebuild, Boolean loadFromFile)
        {
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

        #region Geometry reading helper functions

        /*
        /// <summary>
        /// Reads all boundingboxes of features in the shapefile. 
        /// This is used for spatial indexing.
        /// </summary>
        /// <returns></returns>
        private List<BoundingBox> getAllFeatureBoundingBoxes()
        {
            enableReading();
            List<BoundingBox> boxes = new List<BoundingBox>();

            foreach (KeyValuePair<UInt32, ShapeFileIndex.IndexEntry> kvp in _shapeFileIndex)
            {
                _shapeFileStream.Seek(kvp.Value.AbsoluteByteOffset + ShapeFileConstants.ShapeRecordHeaderByteLength,
                                      SeekOrigin.Begin);

                if ((ShapeType) ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()) != ShapeType.Null)
                {
                    Double xMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    Double yMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    Double xMax, yMax;

                    if (ShapeType == ShapeType.Point)
                    {
                        xMax = xMin;
                        yMax = yMin;
                    }
                    else
                    {
                        xMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                        yMax = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    }

                    boxes.Add(new BoundingBox(xMin, yMin, yMax, yMax));
                }
            }

            return boxes;
        }
        */

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile.
        /// </summary>
        /// <remarks>
        /// <see cref="FilterDelegate">Filtering</see> is not applied to this method.
        /// </remarks>
        /// <param name="oid">Object ID</param>
        /// <returns>
        /// <see cref="GeoAPI.Geometries.IGeometry"/> instance from the ShapeFile corresponding to <paramref name="oid"/>.
        /// </returns>
        private IGeometry readGeometry(UInt32 oid)
        {
            //enableReading();
            _shapeFileReader.BaseStream.Seek(
                _shapeFileIndex[oid].AbsoluteByteOffset + ShapeFileConstants.ShapeRecordHeaderByteLength,
                SeekOrigin.Begin);

            // Shape type is a common value to all geometry
            ShapeType type = (ShapeType)ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            // Null geometries encode deleted lines, so object ids remain consistent
            if (type == ShapeType.Null)
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
                    throw new ShapeFileUnsupportedGeometryException(
                        "ShapeFile type " + ShapeType + " not supported");
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
            Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
            IPoint point = _geoFactory.CreatePoint(_geoFactory.CoordinateFactory.Create(x, y));

            return point;
        }

        private IGeometry readMultiPoint()
        {
            // Skip min/max box
            _shapeFileReader.BaseStream.Seek(ShapeFileConstants.BoundingBoxFieldByteLength, SeekOrigin.Current);

            IMultiPoint geometry = _geoFactory.CreateMultiPoint();

            // Get the number of points
            Int32 nPoints = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());

            if (nPoints == 0)
            {
                return null;
            }

            for (Int32 i = 0; i < nPoints; i++)
            {
                Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                IPoint point = _geoFactory.CreatePoint(_geoFactory.CoordinateFactory.Create(x, y));
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

            IMultiLineString mline = _geoFactory.CreateMultiLineString();

            for (Int32 lineId = 0; lineId < parts; lineId++)
            {
                ICoordinateSequence coordinates = _geoFactory.CoordinateSequenceFactory.Create(2);

                for (Int32 i = segments[lineId]; i < segments[lineId + 1]; i++)
                {
                    Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    coordinates.Add(_geoFactory.CoordinateFactory.Create(x, y));
                }

                ILineString line = _geoFactory.CreateLineString(coordinates);

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
            List<ILinearRing> rings = new List<ILinearRing>();

            for (Int32 ringId = 0; ringId < parts; ringId++)
            {
                ICoordinateSequence coordinates = _geoFactory.CoordinateSequenceFactory.Create(2);

                for (Int32 i = segments[ringId]; i < segments[ringId + 1]; i++)
                {
                    Double x = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    Double y = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    coordinates.Add(_geoFactory.CoordinateFactory.Create(x, y));
                }

                ILinearRing ring = _geoFactory.CreateLinearRing(coordinates);
                rings.Add(ring);
            }

            Boolean[] isCounterClockWise = new Boolean[rings.Count];
            Int32 polygonCount = 0;

            for (Int32 i = 0; i < rings.Count; i++)
            {
                isCounterClockWise[i] = rings[i].IsCcw;

                if (!isCounterClockWise[i])
                {
                    polygonCount++;
                }
            }

            //We only have one polygon
            if (polygonCount == 1)
            {
                if (rings.Count == 1)
                {
                    return _geoFactory.CreatePolygon(rings[0]);
                }
                else
                {
                    return _geoFactory.CreatePolygon(
                        Slice.GetFirst(rings), Enumerable.Skip(rings, 1));
                }
            }
            else
            {
                IMultiPolygon mpoly = _geoFactory.CreateMultiPolygon();

                for (Int32 i = 0; i < rings.Count; i++)
                {
                    IPolygon poly;
                    List<ILinearRing> polyRings = getPolyRings(rings, isCounterClockWise, ref i);

                    if (polyRings.Count == 1)
                    {
                        poly = _geoFactory.CreatePolygon(Slice.GetFirst(polyRings));
                    }
                    else
                    {
                        poly = _geoFactory.CreatePolygon(
                            Slice.GetFirst(polyRings), Enumerable.Skip(polyRings, 1));
                    }

                    mpoly.Add(poly);
                }

                return mpoly;
            }
        }

        private static List<ILinearRing> getPolyRings(IList<ILinearRing> rings, Boolean[] isCounterClockWise, ref Int32 i)
        {
            List<ILinearRing> singlePoly = new List<ILinearRing>();

            // make sure the first ring is an exterior ring
            Debug.Assert(isCounterClockWise[i] == false);

            singlePoly.Add(rings[i++]);

            while (isCounterClockWise[i])
            {
                singlePoly.Add(rings[i]);
                i++;
            }

            return singlePoly;
        }

        #endregion

        #region File parsing helpers

        /// <summary>
        /// Reads and parses the projection if a projection file exists
        /// </summary>
        private void parseProjection()
        {
            String projfile =
                Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename) + ".prj");

            if (File.Exists(projfile))
            {
                try
                {
                    String wkt = File.ReadAllText(projfile);
                    _coordinateSystem = (ICoordinateSystem)WktDecoder.ToCoordinateSystemInfo(wkt, _coordSysFactory);
                    _geoFactory.SpatialReference = _coordinateSystem;
                    _coordsysReadFromFile = true;
                }
                catch (ArgumentException ex)
                {
                    Trace.TraceWarning("Coordinate system file '" + projfile
                                       + "' found, but could not be parsed. WKT parser returned:" + ex.Message);

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
            _shapeFileStream.Position = recordOffsetInWords * 2;

            // Record numbers are 1- based in shapefile
            recordNumber += 1;

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
                case ShapeType.Polygon:
                    writePolygon(g as IPolygon);
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
                                                        "Writing geometry type {0} is not supported in the current version.",
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

        private void writePolySegments(IExtents extents, Int32[] parts, IEnumerable<ICoordinate> points, Int32 pointCount)
        {
            writeBoundingBox(extents);
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(parts.Length));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(pointCount));

            foreach (Int32 partIndex in parts)
            {
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(partIndex));
            }

            foreach (IPoint point in points)
            {
                writeCoordinate(point[Ordinates.X], point[Ordinates.Y]);
            }
        }

        private void writeLineString(ILineString lineString)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.PolyLine));
            
            writePolySegments(lineString.Extents, new Int32[] { 0 }, 
                (IEnumerable<ICoordinate>)lineString.Coordinates, 
                lineString.Coordinates.Count);
        }

        private void writeMultiLineString(IMultiLineString multiLineString)
        {
            Int32[] parts = new Int32[multiLineString.Count];
            List<ICoordinate> allPoints = new List<ICoordinate>();

            Int32 currentPartsIndex = 0;

            foreach (ILineString line in (IEnumerable<ILineString>)multiLineString)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange((IEnumerable<ICoordinate>)line.Coordinates);
            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.PolyLine));
            writePolySegments(multiLineString.Extents, parts, allPoints.ToArray(), allPoints.Count);
        }

        private void writePolygon(IPolygon polygon)
        {
            Int32[] parts = new Int32[polygon.InteriorRingsCount + 1];
            List<ICoordinate> allPoints = new List<ICoordinate>();
            Int32 currentPartsIndex = 0;
            parts[currentPartsIndex++] = 0;
            allPoints.AddRange((IEnumerable<ICoordinate>)polygon.ExteriorRing.Coordinates);

            foreach (ILinearRing ring in polygon.InteriorRings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange((IEnumerable<ICoordinate>)ring.Coordinates);
            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((Int32)ShapeType.Polygon));
            writePolySegments(polygon.Extents, parts, allPoints.ToArray(), allPoints.Count);
        }

        #endregion
    }
}