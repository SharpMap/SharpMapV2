// Copyright 2006 - Humberto Ferreira
// Oracle provider by Humberto Ferreira (humbertojdf@hotmail.com)
//
// Date 2006-09-05
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
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using Oracle.DataAccess.Client;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;

namespace SharpMap.Extensions.Data.Providers.Oracle
{
	/// <summary>
	/// Oracle dataprovider
	/// </summary>
	/// <remarks>
	/// <para>
	/// This provider needs the Oracle software client installed 
	/// on the PC where the application runs.
	/// If you need to connect to an Oracle database, it has to have an
	/// Oracle client (or Oracle Instant Client) installed.
	/// </para>
	/// <para>
	/// You can download Oracle Client here:
	/// http://www.oracle.com/technology/software/index.html
	/// </para>
	/// <para>
	/// If the client doesn't need an instance of Oracle, 
	/// a better option is to use Oracle Instant client, found here:
	/// http://www.oracle.com/technology/tech/oci/instantclient/index.html
	/// </para>
	/// <example>
	/// Adding a datasource to a layer:
	/// <code lang="C#">
	///	string connStr = "Server=127.0.0.1;Port=5432;User Id=userid;Password=password;Database=myGisDb;";
	/// SharpMap.Extensions.Data.Providers.Oracle.OracleSpatialProvider dataSource = new OracleSpatialProvider(
	///		connStr, "myTable", "GeomColumn", "OidColumn");
	/// SharpMap.Layers.VectorLayer myLayer = new VectorLayer("My layer", dataSource);
	/// </code>
	/// </example>
	/// <para>
	/// SharpMap Oracle provider originally by Humberto Ferreira (humbertojdf at hotmail com).
	/// </para>
	/// </remarks>
	[Serializable]
	public class OracleSpatialProvider : IVectorLayerProvider<uint>
	{
		private const string RetrievedGeometryColumnName = "sharpmap_tempgeometry";

		private static readonly NumberFormatInfo NumberFormat_enUS
			= new CultureInfo("en-US", false).NumberFormat;

		private string _defintionQuery;
		private string _objectIdColumn;
		private string _geometryColumn;
		private string _table;
		private bool _isOpen;
		private string _connectionString;
		private bool _isDisposed = false;
		private int? _srid = null;

		#region Object construction and disposal

		#region Constructors

		/// <summary>
		/// Initializes a new connection to Oracle
		/// </summary>
		/// <param name="connectionString">Connectionstring</param>
		/// <param name="tablename">Name of data table</param>
		/// <param name="geometryColumnName">Name of geometry column</param>
		/// /// <param name="oidColumnName">Name of column with unique identifier</param>
		public OracleSpatialProvider(string connectionString, string tablename, string geometryColumnName,
									 string oidColumnName)
		{
			ConnectionString = connectionString;
			Table = tablename;
			GeometryColumn = geometryColumnName;
			ObjectIdColumn = oidColumnName;
		}

		/// <summary>
		/// Initializes a new connection to Oracle
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="password">Password</param>
		/// <param name="datasource">Datasoure</param>
		/// <param name="tablename">Tablename</param>
		/// <param name="geometryColumnName">Geometry column name</param>
		/// <param name="oidColumnName">Object ID column</param>
		public OracleSpatialProvider(string username, string password, string datasource, string tablename,
									 string geometryColumnName, string oidColumnName)
			: this(String.Format("User Id={0};Password={1};Data Source={2}", username, password, datasource),
				   tablename, geometryColumnName, oidColumnName)
		{
		}


		/// <summary>
		/// Initializes a new connection to Oracle
		/// </summary>
		/// <param name="connectionString">Connectionstring</param>
		/// <param name="tablename">Name of data table</param>
		/// <param name="oidColumnName">Name of column with unique identifier</param>
		public OracleSpatialProvider(string connectionString, string tablename, string oidColumnName)
			: this(connectionString, tablename, "", oidColumnName)
		{
			GeometryColumn = GetGeometryColumn();
		}

		#endregion

		#region Disposers and finalizers

		/// <summary>
		/// Finalizer
		/// </summary>
		~OracleSpatialProvider()
		{
			Dispose(false);
		}

