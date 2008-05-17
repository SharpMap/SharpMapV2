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
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    public class FeatureQueryExpression : QueryExpression, IEquatable<FeatureQueryExpression>
    {
        public FeatureQueryExpression(IGeometry geometry, SpatialOperation op, ILayer layer)
            : base(new AllAttributesExpression(), new SpatialBinaryExpression(new SpatialExpression(geometry),
                                                                              op,
                                                                              new LayerExpression(layer))) { }

        public FeatureQueryExpression(IGeometry geometry, SpatialOperation op, FeatureDataTable features)
            : base(new AllAttributesExpression(),
                   new SpatialBinaryExpression(new SpatialExpression(geometry),
                                               op,
                                               new FeaturesCollectionExpression(features))) { }

        public FeatureQueryExpression(AttributeBinaryExpression attributeFilter)
            : base(new AllAttributesExpression(), attributeFilter) { }

        public FeatureQueryExpression(AttributeBinaryExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(new AllAttributesExpression(), new BinaryExpression(attributeFilter,
                                                                       BinaryOperator.And,
                                                                       spatialFilter)) { }

        public FeatureQueryExpression(AttributesProjectionExpression attributes,
                                      AttributeBinaryExpression attributeFilter,
                                      SpatialBinaryExpression spatialFilter)
            : base(attributes, new BinaryExpression(attributeFilter,
                                                    BinaryOperator.And,
                                                    spatialFilter)) { }

        protected internal FeatureQueryExpression(ProjectionExpression projection, 
                                                  PredicateExpression predicate)
            : base(projection, predicate) { }

        public SpatialBinaryExpression SpatialPredicate
        {
            get { throw new NotImplementedException(); }
        }

        public AttributeBinaryExpression AttributePredicate
        {
            get { throw new NotImplementedException(); }
        }

        public OidCollectionExpression OidPredicate
        {
            get { throw new NotImplementedException(); }
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
    }
}