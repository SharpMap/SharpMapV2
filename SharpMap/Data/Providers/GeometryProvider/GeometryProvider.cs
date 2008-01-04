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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Data;
using GeoAPI.Coordinates;
using GeoAPI.IO.WellKnownBinary;
using GeoAPI.IO.WellKnownText;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using System.Globalization;
using System.Collections;
using NPack.Interfaces;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.GeometryProvider
{
	/// <summary>
	/// Datasource for storing a limited set of geometries.
	/// </summary>
	/// <remarks>
	/// <para>The GeometryProvider doesn’t utilize performance optimizations of spatial indexing,
	/// and thus is primarily meant for rendering a limited set of Geometries.</para>
	/// <para>A common use of the GeometryProvider is for highlighting a set of selected features.</para>
	/// <example>
	/// The following example gets data within a BoundingBox of another datasource and adds it to the map.
	/// <code lang="C#">
	/// List{Geometry} geometries = myMap.Layers[0].DataSource.GetGeometriesInView(myBox);
	/// FeatureLayer laySelected = new FeatureLayer("Selected Features");
	/// laySelected.DataSource = new GeometryProvider(geometries);
	/// laySelected.Style.Outline = new Pen(Color.Magenta, 3f);
	/// laySelected.Style.EnableOutline = true;
	/// myMap.Layers.Add(laySelected);
	/// </code>
	/// </example>
	/// <example>
	/// Adding points of interest to the map. This is useful for vehicle tracking etc.
	/// <code lang="C#">
	/// List{SharpMap.Geometries.Geometry} geometries = new List{SharpMap.Geometries.Geometry}();
	/// //Add two points
	/// geometries.Add(new SharpMap.Geometries.Point(23.345,64.325));
	/// geometries.Add(new SharpMap.Geometries.Point(23.879,64.194));
	/// SharpMap.Layers.FeatureLayer layerVehicles = new SharpMap.Layers.FeatureLayer("Vechicles");
	/// layerVehicles.DataSource = new SharpMap.Data.Providers.GeometryProvider(geometries);
	/// layerVehicles.Style.Symbol = Bitmap.FromFile(@"C:\data\car.gif");
	/// myMap.Layers.Add(layerVehicles);
	/// </code>
	/// </example>
	/// </remarks>
    public class GeometryProvider : IFeatureLayerProvider<UInt32>
	{
		private ICoordinateTransformation _coordinateTransformation;
		private ICoordinateSystem _coordinateSystem;
        private readonly List<IGeometry> _geometries = new List<IGeometry>();
		private Int32? _srid = null;
		private Boolean _isDisposed;

		#region Object Construction / Disposal

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>.
		/// </summary>
		/// <param name="geometry">
		/// Geometry to be added to this datasource.
		/// </param>
		public GeometryProvider(IGeometry geometry)
		{
            _geometries = new List<IGeometry>();
			_geometries.Add(geometry);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>.
		/// </summary>
		/// <param name="geometries">
		/// Set of geometries to add to this datasource.
		/// </param>
        public GeometryProvider(IEnumerable<IGeometry> geometries)
		{
			_geometries.AddRange(geometries);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>.
		/// </summary>
		/// <param name="feature">
		/// Feature which has geometry to be used in this datasource.
		/// </param>
		public GeometryProvider(FeatureDataRow feature)
		{
            _geometries = new List<IGeometry>();
			_geometries.Add(feature.Geometry);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>.
		/// </summary>
		/// <param name="features">
		/// Features which have geometry to be used in this datasource.
		/// </param>
		public GeometryProvider(IEnumerable<FeatureDataRow> features)
		{
            _geometries = new List<IGeometry>();

			foreach (FeatureDataRow row in features)
			{
				_geometries.Add(row.Geometry);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>.
		/// </summary>
		/// <param name="wellKnownBinaryGeometry">
		/// An <see cref="GeoAPI.Geometries.IGeometry"/> instance as Well-Known Binary 
		/// to add to this datasource.
		/// </param>
		public GeometryProvider(Byte[] wellKnownBinaryGeometry, IGeometryFactory factory)
            : this(GeometryFromWkb.Parse(wellKnownBinaryGeometry, factory))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryProvider"/>
		/// </summary>
		/// <param name="wellKnownTextGeometry">
        /// An <see cref="GeoAPI.Geometries.IGeometry"/> instance as Well-Known Text 
		/// to add to this datasource.
		/// </param>
        public GeometryProvider(String wellKnownTextGeometry, IGeometryFactory factory)
            : this(GeometryFromWkt.Parse(wellKnownTextGeometry, factory))
		{
		}

		#region Dispose Pattern

		~GeometryProvider()
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
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		#endregion

        /// <summary>
        /// Gets a value indicating whether <see cref="Dispose"/> 
        /// has been called on the instance.
        /// </summary>
		public Boolean IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		protected void Dispose(Boolean disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				_geometries.Clear();
			}
		}

		#endregion

		#endregion

        #region Geometries Collection
        /// <summary>
		/// Gets or sets the geometries this datasource contains.
		/// </summary>
		public IList<IGeometry> Geometries
		{
			get { return _geometries; }
			set
			{
				_geometries.Clear();
				_geometries.AddRange(value);
			}
        }
        #endregion

        #region ILayerProvider Members

        /// <summary>
		/// Gets the connection ID of the datasource
		/// </summary>
		/// <remarks>
		/// The ConnectionID is meant for Connection Pooling which doesn't apply to this datasource. Instead
		/// <c>String.Empty</c> is returned.
		/// </remarks>
		public String ConnectionId
		{
			get { return String.Empty; }
		}

		public ICoordinateTransformation CoordinateTransformation
		{
			get { return _coordinateTransformation; }
			set { _coordinateTransformation = value; }
		}

		/// <summary>
		/// Returns true if the datasource is currently open
		/// </summary>
		public Boolean IsOpen
		{
			get { return true; }
		}

		public ICoordinateSystem SpatialReference
		{
			get { return _coordinateSystem; }
			set { _coordinateSystem = value; }
		}

		/// <summary>
		/// The spatial reference ID.
		/// </summary>
		public Int32? Srid
		{
			get { return _srid; }
			set { _srid = value; }
		}

		/// <summary>
		/// Closes the datasource
		/// </summary>
		public void Close()
		{
			//Do nothing;
		}

		/// <summary>
		/// The extents of the data source.
		/// </summary>
		/// <returns>
		/// An <see cref="IExtents"/> instance describing the extents of the 
		/// data available in the data source.
		/// </returns>
		public IExtents GetExtents()
		{
			IExtents box = null;

			if (_geometries.Count == 0)
			{
				return box;
			}

			foreach (IGeometry g in Geometries)
			{
				if (!g.IsEmpty)
				{
                    if (box == null)
                    {
                        box = g.Extents;
                    }
                    else
                    {
                        box.ExpandToInclude(g.Extents);
                    }
				}
			}

			return box;
		}

		/// <summary>
		/// Opens the datasource
		/// </summary>
		public void Open()
		{
			//Do nothing;
		}

		#endregion

        #region IFeatureLayerProvider Members

        public IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, QueryExecutionOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetFeatures(IEnumerable oids, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public FeatureDataTable CreateNewTable()
        {
            return null;
        }

        public void EndExecuteFeatureQuery(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureDataRecord> EndGetFeatures(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
		/// Throws an NotSupportedException. 
		/// </summary>
        /// <param name="geometry">The geometry used to query with.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        public IFeatureDataReader ExecuteFeatureQuery(FeatureSpatialExpression query)
		{
			throw new NotSupportedException();
		}

	    /// <summary>
		/// Throws an NotSupportedException.
		/// </summary>
        /// <param name="geometry">The geometry used to query with.</param>
		/// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table)
		{
			throw new NotSupportedException();
		}

	    /// <summary>
		/// Throws an NotSupportedException. 
		/// </summary>
		/// <param name="geometry">The geometry used to query with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        public void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet)
		{
			throw new NotSupportedException();
		}

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="box"/>.
        /// </summary>
        /// <param name="box">BoundingBox to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
	    public IFeatureDataReader ExecuteIntersectionQuery(IExtents box)
		{
	        return ExecuteIntersectionQuery(box, QueryExecutionOptions.Geometries);
        }

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="box"/>.
        /// </summary>
        /// <param name="box">BoundingBox to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        /// <param name="options">Options indicating which data to retrieve.</param>
        public IFeatureDataReader ExecuteIntersectionQuery(IExtents box, QueryExecutionOptions options)
        {
            return new GeometryDataReader(this, box);
        }

	    /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet)
	    {
            ExecuteIntersectionQuery(bounds, dataSet, QueryExecutionOptions.Geometries);
        }

	    /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, QueryExecutionOptions options)
		{
            if (dataSet == null) throw new ArgumentNullException("dataSet");

			FeatureDataTable table = dataSet.Tables[ConnectionId];

			if(table == null)
			{
				table = new FeatureDataTable(ConnectionId);
				dataSet.Tables.Add(table);
			}

			ExecuteIntersectionQuery(bounds, table);
		}

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table)
	    {
            ExecuteIntersectionQuery(bounds, table, QueryExecutionOptions.Geometries);
	    }

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options)
	    {
	        if (table == null) throw new ArgumentNullException("table");

	        List<IGeometry> intersection = new List<IGeometry>();

	        foreach (IGeometry geometry in Geometries)
	        {
                if (bounds.Intersects(geometry.Extents))
	            {
	                intersection.Add(geometry);
	            }
	        }

	        foreach (FeatureDataRow row in table)
	        {
	            if (intersection.Exists(delegate(IGeometry match) { return match.Equals(row.Geometry); }))
	            {
	                intersection.Remove(row.Geometry);
	            }
	        }

	        foreach (IGeometry geometry in intersection)
	        {
	            FeatureDataRow row = table.NewRow();
	            row.Geometry = geometry;
	            table.AddRow(row);
	        }
	    }

	    /// <summary>
	    /// Returns features within the specified bounding box.
	    /// </summary>
	    /// <param name="box">The bounding box to intersect with.</param>
	    /// <returns>An enumeration of all geometries which intersect <paramref name="box"/>.</returns>
        public IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IExtents box)
	    {
	        List<IGeometry> list = new List<IGeometry>();

	        IGeometry boxGeom = box.ToGeometry();

	        foreach (IGeometry g in _geometries)
	        {
	            if (!g.IsEmpty)
	            {
                    if (g.Intersects(boxGeom))
	                {
	                    list.Add(g);
	                }
	            }
	        }

	        return list.AsReadOnly();
	    }

        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable oids)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
		/// Retrieves the number of features accessible with this provider.
		/// </summary>
		/// <returns>The number of features this provider can access.</returns>
		public Int32 GetFeatureCount()
		{
			return _geometries.Count;
        }

	    public DataTable GetSchemaTable()
	    {
	        throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
	    }

	    public CultureInfo Locale
	    {
	        get { return CultureInfo.InvariantCulture; }
	    }

	    #endregion

		#region IFeatureLayerProvider<UInt32> Members

		/// <summary>
		/// Returns all objects whose boundingbox intersects 'bbox'.
		/// </summary>
		/// <param name="box"></param>
		/// <returns></returns>
        public IEnumerable<UInt32> GetIntersectingObjectIds(IExtents box)
		{
		    IGeometry intersect = box.ToGeometry();

			for (UInt32 i = 0; i < _geometries.Count; i++)
			{
                if (_geometries[(Int32)i].Intersects(intersect))
				{
					yield return i;
				}
			}
		}

		/// <summary>
		/// Throws an NotSupportedException. Attribute data is not supported by this datasource.
		/// </summary>
		/// <param name="oid"></param>
		/// <returns></returns>
		public IFeatureDataRecord GetFeature(UInt32 oid)
		{
			throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
		}

		/// <summary>
		/// Returns the geometry corresponding to the Object ID
		/// </summary>
		/// <param name="oid">Object ID</param>
		/// <returns>geometry</returns>
		public IGeometry GetGeometryById(UInt32 oid)
		{
			return _geometries[(Int32)oid];
		}

		public void SetTableSchema(FeatureDataTable<UInt32> table)
		{
            SetTableSchema(table, SchemaMergeAction.Add | SchemaMergeAction.Key);
        }

        public void SetTableSchema(FeatureDataTable<UInt32> table, SchemaMergeAction schemaMergeAction)
        {
            if (table == null) throw new ArgumentNullException("table");

            table.Columns.Clear();
        }

		#endregion

		#region IFeatureLayerProvider Explicit Members
		void IFeatureLayerProvider.SetTableSchema(FeatureDataTable table)
		{
			table.Clear();
		}
		#endregion

        #region IFeatureLayerProvider<UInt32> Members


        public IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<UInt32> oids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}