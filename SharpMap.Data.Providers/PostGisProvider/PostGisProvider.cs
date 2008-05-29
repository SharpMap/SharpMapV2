// Portions copyright 2005, 2006 - Christian Gräfe (www.sharptools.de)
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using PostgreSql.Data.PgTypes;
using PostgreSql.Data.PostgreSqlClient;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Expressions;
using SharpMap.Geometries;

// more info at http://sf.net/projects/pgsqlclient

namespace SharpMap.Data.Providers.PostGis
{
    /// <summary>
    /// SharpMap data provider for PostGIS (GIS extension for Postgresql database).
    /// </summary>
    [Serializable]
    public class PostGisProvider : IFeatureLayerProvider<uint>
    {
        private static readonly NumberFormatInfo NumberFormat_enUS
            = new CultureInfo("en-US", false).NumberFormat;

        private const string RetrievedGeometryColumnName = "sharpmap_tempgeometry";

        private bool _isOpen;
        private bool _isDisposed = false;
        private string _objectIdColumn;
        private string _geometryColumn;
        private string _connectionString;
        private string _table;
        private int? _srid = null;
        private string _defintionQuery;

        #region Object construction and disposal

        #region Constructors

        /// <summary>
        /// Initializes a new connection to a PostGIS instance.
        /// </summary>
        /// <param name="connectionString">Connection string to Postgresql instance.</param>
        /// <param name="tableName">Name of data table to use for layer data.</param>
        /// <param name="geometryColumnName">Name of geometry column.</param>
        /// <param name="oidColumnName">Name of column with unique feature identifier.</param>
        public PostGisProvider(string connectionString, string tableName, string geometryColumnName, string oidColumnName)
        {
            ConnectionString = connectionString;
            Table = tableName;
            GeometryColumn = geometryColumnName;
            ObjectIdColumn = oidColumnName;
        }

        /// <summary>
        /// Initializes a new connection to a PostGIS instance.
        /// </summary>
        /// <param name="connectionString">Connection string to Postgresql instance.</param>
        /// <param name="tableName">Name of data table to use for layer data.</param>
        /// <param name="oidColumnName">Name of column with unique feature identifier.</param>
        public PostGisProvider(string connectionString, string tableName, string oidColumnName)
            : this(connectionString, tableName, "", oidColumnName)
        {
            GeometryColumn = getGeometryColumn();
        }

        #endregion

