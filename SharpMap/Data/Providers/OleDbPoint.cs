// Copyright 2006 - Morten Nielsen (www.iter.dk)
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
using System.Text;
using System.Data.OleDb;

using SharpMap.Geometries;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.CoordinateSystems;

namespace SharpMap.Data.Providers
{
	/// <summary>
	/// The OleDbPoint provider is used for rendering point data from an OleDb compatible datasource.
	/// </summary>
	/// <remarks>
	/// <para>The data source will need to have two double-type columns, xColumn and yColumn that contains the coordinates of the point,
	/// and an integer-type column containing a unique identifier for each row.</para>
	/// <para>To get good performance, make sure you have applied indexes on ID, xColumn and yColumns in your datasource table.</para>
	/// </remarks>
	public class OleDbPoint : IProvider<uint>, IDisposable
	{
		private ICoordinateSystem _coordinateSystem;
		private ICoordinateTransformation _coordinateTransformation;
		private int _srid = -1;
		private string _defintionQuery;
		private string _connectionString;
		private string _yColumn;
		private string _xColumn;
		private string _objectIdColumn;
		private string _table;
        private bool _isOpen;
        private bool _disposed = false;

        #region Object Construction/Destruction
        /// <summary>
		/// Initializes a new instance of the OleDbPoint provider
		/// </summary>
		/// <param name="ConnectionStr"></param>
		/// <param name="tablename"></param>
		/// <param name="OID_ColumnName"></param>
		/// <param name="xColumn"></param>
		/// <param name="yColumn"></param>
		public OleDbPoint(string connectionString, string tableName, string oidColumnName, string xColumn, string yColumn)
		{
			this.Table = tableName;
			this.XColumn = xColumn;
			this.YColumn = yColumn;
			this.ObjectIdColumn = oidColumnName;
			this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~OleDbPoint()
        {
            Dispose();
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the OleDbPoint provider, and releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            Disposed = true;
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {

        }

        protected bool Disposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }
        #endregion
        #endregion

        /// <summary>
		/// Data table name
		/// </summary>
		public string Table
		{
			get { return _table; }
			set { _table = value; }
		}

		/// <summary>
		/// Name of column that contains the Object ID
		/// </summary>
		public string ObjectIdColumn
		{
			get { return _objectIdColumn; }
			set { _objectIdColumn = value; }
		}

		/// <summary>
		/// Name of column that contains X coordinate
		/// </summary>
		public string XColumn
		{
			get { return _xColumn; }
			set { _xColumn = value; }
		}

		/// <summary>
		/// Name of column that contains Y coordinate
		/// </summary>
		public string YColumn
		{
			get { return _yColumn; }
			set { _yColumn = value; }
		}

		/// <summary>
		/// Connectionstring
		/// </summary>
		public string ConnectionString
		{
			get { return _connectionString; }
			set { _connectionString = value;}
		}

		#region IProvider Members

		/// <summary>
		/// Returns geometries within the specified bounding box
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox bbox)
		{
			using(OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select " + this.XColumn + ", " + this.YColumn + " FROM " + this.Table + " WHERE ";
				
				if (_defintionQuery != null && _defintionQuery != "")
				{
					strSQL += _defintionQuery + " AND ";
				}

				//Limit to the points within the boundingbox
                strSQL += this.XColumn + " BETWEEN " + bbox.Left.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Right.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " +
                    this.YColumn + " BETWEEN " + bbox.Bottom.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Top.ToString(SharpMap.Map.NumberFormat_EnUS);
				
				using (OleDbCommand command = new OleDbCommand(strSQL, conn))
				{
					conn.Open();

					using (OleDbDataReader dr = command.ExecuteReader())
					{
						while (dr.Read())
						{
							if (dr[0] != DBNull.Value && dr[1] != DBNull.Value)
							{
								yield return new Point((double)dr[0], (double)dr[1]);
							}
						}
					}

					conn.Close();
				}
			}
		}

		public ICoordinateSystem CoordinateSystem
		{
			get { return _coordinateSystem; }
		}

		public ICoordinateTransformation CoordinateTransformation
		{
			get { return _coordinateTransformation; }
			set { _coordinateTransformation = value; }
		}

