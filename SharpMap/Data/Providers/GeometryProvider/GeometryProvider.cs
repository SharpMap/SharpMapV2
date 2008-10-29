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
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
    /// Data source for storing a set of geometries in memory.
    /// </summary>
    public class GeometryProvider : ProviderBase, IFeatureProvider<UInt32>
    {
        private static readonly PropertyDescriptorCollection _geometryProviderTypeProperties;

        static GeometryProvider()
        {
            _geometryProviderTypeProperties = TypeDescriptor.GetProperties(typeof(GeometryProvider));
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="Geometries"/> property.
        /// </summary>
        public static PropertyDescriptor GeometriesProperty
        {
            get { return _geometryProviderTypeProperties.Find("Geometries", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="GeometryFactory"/> property.
        /// </summary>
        public static PropertyDescriptor GeometryFactoryProperty
        {
            get { return _geometryProviderTypeProperties.Find("GeometryFactory", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="GeometryProvider"/>'s <see cref="Locale"/> property.
        /// </summary>
        public static PropertyDescriptor LocaleProperty
        {
            get { return _geometryProviderTypeProperties.Find("Locale", false); }
        }

        private IGeometryFactory _geoFactory;
        private readonly List<IGeometry> _geometries = new List<IGeometry>();
        private IExtents _extents;

        #region Object Construction / Disposal

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="geometry">
        /// Geometry to be added to this data source.
        /// </param>
        public GeometryProvider(IGeometry geometry)
        {
            if (geometry == null) throw new ArgumentNullException("geometry");

            OriginalSpatialReference = geometry.SpatialReference;
            OriginalSrid = geometry.Srid;
            _geoFactory = geometry.Factory;
            _geometries.Add(geometry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="geometries">
        /// Set of geometries to add to this data source.
        /// </param>
        public GeometryProvider(IEnumerable<IGeometry> geometries)
        {
            if (geometries == null) throw new ArgumentNullException("geometries");

            _geoFactory = Enumerable.First(geometries).Factory;
            OriginalSpatialReference = _geoFactory == null ? null : _geoFactory.SpatialReference;
            OriginalSrid = _geoFactory == null ? null : _geoFactory.Srid;
            _geometries.AddRange(geometries);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="feature">
        /// Feature which has geometry to be used in this data source.
        /// </param>
        public GeometryProvider(FeatureDataRow feature)
        {
            if (feature == null) throw new ArgumentNullException("feature");

            if (feature.Geometry == null)
            {
                return;
            }

            _geoFactory = feature.Geometry.Factory;
            OriginalSpatialReference = _geoFactory == null ? null : _geoFactory.SpatialReference;
            OriginalSrid = _geoFactory == null ? null : _geoFactory.Srid;
            _geometries.Add(feature.Geometry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="features">
        /// Features which have geometry to be used in this data source.
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
                    OriginalSpatialReference = _geoFactory == null ? null : _geoFactory.SpatialReference;
                    OriginalSrid = _geoFactory == null ? null : _geoFactory.Srid;
                }

                _geometries.Add(row.Geometry);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>.
        /// </summary>
        /// <param name="wellKnownBinaryGeometry">
        /// An <see cref="IGeometry"/> instance as Well-Known Binary 
        /// to add to this data source.
        /// </param>
        /// <param name="factory">
        /// An <see cref="IGeometryFactory"/> instance to generate <see cref="IGeometry"/> instances.
        /// </param>
        public GeometryProvider(Byte[] wellKnownBinaryGeometry, IGeometryFactory factory)
            : this(factory.WkbReader.Read(wellKnownBinaryGeometry)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="wellKnownTextGeometry">
        /// An <see cref="IGeometry"/> instance as Well-Known Text 
        /// to add to this data source.
        /// </param>
        /// <param name="factory">
        /// An <see cref="IGeometryFactory"/> instance to generate <see cref="IGeometry"/> instances.
        /// </param>
        public GeometryProvider(String wellKnownTextGeometry, IGeometryFactory factory)
            : this(factory.WktReader.Read(wellKnownTextGeometry)) { }

        #region Dispose Pattern

        protected override void Dispose(Boolean disposing)
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
        /// Gets or sets the geometries this data source contains.
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
        /// Closes the data source
        /// </summary>
        public override void Close()
        {
            //Do nothing;
        }

        /// <summary>
        /// Gets the connection ID of the data source
        /// </summary>
        /// <remarks>
        /// The connection ID is meant for connection pooling which doesn't apply to this data source. Instead
        /// <see cref="String.Empty"/> is returned.
        /// </remarks>
        public override String ConnectionId
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Throws a <see cref="NotImplementedException"/>. 
        /// </summary>
        public override Object ExecuteQuery(Expression query)
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
        public override IExtents GetExtents()
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
        /// Returns <see langword="true"/> if the data source is currently open.
        /// </summary>
        public override Boolean IsOpen
        {
            get { return true; }
        }

        /// <summary>
        /// Opens the data source.
        /// </summary>
        public override void Open()
        {
            //Do nothing;
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

        public IExtents GetExtentsByOid(UInt32 oid)
        {
            throw new System.NotImplementedException();
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
            SetTableSchema(table, SchemaMergeAction.AddWithKey);
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

        protected override PropertyDescriptorCollection GetClassProperties()
        {
            return _geometryProviderTypeProperties;
        }

        protected override void SetObjectProperty(String propertyName, Object value)
        {
            if (propertyName.Equals(LocaleProperty.Name))
            {
                throw new InvalidOperationException("The property '" + propertyName + "' is read-only.");
            }

            if (propertyName.Equals(GeometriesProperty.Name))
            {
                Geometries = value as IList<IGeometry>;
            }
            else if (propertyName.Equals(GeometryFactoryProperty.Name))
            {
                GeometryFactory = value as IGeometryFactory;
            }
            else
            {
                base.SetObjectProperty(propertyName, value);
            }
        }

        protected override object GetObjectProperty(string propertyName)
        {
            if (propertyName.Equals(GeometriesProperty.Name))
            {
                return Geometries;
            }

            if (propertyName.Equals(GeometryFactoryProperty.Name))
            {
                return GeometryFactory;
            }

            if (propertyName.Equals(LocaleProperty.Name))
            {
                return Locale;
            }

            return base.GetObjectProperty(propertyName);
        }

        private Boolean isGeometryAtIndexAMatch(Int32 index, SpatialOperation op, Boolean isLeft, IGeometry filterGeometry)
        {
            IGeometry current = _geometries[index];

            return SpatialBinaryExpression.IsMatch(op, isLeft, filterGeometry, current);
        }
    }
}