        #region Disposers and finalizers

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PostGisProvider()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }
        }

        /// <summary>
        /// Gets a value indicating if the object is disposed.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the object is disposed; 
        /// <see langword="false"/> otherwise.
        /// </value>
        public bool IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection id for the data source.
        /// </summary>
        /// <value>
        /// The ConnectionId is the connection string for the PostGIS instance.
        /// </value>
        /// <seealso cref="ConnectionString"/>
        public string ConnectionId
        {
            get
            {
                checkDisposed();
                return _connectionString;
            }
        }

        /// <summary>
        /// Gets or sets the connection string to Postgresql instance.
        /// </summary>
        /// <seealso cref="ConnectionId"/>
        public string ConnectionString
        {
            get
            {
                checkDisposed();
                return _connectionString;
            }
            set { _connectionString = value; }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                checkDisposed();
                throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Definition query used for limiting dataset
        /// </summary>
        public string DefinitionQuery
        {
            get
            {
                checkDisposed();
                return _defintionQuery;
            }
            set { _defintionQuery = value; }
        }

        /// <summary>
        /// Gets or sets the name of geometry column.
        /// </summary>
        public string GeometryColumn
        {
            get
            {
                checkDisposed();
                return _geometryColumn;
            }
            set
            {
                checkDisposed();
                if (IsOpen)
                {
                    throw new PostGisInvalidOperationException(
                        "Geometry column cannot be changed when data source is open.");
                }
                _geometryColumn = value;
            }
        }

        /// <summary>
        /// Gets a value indicating if the data source is currently open.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the data source is open; 
        /// <see langword="false"/> otherwise.
        /// </value>
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Gets or sets the name of column with unique feature identifier.
        /// </summary>
        public string ObjectIdColumn
        {
            get
            {
                checkDisposed();
                return _objectIdColumn;
            }
            set { _objectIdColumn = value; }
        }

        public ICoordinateSystem SpatialReference
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the Spatial Reference ID of the <see cref="Table"/>'s
        /// <see cref="GeometryColumn"/>.
        /// </summary>
        public int? Srid
        {
            get
            {
                checkDisposed();
                return _srid;
            }
            set { throw new NotSupportedException("Spatial Reference ID cannot by set on a PostGIS table"); }
        }

        /// <summary>
        /// Gets or sets the name of data table to use for layer data.
        /// </summary>
        public string Table
        {
            get
            {
                checkDisposed();
                return _table;
            }
            set { _table = value; }
        }

        #endregion

        #region IFeatureLayerProvider<uint> Members
        /// <summary>
        /// Returns geometry object ids whose bounding box intersects
        /// <paramref name="boundingBox"/>.
        /// </summary>
        /// <param name="boundingBox">The bounding box used to intersect.</param>
        /// <returns></returns>
        public IEnumerable<uint> GetIntersectingObjectIds(BoundingBox boundingBox)
        {
            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string boundingBoxClause = getBoundingBoxSql(boundingBox, Srid);

                String sql = String.Format("SELECT {0} FROM {1} WHERE ", ObjectIdColumn, Table);

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    sql += DefinitionQuery + " AND ";
                }

                sql += GeometryColumn + " && " + boundingBoxClause;

                using (PgCommand command = new PgCommand(sql, conn))
                {
                    conn.Open();

                    using (PgDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                yield return (uint)(int)reader[0];
                            }
                        }
                    }

                    conn.Close();
                }
            }
        }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<uint> oids)
        {
            throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable<uint> table, SchemaMergeAction schemaAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the data source.
        /// </summary>
        public void Open()
        {
            checkDisposed();
            obtainSpatialReference();
            _isOpen = true;
        }

        /// <summary>
        /// Closes the data source.
        /// </summary>
        public void Close()
        {
            checkDisposed();
            _isOpen = false;
        }

        /// <summary>
        /// Computes a bounding box which covers all geometries in <see cref="Table"/>.
        /// </summary>
        /// <returns>
        /// The bounding box which describes the maximum extents 
        /// of the data retrieved by the data source.
        /// </returns>
        public BoundingBox GetExtents()
        {
            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string sql = String.Format("SELECT EXTENT({0}) FROM {1}",
                                           GeometryColumn,
                                           Table);

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    sql += " WHERE " + DefinitionQuery;
                }


                sql += ";";

                using (PgCommand command = new PgCommand(sql, conn))
                {
                    conn.Open();

                    BoundingBox bbox;

                    try
                    {
                        PgBox2D result = (PgBox2D) command.ExecuteScalar();
                        bbox =
                            new BoundingBox(result.LowerLeft.X, result.LowerLeft.Y, result.UpperRight.X,
                                            result.UpperRight.Y);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Box2d couldn't fetched from table. ", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return bbox;
                }
            }
        }

        /// <summary>
        /// Returns a datarow based on an object id.
        /// </summary>
        /// <param name="oid">The id of the feature to retrieve.</param>
        /// <returns>
        /// A FeatureDataRow which has the feature geometry and attributes.
        /// </returns>
        public FeatureDataRow<uint> GetFeature(uint oid)
        {
            string sql = String.Format("SELECT *, AsBinary({0}) as {1} FROM {2} WHERE {3} = '{4}'",
                                       GeometryColumn, RetrievedGeometryColumnName, Table, ObjectIdColumn, oid);

            using (PgConnection conn = new PgConnection(_connectionString))
            {
                using (PgCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    conn.Open();

                    using (PgDataReader reader = cmd.ExecuteReader())
                    {
                        FeatureDataTable<uint> fdt = new FeatureDataTable<uint>(Table, ObjectIdColumn);

                        DataTable schemaTable = reader.GetSchemaTable();

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string columnName = row["ColumnName"] as string;
                            if (String.Compare(columnName, GeometryColumn, StringComparison.CurrentCultureIgnoreCase) ==
                                0
                                && columnName != RetrievedGeometryColumnName)
                            {
                                fdt.Columns.Add(columnName, row["DataType"] as Type);
                            }
                        }

                        while (reader.Read())
                        {
                            FeatureDataRow<uint> fdr = fdt.NewRow((uint) reader[ObjectIdColumn]);

                            foreach (DataColumn col in fdt.Columns)
                            {
                                if (
                                    String.Compare(col.ColumnName, GeometryColumn,
                                                   StringComparison.CurrentCultureIgnoreCase) == 0
                                    && col.ColumnName != RetrievedGeometryColumnName)
                                {
                                    fdr[col.ColumnName] = reader[col.ColumnName];
                                }
                            }

                            fdr.Geometry = GeometryFromWkb.Parse((byte[]) reader[RetrievedGeometryColumnName]);
                            return fdr;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the number of features in the dataset
        /// </summary>
        /// <returns>number of features</returns>
        public int GetFeatureCount()
        {
            int count;

            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string sql = "SELECT COUNT(*) FROM " + Table;

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    sql += " WHERE " + DefinitionQuery;
                }

                using (PgCommand command = new PgCommand(sql, conn))
                {
                    conn.Open();
                    count = (int) command.ExecuteScalar();
                    conn.Close();
                }
            }
            return count;
        }

        /// <summary>
        /// Returns an enumeration of Geometry objects which
        /// intersect the specified bounding box.
        /// </summary>
        /// <param name="boundingBox">
        /// The region to compute intersection with.
        /// </param>
        /// <returns>
        /// A set of geometries which are at least partially 
        /// contained within <paramref name="boundingBox"/>.
        /// </returns>
        public IEnumerable<Geometry> GetGeometriesInView(BoundingBox boundingBox)
        {
            Collection<Geometry> features = new Collection<Geometry>();
            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string strBbox = getBoundingBoxSql(boundingBox, Srid);

                String strSql = String.Format("SELECT AsBinary({0}) as geom FROM {1} WHERE ",
                                              GeometryColumn,
                                              Table);

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    strSql += DefinitionQuery + " AND ";
                }

                strSql += String.Format("{0} && {1}", GeometryColumn, strBbox);

                using (PgCommand command = new PgCommand(strSql, conn))
                {
                    conn.Open();
                    using (PgDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //object obj = dr[0];
                            Geometry geom = null;

                            //if (typeof(PgPoint) == obj.GetType())
                            //    geom = new SharpMap.Geometries.Point(((PgPoint)obj).X, ((PgPoint)obj).Y);
                            //else 
                            if (dr[0] != DBNull.Value)
                            {
                                geom = GeometryFromWkb.Parse((byte[]) dr[0]);
                            }

                            if (geom != null)
                            {
                                features.Add(geom);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return features;
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        public Geometry GetGeometryById(uint oid)
        {
            Geometry geom = null;

            using (PgConnection conn = new PgConnection(_connectionString))
            {
                String strSql = String.Format("SELECT AsBinary({0}) As Geom FROM {1} WHERE {2} = '{3}'",
                                              GeometryColumn,
                                              Table,
                                              ObjectIdColumn,
                                              oid);

                conn.Open();

                using (PgCommand command = new PgCommand(strSql, conn))
                {
                    using (PgDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            object obj = dr[0];

                            if (obj is PgPoint)
                            {
                                PgPoint point = (PgPoint) obj;
                                geom = new Point(point.X, point.Y);
                            }
                            else if (obj != DBNull.Value)
                            {
                                geom = GeometryFromWkb.Parse((byte[]) dr[0]);
                            }
                        }
                    }
                }

                conn.Close();
            }

            return geom;
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all features with the given view bounds.
        /// </summary>
        /// <param name="bounds">
        /// The bounds of the view to query for intersection.
        /// </param>
        /// <param name="ds">
        /// FeatureDataSet to fill data into.
        /// </param>
        public void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet ds)
        {
            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string strBbox = getBoundingBoxSql(bounds, Srid);

                string sql = String.Format("SELECT *, AsBinary({0}) AS {1} FROM {2} WHERE ",
                                           GeometryColumn, RetrievedGeometryColumnName, Table);

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    sql += DefinitionQuery + " AND ";
                }

                sql += GeometryColumn + " && " + strBbox;

                using (PgDataAdapter adapter = new PgDataAdapter(sql, conn))
                {
                    conn.Open();
                    DataSet ds2 = new DataSet();
                    adapter.Fill(ds2);
                    conn.Close();

                    if (ds2.Tables.Count > 0)
                    {
                        FeatureDataTable fdt = new FeatureDataTable(ds2.Tables[0]);

                        foreach (DataColumn col in ds2.Tables[0].Columns)
                        {
                            if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
                            {
                                fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
                            }
                        }

                        foreach (DataRow dr in ds2.Tables[0].Rows)
                        {
                            FeatureDataRow fdr = fdt.NewRow();

                            foreach (DataColumn col in ds2.Tables[0].Columns)
                            {
                                if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
                                {
                                    fdr[col.ColumnName] = dr[col];
                                }
                            }

                            fdr.Geometry = GeometryFromWkb.Parse((byte[]) dr[RetrievedGeometryColumnName]);
                            fdt.AddRow(fdr);
                        }

                        ds.Tables.Add(fdt);
                    }
                }
            }
        }

        public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the features that intersects with 'geom'
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
        {
            //List<Geometries.Geometry> features = new List<SharpMap.Geometries.Geometry>();
            using (PgConnection conn = new PgConnection(_connectionString))
            {
                string strGeom = "GeomFromText('" + geom.AsText() + "')";

                if (Srid > 0)
                {
                    strGeom = "setSrid(" + strGeom + "," + Srid + ")";
                }

                string sql = "SELECT * , AsBinary(" + GeometryColumn + ") As " + RetrievedGeometryColumnName + " FROM " +
                             Table + " WHERE ";

                if (!String.IsNullOrEmpty(_defintionQuery))
                {
                    sql += DefinitionQuery + " AND ";
                }

                sql += GeometryColumn + " && " + strGeom + " AND distance(" + GeometryColumn + ", " + strGeom + ") < 0";

                using (PgDataAdapter adapter = new PgDataAdapter(sql, conn))
                {
                    conn.Open();
                    adapter.Fill(ds);
                    conn.Close();
                    if (ds.Tables.Count > 0)
                    {
                        FeatureDataTable fdt = new FeatureDataTable(ds.Tables[0]);
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
                            {
                                fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
                            }
                        }
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            FeatureDataRow fdr = fdt.NewRow();
                            foreach (DataColumn col in ds.Tables[0].Columns)
                            {
                                if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
                                {
                                    fdr[col.ColumnName] = dr[col];
                                }
                            }
                            fdr.Geometry = GeometryFromWkb.Parse((byte[]) dr[RetrievedGeometryColumnName]);
                            fdt.AddRow(fdr);
                        }
                        ds.Tables.Add(fdt);
                    }
                }
            }
        }

        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable<uint> table)
        {
            throw new NotImplementedException();
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

        public IAsyncResult BeginExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet dataSet,
                                                          AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet dataSet,
                                                          QueryExecutionOptions options, AsyncCallback callback,
                                                          object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(BoundingBox bounds, FeatureDataTable table,
                                                          AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(BoundingBox bounds, FeatureDataTable table,
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
            throw new NotImplementedException();
        }

        public void EndExecuteFeatureQuery(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureDataRecord> EndGetFeatures(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureSpatialExpression query)
        {
            throw new NotImplementedException();
        }

        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Geometry> ExecuteGeometryIntersectionQuery(BoundingBox bounds)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox bounds, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet dataSet, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataTable table, QueryExecutionOptions options)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable oids)
        {
            throw new NotImplementedException();
        }

        public CultureInfo Locale
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Queries the PostGIS database to get the name of the Geometry Column. This is used if the columnname isn't specified in the constructor
        /// </summary>
        /// <remarks></remarks>
        /// <returns>Name of column containing geometry</returns>
        private string getGeometryColumn()
        {
            string strSQL = "select f_geometry_column from geometry_columns WHERE f_table_name = @Table'";

            using (PgConnection conn = new PgConnection(_connectionString))
            {
                using (PgCommand command = new PgCommand(strSQL, conn))
                {
                    conn.Open();

                    command.Parameters.Add(new PgParameter("@Table", PgDbType.VarChar));
                    command.Parameters[0].Value = _table;

                    object columnname = command.ExecuteScalar();
                    conn.Close();

                    if (columnname == DBNull.Value)
                    {
                        throw new ApplicationException("Table '" + Table + "' does not contain a geometry column");
                    }
                    return (string)columnname;
                }
            }
        }

        private void checkDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        private void obtainSpatialReference()
        {
            // Get the Srid for the table
            string sql = "SELECT srid FROM geometry_columns WHERE f_table_name = @Table";

            using (PgConnection conn = new PgConnection(_connectionString))
            {
                using (PgCommand command = new PgCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();

                        command.Parameters.Add(new PgParameter("@Table", PgDbType.VarChar));
                        command.Parameters[0].Value = Table;

                        _srid = (int) command.ExecuteScalar();
                    }
                    catch (PgException) {}
                }
            }
        }

        #region * Sql builder methods *

        /// <summary>
        /// Creates a SQL statement for a bounding box clause.
        /// </summary>
        /// <param name="bbox">The bounding box to create the statement for.</param>
        /// <param name="srid">Spatial Reference Id of the bounding box.</param>
        /// <returns>String</returns>
        private static string getBoundingBoxSql(BoundingBox bbox, int? srid)
        {
            string bboxString = String.Format("box2d('BOX3D({0} {1}, {2} {3})'::box3d)",
                                              bbox.Left.ToString(NumberFormat_enUS),
                                              bbox.Bottom.ToString(NumberFormat_enUS),
                                              bbox.Right.ToString(NumberFormat_enUS),
                                              bbox.Top.ToString(NumberFormat_enUS));

            if (srid != null)
            {
                bboxString = String.Format(NumberFormat_enUS, "SetSrid({0}, {1})", bboxString, srid);
            }

            return bboxString;
        }

        #endregion

        #endregion

        #region IFeatureLayerProvider<uint> Explicit Members
        /// <summary>
        /// Returns all objects whose <see cref="BoundingBox"/> 
        /// intersects <paramref name="boundingBox"/>.
        /// </summary>
        /// <remarks>
        /// This method is usually much faster than the ExecuteIntersectionQuery method, 
        /// because intersection tests are performed on objects simplifed by 
        /// their <see cref="BoundingBox"/>, often using
        /// spatial indexing to retrieve the id values.
        /// </remarks>
        /// <param name="boundingBox">BoundingBox that objects should intersect.</param>
        /// <returns>An enumeration of all intersecting objects' ids.</returns>
        [Obsolete]
        IEnumerable<uint> IFeatureLayerProvider<uint>.GetObjectIdsInView(BoundingBox boundingBox)
        {
            return GetIntersectingObjectIds(boundingBox);
        }
        #endregion
    }
}