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
using System.Collections;
using GeoAPI.Geometries;

namespace SharpMap.Expressions
{
    public class FeatureSpatialExpression : SpatialExpression, IEquatable<FeatureSpatialExpression>
    {
        private static readonly IEnumerable _emptyEnumeration = generateEmptyEnumeration();
        private readonly IEnumerable _oids;
        private readonly Boolean _hasOidFilter;

        public FeatureSpatialExpression(IGeometry queryRegion, IEnumerable oids)
            : this(queryRegion, SpatialExpressionType.None, oids) { }

        public FeatureSpatialExpression(IGeometry queryRegion, SpatialExpressionType type)
            : this(queryRegion, type, null) {}

        public FeatureSpatialExpression(IGeometry queryRegion, SpatialExpressionType type, IEnumerable oids)
            : base(queryRegion, type)
        {
            _oids = oids;
            _hasOidFilter = oids != null;
        }

        public IEnumerable Oids
        {
            get 
            {
                return _hasOidFilter
                           ? generateOidFilter()
                           : _emptyEnumeration;
            }
        }

        public static Boolean operator !=(FeatureSpatialExpression lhs, FeatureSpatialExpression rhs)
        {
            return !(lhs == rhs);
        }

        public static Boolean operator ==(FeatureSpatialExpression lhs, FeatureSpatialExpression rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (!ReferenceEquals(lhs, null))
            {
                return lhs.Equals(rhs);
            }
            else
            {
                return rhs.Equals(lhs);
            }
        }

        public Boolean Equals(FeatureSpatialExpression other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (!base.Equals(other))
            {
                return false;
            }

            if (ReferenceEquals(_oids, other._oids))
            {
                return true;
            }

            if (!ReferenceEquals(other._oids, null))
            {
                return other._oids.Equals(_oids);
            }
            else
            {
                return _oids.Equals(other._oids);
            }
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as FeatureSpatialExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() ^ 131 *
                    (_oids != null ? _oids.GetHashCode() : 0x38ff);
            }
        }

        public new FeatureSpatialExpression Clone()
        {
            FeatureSpatialExpression clone = new FeatureSpatialExpression(
                QueryRegion.Clone(), QueryType, Oids);

            return clone;
        }

        private static IEnumerable generateEmptyEnumeration()
        {
            yield break;
        }

        private IEnumerable generateOidFilter()
        {
            if (ReferenceEquals(_oids, null))
            {
                yield break;
            }

            foreach (Object oid in _oids)
            {
                yield return oid;
            }
        }
    }
}