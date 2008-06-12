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
using GeoAPI.Diagnostics;
using GeoAPI.Geometries;
using SharpMap.Expressions;
#if DOTNET35
using Enumerable = System.Linq.Enumerable;
#else
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif

namespace SharpMap.Data.Providers.GeometryProvider
{
    /// <summary>
    /// Datasource for storing a limited set of geometries.
    /// </summary>
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
            if (geometry == null) throw new ArgumentNullException("geometry");

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
            if (geometries == null) throw new ArgumentNullException("geometries");

            _geoFactory = Enumerable.First(geometries).Factory;
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
            if (feature == null) throw new ArgumentNullException("feature");

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

        public IEnumerable<UInt32> ExecuteOidQuery(SpatialBinaryExpression query)
        {
            if (query == null) throw new ArgumentNullException("query");

            SpatialExpression spatialExpression = query.SpatialExpression;

            if (SpatialExpression.IsNullOrEmpty(spatialExpression))
            {
                yield break;
            }

            if (query.Expression == null)
            {
                throw new ArgumentException("The SpatialQueryExpression must have " +
                                            "a non-null Expression.");
            }

            ExtentsExpression extentsExpression = spatialExpression as ExtentsExpression;
            GeometryExpression geometryExpression = spatialExpression as GeometryExpression;

            IExtents filterExtents = extentsExpression != null
                                         ? extentsExpression.Extents
                                         : null;
            IGeometry filterGeometry = geometryExpression != null
                                           ? geometryExpression.Geometry
                                           : null;

            Assert.IsTrue(filterExtents != null || filterGeometry != null);

            Boolean isLeft = query.IsSpatialExpressionLeft;
            SpatialOperation op = query.Op;

            LayerExpression layerExpression = query.Expression as LayerExpression;

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

            OidCollectionExpression oidsCollection = query.Expression as OidCollectionExpression;

            if (oidsCollection != null)
            {
                if (oidsCollection.Right == null)
                {
                    throw new ArgumentException("The OidCollectionExpression in the query " +
                                                "has a null collection");
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