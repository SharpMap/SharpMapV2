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
using System.Collections.Generic;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    public class FeatureQueryExpression : QueryExpression, IEquatable<FeatureQueryExpression>
    {
        public static FeatureQueryExpression Intersects(IExtents extents)
        {
            return new FeatureQueryExpression(extents, SpatialOperation.Intersects);
        }

        public static FeatureQueryExpression Intersects(IGeometry geometry)
        {
            return new FeatureQueryExpression(geometry, SpatialOperation.Intersects);
        }

        public static FeatureQueryExpression Intersects(SelectExpression projection, IExtents extents)
        {
            return new FeatureQueryExpression(projection,
                                              null,
                                              SpatialBinaryExpression.Intersects(extents),
                                              null);
        }

        public static FeatureQueryExpression Intersects(SelectExpression projection, IGeometry geometry)
        {
            return new FeatureQueryExpression(projection,
                                              null,
                                              SpatialBinaryExpression.Intersects(geometry),
                                              null);
        }

        public static FeatureQueryExpression Create(IGeometry geometry, SpatialOperation op)
        {
            return new FeatureQueryExpression(geometry, op);
        }

        public FeatureQueryExpression(IExtents extents, SpatialOperation op)
            : base(new AllAttributesExpression(), new SpatialBinaryExpression(new ExtentsExpression(extents),
                                                                              op,
                                                                              new ThisExpression())) { }

        public FeatureQueryExpression(IGeometry geometry, SpatialOperation op)
            : base(new AllAttributesExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                              op,
                                                                              new ThisExpression())) { }

        public FeatureQueryExpression(IGeometry geometry, SpatialOperation op, ILayer layer)
            : base(new AllAttributesExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                              op,
                                                                              new LayerExpression(layer))) { }

        public FeatureQueryExpression(IGeometry geometry, SpatialOperation op, IFeatureProvider provider)
            : base(new AllAttributesExpression(), new SpatialBinaryExpression(new GeometryExpression(geometry),
                                                                              op,
                                                                              new ProviderExpression(provider))) { }
        public FeatureQueryExpression(IExtents extents,
                                      SpatialOperation op,
                                      IEnumerable<IFeatureDataRecord> features)
            : base(new AllAttributesExpression(),
                   new SpatialBinaryExpression(new ExtentsExpression(extents),
                                               op,
                                               new FeaturesCollectionExpression(features))) { }

        public FeatureQueryExpression(IGeometry geometry,
                                      SpatialOperation op,
                                      IEnumerable<IFeatureDataRecord> features)
            : base(new AllAttributesExpression(),
                   new SpatialBinaryExpression(new GeometryExpression(geometry),
                                               op,
                                               new FeaturesCollectionExpression(features))) { }

        public FeatureQueryExpression(AttributeBinaryExpression attributeFilter)
            : base(new AllAttributesExpression(), attributeFilter) { }

        public FeatureQueryExpression(AttributesPredicateExpression attributeFilter)
            : base(new AllAttributesExpression(), attributeFilter) { }

        public FeatureQueryExpression(SpatialBinaryExpression spatialFilter)
            : base(new AllAttributesExpression(), spatialFilter) { }

        public FeatureQueryExpression(AttributeBinaryExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(new AllAttributesExpression(), new BinaryLogicExpression(attributeFilter,
                                                                       BinaryLogicOperator.And,
                                                                       spatialFilter)) { }

        public FeatureQueryExpression(AttributesPredicateExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(new AllAttributesExpression(), new BinaryLogicExpression(attributeFilter,
                                                                       BinaryLogicOperator.And,
                                                                       spatialFilter)) { }

        public FeatureQueryExpression(AttributesProjectionExpression attributes,
                                      AttributeBinaryExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(attributes, new BinaryLogicExpression(attributeFilter,
                                                    BinaryLogicOperator.And,
                                                    spatialFilter)) { }

        public FeatureQueryExpression(AttributesProjectionExpression attributes,
                                      AttributesPredicateExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(attributes, new BinaryLogicExpression(attributeFilter,
                                                    BinaryLogicOperator.And,
                                                    spatialFilter)) { }

        public FeatureQueryExpression(AttributeBinaryExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter,
                                      OidCollectionExpression oidFilter)
            : this(new AllAttributesExpression(), attributeFilter, spatialFilter, oidFilter) { }

        public FeatureQueryExpression(AttributesPredicateExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter,
                                      OidCollectionExpression oidFilter)
            : this(new AllAttributesExpression(), attributeFilter, spatialFilter, oidFilter) { }

        public FeatureQueryExpression(SelectExpression projectionExpression,
                                      BinaryLogicExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter,
                                      OidCollectionExpression oidFilter)
            // TODO: Well, this is crazy. We need an init() function, and perhaps some more static creator methods.
            : base(projectionExpression,
                   attributeFilter == null
                        ? spatialFilter == null
                                ? oidFilter
                                : oidFilter == null
                                    ? (LogicExpression)spatialFilter
                                    : new BinaryLogicExpression(spatialFilter, BinaryLogicOperator.And, oidFilter)
                        : spatialFilter == null
                                ? oidFilter == null
                                    ? attributeFilter
                                    : new BinaryLogicExpression(attributeFilter, BinaryLogicOperator.And, oidFilter)
                                : oidFilter == null
                                    ? new BinaryLogicExpression(attributeFilter, BinaryLogicOperator.And, spatialFilter)
                                    : new BinaryLogicExpression(attributeFilter,
                                                           BinaryLogicOperator.And,
                                                           new BinaryLogicExpression(oidFilter,
                                                                                BinaryLogicOperator.And,
                                                                                spatialFilter))) { }

        public FeatureQueryExpression(FeatureQueryExpression expressionToCopy,
                                      SpatialBinaryExpression replacementSpatialExpression)
            : this(expressionToCopy.Projection,
                   expressionToCopy.SingleAttributePredicate,
                   replacementSpatialExpression,
                   expressionToCopy.OidPredicate) { }

        public FeatureQueryExpression(SelectExpression projection,
                                                  LogicExpression predicate)//jd: made public
            : base(projection, predicate) { }

        public SpatialBinaryExpression SpatialPredicate
        {
            get
            {
                SpatialBinaryExpression spatialBinaryExpression =
                    Predicate as SpatialBinaryExpression;

                if (spatialBinaryExpression != null)
                {
                    return spatialBinaryExpression;
                }

                BinaryLogicExpression binaryExpression = Predicate as BinaryLogicExpression;

                if (binaryExpression == null)
                {
                    return null;
                }

                spatialBinaryExpression = binaryExpression.Left as SpatialBinaryExpression;

                if (spatialBinaryExpression != null)
                {
                    return spatialBinaryExpression;
                }

                spatialBinaryExpression = binaryExpression.Right as SpatialBinaryExpression;

                return spatialBinaryExpression;
            }
        }

        public AttributeBinaryExpression SingleAttributePredicate
        {
            get
            {
                AttributeBinaryExpression attributesExpression =
                    Predicate as AttributeBinaryExpression;

                if (attributesExpression != null)
                {
                    return attributesExpression;
                }

                BinaryLogicExpression binaryExpression = Predicate as BinaryLogicExpression;

                return findExpressionType<AttributeBinaryExpression>(binaryExpression);
            }
        }

        public AttributesPredicateExpression MultiAttributePredicate
        {
            get
            {
                AttributesPredicateExpression attributesExpression =
                    Predicate as AttributesPredicateExpression;

                if (attributesExpression != null)
                {
                    return attributesExpression;
                }

                BinaryLogicExpression binaryExpression = Predicate as BinaryLogicExpression;

                return findExpressionType<AttributesPredicateExpression>(binaryExpression);
            }
        }

        public OidCollectionExpression OidPredicate
        {
            get
            {
                OidCollectionExpression oidsExpression =
                    Predicate as OidCollectionExpression;

                if (oidsExpression != null)
                {
                    return oidsExpression;
                }

                BinaryLogicExpression binaryExpression = Predicate as BinaryLogicExpression;

                if (binaryExpression == null)
                {
                    return null;
                }

                oidsExpression = binaryExpression.Left as OidCollectionExpression;

                if (oidsExpression != null)
                {
                    return oidsExpression;
                }

                oidsExpression = binaryExpression.Right as OidCollectionExpression;

                return oidsExpression;
            }
        }

        public Boolean IsSpatialPredicateNonEmpty
        {
            get
            {
                return SpatialPredicate != null &&
                       SpatialPredicate.SpatialExpression != null &&
                       !SpatialExpression.IsNullOrEmpty(SpatialPredicate.SpatialExpression);
            }
        }

        public Boolean IsOidPredicateNonEmpty
        {
            get
            {
                return OidPredicate != null &&
                       !Slice.IsEmpty(OidPredicate.OidValues);
            }
        }

        public Boolean IsSingleAttributePredicateNonEmpty
        {
            get
            {
                return SingleAttributePredicate != null &&
                       (SingleAttributePredicate.HasCollectionValueExpression ||
                        SingleAttributePredicate.HasSingleValueExpression);
            }
        }

        public Boolean IsMultiAttributePredicateNonEmpty
        {
            get
            {
                return MultiAttributePredicate != null;
            }
        }

        public override Expression Clone()
        {
            return new FeatureQueryExpression(Projection, Predicate);
        }

        public Boolean Equals(FeatureQueryExpression other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public override Boolean Equals(Object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as FeatureQueryExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() ^ 131;
            }
        }

        private static TExpression findExpressionType<TExpression>(BinaryLogicExpression binaryExpression)
            where TExpression : BinaryLogicExpression
        {
            if (binaryExpression == null)
            {
                return null;
            }

            TExpression tExpression = binaryExpression as TExpression;

            if (tExpression != null)
            {
                return tExpression;
            }

            tExpression = findExpressionType<TExpression>(binaryExpression.Left as BinaryLogicExpression);

            if (tExpression != null)
            {
                return tExpression;
            }

            tExpression = findExpressionType<TExpression>(binaryExpression.Right as BinaryLogicExpression);

            return tExpression;
        }
    }
}