		#region IDisposable Memebers

		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
			}
		}

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		#endregion

		#endregion

		/// <summary>
		/// Connectionstring
		/// </summary>
		public string ConnectionString
		{
			get { return _connectionString; }
			set { _connectionString = value; }
		}

		/// <summary>
		/// Definition query used for limiting dataset
		/// </summary>
		public string DefinitionQuery
		{
			get { return _defintionQuery; }
			set { _defintionQuery = value; }
		}

		/// <summary>
		/// Gets the connection id of the datasource
		/// </summary>
		public string ConnectionId
		{
			get { return _connectionString; }
		}

		public ICoordinateTransformation CoordinateTransformation
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Name of geometry column
		/// </summary>
		public string GeometryColumn
		{
			get { return _geometryColumn; }
			set { _geometryColumn = value; }
		}

		/// <summary>
		/// Returns true if the datasource is currently open
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen; }
		}

		/// <summary>
		/// Name of column that contains the Object ID
		/// </summary>
		public string ObjectIdColumn
		{
			get { return _objectIdColumn; }
			set { _objectIdColumn = value; }
		}

		public ICoordinateSystem SpatialReference
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the id of a well-known spatial reference system.
		/// </summary>
		public int? Srid
		{
			get
			{
				if (_srid == null)
				{
					string sql = "SELECT SRID FROM USER_SDO_GEOM_METADATA WHERE TABLE_NAME='" + Table + "'";

					using (OracleConnection conn = new OracleConnection(_connectionString))
					{
						using (OracleCommand command = new OracleCommand(sql, conn))
						{
							try
							{
								conn.Open();
								_srid = (int)(decimal)command.ExecuteScalar();
								conn.Close();
							}
							catch (OracleException) { }
						}
					}
				}

				return _srid;
			}
			set { throw new NotSupportedException("Spatial Reference ID cannot be set on an Oracle table"); }
		}

		/// <summary>
		/// Data table name
		/// </summary>
		public string Table
		{
			get { return _table; }
			set { _table = value; }
		}

		/// <summary>
		/// Closes the datasource
		/// </summary>
		public void Close()
		{
			//Don't really do anything. Oracle's connection pooling takes over here.
			_isOpen = false;
		}

		public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
		{
			throw new NotImplementedException();
		}

		public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns all features with the view box
		/// </summary>
		/// <param name="bbox">view box</param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(BoundingBox bbox, FeatureDataSet ds)
		{
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				//Get bounding box string
				string strBbox = getBoxFilterClause(bbox);

				string sql = "SELECT g.*, g." + GeometryColumn + ".Get_WKB() AS " + RetrievedGeometryColumnName
							 + "FROM " + Table + " g WHERE ";

				if (!String.IsNullOrEmpty(_defintionQuery))
				{
					sql += DefinitionQuery + " AND ";
				}

				sql += strBbox;

				using (OracleDataAdapter adapter = new OracleDataAdapter(sql, conn))
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

							fdr.Geometry = GeometryFromWkb.Parse((byte[])dr[RetrievedGeometryColumnName]);
							fdt.AddRow(fdr);
						}
						ds.Tables.Add(fdt);
					}
				}
			}
		}

		public IFeatureDataReader ExecuteIntersectionQuery(Geometry geom)
		{
			throw new NotImplementedException();
		}

		public void ExecuteIntersectionQuery(Geometry geom, FeatureDataTable table)
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
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				string strGeom = "MDSYS.SDO_GEOMETRY('" + geom.AsText() + "', #SRID#)";

				if (Srid != null)
				{
					strGeom = strGeom.Replace("#SRID#", Srid.Value.ToString(NumberFormat_enUS));
				}
				else
				{
					strGeom = strGeom.Replace("#SRID#", "NULL");
				}

				strGeom = "SDO_RELATE(g." + GeometryColumn + ", " + strGeom + ", 'mask=ANYINTERACT querytype=WINDOW') = 'TRUE'";

				string strSQL = "SELECT g.*, g." + GeometryColumn + ").Get_WKB() As " + RetrievedGeometryColumnName
								+ " FROM " + Table + " g WHERE ";

				if (!String.IsNullOrEmpty(_defintionQuery))
					strSQL += DefinitionQuery + " AND ";

				strSQL += strGeom;

				using (OracleDataAdapter adapter = new OracleDataAdapter(strSQL, conn))
				{
					conn.Open();
					adapter.Fill(ds);
					conn.Close();
					if (ds.Tables.Count > 0)
					{
						FeatureDataTable fdt = new FeatureDataTable(ds.Tables[0]);
						foreach (DataColumn col in ds.Tables[0].Columns)
							if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
								fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
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

							fdr.Geometry = GeometryFromWkb.Parse((byte[])dr[RetrievedGeometryColumnName]);
							fdt.AddRow(fdr);
						}
						ds.Tables.Add(fdt);
					}
				}
			}
		}

		/// <summary>
		/// Computes the full extents of the data which this data source represents.
		/// </summary>
		/// <returns>A BoundingBox fully containing all the available features.</returns>
		public BoundingBox GetExtents()
		{
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				string sql = "SELECT SDO_AGGR_MBR(g." + GeometryColumn + ").Get_WKT() FROM " + Table + " g ";

				if (!String.IsNullOrEmpty(_defintionQuery))
				{
					sql += " WHERE " + DefinitionQuery;
				}

				using (OracleCommand command = new OracleCommand(sql, conn))
				{
					conn.Open();
					object result = command.ExecuteScalar();
					conn.Close();

					if (result == DBNull.Value)
					{
						return BoundingBox.Empty;
					}

					string boxClause = (string)result;

					if (boxClause.StartsWith("POLYGON", StringComparison.InvariantCultureIgnoreCase))
					{
						boxClause = boxClause.Replace("POLYGON", "");
						boxClause = boxClause.Trim();
						boxClause = boxClause.Replace("(", "");
						boxClause = boxClause.Replace(")", "");

						List<double> xX = new List<double>();
						List<double> yY = new List<double>();

						String[] points = boxClause.Split(',');
						string point;

						foreach (string s in points)
						{
							String[] nums;
							point = s.Trim();
							nums = point.Split(' ');
							xX.Add(double.Parse(nums[0], NumberFormat_enUS));
							yY.Add(double.Parse(nums[1], NumberFormat_enUS));
						}

						double minX = Double.MaxValue;
						double minY = Double.MaxValue;
						double maxX = Double.MinValue;
						double maxY = Double.MinValue;

						foreach (double d in xX)
						{
							if (d > maxX)
							{
								maxX = d;
							}
							if (d < minX)
							{
								minX = d;
							}
						}

						foreach (double d in yY)
						{
							if (d > maxY)
							{
								maxY = d;
							}
							if (d < minY)
							{
								minY = d;
							}
						}

						return new BoundingBox(minX, minY, maxX, maxY);
					}
					else
					{
						return BoundingBox.Empty;
					}
				}
			}
		}

		/// <summary>
		/// Returns a feature based on an object id.
		/// </summary>
		/// <param name="oid"></param>
		/// <returns>datarow</returns>
		public FeatureDataRow<uint> GetFeature(uint oid)
		{
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				string sql = "SELECT g.* , g." + GeometryColumn + ").Get_WKB() As " + RetrievedGeometryColumnName
							 + " FROM " + Table + " g WHERE " + ObjectIdColumn + "='" + oid + "'";

				using (OracleDataAdapter adapter = new OracleDataAdapter(sql, conn))
				{
					FeatureDataSet ds = new FeatureDataSet();
					conn.Open();
					adapter.Fill(ds);
					conn.Close();

					if (ds.Tables.Count > 0)
					{
						FeatureDataTable<uint> fdt = new FeatureDataTable<uint>(ds.Tables[0], ObjectIdColumn);

						foreach (DataColumn col in ds.Tables[0].Columns)
						{
							if (col.ColumnName != GeometryColumn && col.ColumnName != RetrievedGeometryColumnName)
							{
								fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
							}
						}

						if (ds.Tables[0].Rows.Count > 0)
						{
							DataRow dr = ds.Tables[0].Rows[0];
							FeatureDataRow<uint> fdr = fdt.NewRow((uint)dr[ObjectIdColumn]);

							foreach (DataColumn col in ds.Tables[0].Columns)
							{
								if (col.ColumnName != GeometryColumn 
									&& col.ColumnName != RetrievedGeometryColumnName
									&& col.ColumnName != ObjectIdColumn)
								{
									fdr[col.ColumnName] = dr[col];
								}
							}

							fdr.Geometry = GeometryFromWkb.Parse((byte[])dr[RetrievedGeometryColumnName]);
							return fdr;
						}
						else
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Returns the number of features in the dataset
		/// </summary>
		/// <returns>number of features</returns>
		public int GetFeatureCount()
		{
			int count;
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				string strSQL = "SELECT COUNT(*) FROM " + Table;
				if (!String.IsNullOrEmpty(_defintionQuery))
					strSQL += " WHERE " + DefinitionQuery;
				using (OracleCommand command = new OracleCommand(strSQL, conn))
				{
					conn.Open();
					count = (int)command.ExecuteScalar();
					conn.Close();
				}
			}
			return count;
		}

		/// <summary>
		/// Returns geometries within the specified bounding box
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox bbox)
		{
			Collection<Geometry> features = new Collection<Geometry>();
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				//Get bounding box string
				string strBbox = getBoxFilterClause(bbox);

				//string strSQL = "SELECT AsBinary(" + this.GeometryColumn + ") AS Geom ";
				string sql = "SELECT g." + GeometryColumn + ".Get_WKB() ";
				sql += " FROM " + Table + " g WHERE ";

				if (!String.IsNullOrEmpty(_defintionQuery))
				{
					sql += DefinitionQuery + " AND ";
				}

				sql += strBbox;

				using (OracleCommand command = new OracleCommand(sql, conn))
				{
					conn.Open();
					using (OracleDataReader dr = command.ExecuteReader())
					{
						while (dr.Read())
						{
							if (dr[0] != DBNull.Value)
							{
								Geometry geom = GeometryFromWkb.Parse((byte[])dr[0]);
								if (geom != null)
								{
									features.Add(geom);
								}
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
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				string sql = "SELECT g." + GeometryColumn + ".Get_WKB() FROM " + Table
							 + " g WHERE " + ObjectIdColumn + "='" + oid + "'";

				conn.Open();

				using (OracleCommand command = new OracleCommand(sql, conn))
				{
					using (OracleDataReader dr = command.ExecuteReader())
					{
						while (dr.Read())
						{
							if (dr[0] != DBNull.Value)
							{
								geom = GeometryFromWkb.Parse((byte[])dr[0]);
							}
						}
					}
				}
				conn.Close();
			}

			return geom;
		}

		/// <summary>
		/// Queries the Oracle database to get the name of the Geometry Column. This is used if the columnname isn't specified in the constructor
		/// </summary>
		/// <remarks></remarks>
		/// <returns>Name of column containing geometry</returns>
		private string GetGeometryColumn()
		{
			string strSQL = "select COLUMN_NAME from USER_SDO_GEOM_METADATA WHERE TABLE_NAME='" + Table + "'";
			using (OracleConnection conn = new OracleConnection(_connectionString))
			using (OracleCommand command = new OracleCommand(strSQL, conn))
			{
				conn.Open();
				object columnname = command.ExecuteScalar();
				conn.Close();
				if (columnname == DBNull.Value)
					throw new ApplicationException("Table '" + Table + "' does not contain a geometry column");
				return (string)columnname;
			}
		}

		/// <summary>
		/// Returns geometry Object IDs whose bounding box intersects 'bbox'
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<uint> GetObjectIdsInView(BoundingBox bbox)
		{
			List<uint> objectlist = new List<uint>();
			using (OracleConnection conn = new OracleConnection(_connectionString))
			{
				//Get bounding box string
				string boundsClause = getBoxFilterClause(bbox);

				string sql = "SELECT g." + ObjectIdColumn + " ";
				sql += "FROM " + Table + " g WHERE ";

				if (!String.IsNullOrEmpty(_defintionQuery))
				{
					sql += DefinitionQuery + " AND ";
				}

				sql += boundsClause;

				using (OracleCommand command = new OracleCommand(sql, conn))
				{
					conn.Open();
					using (OracleDataReader dr = command.ExecuteReader())
					{
						while (dr.Read())
						{
							if (dr[0] != DBNull.Value)
							{
								uint id = (uint)(decimal)dr[0];
								objectlist.Add(id);
							}
						}
					}
					conn.Close();
				}
			}
			return objectlist;
		}

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Opens the datasource
		/// </summary>
		public void Open()
		{
			//Don't really do anything. Oracle's connection pooling takes over here.
			_isOpen = true;
		}

		public void SetTableSchema(FeatureDataTable<uint> table)
		{
			throw new NotImplementedException();
		}

		public void SetTableSchema(FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		#region Private helper methods

		/// <summary>
		/// Returns the box filter string needed in SQL query
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		private string getBoxFilterClause(BoundingBox bbox)
		{
			string strBbox = "SDO_FILTER(g." + GeometryColumn + ", mdsys.sdo_geometry(2003,#SRID#,NULL," +
							 "mdsys.sdo_elem_info_array(1,1003,3)," +
							 "mdsys.sdo_ordinate_array(" +
							 bbox.Min.X.ToString(NumberFormat_enUS) + ", " +
							 bbox.Min.Y.ToString(NumberFormat_enUS) + ", " +
							 bbox.Max.X.ToString(NumberFormat_enUS) + ", " +
							 bbox.Max.Y.ToString(NumberFormat_enUS) + ")), " +
							 "'querytype=window') = 'TRUE'";

			if (Srid != null)
			{
				strBbox = strBbox.Replace("#SRID#", Srid.Value.ToString(NumberFormat_enUS));
			}
			else
			{
				strBbox = strBbox.Replace("#SRID#", "NULL");
			}

			return strBbox;
		}
		#endregion
	}
}