		/// <summary>
		/// Returns geometry Object IDs whose bounding box intersects 'bbox'
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
        public IEnumerable<uint> GetObjectIdsInView(BoundingBox bbox)
		{
			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select " + this.ObjectIdColumn + " FROM " + this.Table + " WHERE ";

				if (_defintionQuery != null && _defintionQuery != "")
				{
					strSQL += _defintionQuery + " AND ";
				}

				//Limit to the points within the boundingbox
                strSQL += this.XColumn + " BETWEEN " + bbox.Left.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Right.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + this.YColumn +
                    " BETWEEN " + bbox.Bottom.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Top.ToString(SharpMap.Map.NumberFormat_EnUS);

				using (OleDbCommand command = new OleDbCommand(strSQL, conn))
				{
					conn.Open();

					using (OleDbDataReader dr = command.ExecuteReader())
					{
						while (dr.Read())
						{
							if (!dr.IsDBNull(0))
							{
								yield return (uint)dr.GetInt32(0);
							}
						}
					}

					conn.Close();
				}
			}
		}

		/// <summary>
		/// Returns the geometry corresponding to the Object ID
		/// </summary>
		/// <param name="oid">Object ID</param>
		/// <returns>geometry</returns>
		public Geometry GetGeometryById(uint oid)
		{			
			Geometry geom = null;

			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select " + this.XColumn + ", " + this.YColumn + " FROM " + this.Table + " WHERE " + this.ObjectIdColumn + "=" + oid.ToString();
				
				using (OleDbCommand command = new OleDbCommand(strSQL, conn))
				{
					conn.Open();
					
					using (OleDbDataReader dr = command.ExecuteReader())
					{
						if(dr.Read())
						{
							//If the read row is OK, create a point geometry from the XColumn and YColumn and return it
							if (dr[0] != DBNull.Value && dr[1] != DBNull.Value)
							{
								geom = new Point((double)dr[0], (double)dr[1]);
							}
						}
					}

					conn.Close();
				}				
			}

