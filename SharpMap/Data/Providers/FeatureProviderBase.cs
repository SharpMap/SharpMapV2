// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using System.Globalization;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Provides basic functionality for providers which access feature data.
    /// </summary>
    public abstract class FeatureProviderBase : ProviderBase, IFeatureProvider
    {
        private IGeometryFactory _geoFactory;

        #region IFeatureProvider Members

        public abstract FeatureDataTable CreateNewTable();

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            if (CoordinateTransformation != null && query.IsSpatialPredicateNonEmpty)
            {
                query = transformQuery(query);
            }

            return InternalExecuteFeatureQuery(query);
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query,
                                                      FeatureQueryExecutionOptions options)
        {
            if (CoordinateTransformation != null && query.IsSpatialPredicateNonEmpty)
            {
                query = transformQuery(query);
            }

            return InternalExecuteFeatureQuery(query, options);
        }

        public virtual IGeometryFactory GeometryFactory
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

        public abstract Int32 GetFeatureCount();

        public abstract DataTable GetSchemaTable();

        public abstract CultureInfo Locale { get; }

        public abstract void SetTableSchema(FeatureDataTable table);

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected members
        protected abstract IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query,
                                                                          FeatureQueryExecutionOptions options);

        protected abstract IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query);
        #endregion

        #region Private members
        private FeatureQueryExpression transformQuery(FeatureQueryExpression query)
        {
            SpatialBinaryExpression spatial = query.SpatialPredicate;
            ICoordinateSystem querySpatialReference = spatial.SpatialExpression.SpatialReference;

            if (querySpatialReference.EqualParams(OriginalSpatialReference))
            {
                return query;
            }

            //if (querySpatialReference != OriginalSpatialReference)
            //{
            //    throw new InvalidOperationException("The query's spatial reference doesn't match the provider's.");
            //}

            GeometryExpression geoExpression = spatial.SpatialExpression as GeometryExpression;

            if (geoExpression != null)
            {
                IGeometry transformed = CoordinateTransformation.InverseTransform(geoExpression.Geometry,
                                                                                  GeometryFactory);
                geoExpression = new GeometryExpression(transformed);
                spatial = spatial.IsSpatialExpressionLeft
                              ? new SpatialBinaryExpression(geoExpression, spatial.Op, spatial.Expression)
                              : new SpatialBinaryExpression(spatial.Expression, spatial.Op, geoExpression);
            }
            else
            {
                IExtents transformed = CoordinateTransformation.InverseTransform(spatial.SpatialExpression.Extents,
                                                                                 GeometryFactory);
                ExtentsExpression extentsExpression = new ExtentsExpression(transformed);
                spatial = spatial.IsSpatialExpressionLeft
                              ? new SpatialBinaryExpression(extentsExpression, spatial.Op, spatial.Expression)
                              : new SpatialBinaryExpression(spatial.Expression, spatial.Op, extentsExpression);
            }

            query = new FeatureQueryExpression(query, spatial);
            return query;
        }
        #endregion
    }
}
