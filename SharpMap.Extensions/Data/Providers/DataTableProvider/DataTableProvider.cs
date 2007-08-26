// Copyright 2007 - Dan and Joel
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
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;

namespace SharpMap.Extensions.Data.Providers.DataTableProvider
{
	/// <summary>
	/// The DataTableProvider provider is used for rendering point data 
	/// from a System.Data.DataTable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The data source will need to have two double-type columns, 
	/// xColumn and yColumn that contains the coordinates of the point,
	/// and an integer-type column containing a unique identifier for each row.
	/// </para>
	/// </remarks>
	public class DataTableProvider : IVectorLayerProvider<uint>
	{
		private static readonly NumberFormatInfo NumberFormat_enUS
			= new CultureInfo("en-US", false).NumberFormat;

		private ICoordinateTransformation _coordinateTransform;
		private ICoordinateSystem _spatialReference;
		private string _xColumn;
		private string _yColumn;
		private string _objectIdColumn;
		private DataTable _table;
		private bool _isOpen;
		private int? _srid = null;
		private bool _isDisposed = false;

		#region Object construction and disposal
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the DataTablePoint provider
		/// </summary>
		/// <param name="dataTable">
		/// Instance of <see cref="DataTable"/> to use as data source.
		/// </param>
		/// <param name="oidColumnName">
		/// Name of the OID column.
		/// </param>
		/// <param name="xColumn">
		/// Name of column where point's X value is stored.
		/// </param>
		/// <param name="yColumn">
		/// Name of column where point's Y value is stored.
		/// </param>
		public DataTableProvider(DataTable dataTable, string oidColumnName,
		                         string xColumn, string yColumn)
			: this(dataTable, oidColumnName, xColumn, yColumn, null) { }

		/// <summary>
		/// Initializes a new instance of the DataTablePoint provider
		/// </summary>
		/// <param name="dataTable">
		/// Instance of <see cref="DataTable"/> to use as data source.
		/// </param>
		/// <param name="oidColumnName">
		/// Name of the OID column.
		/// </param>
		/// <param name="xColumn">
		/// Name of column where point's X value is stored.
		/// </param>
		/// <param name="yColumn">
		/// Name of column where point's Y value is stored.
		/// </param>
		/// <param name="spatialReference">
		/// The spatial reference system which the data uses.
		/// </param>
		public DataTableProvider(DataTable dataTable, string oidColumnName,
			string xColumn, string yColumn, ICoordinateSystem spatialReference)
		{
			Table = dataTable;
			XColumn = xColumn;
			YColumn = yColumn;
			ObjectIdColumn = oidColumnName;
			SpatialReference = spatialReference;
		}
		#endregion

		#region Disposers and finalizers

		/// <summary>
		/// Finalizer
		/// </summary>
		~DataTableProvider()
		{
			Dispose(false);
		}

		#region IDisposable Members
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
				Table.Dispose();
				Table = null;
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
		/// Gets the connection ID of the datasource.
		/// </summary>
		public string ConnectionId
		{
			get { checkDisposed(); return Table.TableName; }
		}

		/// <summary>
		/// Gets or sets a transform which converts the 
		/// spatial reference system of the data to another 
		/// spatial reference system.
		/// </summary>
		public ICoordinateTransformation CoordinateTransformation
		{
			get
			{
				checkDisposed(); 
				return _coordinateTransform;
			}
			set
			{
				checkDisposed(); 
				_coordinateTransform = value;
			}
		}

		/// <summary>
		/// Gets true if the data source is currently open.
		/// </summary>
		public bool IsOpen
		{
			get { return _isOpen; }
		}

		/// <summary>
		/// Gets or sets the name of the column that contains the object ID.
		/// </summary>
		public string ObjectIdColumn
		{
			get
			{
				checkDisposed(); 
				return _objectIdColumn;
			}
			set
			{
				checkDisposed(); 
				_objectIdColumn = value;
			}
		}

		/// <summary>
		/// Gets or sets the spatial reference system used by the data.
		/// </summary>
		public ICoordinateSystem SpatialReference
		{
			get
			{
				checkDisposed(); 
				return _spatialReference;
			}
			set
			{
				checkDisposed(); 
				_spatialReference = value;
			}
		}

