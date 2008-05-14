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
using System.Diagnostics;
using GeoAPI.Geometries;

namespace SharpMap.Expressions
{
    public class SpatialBinaryExpression : BinaryExpressionBase<SpatialOperation>,
                                           IEquatable<SpatialBinaryExpression>
    {
        public SpatialBinaryExpression(Expression left, SpatialOperation op, SpatialExpression right)
            : base(left, op, right) { }

        public SpatialBinaryExpression(SpatialExpression left, SpatialOperation op, Expression right)
            : base(left, op, right) { }

        protected SpatialBinaryExpression(Expression left, SpatialOperation op, Expression right)
            : base(left, op, right) { }

        public override Boolean Matches(Expression expression)
        {
            throw new NotImplementedException();
        }

        //public Boolean Matches(IGeometry geometry)
        //{
        //    switch (Op)
        //    {
        //        case SpatialOperation.None:
        //            return false;
        //        case SpatialOperation.Contains:
        //            return QueryGeometry.Contains(geometry);
        //        case SpatialOperation.Crosses:
        //            return QueryGeometry.Crosses(geometry);
        //        case SpatialOperation.Disjoint:
        //            return QueryGeometry.Disjoint(geometry);
        //        case SpatialOperation.Equals:
        //            return QueryGeometry.Equals(geometry);
        //        case SpatialOperation.Intersects:
        //            return QueryGeometry.Intersects(geometry);
        //        case SpatialOperation.Overlaps:
        //            return QueryGeometry.Overlaps(geometry);
        //        case SpatialOperation.Touches:
        //            return QueryGeometry.Touches(geometry);
        //        case SpatialOperation.Within:
        //            return QueryGeometry.Within(geometry);
        //        default:
        //            return false;
        //    }
        //}

        //public static Boolean operator !=(SpatialBinaryExpression lhs, SpatialBinaryExpression rhs)
        //{
        //    return !(lhs == rhs);
        //}

        //public static Boolean operator ==(SpatialBinaryExpression lhs, SpatialBinaryExpression rhs)
        //{
        //    if (ReferenceEquals(lhs, rhs))
        //    {
        //        return true;
        //    }

        //    return !ReferenceEquals(lhs, null)
        //               ? lhs.Equals(rhs)
        //               : rhs.Equals(lhs);
        //}

        public Boolean Equals(SpatialBinaryExpression other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return Equals(Left, other.Left) &&
                   Op == other.Op &&
                   Equals(Right, other.Right);
        }

        public override Boolean Equals(Expression other)
        {
            return Equals(other as SpatialBinaryExpression);
        }

        public override Boolean Equals(Object obj)
        {
            return Equals(obj as SpatialBinaryExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (Left == null ? 29 : Left.GetHashCode()) ^
                       Op.GetHashCode() ^
                       (Right == null ? 31 : Right.GetHashCode());
            }
        }

        protected override BinaryExpressionBase<SpatialOperation> Create(Expression left,
                                                                         SpatialOperation op,
                                                                         Expression right)
        {
            if (!(left is SpatialExpression || right is SpatialExpression))
            {
                throw new ArgumentException("Either parameter 'left' or parameter 'right' " +
                                            "must be a SpatialExpression instance.");
            }

            return new SpatialBinaryExpression(left, op, right);
        }
    }
}