			return geom;
		}

		/// <summary>
		/// Throws NotSupportedException. 
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
		{
			throw new NotSupportedException("ExecuteIntersectionQuery(Geometry) is not supported by the OleDbPointProvider.");
			//When relation model has been implemented the following will complete the query
			/*
			ExecuteIntersectionQuery(geom.GetBoundingBox(), ds);
			if (ds.Tables.Count > 0)
			{
				for(int i=ds.Tables[0].Count-1;i>=0;i--)
				{
					if (!geom.Intersects(ds.Tables[0][i].Geometry))
						ds.Tables.RemoveAt(i);
				}
			}
			*/
		}

		/// <summary>
		/// Returns all features with the given bounding box.
		/// </summary>
		/// <param name="bbox">BoundingBox to query within.</param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(BoundingBox bbox, FeatureDataSet ds)
		{
			List<Geometry> features = new List<Geometry>();
			
			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select * FROM " + this.Table + " WHERE ";
				if (_defintionQuery != null && _defintionQuery != "") //If a definition query has been specified, add this as a filter on the query
					strSQL += _defintionQuery + " AND ";
				//Limit to the points within the boundingbox
                strSQL += this.XColumn + " BETWEEN " + bbox.Left.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Right.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + this.YColumn +
                    " BETWEEN " + bbox.Bottom.ToString(SharpMap.Map.NumberFormat_EnUS) + " AND " + bbox.Top.ToString(SharpMap.Map.NumberFormat_EnUS);

				using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn))
				{
					conn.Open();
					System.Data.DataSet ds2 = new System.Data.DataSet();
					adapter.Fill(ds2);
					conn.Close();

					if (ds2.Tables.Count > 0)
					{
						FeatureDataTable fdt = new FeatureDataTable(ds2.Tables[0]);

						foreach (System.Data.DataColumn col in ds2.Tables[0].Columns)
						{
							fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
						}

						foreach (System.Data.DataRow dr in ds2.Tables[0].Rows)
						{
							FeatureDataRow fdr = fdt.NewRow();

							foreach (System.Data.DataColumn col in ds2.Tables[0].Columns)
							{
								fdr[col.ColumnName] = dr[col];
							}

							if (dr[this.XColumn] != DBNull.Value && dr[this.YColumn] != DBNull.Value)
							{
								fdr.Geometry = new Point((double)dr[this.XColumn], (double)dr[this.YColumn]);
							}

							fdt.AddRow(fdr);
						}

						ds.Tables.Add(fdt);
					}
				}
			}
		}

		/// <summary>
		/// Returns the number of features in the dataset
		/// </summary>
		/// <returns>Total number of features</returns>
		public int GetFeatureCount()
		{
			int count = 0;

			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select Count(*) FROM " + this.Table;

				if (_defintionQuery != null && _defintionQuery != "") //If a definition query has been specified, add this as a filter on the query
				{
					strSQL += " WHERE " + _defintionQuery;
				}

				using (OleDbCommand command = new OleDbCommand(strSQL, conn))
				{
					conn.Open();
					count = (int)command.ExecuteScalar();
					conn.Close();
				}				
			}

			return count;
		}

		/// <summary>
		/// Definition query used for limiting dataset.
		/// </summary>
		public string DefinitionQuery
		{
			get { return _defintionQuery; }
			set { _defintionQuery = value; }
		}

		/// <summary>
        /// Returns a datarow based on a oid
		/// </summary>
        /// <param name="oid"></param>
		/// <returns>datarow</returns>
        public FeatureDataRow<uint> GetFeature(uint oid)
		{
			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "select * from " + this.Table + " WHERE " + this.ObjectIdColumn + "=" + oid.ToString();
				
				using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, conn))
				{
					conn.Open();
					System.Data.DataSet ds = new System.Data.DataSet();
					adapter.Fill(ds);
					conn.Close();

					if (ds.Tables.Count > 0)
					{
						FeatureDataTable<uint> fdt = new FeatureDataTable<uint>(ds.Tables[0], "OID");

						foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
						{
							fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
						}

						if (ds.Tables[0].Rows.Count > 0)
						{
							System.Data.DataRow dr = ds.Tables[0].Rows[0];
							FeatureDataRow<uint> fdr = fdt.NewRow(oid);

							foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
							{
								if (String.Compare(col.ColumnName, "oid", StringComparison.CurrentCultureIgnoreCase) != 0)
								{
									fdr[col.ColumnName] = dr[col];
								}
							}

							if (dr[this.XColumn] != DBNull.Value && dr[this.YColumn] != DBNull.Value)
							{
								fdr.Geometry = new Point((double)dr[this.XColumn], (double)dr[this.YColumn]);
							}

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
		/// Boundingbox of dataset
		/// </summary>
		/// <returns>boundingbox</returns>
		public BoundingBox GetExtents()
		{
            BoundingBox box = BoundingBox.Empty;

			using (OleDbConnection conn = new OleDbConnection(_connectionString))
			{
				string strSQL = "Select Min(" + this.XColumn + ") as MinX, Min(" + this.YColumn + ") As MinY, " +
									   "Max(" + this.XColumn + ") As MaxX, Max(" + this.YColumn + ") As MaxY FROM " + this.Table;

				if (_defintionQuery != null && _defintionQuery != "") //If a definition query has been specified, add this as a filter on the query
				{
					strSQL += " WHERE " + _defintionQuery;
				}

				using (OleDbCommand command = new OleDbCommand(strSQL, conn))
				{
					conn.Open();
					
					using (OleDbDataReader dr = command.ExecuteReader())
					{
						if(dr.Read())
						{
							//If the read row is OK, create a point geometry from the XColumn and YColumn and return it
							if (dr[0] != DBNull.Value && dr[1] != DBNull.Value && dr[2] != DBNull.Value && dr[3] != DBNull.Value)
							{
								box = new BoundingBox((double)dr[0], (double)dr[1], (double)dr[2], (double)dr[3]);
							}
						}
					}

					conn.Close();
				}
			}

			return box;
		}

		/// <summary>
		/// Gets the connection ID of the datasource.
		/// </summary>
		public string ConnectionId
		{
			get { return _connectionString; }
		}

		/// <summary>
		/// Opens the datasource.
		/// </summary>
		public void Open()
		{
			//Don't really do anything. OleDb's ConnectionPooling takes over here
			_isOpen = true;
		}

		/// <summary>
		/// Closes the datasource.
		/// </summary>
		public void Close()
		{
			//Don't really do anything. OleDb's ConnectionPooling takes over here
			_isOpen = false;
		}

		/// <summary>
		/// Returns true if the datasource is currently open.
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen; }
		}

		/// <summary>
		/// The spatial reference ID (CRS).
		/// </summary>
		public int Srid
		{
			get { return _srid; }
			set { _srid = value; }
		}

		#endregion
	}
}
