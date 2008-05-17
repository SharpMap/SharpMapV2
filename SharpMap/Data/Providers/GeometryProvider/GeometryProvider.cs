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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
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
    public class GeometryProvider : IFeatureProvider<UInt32>
    {
        private IGeometryFactory _geoFactory;
        private ICoordinateTransformation _coordinateTransformation;
        private ICoordinateSystem _coordinateSystem;
        private readonly List<IGeometry> _geometries = new List<IGeometry>();
        private Int32? _srid;
        private Boolean _isDisposed;
        private IExtents _extents;

        #region Object Construction / Disposal

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="geometry">
        /// Geometry to be added to this datasource.
        /// </param>
        public GeometryProvider(IGeometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }

            _geoFactory = geometry.Factory;
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
            if (geometries == null)
            {
                throw new ArgumentNullException("geometries");
            }

            _geoFactory = Slice.GetFirst(geometries).Factory;
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
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            if (feature.Geometry == null)
            {
                return;
            }

            _geoFactory = feature.Geometry.Factory;
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
            foreach (FeatureDataRow row in features)
            {
                if (row.Geometry == null)
                {
                    continue;
                }

                if (_geoFactory == null)
                {
                    _geoFactory = row.Geometry.Factory;
                }

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
            : this(factory.WkbReader.Read(wellKnownBinaryGeometry))
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
            : this(factory.WktReader.Read(wellKnownTextGeometry))
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

        #region IProvider Members

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            //Do nothing;
        }

        /// <summary>
        /// Gets the connection ID of the datasource
        /// </summary>
        /// <remarks>
        /// The ConnectionId is meant for Connection Pooling which doesn't apply to this datasource. Instead
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
        /// Throws a <see cref="NotImplementedException"/>. 
        /// </summary>
        public Object ExecuteQuery(Expression query)
        {
            throw new NotImplementedException();
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
            if (_extents != null)
            {
                return _extents;
            }

            if (_geometries.Count == 0)
            {
                _extents = _geoFactory.CreateExtents();
            }
            else
            {
                foreach (IGeometry g in Geometries)
                {
                    if (g.IsEmpty)
                    {
                        continue;
                    }

                    if (_extents == null)
                    {
                        _extents = g.Extents;
                    }
                    else
                    {
                        _extents.ExpandToInclude(g.Extents);
                    }
                }
            }

            return _extents;
        }

        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>
        public Boolean IsOpen
        {
            get { return true; }
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            //Do nothing;
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

        #endregion

        #region IFeatureProvider Members

        public FeatureDataTable CreateNewTable()
        {
            return null;
        }

        /// <summary>
        /// Throws an NotSupportedException. 
        /// </summary>
        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws an NotSupportedException. 
        /// </summary>
        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            throw new NotSupportedException();
        }

        ///// <summary>
        ///// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        ///// are intersected by <paramref name="bounds"/>.
        ///// </summary>
        ///// <param name="bounds">BoundingBox to intersect with.</param>
        ///// <returns>An IFeatureDataReader to iterate over the results.</returns>
        //public IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds)
        //{
        //    return ExecuteIntersectionQuery(bounds, QueryExecutionOptions.Geometries);
        //}

        ///// <summary>
        ///// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        ///// are intersected by <paramref name="bounds"/>.
        ///// </summary>
        ///// <param name="bounds">BoundingBox to intersect with.</param>
        ///// <returns>An IFeatureDataReader to iterate over the results.</returns>
        ///// <param name="options">Options indicating which data to retrieve.</param>
        //public IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds, QueryExecutionOptions options)
        //{
        //    return new GeometryDataReader(this, bounds);
        //}


        ///// <summary>
        ///// Retrieves the data associated with all the features that 
        ///// are intersected by <paramref name="bounds"/>.
        ///// </summary>
        ///// <param name="bounds">BoundingBox to intersect with.</param>
        ///// <param name="table">FeatureDataTable to fill data into.</param>
        //public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table)
        //{
        //    ExecuteIntersectionQuery(bounds, table, QueryExecutionOptions.Geometries);
        //}

        ///// <summary>
        ///// Retrieves the data associated with all the features that 
        ///// are intersected by <paramref name="bounds"/>.
        ///// </summary>
        ///// <param name="bounds">BoundingBox to intersect with.</param>
        ///// <param name="table">FeatureDataTable to fill data into.</param>
        ///// <param name="options">Options indicating which data to retrieve.</param>
        //public void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options)
        //{
        //    if (table == null) throw new ArgumentNullException("table");

        //    List<IGeometry> intersection = new List<IGeometry>();

        //    foreach (IGeometry geometry in Geometries)
        //    {
        //        if (bounds.Intersects(geometry.Extents))
        //        {
        //            intersection.Add(geometry);
        //        }
        //    }

        //    foreach (FeatureDataRow row in table)
        //    {
        //        if (intersection.Exists(delegate(IGeometry match) { return match.Equals(row.Geometry); }))
        //        {
        //            intersection.Remove(row.Geometry);
        //        }
        //    }

        //    foreach (IGeometry geometry in intersection)
        //    {
        //        FeatureDataRow row = table.NewRow();
        //        row.Geometry = geometry;
        //        table.AddRow(row);
        //    }
        //}

        ///// <summary>
        ///// Returns features within the specified bounding bounds.
        ///// </summary>
        ///// <param name="bounds">The bounding bounds to intersect with.</param>
        ///// <returns>An enumeration of all geometries which intersect <paramref name="bounds"/>.</returns>
        //public IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IExtents bounds)
        //{
        //    List<IGeometry> list = new List<IGeometry>();

        //    IGeometry boxGeom = bounds.ToGeometry();

        //    foreach (IGeometry g in _geometries)
        //    {
        //        if (!g.IsEmpty)
        //        {
        //            if (g.Intersects(boxGeom))
        //            {
        //                list.Add(g);
        //            }
        //        }
        //    }

        //    return list.AsReadOnly();
        //}

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
            set { _geoFactory = value; }
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

        #region IFeatureProvider<UInt32> Members

        public IEnumerable<UInt32> ExecuteOidQuery(Expression query)
        {
            if (query == null) throw new ArgumentNullException("query");

            SpatialBinaryExpression sqe = query as SpatialBinaryExpression;

            if (sqe == null)
            {
                throw new NotSupportedException("Expression type not supported: " +
                                                query.GetType());
            }

            SpatialExpression spatialExpression = sqe.SpatialExpression;

            if (spatialExpression == null)
            {
                throw new ArgumentException("The SpatialQueryExpression must have " +
                                            "a non-null SpatialExpression.");
            }

            if (sqe.Expression == null)
            {
                throw new ArgumentException("The SpatialQueryExpression must have " +
                                            "a non-null Expression.");
            }

            IGeometry filterGeometry = spatialExpression.Geometry;
            Boolean isLeft = sqe.IsSpatialExpressionLeft;
            SpatialOperation op = sqe.Op;

            LayerExpression layerExpression = sqe.Expression as LayerExpression;

            if (layerExpression != null)
            {
                for (UInt32 i = 0; i < _geometries.Count; i++)
                {
                    if (isGeometryAtIndexAMatch((Int32)i, op, isLeft, filterGeometry))
                    {
                        yield return i;
                    }
                }

                yield break;
            }

            OidCollectionExpression oidsCollection = sqe.Expression as OidCollectionExpression;

            if (oidsCollection != null)
            {
                if (oidsCollection.Right == null)
                {
                    throw new ArgumentException("The OidCollectionExpression in the query has a null collection");
                }

                IEnumerable oids = oidsCollection.Right.Collection;

                if (oids == null)
                {
                    yield break;
                }

                foreach (Object oid in oids)
                {
                    if (isGeometryAtIndexAMatch((Int32)oid, op, isLeft, filterGeometry))
                    {
                        yield return (UInt32)oid;
                    }
                }

                yield break;
            }
        }

        public IFeatureDataRecord GetFeatureByOid(UInt32 oid)
        {
            throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
        }

        public IGeometry GetGeometryByOid(UInt32 oid)
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

        #region IFeatureProvider Explicit Members
        void IFeatureProvider.SetTableSchema(FeatureDataTable table)
        {
            table.Clear();
        }
        #endregion

        private Boolean isGeometryAtIndexAMatch(Int32 index, SpatialOperation op, Boolean isLeft, IGeometry filterGeometry)
        {
            IGeometry current = _geometries[index];

            return SpatialBinaryExpression.IsMatch(op, isLeft, filterGeometry, current);
        }
    }
}