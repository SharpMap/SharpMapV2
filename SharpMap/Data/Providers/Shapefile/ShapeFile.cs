// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Collections.Generic;
using System.Data;
using System.IO;

using SharpMap.CoordinateSystems;
using SharpMap.Indexing;
using SharpMap.Indexing.RTree;
using SharpMap.Geometries;
using SharpMap.Utilities;
using System.Text;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// A data provider for the ESRI ShapeFile spatial data format.
    /// </summary>
    /// <remarks>
    /// <para>The ShapeFile provider is used for accessing ESRI ShapeFiles. The ShapeFile should at least contain the
    /// [filename].shp and, if feature-data is to be used, also [filename].dbf file.</para>
    /// <para>The first time the ShapeFile is accessed, SharpMap will automatically create a spatial index
    /// of the ShapeFile, and save it as [filename].shp.sidx. If you change or update the contents of the .shp file,
    /// delete the .sidx file to force SharpMap to rebuild it. In web applications, the index will automatically
    /// be cached to memory for faster access, so to reload the index, you will need to restart the web application
    /// as well.</para>
    /// <para>
    /// M and Z values in a shapefile is currently ignored by SharpMap.
    /// </para>
    /// </remarks>
    /// <example>
    /// Adding a datasource to a layer:
    /// <code lang="C#">
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    /// myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\data\MyShapeData.shp");
    /// </code>
    /// </example>
    public class ShapeFile : IWritableProvider<uint>, IDisposable
    {
        #region FilterMethod
        /// <summary>
        /// A delegate to a filter method for feature data.
        /// </summary>
        /// <remarks>
        /// The FilterMethod delegate is used for applying a method that filters data from the dataset.
        /// The method should return 'true' if the feature should be included and false if not.
        /// <para>See the <see cref="FilterDelegate"/> property for more info</para>
        /// </remarks>
        /// <seealso cref="FilterDelegate"/>
        /// <param name="dr"><see cref="FeatureDataRow"/> to test on</param>
        /// <returns>true if this feature should be included, false if it should be filtered</returns>
        public delegate bool FilterMethod(FeatureDataRow dr);
        #endregion

        #region Constant lookup values
        private static readonly int HeaderSizeBytes = 100;
        private static readonly int HeaderStartCode = 9994;
        private static readonly int VersionCode = 1000;
        private static readonly int ShapeRecordHeaderByteLength = 8;
        private static readonly int BoundingBoxFieldByteLength = 32;
        public static readonly string IdColumnName = "OID";
        #endregion

        #region Fields
        private FilterMethod _filterDelegate;
        private ShapeType _shapeType;
        private int _srid = -1;
        private string _filename;
        private BoundingBox _envelope = BoundingBox.Empty;
        private DbaseReader _dbaseReader;
        private DbaseWriter _dbaseWriter;
        private FileStream _shapeFileStream;
        private BinaryReader _shapeFileReader;
        private BinaryWriter _shapeFileWriter;
        private bool _hasFileBasedSpatialIndex;
        private bool _isOpen;
        private bool _coordsysReadFromFile = false;
        private bool _exclusiveMode = false;
        private ICoordinateSystem _coordinateSystem;
        readonly Dictionary<uint, IndexEntry> _shapeIndex = new Dictionary<uint, IndexEntry>();
        private bool _disposed = false;
        private DynamicRTree<uint> _tree;
        #endregion

        #region Object Construction/Destruction
        /// <summary>
        /// Initializes a ShapeFile data provider without a file-based spatial index.
        /// </summary>
        /// <param name="filename">Path to shapefile (.shp file).</param>
        public ShapeFile(string filename) : this(filename, false) { }

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
        public ShapeFile(string filename, bool fileBasedIndex)
        {
            _filename = filename;
            _hasFileBasedSpatialIndex = fileBasedIndex;

            // Initialize DBF
            if (HasDbf)
            {
                _dbaseReader = new DbaseReader(DbfFilename);
            }
        }

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~ShapeFile()
        {
            dispose(false);
        }

        #region Dispose pattern
        /// <summary>
        /// Disposes the object
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!Disposed)
            {
                dispose(true);
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbaseReader != null)
                {
                    _dbaseReader.Close();
                }

                if (_dbaseWriter != null)
                {
                    _dbaseWriter.Close();
                }

                if (_shapeFileReader != null)
                {
                    _shapeFileReader.Close();
                }

                if (_shapeFileWriter != null)
                {
                    _shapeFileWriter.Close();
                }

                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                }

                if (_tree != null)
                {
                    _tree.Dispose();
                    _tree = null;
                }
            }

            _isOpen = false;
        }

        protected internal bool Disposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }
        #endregion
        #endregion

        #region Public Methods and Properties (SharpMap ShapeFile API)
        /// <summary>
        /// Creates a new <see cref="ShapeFile"/> instance and .shp and .shx file on disk.
        /// </summary>
        /// <param name="directory">Directory to create the shapefile in.</param>
        /// <param name="layerName">Name of the shapefile.</param>
        /// <param name="type">Type of shape to store in the shapefile.</param>
        /// <returns>A ShapeFile instance.</returns>
        public static ShapeFile Create(string directory, string layerName, ShapeType type)
        {
            return Create(directory, layerName, type, null);
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
        /// <exception cref="ArgumentException">Thrown if <paramref name="directory"/> is not a valid path.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="layerName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="layerName"/> has invalid path characters.</exception>
        public static ShapeFile Create(string directory, string layerName, ShapeType type, FeatureDataTable schema)
        {
            if (String.IsNullOrEmpty(directory) || directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Parameter must be a valid path", "path");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            
            return Create(directoryInfo, layerName, type, schema);
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="layerName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="layerName"/> has invalid path characters.</exception>
        public static ShapeFile Create(DirectoryInfo directory, string layerName, ShapeType type, FeatureDataTable schema)
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

            if (schema != null)
            {
                schemaTable = DbaseSchema.DeriveSchemaTable(schema);
            }

            string shapeFile = Path.Combine(directory.FullName, layerName + ".shp");

            using (MemoryStream buffer = new MemoryStream(100))
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(ByteEncoder.GetBigEndian(HeaderStartCode));
                writer.Write(new byte[20]);
                writer.Write(ByteEncoder.GetBigEndian(HeaderSizeBytes / 2));
                writer.Write(ByteEncoder.GetLittleEndian(VersionCode));
                writer.Write(ByteEncoder.GetLittleEndian((int)type));
                writer.Write(ByteEncoder.GetLittleEndian(0.0));
                writer.Write(ByteEncoder.GetLittleEndian(0.0));
                writer.Write(ByteEncoder.GetLittleEndian(0.0));
                writer.Write(ByteEncoder.GetLittleEndian(0.0));
                writer.Write(new byte[32]); // Z-values and M-values

                byte[] header = buffer.ToArray();

                using (FileStream shape = File.Create(shapeFile))
                {
                    shape.Write(header, 0, header.Length);
                }

                using (FileStream index = File.Create(Path.Combine(directory.FullName, layerName + ".shx")))
                {
                    index.Write(header, 0, header.Length);
                }
            }

            if (schemaTable != null)
            {
                using (FileStream dbf = File.Create(Path.Combine(directory.FullName, layerName + ".dbf")))
                {
                    DbaseWriter dbaseWriter = new DbaseWriter(dbf, schemaTable);
                    dbaseWriter.Close();
                }
            }

            return new ShapeFile(shapeFile);
        }


        /// <summary>
        /// Forces a rebuild of the spatial index. 
        /// If the instance of the ShapeFile provider
        /// uses a file-based index the file is rewritten to disk,
        /// otherwise it is kept only in memory.
        /// </summary>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is executed and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void RebuildSpatialIndex()
        {
            checkOpen();
            enableReading();

            if (_hasFileBasedSpatialIndex)
            {
                if (File.Exists(_filename + ".sidx"))
                {
                    File.Delete(_filename + ".sidx");
                }

                _tree = CreateSpatialIndexFromFile(_filename);
            }
            else
            {
                _tree = CreateSpatialIndex();
            }

            // TODO: Remove this when connection pooling is implemented:
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Cache.Insert(_filename, _tree, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system of the ShapeFile. If a shapefile has 
        /// a corresponding [filename].prj file containing a Well-Known Text 
        /// description of the coordinate system this will automatically be read.
        /// If this is not the case, the coordinate system will default to null.
        /// </summary>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if property is set and the coordinate system is read from file.
        /// </exception>
        public ICoordinateSystem CoordinateSystem
        {
            get { return _coordinateSystem; }
            set
            {
                //checkOpen();
                if (_coordsysReadFromFile)
                {
                    throw new InvalidShapeFileOperationException(
                        "Coordinate system is specified in projection file and is read only");
                }

                _coordinateSystem = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="SharpMap.Data.Providers.ShapeType">
        /// shape geometry type</see> in this shapefile.
        /// </summary>
        /// <remarks>
        /// The property isn't set until the first time the datasource has been opened,
        /// and will throw an exception if this property has been called since initialization. 
        /// <para>All the non-<see cref="ShapeType.Null"/> shapes in a shapefile are required 
        /// to be of the same shape type.</para>
        /// </remarks>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if property is read and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public ShapeType ShapeType
        {
            get
            {
                checkOpen();
                return _shapeType;
            }
        }

        /// <summary>
        /// Gets or sets the filename of the shapefile
        /// </summary>
        /// <remarks>If the filename changes, indexes will be rebuilt</remarks>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is executed and the shapefile is open. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown if set and the specified filename can't be found.
        /// </exception>
        /// <exception cref="InvalidShapeFileException">
        /// Thrown if set and the shapefile cannot be opened after a rename.
        /// </exception>
        public string Filename
        {
            get { return _filename; }
            set
            {
                if (value != _filename)
                {
                    if (this.IsOpen)
                    {
                        throw new InvalidShapeFileOperationException(
                            "Cannot change filename while datasource is open.");
                    }

                    if (!File.Exists(value))
                    {
                        throw new FileNotFoundException(
                            "Can't find the shapefile specified.", value);
                    }

                    if (Path.GetExtension(value).ToLower() != ".shp")
                    {
                        throw new InvalidShapeFileException(
                            String.Format("Invalid shapefile filename: {0}.", value));
                    }

                    try
                    {
                        File.OpenRead(value).Close();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidShapeFileException("Can't open shapefile.", ex);
                    }

                    _filename = value;
                }
            }
        }

        /// <summary>
        /// Gets the record index (.shx file) filename for the given shapefile
        /// </summary>
        public string IndexFilename
        {
            get 
            {
                return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_filename)), 
                    Path.GetFileNameWithoutExtension(_filename) + ".shx"); 
            }
        }

        /// <summary>
        /// Gets the name of the DBase attribute file.
        /// </summary>
        public string DbfFilename
        {
            get 
            { 
                return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_filename)), 
                    Path.GetFileNameWithoutExtension(_filename) + ".dbf"); 
            }
        }

        /// <summary>
        /// Gets true if the shapefile has an attributes file, false otherwise.
        /// </summary>
        public bool HasDbf
        {
            get { return File.Exists(DbfFilename); }
        }

        /// <summary>
        /// Gets or sets the encoding used for parsing strings from the DBase DBF file.
        /// </summary>
        /// <remarks>
        /// The DBase default encoding is <see cref="System.Text.Encoding.UTF7"/>.
        /// </remarks>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if property is read or set and the shapefile is closed. 
        /// Check <see cref="IsOpen"/> before calling.
        /// </exception>
        /// <exception cref="InvalidShapeFileException">
        /// Thrown if set and there is no DBase file with this shapefile.
        /// </exception>
        public Encoding Encoding
        {
            get
            {
                checkOpen();

                if (!HasDbf)
                {
                    return System.Text.Encoding.UTF7;
                }

                enableReading();
                return _dbaseReader.Encoding;
            }
            set
            {
                if (!HasDbf)
                {
                    throw new InvalidShapeFileException(
                        "The Encoding property can't be set when there is no Dbase file (.dbf) associated with this shapefile.");
                }

                checkOpen();
                enableReading();
                _dbaseReader.Encoding = value;
            }
        }

        #region IProvider Members
        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>		
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            Open(false);
        }

        /// <summary>
        /// Opens the shapefile with optional exclusive access for faster write performance during bulk updates.
        /// </summary>
        /// <param name="exclusive">True if exclusive access is desired, false otherwise</param>
        public void Open(bool exclusive)
        {
            // TODO:
            // Get a Connector.  The connector returned is guaranteed to be connected and ready to go.
            // Pooling.Connector connector = Pooling.ConnectorPool.ConnectorPoolManager.RequestConnector(this,true);

            if (!_isOpen)
            {
                _exclusiveMode = exclusive;

                try
                {
                    enableReading();
                    _isOpen = true;

                    // Parse shape header
                    ParseHeader();

                    // Read projection file
                    ParseProjection();

                    // Read in .shx index
                    ParseIndex();

                    // Load spatial (r-tree) index
                    LoadSpatialIndex(_hasFileBasedSpatialIndex);
                }
                catch (Exception)
                {
                    _isOpen = false;
                    throw;
                }
            }
        }

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            (this as IDisposable).Dispose();
        }

        /// <summary>
        /// Returns geometries whose bounding box intersects <paramref name="bounds"/>.
        /// </summary>
        /// <remarks>
        /// <para>Please note that this method doesn't guarantee that the geometries returned actually 
        /// intersect <paramref name="bounds"/>, but only that their bounding box intersects <paramref name="bounds"/>.</para>
        /// <para>This method is much faster than the QueryFeatures method, because intersection tests
        /// are performed on objects simplifed by their boundingbox, and using the Spatial Index.</para>
        /// </remarks>
        /// <param name="bounds"><see cref="BoundingBox"/> which determines the view</param>
		/// <returns>A <see cref="IEnumerable{Geometry}"/> containing the <see cref="Geometry"/> objects
        /// which are at least partially contained within the given <paramref name="bounds">.</returns>
        /// <exception cref="InvalidShapeFileOperationException">Thrown if method is called and the 
		/// shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox bounds)
        {
            checkOpen();
            enableReading();

			foreach (uint oid in GetObjectIdsInView(bounds))
            {
                Geometry g = GetGeometryById(oid);

                if (!Object.ReferenceEquals(g, null))
                {
					yield return g;
                }
            }
        }

        /// <summary>
		/// Returns all objects whose BoundingBox intersects <paramref name="bounds"/>.
        /// </summary>
        /// <remarks>
        /// <para>
		/// Please note that this method doesn't guarantee that the geometries returned actually 
		/// intersect <paramref name="bounds"/>, but only that their <see cref="BoundingBox"/> intersects bounds.
        /// </para>
        /// <para>This method is much faster than the QueryFeatures method, because intersection tests
		/// are performed on objects simplifed by their BoundingBox, and using the spatial index.</para>
        /// </remarks>
		/// <param name="bounds"><see cref="BoundingBox"/> which determines the view.</param>
        /// <param name="ds">The <see cref="SharpMap.Data.FeatureDataSet"/> to fill 
		/// with features within the <paramref name="bounds">view</paramref>.</param>
        /// <exception cref="InvalidShapeFileOperationException">
		/// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
		/// </exception>
        public void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet ds)
        {
            checkOpen();
            enableReading();

            //Use the spatial index to get a list of features whose boundingbox intersects bbox
            IEnumerable<uint> objects = GetObjectIdsInView(bounds);
            FeatureDataTable<uint> dt = HasDbf ? _dbaseReader.NewTable : FeatureDataTable<uint>.CreateEmpty(IdColumnName);

            foreach (uint oid in objects)
            {
                FeatureDataRow<uint> fdr = HasDbf ? _dbaseReader.GetFeature(oid, dt) : dt.NewRow(oid);

                fdr.Geometry = readGeometry(oid);

                if (fdr.Geometry != null)
                {
                    if (fdr.Geometry.GetBoundingBox().Intersects(bounds))
                    {
                        if (FilterDelegate == null || FilterDelegate(fdr))
                        {
                            dt.AddRow(fdr);
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
        }

        /// <summary>
        /// Returns the data associated with all the geometries that are intersected by 'geom'.
        /// Please note that the ShapeFile provider currently doesn't fully support geometryintersection
        /// and thus only BoundingBox/BoundingBox querying are performed. The results are NOT
        /// guaranteed to lie withing 'geom'.
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="ds">FeatureDataSet to fill with data.</param>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
        {
            checkOpen();
            enableReading();

            FeatureDataTable<uint> dt = HasDbf ? _dbaseReader.NewTable : FeatureDataTable<uint>.CreateEmpty(IdColumnName);
            BoundingBox boundingBox = geom.GetBoundingBox();

            //Get candidates by intersecting the spatial index tree
            IEnumerable<uint> oidList = getKeysFromIndexEntries(_tree.Search(boundingBox));

            foreach (uint oid in oidList)
            {
                for (uint i = (uint)dt.Rows.Count - 1; i >= 0; i--)
                {
                    FeatureDataRow<uint> fdr = getFeature(oid, dt);

                    if (fdr.Geometry != null)
                    {
                        if (fdr.Geometry.GetBoundingBox().Intersects(boundingBox))
                        {
                            // TODO: replace above line with this:  if(fdr.Geometry.Intersects(bbox))  when relation model is complete
                            if (FilterDelegate == null || FilterDelegate(fdr))
                            {
                                dt.AddRow(fdr);
                            }
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
        }

        /// <summary>
        /// Returns geometry Object IDs whose bounding box intersects <paramref name="bbox"/>.
        /// </summary>
        /// <param name="bbox">Bounds which to search for objects in.</param>
        /// <returns>
        /// An enumeration of object ids which have geometries whose 
        /// bounding box is intersected by <paramref name="bbox"/>.
        /// </returns>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public IEnumerable<uint> GetObjectIdsInView(BoundingBox bbox)
        {
            checkOpen();
            enableReading();

			foreach (uint id in getKeysFromIndexEntries(_tree.Search(bbox)))
			{
				yield return id;
			}
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="Geometry"/></returns>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public Geometry GetGeometryById(uint oid)
        {
            checkOpen();
            enableReading();

            if (FilterDelegate != null) //Apply filtering
            {
                FeatureDataRow fdr = GetFeature(oid);

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
        /// Returns the total number of features in the datasource (without any filter applied)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidShapeFileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public int GetFeatureCount()
        {
            if (IsOpen)
            {
                return _shapeIndex.Count;
            }
            else // Assume the feature count from the fixed record-length .shx file
            {
                FileInfo info = new FileInfo(IndexFilename);
                return (int)((info.Length - HeaderSizeBytes) / ShapeRecordHeaderByteLength);
            }
        }

        /// <summary>
        /// Filter Delegate Method for limiting the datasource
        /// </summary>
        /// <remarks>
        /// <example>
        /// Using an anonymous method for filtering all features where the NAME column starts with S:
        /// <code lang="C#">
        /// myShapeDataSource.FilterDelegate = new SharpMap.Data.Providers.ShapeFile.FilterMethod(delegate(FeatureDataRow row) { return (!row["NAME"].ToString().StartsWith("S")); });
        /// </code>
        /// </example>
        /// <example>
        /// Declaring a delegate method for filtering (multi)polygon-features whose area is larger than 5.
        /// <code>
        /// myShapeDataSource.FilterDelegate = CountryFilter;
        /// [...]
        /// public static bool CountryFilter(FeatureDataRow row)
        /// {
        ///		if(row.Geometry.GetType()==typeof(SharpMap.Geometries.Polygon))
        ///			return ((row.Geometry as SharpMap.Geometries.Polygon).Area>5);
        ///		if (row.Geometry.GetType() == typeof(SharpMap.Geometries.MultiPolygon))
        ///			return ((row.Geometry as SharpMap.Geometries.MultiPolygon).Area > 5);
        ///		else return true;
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <seealso cref="FilterMethod"/>
        public FilterMethod FilterDelegate
        {
            get
            {
                return _filterDelegate;
            }
            set
            {
                _filterDelegate = value;
            }
        }

        /// <summary>
        /// Gets a feature row from the datasource with the specified id.
        /// </summary>
        /// <param name="oid">Id of the feautre to return.</param>
        /// <returns>
        /// The feature corresponding to <paramref name="oid" />, or null if no feature is found.
        /// </returns>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public FeatureDataRow<uint> GetFeature(uint oid)
        {
            return getFeature(oid, null);
        }

        /// <summary>
        /// Computes the extents of the data source.
        /// </summary>
        /// <returns>
        /// A BoundingBox instance describing the extents of the entire data source.
        /// </returns>
        public BoundingBox GetExtents()
        {
            if (_tree != null)
            {
                return _tree.Root.BoundingBox;
            }

            if (_envelope != BoundingBox.Empty)
            {
                return _envelope;
            }

            enableReading();
            ParseHeader();
            return _envelope;
        }

        /// <summary>
        /// Gets the connection ID of the datasource.
        /// </summary>
        /// <remarks>
        /// The connection ID of a shapefile is its filename.
        /// </remarks>
        public string ConnectionId
        {
            get { return this._filename; }
        }

        /// <summary>
        /// Gets or sets the spatial reference ID (CRS)
        /// </summary>
        public int Srid
        {
            get { return _srid; }
            set { _srid = value; }
        }

        #endregion

        #region IWritableProvider Members

        /// <summary>
        /// Saves a feature to a shapefile
        /// </summary>
        /// <param name="feature">Feature to save</param>
        /// <exception cref="InvalidShapeFileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        public void Save(FeatureDataRow<uint> feature)
        {
            //throw new NotImplementedException("Not implemented in this version");

            checkOpen();
            
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            enableWriting();

            writeFeatureRow(feature);

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Saves features to a shapefile.
        /// </summary>
        /// <param name="feature">Enumeration of features to save.</param>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void Save(IEnumerable<FeatureDataRow<uint>> rows)
        {
            checkOpen();
            
            if (rows == null)
            {
                throw new ArgumentNullException("rows");
            }

            enableWriting();

            foreach (FeatureDataRow row in rows)
            {
                writeFeatureRow(row);
            }

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Saves features to the shapefile.
        /// </summary>
        /// <param name="table">
        /// A FeatureDataTable containing feature data and geometry.
        /// </param>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        public void Save(FeatureDataTable<uint> table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            checkOpen();
            enableWriting();

            _shapeFileStream.Position = HeaderSizeBytes;
            foreach (FeatureDataRow row in table.Rows)
            {
                if (row is FeatureDataRow<uint>)
                {
                    _tree.Insert(new RTreeIndexEntry<uint>((row as FeatureDataRow<uint>).Id, row.Geometry.GetBoundingBox()));
                }
                else
                {
                    _tree.Insert(new RTreeIndexEntry<uint>(getNextId(), row.Geometry.GetBoundingBox()));
                }

                writeFeatureRow(row);
            }

            writeIndex();
            writeHeader(_shapeFileWriter);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="feature">Feature to delete.</param>
        public void Delete(FeatureDataRow<uint> feature)
        {
            throw new NotImplementedException("Not implemented in this version");
        }
        #endregion
        #endregion

        #region General helper functions
        private void checkOpen()
        {
            if (!IsOpen)
            {
                throw new InvalidShapeFileOperationException("An attempt was made to access a closed datasource.");
            }
        }

        private uint getNextId()
        {
            return (uint)_shapeIndex.Count;
        }

        private int computeFileLengthInWords()
        {
            int length = HeaderSizeBytes / 2;

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                length += kvp.Value.Length + ShapeFile.ShapeRecordHeaderByteLength / 2;
            }

            return length;
        }

        private int computeGeometryLengthInWords(Geometry geometry)
        {
            if (geometry == null)
            {
                throw new NotSupportedException("Writing null shapes not supported in this version");
            }

            int byteCount = 0;

            if (geometry is Point)
            {
                byteCount = 20; // ShapeType integer + 2 doubles at 8 bytes each
            }
            else if (geometry is MultiPoint)
            {
                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 /* NumPoints integer */ + 16 * (geometry as MultiPoint).Points.Count;
            }
            else if (geometry is LineString)
            {
                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 /* Parts Array 1 integer long */ + 16 * (geometry as LineString).Vertices.Count;
            }
            else if (geometry is MultiLineString)
            {
                int pointCount = 0;

                foreach (LineString line in (geometry as MultiLineString).LineStrings)
                {
                    pointCount += line.Vertices.Count;
                }

                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 * (geometry as MultiLineString).LineStrings.Count /* Parts array of integer indexes */
                    + 16 * pointCount;
            }
            else if (geometry is Polygon)
            {
                int pointCount = (geometry as Polygon).ExteriorRing.Vertices.Count;

                foreach (LinearRing ring in (geometry as Polygon).InteriorRings)
                {
                    pointCount += ring.Vertices.Count;
                }

                byteCount = 4 /* ShapeType Integer */ + BoundingBoxFieldByteLength + 4 + 4 /* NumPoints and NumParts integers */
                    + 4 * ((geometry as Polygon).InteriorRings.Count + 1 /* Parts array of rings: count of interior + 1 for exterior ring */)
                    + 16 * pointCount;
            }
            else
            {
                throw new NotSupportedException("Currently unsupported geometry type.");
            }

            return byteCount / 2; // number of 16-bit words
        }


        private IEnumerable<uint> getKeysFromIndexEntries(IEnumerable<RTreeIndexEntry<uint>> entries)
        {
            foreach (RTreeIndexEntry<uint> entry in entries)
            {
                yield return entry.Value;
            }
        }

        /// <summary>
        /// Gets a datarow from the datasource with the 
        /// specified object id belonging to the specified datatable.
        /// </summary>
        /// <param name="oid">Object id to lookup.</param>
        /// <param name="dt">Datatable to feature should belong to.</param>
        /// <returns>Row corresponding to the object id.</returns>
        /// <exception cref="InvalidShapeFileOperationException">
        /// Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.
        /// </exception>
        private FeatureDataRow<uint> getFeature(uint oid, FeatureDataTable<uint> dt)
        {
            checkOpen();
            enableReading();

            if (dt == null)
            {
                if (!HasDbf)
                {
                    dt = FeatureDataTable<uint>.CreateEmpty(IdColumnName);
                }
                else
                {
                    dt = _dbaseReader.NewTable;
                }
            }

            FeatureDataRow<uint> dr = HasDbf ? _dbaseReader.GetFeature(oid, dt) : dt.NewRow(oid);
            dr.Geometry = readGeometry(oid);

            if (FilterDelegate == null || FilterDelegate(dr))
            {
                return dr;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Spatial indexing helper functions
        /// <summary>
        /// Loads a spatial index from a file. If it doesn't exist, one is created and saved
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>QuadTree index</returns>
        private DynamicRTree<uint> CreateSpatialIndexFromFile(string filename)
        {
            if (System.IO.File.Exists(filename + ".sidx"))
            {
                try
                {
                    using (FileStream indexStream = new FileStream(filename + ".sidx", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return DynamicRTree<uint>.FromStream(indexStream);
                    }
                }
                catch (ObsoleteIndexFileFormatException)
                {
                    File.Delete(filename + ".sidx");
                    return CreateSpatialIndexFromFile(filename);
                }
                catch (Exception) { throw; }
            }
            else
            {
                DynamicRTree<uint> tree = CreateSpatialIndex();

                using (FileStream indexStream = new FileStream(filename + ".sidx", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    tree.SaveIndex(indexStream);
                }

                return tree;
            }
        }

        /// <summary>
        /// Generates a spatial index for a specified shape file.
        /// </summary>
        /// <param name="filename"></param>
        private DynamicRTree<uint> CreateSpatialIndex()
        {
            // TODO: implement Post-optimization restructure strategy
            IIndexRestructureStrategy restructureStrategy = new NullRestructuringStrategy();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.Default, 4.0);
            IEntryInsertStrategy<RTreeIndexEntry<uint>> insertStrategy = new GuttmanQuadraticInsert<uint>();
            INodeSplitStrategy nodeSplitStrategy = new GuttmanQuadraticSplit<uint>();
            DynamicRTreeBalanceHeuristic indexHeuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);

            DynamicRTree<uint> index = new SelfOptimizingDynamicSpatialIndex<uint>(restructureStrategy, restructureHeuristic, insertStrategy, nodeSplitStrategy, indexHeuristic);

            for (uint i = 0; i < (uint)GetFeatureCount(); i++)
            {
                Geometry geom = readGeometry(i);
                if (geom == null)
                    continue;

                BoundingBox box = geom.GetBoundingBox();
                if (!double.IsNaN(box.Left) && !double.IsNaN(box.Right) && !double.IsNaN(box.Bottom) && !double.IsNaN(box.Top))
                    index.Insert(new RTreeIndexEntry<uint>(i, box));
            }

            return index;
        }

        private void LoadSpatialIndex()
        {
            LoadSpatialIndex(false, false);
        }

        private void LoadSpatialIndex(bool LoadFromFile)
        {
            LoadSpatialIndex(false, LoadFromFile);
        }

        private void LoadSpatialIndex(bool ForceRebuild, bool LoadFromFile)
        {
            //Only load the tree if we haven't already loaded it, or if we want to force a rebuild
            if (_tree == null || ForceRebuild)
            {
                // Is this a web application? If so lets store the index in the cache so we don't
                // need to rebuild it for each request
                if (System.Web.HttpContext.Current != null)
                {
                    //Check if the tree exists in the cache
                    if (System.Web.HttpContext.Current.Cache[_filename] != null)
                    {
                        _tree = (DynamicRTree<uint>)System.Web.HttpContext.Current.Cache[_filename];
                    }
                    else
                    {
                        if (!LoadFromFile)
                        {
                            _tree = CreateSpatialIndex();
                        }
                        else
                        {
                            _tree = CreateSpatialIndexFromFile(_filename);
                        }

                        //Store the tree in the web cache
                        // TODO: Remove this when connection pooling is implemented
                        System.Web.HttpContext.Current.Cache.Insert(_filename, _tree, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
                    }
                }
                else
                {
                    if (!LoadFromFile)
                    {
                        _tree = CreateSpatialIndex();
                    }
                    else
                    {
                        _tree = CreateSpatialIndexFromFile(_filename);
                    }
                }
            }
        }
        #endregion

        #region Geometry reading helper functions
        /// <summary>
        /// Reads all boundingboxes of features in the shapefile. This is used for spatial indexing.
        /// </summary>
        /// <returns></returns>
        private List<BoundingBox> getAllFeatureBoundingBoxes()
        {
            //int[] offsetOfRecord = ReadIndex(); //Read the whole .idx file

            enableReading();
            List<BoundingBox> boxes = new List<BoundingBox>();

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                _shapeFileStream.Seek(kvp.Value.AbsoluteByteOffset + ShapeRecordHeaderByteLength, SeekOrigin.Begin);

                if ((ShapeType)ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()) != ShapeType.Null)
                {
                    double xMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    double yMin = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble());
                    double xMax, yMax;

                    if (_shapeType == ShapeType.Point)
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

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile
        /// </summary>
        /// <remarks><see cref="FilterDelegate">Filtering</see> is not applied to this method</remarks>
        /// <param name="oid">Object ID</param>
        /// <returns><see cref="SharpMap.Geometries.Geometry"/> instance from the ShapeFile corresponding to <paramref name="oid"/></returns>
        private Geometry readGeometry(uint oid)
        {
            enableReading();
            _shapeFileReader.BaseStream.Seek(_shapeIndex[oid].AbsoluteByteOffset + ShapeRecordHeaderByteLength, SeekOrigin.Begin);
            ShapeType type = (ShapeType)ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()); //Shape type

            if (type == ShapeType.Null)
                return null;

            switch (_shapeType)
            {
                case ShapeType.Point:
                    return readPoint();
                case ShapeType.PolyLine:
                    return readPolyLine();
                case ShapeType.Polygon:
                    return readPolygon();
                case ShapeType.MultiPoint:
                    return readMultiPoint();
                case ShapeType.PointZ:
                    return readPointZ();
                case ShapeType.PolyLineZ:
                    return readPolyLineZ();
                case ShapeType.PolygonZ:
                    return readPolygonZ();
                case ShapeType.MultiPointZ:
                    return readMultiPointZ();
                case ShapeType.PointM:
                    return readPointM();
                case ShapeType.PolyLineM:
                    return readPolyLineM();
                case ShapeType.PolygonM:
                    return readPolygonM();
                case ShapeType.MultiPointM:
                    return readMultiPointM();
                default:
                    throw new UnsupportedShapeFileGeometryException("ShapeFile type " + _shapeType.ToString() + " not supported");
            }
        }

        private Geometry readMultiPointM()
        {
            throw new NotSupportedException("MultiPointM features are not currently supported");
        }

        private Geometry readPolygonM()
        {
            throw new NotSupportedException("PolygonM features are not currently supported");
        }

        private Geometry readMultiPointZ()
        {
            throw new NotSupportedException("MultiPointZ features are not currently supported");
        }

        private Geometry readPolyLineZ()
        {
            throw new NotSupportedException("PolyLineZ features are not currently supported");
        }

        private Geometry readPolyLineM()
        {
            throw new NotSupportedException("PolyLineM features are not currently supported");
        }

        private Geometry readPointM()
        {
            throw new NotSupportedException("PointM features are not currently supported");
        }

        private Geometry readPolygonZ()
        {
            throw new NotSupportedException("PolygonZ features are not currently supported");
        }

        private Geometry readPointZ()
        {
            throw new NotSupportedException("PointZ features are not currently supported");
        }

        private Geometry readPoint()
        {
            Point tempFeature = new Point();
            return new Point(ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()), ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()));
        }

        private Geometry readMultiPoint()
        {
            _shapeFileReader.BaseStream.Seek(BoundingBoxFieldByteLength, SeekOrigin.Current); //skip min/max box
            MultiPoint feature = new MultiPoint();
            int nPoints = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()); // get the number of points

            if (nPoints == 0)
            {
                return null;
            }

            for (int i = 0; i < nPoints; i++)
            {
                feature.Points.Add(new Point(ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()), ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble())));
            }

            return feature;
        }

        private void readPolyStructure(out int parts, out int points, out int[] segments)
        {
            _shapeFileReader.BaseStream.Seek(BoundingBoxFieldByteLength, SeekOrigin.Current); //skip min/max box
            parts = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()); // get number of parts (segments)
            points = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32()); // get number of points
            segments = new int[parts + 1];

            //Read in the segment indexes
            for (int b = 0; b < parts; b++)
            {
                segments[b] = ByteEncoder.GetLittleEndian(_shapeFileReader.ReadInt32());
            }

            //add end point
            segments[parts] = points;
        }

        private Geometry readPolyLine()
        {
            int parts;
            int points;
            int[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
            {
                return null;
            }

            MultiLineString mline = new MultiLineString();

            for (int lineId = 0; lineId < parts; lineId++)
            {
                LineString line = new LineString();

                for (int i = segments[lineId]; i < segments[lineId + 1]; i++)
                {
                    line.Vertices.Add(new Point(ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()), ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble())));
                }

                mline.LineStrings.Add(line);
            }

            if (mline.LineStrings.Count == 1)
            {
                return mline[0];
            }

            return mline;
        }

        private Geometry readPolygon()
        {
            int parts;
            int points;
            int[] segments;
            readPolyStructure(out parts, out points, out segments);

            if (parts == 0)
                return null;

            //First read all the rings
            List<LinearRing> rings = new List<LinearRing>();

            for (int ringId = 0; ringId < parts; ringId++)
            {
                LinearRing ring = new LinearRing();

                for (int i = segments[ringId]; i < segments[ringId + 1]; i++)
                {
                    ring.Vertices.Add(new Point(ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()), ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble())));
                }

                rings.Add(ring);
            }

            bool[] isCounterClockWise = new bool[rings.Count];
            int PolygonCount = 0;

            for (int i = 0; i < rings.Count; i++)
            {
                isCounterClockWise[i] = rings[i].IsCcw();

                if (!isCounterClockWise[i])
                {
                    PolygonCount++;
                }
            }

            if (PolygonCount == 1) //We only have one polygon
            {
                Polygon poly = new Polygon();
                poly.ExteriorRing = rings[0];

                if (rings.Count > 1)
                {
                    for (int i = 1; i < rings.Count; i++)
                    {
                        poly.InteriorRings.Add(rings[i]);
                    }
                }

                return poly;
            }
            else
            {
                MultiPolygon mpoly = new MultiPolygon();
                Polygon poly = new Polygon();
                poly.ExteriorRing = rings[0];

                for (int i = 1; i < rings.Count; i++)
                {
                    if (!isCounterClockWise[i])
                    {
                        mpoly.Polygons.Add(poly);
                        poly = new Polygon(rings[i]);
                    }
                    else
                    {
                        poly.InteriorRings.Add(rings[i]);
                    }
                }

                mpoly.Polygons.Add(poly);
                return mpoly;
            }
        }
        #endregion

        #region File parsing helper functions

        /// <summary>
        /// Reads and parses the header of the .shp index file
        /// </summary>
        /// <remarks>
        /// From ESRI ShapeFile Technical Description document
        /// 
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
        /// 
        /// Byte
        /// Position    Field           Value       Type    Order
        /// -----------------------------------------------------
        /// Byte 0      File Code       9994        Integer Big
        /// Byte 4      Unused          0           Integer Big
        /// Byte 8      Unused          0           Integer Big
        /// Byte 12     Unused          0           Integer Big
        /// Byte 16     Unused          0           Integer Big
        /// Byte 20     Unused          0           Integer Big
        /// Byte 24     File Length     File Length Integer Big
        /// Byte 28     Version         1000        Integer Little
        /// Byte 32     Shape Type      Shape Type  Integer Little
        /// Byte 36     Bounding Box    Xmin        Double  Little
        /// Byte 44     Bounding Box    Ymin        Double  Little
        /// Byte 52     Bounding Box    Xmax        Double  Little
        /// Byte 60     Bounding Box    Ymax        Double  Little
        /// Byte 68*    Bounding Box    Zmin        Double  Little
        /// Byte 76*    Bounding Box    Zmax        Double  Little
        /// Byte 84*    Bounding Box    Mmin        Double  Little
        /// Byte 92*    Bounding Box    Mmax        Double  Little
        /// 
        /// * Unused, with value 0.0, if not Measured or Z type
        /// 
        /// The "Integer" type corresponds to the CLS Int32 type, and "Double" to CLS Double (IEEE 754).
        /// </remarks>
        private void ParseHeader()
        {
            _shapeFileReader.BaseStream.Seek(0, SeekOrigin.Begin);
            //Check file header
            if (ByteEncoder.GetBigEndian(_shapeFileReader.ReadInt32()) != HeaderStartCode)
            {
                throw new InvalidShapeFileException("Invalid ShapeFile (.shp)");
            }

            _shapeFileReader.BaseStream.Seek(24, 0); //seek to File Length
            int fileLength = ByteEncoder.GetBigEndian(_shapeFileReader.ReadInt32()); //Read filelength as big-endian. The length is number of 16-bit words in file

            _shapeFileReader.BaseStream.Seek(32, 0); //seek to ShapeType
            _shapeType = (ShapeType)_shapeFileReader.ReadInt32();

            //Read the spatial bounding box of the contents
            _shapeFileReader.BaseStream.Seek(36, 0); //seek to box
            _envelope = new SharpMap.Geometries.BoundingBox(
                ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()),
                ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()),
                ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()),
                ByteEncoder.GetLittleEndian(_shapeFileReader.ReadDouble()));
        }

        /// <summary>
        /// Parses the .shx shapefile index file
        /// </summary>
        /// <remarks>
        /// The index file is organized to give a matching offset and content length for each entry in the .shp file.
        /// 
        /// From ESRI ShapeFile Technical Description document
        /// 
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
        /// 
        /// Byte
        /// Position    Field           Value           Type    Order
        /// ---------------------------------------------------------
        /// Byte 0      Offset          Offset          Integer Big
        /// Byte 4      Content Length  Content Length  Integer Big
        /// 
        /// The Integer type corresponds to the CLS Int32 type.
        /// </remarks>
        private void ParseIndex()
        {
            FileStream indexStream;
            BinaryReader indexReader;

            using (indexStream = new FileStream(IndexFilename, FileMode.Open, FileAccess.Read))
            using (indexReader = new BinaryReader(indexStream, System.Text.Encoding.Unicode))
            {
                indexStream.Seek(HeaderSizeBytes, SeekOrigin.Begin);
                uint recordNumber = 0;
                while (indexStream.Position < indexStream.Length)
                {
                    IndexEntry entry = new IndexEntry();
                    entry.Offset = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
                    entry.Length = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
                    _shapeIndex[recordNumber++] = entry;
                }
            }
        }

        /// <summary>
        /// Reads and parses the projection if a projection file exists
        /// </summary>
        private void ParseProjection()
        {
            string projfile = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename) + ".prj");

            if (System.IO.File.Exists(projfile))
            {
                try
                {
                    string wkt = System.IO.File.ReadAllText(projfile);
                    _coordinateSystem = (ICoordinateSystem)SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(wkt);
                    _coordsysReadFromFile = true;
                }
                catch (ArgumentException ex)
                {
                    System.Diagnostics.Trace.TraceWarning("Coordinate system file '" + projfile + "' found, but could not be parsed. WKT parser returned:" + ex.Message);
                    throw new InvalidShapeFileException("Invalid .prj file", ex);
                }
            }
        }
        #endregion

        #region File writing helper functions
        private void writeFeatureRow(FeatureDataRow feature)
        {
            uint recordNumber = addIndexEntry(feature);

            if (HasDbf)
            {
                _dbaseWriter.AddRow(feature);
            }

            writeGeometry(feature.Geometry, recordNumber, _shapeIndex[recordNumber].Length);
        }

        private void writeFeatureRow(FeatureDataRow<uint> feature)
        {
            uint recordNumber = feature.Id;

            if (!_shapeIndex.ContainsKey(recordNumber))
            {
                recordNumber = addIndexEntry(feature);

                if (HasDbf)
                {
                    _dbaseWriter.AddRow(feature);
                }
            }
            else if (HasDbf)
            {
                _dbaseWriter.UpdateRow(recordNumber, feature);
            }

            writeGeometry(feature.Geometry, recordNumber, _shapeIndex[recordNumber].Length);
        }

        private uint addIndexEntry(FeatureDataRow feature)
        {
            IndexEntry entry = new IndexEntry();
            entry.Length = computeGeometryLengthInWords(feature.Geometry);
            entry.Offset = computeFileLengthInWords();
            uint id = getNextId();
            _shapeIndex[id] = entry;
            return id;
        }

        private void enableReading()
        {
            if (_shapeFileReader == null || !_shapeFileStream.CanRead)
            {
                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                }

                if (_exclusiveMode)
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                else
                {
                    _shapeFileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                _shapeFileReader = new BinaryReader(_shapeFileStream);
            }

            if (HasDbf)
            {
                if (_dbaseReader == null)
                {
                    if (!_exclusiveMode)
                    {
                        if (_dbaseWriter != null)
                        {
                            _dbaseWriter.Close();
                        }

                        _dbaseWriter = null;
                    }

                    _dbaseReader = new DbaseReader(DbfFilename);
                }

                if (!_dbaseReader.IsOpen)
                {
                    _dbaseReader.Open();
                }
            }
        }

        private void enableWriting()
        {
            if (_shapeFileWriter == null || !_shapeFileStream.CanWrite)
            {
                if (_shapeFileStream != null)
                {
                    _shapeFileStream.Close();
                }

                if (_exclusiveMode)
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                else
                {
                    _shapeFileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                }

                _shapeFileWriter = new BinaryWriter(_shapeFileStream);
            }

            if (HasDbf)
            {
                if (_dbaseWriter == null)
                {
                    if (!_exclusiveMode)
                    {
                        if (_dbaseReader != null)
                        {
                            _dbaseReader.Close();
                        }

                        _dbaseReader = null;
                    }

                    // Workaround for trying to open the file for exclusive writing too quickly after disposing the reader
                    int numberAttemptsToOpenDbfForWriting = 0;

                    while (_dbaseWriter != null && numberAttemptsToOpenDbfForWriting < 3)
                    {
                        try
                        {
                            _dbaseWriter = new DbaseWriter(DbfFilename);
                        }
                        catch (System.IO.IOException)
                        {
                            System.Threading.Thread.Sleep(200);
                            numberAttemptsToOpenDbfForWriting++;
                        }
                    }

                    if (_dbaseWriter == null)
                    {
                        throw new ShapeFileException("Can't open Dbase file for writing.");
                    }
                }
            }
        }

        private void writeHeader(BinaryWriter writer)
        {
            BoundingBox boundingBox = GetExtents();
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(ByteEncoder.GetBigEndian(HeaderStartCode));
            writer.Write(new byte[20]);
            writer.Write(ByteEncoder.GetBigEndian(computeFileLengthInWords()));
            writer.Write(ByteEncoder.GetLittleEndian(VersionCode));
            writer.Write(ByteEncoder.GetLittleEndian((int)ShapeType));
            writer.Write(ByteEncoder.GetLittleEndian(boundingBox.Left));
            writer.Write(ByteEncoder.GetLittleEndian(boundingBox.Bottom));
            writer.Write(ByteEncoder.GetLittleEndian(boundingBox.Right));
            writer.Write(ByteEncoder.GetLittleEndian(boundingBox.Top));
            writer.Write(new byte[32]); // Z-values and M-values
        }

        private void writeIndex()
        {
            IndexEntry[] indexEntries = new IndexEntry[_shapeIndex.Count];

            foreach (KeyValuePair<uint, IndexEntry> kvp in _shapeIndex)
            {
                indexEntries[kvp.Key] = kvp.Value;
            }

            using (FileStream indexStream = File.Open(IndexFilename, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter indexWriter = new BinaryWriter(indexStream))
            {
                writeHeader(indexWriter);

                foreach (IndexEntry entry in indexEntries)
                {
                    indexWriter.Write(ByteEncoder.GetBigEndian(entry.Offset));
                    indexWriter.Write(ByteEncoder.GetBigEndian(entry.Length));
                }

                indexWriter.Flush();
            }
        }

        private void writeGeometry(Geometry g, uint recordNumber, int recordLengthInWords)
        {
            if (g == null)
            {
                throw new NotSupportedException("Writing null shapes not supported in this version");
            }

            _shapeFileStream.Position = _shapeIndex[recordNumber].AbsoluteByteOffset;
            recordNumber += 1; // Record numbers are 1- based in shapefile
            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordNumber));
            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordLengthInWords));

            if (g is Point)
            {
                writePoint(g as Point);
            }
            else if (g is MultiPoint)
            {
                writeMultiPoint(g as MultiPoint);
            }
            else if (g is LineString)
            {
                writeLineString(g as LineString);
            }
            else if (g is MultiLineString)
            {
                writeMultiLineString(g as MultiLineString);
            }
            else if (g is Polygon)
            {
                writePolygon(g as Polygon);
            }
            else
            {
                throw new NotSupportedException(String.Format("Writing geometry type {0} is not supported in the current version", g.GetType()));
            }
        }

        private void writeCoordinate(double x, double y)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(x));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(y));
        }

        private void writePoint(Point point)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((int)ShapeType.Point));
            writeCoordinate(point.X, point.Y);
        }

        private void writeBoundingBox(BoundingBox box)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.Left));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.Bottom));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.Right));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.Top));
        }

        private void writeMultiPoint(MultiPoint multiPoint)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((int)ShapeType.MultiPoint));
            writeBoundingBox(multiPoint.GetBoundingBox());
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(multiPoint.Points.Count));

            foreach (Point point in multiPoint.Points)
            {
                writeCoordinate(point.X, point.Y);
            }
        }

        private void writePolySegments(BoundingBox bbox, int[] parts, Point[] points)
        {
            writeBoundingBox(bbox);
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(parts.Length));
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(points.Length));

            foreach (int partIndex in parts)
            {
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(partIndex));
            }

            foreach (SharpMap.Geometries.Point point in points)
            {
                writeCoordinate(point.X, point.Y);
            }
        }

        private void writeLineString(LineString lineString)
        {
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((int)ShapeType.PolyLine));
            writePolySegments(lineString.GetBoundingBox(), new int[] { 0 }, lineString.Vertices.ToArray());
        }

        private void writeMultiLineString(MultiLineString multiLineString)
        {
            int[] parts = new int[multiLineString.LineStrings.Count];
            List<Point> allPoints = new List<Point>();

            int currentPartsIndex = 0;
            foreach (LineString line in multiLineString.LineStrings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange(line.Vertices);
            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((int)ShapeType.PolyLine));
            writePolySegments(multiLineString.GetBoundingBox(), parts, allPoints.ToArray());
        }

        private void writePolygon(Polygon polygon)
        {
            int[] parts = new int[polygon.NumInteriorRing + 1];
            List<Point> allPoints = new List<Point>();
            int currentPartsIndex = 0;
            parts[currentPartsIndex++] = 0;
            allPoints.AddRange(polygon.ExteriorRing.Vertices);

            foreach (LinearRing ring in polygon.InteriorRings)
            {
                parts[currentPartsIndex++] = allPoints.Count;
                allPoints.AddRange(ring.Vertices);
            }

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian((int)ShapeType.Polygon));
            writePolySegments(polygon.GetBoundingBox(), parts, allPoints.ToArray());
        }
        #endregion

        #region IndexEntry struct
        /// <summary>
        /// Entry for each feature to determine the position and length of the geometry in the .shp file.
        /// </summary>
        private struct IndexEntry
        {
            private int _offset;
            private int _length;

            /// <summary>
            /// Number of 16-bit words taken up by the record
            /// </summary>
            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }

            /// <summary>
            /// Offset of the record in 16-bit words from the beginning of the shapefile
            /// </summary>
            public int Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }

            /// <summary>
            /// Number of bytes in the record
            /// </summary>
            public int ByteLength
            {
                get { return _length * 2; }
            }

            /// <summary>
            /// Record offest in bytes from the beginning of the shapefile
            /// </summary>
            public int AbsoluteByteOffset
            {
                get { return _offset * 2; }
            }

            public override string ToString()
            {
                return String.Format("[IndexEntry] Offset: {0}; Length: {1}; Stream Position: {2}", Offset, Length, AbsoluteByteOffset);
            }
        }
        #endregion

        #region Obsolete

        ///// <summary>
        ///// Returns all objects within a distance of a geometry
        ///// </summary>
        ///// <param name="geom"></param>
        ///// <param name="distance"></param>
        ///// <returns></returns>
        ///// <exception cref="InvalidShapeFileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        //[Obsolete("Use ExecuteIntersectionQuery instead")]
        //public FeatureDataTable<uint> QueryFeatures(SharpMap.Geometries.Geometry geom, double distance)
        //{
        //    checkOpen();
        //    EnableReading();

        //    FeatureDataTable<uint> dt = _dbaseReader.NewTable;
        //    SharpMap.Geometries.BoundingBox bbox = geom.GetBoundingBox();
        //    bbox.Min.X -= distance; bbox.Max.X += distance;
        //    bbox.Min.Y -= distance; bbox.Max.Y += distance;
        //    //Get candidates by intersecting the spatial index tree
        //    List<uint> objectlist = _tree.Search(bbox);

        //    if (objectlist.Count == 0)
        //        return dt;

        //    SharpMap.Geometries.Geometry geomBuffer = geom.Buffer(distance);
        //    for (int j = 0; j < objectlist.Count; j++)
        //    {
        //        for (uint i = (uint)dt.Rows.Count - 1; i >= 0; i--)
        //        {
        //            FeatureDataRow fdr = GetFeature(objectlist[j], dt);
        //            if (fdr != null && fdr.Geometry.Intersects(geomBuffer))
        //                dt.Rows.Add(fdr);
        //        }
        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// Returns all objects whose boundingbox intersects bbox - OBSOLETE: Use ExecuteIntersectionQuery(box) instead
        ///// </summary>
        ///// <param name="bbox"><see cref="BoundingBox"/> which determines the view</param>
        ///// <param name="ds"></param>
        ///// <exception cref="InvalidShapeFileOperationException">Thrown if method is called and the shapefile is closed. Check <see cref="IsOpen"/> before calling.</exception>
        //[Obsolete("Use ExecuteIntersectionQuery(box) instead")]
        //public void GetFeaturesInView(SharpMap.Geometries.BoundingBox bbox, SharpMap.Data.FeatureDataSet ds)
        //{
        //    checkOpen();
        //    EnableReading();

        //    //Use the spatial index to get a list of features whose boundingbox intersects bbox
        //    List<uint> objectlist = GetObjectIDsInView(bbox);
        //    FeatureDataTable<uint> dt = _dbaseReader.NewTable;

        //    foreach (uint key in objectlist)
        //    {
        //        FeatureDataRow<uint> fdr = _dbaseReader.GetFeature(key, dt);
        //        fdr.Geometry = ReadGeometry(key);
        //        if (fdr.Geometry != null)
        //            if (fdr.Geometry.GetBoundingBox().Intersects(bbox))
        //                if (FilterDelegate == null || FilterDelegate(fdr))
        //                    dt.AddRow(fdr);
        //    }

        //    ds.Tables.Add(dt);
        //}
        #endregion

		#region IProvider<uint> Members

		IEnumerable<uint> IProvider<uint>.GetObjectIdsInView(BoundingBox boundingBox)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IProvider Members

		public SharpMap.CoordinateSystems.Transformations.ICoordinateTransformation CoordinateTransformation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion
	}
}
