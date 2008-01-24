// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Expressions
{
    public class SpatialExpression : IEquatable<SpatialExpression>
    {
        private readonly IGeometry _queryRegion;
        private readonly SpatialExpressionType _queryType;

        public SpatialExpression(IGeometry queryRegion, SpatialExpressionType queryType)
        {
            _queryRegion = queryRegion;
            _queryType = queryType;
        }

        public IGeometry QueryRegion
        {
            // NOTE: changed Point.Empty to null
            get { return _queryRegion; }
        }

        public SpatialExpressionType QueryType
        {
            get { return _queryType; }
        }

        public Boolean HasIntersection(IGeometry geometry)
        {
            switch (QueryType)
            {
                case SpatialExpressionType.None:
                    return false;
                case SpatialExpressionType.Contains:
                    return QueryRegion.Contains(geometry);
                case SpatialExpressionType.Crosses:
                    return QueryRegion.Crosses(geometry);
                case SpatialExpressionType.Disjoint:
                    return QueryRegion.Disjoint(geometry);
                case SpatialExpressionType.Equals:
                    return QueryRegion.Equals(geometry);
                case SpatialExpressionType.Intersects:
                    return QueryRegion.Intersects(geometry);
                case SpatialExpressionType.Overlaps:
                    return QueryRegion.Overlaps(geometry);
                case SpatialExpressionType.Touches:
                    return QueryRegion.Touches(geometry);
                case SpatialExpressionType.Within:
                    return QueryRegion.Within(geometry);
                default:
                    return false;
            }
        }

        public static Boolean operator !=(SpatialExpression lhs, SpatialExpression rhs)
        {
            return !(lhs == rhs);
        }

        public static Boolean operator ==(SpatialExpression lhs, SpatialExpression rhs)
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

        public Boolean Equals(SpatialExpression other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return _queryRegion == other._queryRegion
                && _queryType == other._queryType;
        }

        public override Boolean Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as SpatialExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (_queryRegion != null 
                    ? _queryRegion.GetHashCode() 
                    : 0x1fd3b)
                 ^ 29 * _queryType.GetHashCode();
            }
        }

        public SpatialExpression Clone()
        {
            SpatialExpression clone = new SpatialExpression(
                _queryRegion == null ? null : _queryRegion.Clone(), QueryType);

            return clone;
        }
    }
}