		/// <summary>
		/// The id of a well-known spatial reference system.
		/// </summary>
		public int? Srid
		{
			get
			{
				checkDisposed(); 
				return _srid;
			}
			set
			{
				checkDisposed();
				_srid = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="DataTable"/> used as the data source.
		/// </summary>
		public DataTable Table
		{
			get
			{
				checkDisposed(); 
				return _table;
			}
			private set { _table = value; }
		}

		/// <summary>
		/// Gets or sets the name of the column that 
		/// contains X component of the coordinate.
		/// </summary>
		public string XColumn
		{
			get
			{
				checkDisposed(); 
				return _xColumn;
			}
			set
			{
				checkDisposed();
				_xColumn = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the column that 
		/// contains Y component of the coordinate.
		/// </summary>
		public string YColumn
		{
			get
			{
				checkDisposed(); 
				return _yColumn;
			}
			set
			{
				checkDisposed();
				_yColumn = value;
			}
		}

		#region IVectorLayerProvider<uint> Members

		/// <summary>
		/// Closes the datasource.
		/// </summary>
		public void Close()
		{
			_isOpen = false;
		}

		public IFeatureDataReader ExecuteIntersectionQuery(Geometry geom)
		{
			throw new NotSupportedException();
		}

		public void ExecuteIntersectionQuery(Geometry geom, FeatureDataTable table)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Throws NotSupportedException. 
		/// </summary>
		/// <param name="geom"></param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
		{
			throw new NotSupportedException();
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

		public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
		{
			return new DataTableFeatureDataReader(this, box);
		}

		public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves all features within the given BoundingBox.
		/// </summary>
		/// <param name="bbox">Bounds of the region to search.</param>
		/// <param name="ds">FeatureDataSet to fill data into</param>
		public void ExecuteIntersectionQuery(BoundingBox bbox, FeatureDataSet ds)
		{
			checkDisposed();

			DataRow[] rows;

			if (Table.Rows.Count == 0)
			{
				return;
			}

			string statement = XColumn + " > " + bbox.Left.ToString(NumberFormat_enUS) + " AND " +
							   XColumn + " < " + bbox.Right.ToString(NumberFormat_enUS) + " AND " +
							   YColumn + " > " + bbox.Bottom.ToString(NumberFormat_enUS) + " AND " +
							   YColumn + " < " + bbox.Top.ToString(NumberFormat_enUS);

			rows = Table.Select(statement);

			FeatureDataTable fdt = new FeatureDataTable(Table);

			foreach (DataColumn col in Table.Columns)
			{
				fdt.Columns.Add(col.ColumnName, col.DataType, col.Expression);
			}

			foreach (DataRow dr in rows)
			{
				fdt.ImportRow(dr);
				FeatureDataRow fdr = fdt.Rows[fdt.Rows.Count - 1] as FeatureDataRow;
				fdr.Geometry = new Point((double)dr[XColumn], (double)dr[YColumn]);
			}

			ds.Tables.Add(fdt);
		}

		/// <summary>
		/// Computes the full extents of the data source as a 
		/// <see cref="BoundingBox"/>.
		/// </summary>
		/// <returns>
		/// A BoundingBox instance which minimally bounds all the features
		/// available in this data source.
		/// </returns>
		public BoundingBox GetExtents()
		{
			checkDisposed();

			if (Table.Rows.Count == 0)
			{
				return BoundingBox.Empty;
			}

			BoundingBox box;

			double minX = Double.PositiveInfinity,
				   minY = Double.PositiveInfinity,
				   maxX = Double.NegativeInfinity,
				   maxY = Double.NegativeInfinity;

			foreach (DataRowView dr in Table.DefaultView)
			{
				if (minX > (double)dr[XColumn]) minX = (double)dr[XColumn];
				if (maxX < (double)dr[XColumn]) maxX = (double)dr[XColumn];
				if (minY > (double)dr[YColumn]) minY = (double)dr[YColumn];
				if (maxY < (double)dr[YColumn]) maxY = (double)dr[YColumn];
			}

			box = new BoundingBox(minX, minY, maxX, maxY);

			return box;
		}

		/// <summary>
		/// Returns a feature based on an object id.
		/// </summary>
		/// <param name="oid">The id of the object in the data source.</param>
		/// <returns>
		/// A FeatureDataRow{uint} which has the geometry and attributes of the requested feature.
		/// </returns>
		public FeatureDataRow<uint> GetFeature(uint oid)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the number of features in the dataset
		/// </summary>
		/// <returns>Total number of features</returns>
		public int GetFeatureCount()
		{
			checkDisposed();

			return Table.Rows.Count;
		}

		/// <summary>
		/// Returns geometries within the specified bounding box
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox bbox)
		{
			checkDisposed();

			DataRow[] drow;
			Collection<Geometry> features = new Collection<Geometry>();

			if (Table.Rows.Count == 0)
			{
				return null;
			}

			string strSQL = XColumn + " > " + bbox.Left.ToString(NumberFormat_enUS) + " AND " +
							XColumn + " < " + bbox.Right.ToString(NumberFormat_enUS) + " AND " +
							YColumn + " > " + bbox.Bottom.ToString(NumberFormat_enUS) + " AND " +
							YColumn + " < " + bbox.Top.ToString(NumberFormat_enUS);

			drow = Table.Select(strSQL);

			foreach (DataRow dr in drow)
			{
				features.Add(new Point((double)dr[2], (double)dr[1]));
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
			checkDisposed();

			DataRow[] rows;
			Geometry geom = null;

			if (Table.Rows.Count == 0)
			{
				return null;
			}

			string selectStatement = ObjectIdColumn + " = " + oid;

			rows = Table.Select(selectStatement);

			foreach (DataRow dr in rows)
			{
				geom = new Point((double)dr[XColumn], (double)dr[YColumn]);
			}

			return geom;
		}

		/// <summary>
		/// Returns geometry Object IDs whose bounding box intersects 'bbox'
		/// </summary>
		/// <param name="bbox"></param>
		/// <returns></returns>
		public IEnumerable<uint> GetObjectIdsInView(BoundingBox bbox)
		{
			checkDisposed();

			DataRow[] drow;
			Collection<uint> objectlist = new Collection<uint>();

			if (Table.Rows.Count == 0)
			{
				return null;
			}

			string strSQL = XColumn + " > " + bbox.Left.ToString(NumberFormat_enUS) + " AND " +
							XColumn + " < " + bbox.Right.ToString(NumberFormat_enUS) + " AND " +
							YColumn + " > " + bbox.Bottom.ToString(NumberFormat_enUS) + " AND " +
							YColumn + " < " + bbox.Top.ToString(NumberFormat_enUS);

			drow = Table.Select(strSQL);

			foreach (DataRow dr in drow)
			{
				objectlist.Add((uint)(int)dr[0]);
			}

			return objectlist;
		}

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Opens the datasource.
		/// </summary>
		public void Open()
		{
			checkDisposed();
			_isOpen = true;
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

		#region Private helper methods

		private void checkDisposed()
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}
		#endregion
	}
}