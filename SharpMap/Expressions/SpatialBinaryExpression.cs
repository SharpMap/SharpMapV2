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
using GeoAPI.Geometries.Prepared;

namespace SharpMap.Expressions
{
    /// <summary>
    /// Spatial binary expression class
    /// </summary>
    public class SpatialBinaryExpression : BinaryExpressionBase<SpatialOperation>,
                                           IEquatable<SpatialBinaryExpression>
    {
        /// <summary>
        /// Creates a spatial binary expression that evaluates the
        /// <see cref="SpatialOperation.Intersects"/> predicate against
        /// the provided geometry.
        /// </summary>
        /// <param name="geometry">The geometry argument</param>
        /// <returns>The spatial binary expression</returns>
        public static SpatialBinaryExpression Intersects(IGeometry geometry)
        {
            return new SpatialBinaryExpression(new GeometryExpression(geometry),
                                               SpatialOperation.Intersects,
                                               new ThisExpression());
        }

        /// <summary>
        /// Creates a spatial binary expression that evaluates the
        /// <see cref="SpatialOperation.Intersects"/> predicate against
        /// the provided extents.
        /// </summary>
        /// <param name="extents">The extent argumen</param>
        /// <returns>The spatial binary expression</returns>
        public static SpatialBinaryExpression Intersects(IExtents extents)
        {
            return new SpatialBinaryExpression(new ExtentsExpression(extents),
                                               SpatialOperation.Intersects,
                                               new ThisExpression());
        }

        /// <summary>
        /// Creates a spatial binary expression that evaluates the
        /// <see cref="SpatialOperation.Intersects"/> predicate against
        /// the provided extents.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="spatialExpression"></param>
        /// <returns></returns>
        public static SpatialBinaryExpression Intersects(Expression expression,
                                                         SpatialExpression spatialExpression)
        {
            return new SpatialBinaryExpression(expression,
                                               SpatialOperation.Intersects,
                                               spatialExpression);
        }

        /// <summary>
        /// Creates a spatial binary expression with <see cref="SpatialExpression"/> as right argument.
        /// </summary>
        /// <param name="left">The left argument</param>
        /// <param name="op">The spatial predicate operation</param>
        /// <param name="right">The right argument</param>
        public SpatialBinaryExpression(Expression left, SpatialOperation op, SpatialExpression right)
            : base(left, op, right) { }

        /// <summary>
        /// Creates a spatial binary expression
        /// </summary>
        /// <param name="left">The left argument</param>
        /// <param name="op">The spatial predicate operation</param>
        /// <param name="right">The right argument</param>
        public SpatialBinaryExpression(SpatialExpression left, SpatialOperation op, Expression right)
            : base(left, op, right) { }

        protected SpatialBinaryExpression(Expression left, SpatialOperation op, Expression right)
            : base(left, op, right) { }

        /// <summary>
        /// Gets the fixed spatial predicate
        /// </summary>
        /// <remarks>This is the spatial predicate</remarks>
        public SpatialExpression SpatialExpression
        {
            get { return (Left as SpatialExpression) ?? (Right as SpatialExpression); }
        }

        /// <summary>
        /// Gets the argument to test.
        /// </summary>
        public Expression Expression
        {
            get { return Left is SpatialExpression ? Right : Left; }
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsSpatialExpressionLeft
        {
            get { return Left is SpatialExpression; }
        }

        public override Boolean Contains(Expression expression)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Function to evaluate spatial predicate operation using extents
        /// </summary>
        /// <param name="op">The spatial predicate operation</param>
        /// <param name="aIsLeft">Value indicating that the operation arguments should be switched</param>
        /// <param name="a">The prepared geometry to test</param>
        /// <param name="b">The geometry to test</param>
        /// <returns>The result of the spatial predicate</returns>
        public static Boolean IsMatch(SpatialOperation op, Boolean aIsLeft, IExtents a, IExtents b)
        {
            if (!aIsLeft)
                return IsMatch(op, true, b, a);

            switch (op)
            {
                case SpatialOperation.Contains:
                    return a.Contains(b);
                case SpatialOperation.ContainsProperly:
                    throw new NotSupportedException();
                //return a.ContainsProperly(b);
                case SpatialOperation.Disjoint:
                    return !a.Intersects(b);
                case SpatialOperation.Equals:
                    return a.Equals(b);
                case SpatialOperation.Crosses:
                case SpatialOperation.Intersects:
                    return a.Intersects(b);
                case SpatialOperation.Overlaps:
                    return a.Overlaps(b);
                case SpatialOperation.Touches:
                    return a.Touches(b);
                case SpatialOperation.Within:
                    return a.Within(b);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Function to evaluate spatial predicate operation using geometries
        /// </summary>
        /// <param name="op">The spatial predicate operation</param>
        /// <param name="aIsLeft">Value indicating that the operation arguments should be switched</param>
        /// <param name="a">The prepared geometry to test</param>
        /// <param name="b">The geometry to test</param>
        /// <returns>The result of the spatial predicate</returns>
        public static Boolean IsMatch(SpatialOperation op, Boolean aIsLeft, IGeometry a, IGeometry b)
        {
            if (!aIsLeft)
                return IsMatch(op, true, b, a);

            switch (op)
            {
                case SpatialOperation.Contains:
                    return a.Contains(b);
                case SpatialOperation.ContainsProperly:
                    return a.ContainsProperly(b);
                case SpatialOperation.Crosses:
                    return a.Crosses(b);
                case SpatialOperation.Disjoint:
                    return a.Disjoint(b);
                case SpatialOperation.Equals:
                    return a.Equals(b);
                case SpatialOperation.Intersects:
                    return a.Intersects(b);
                case SpatialOperation.Overlaps:
                    return a.Overlaps(b);
                case SpatialOperation.Touches:
                    return a.Touches(b);
                case SpatialOperation.Within:
                    return a.Within(b);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Function to evaluate spatial predicate using a prepared geometry
        /// </summary>
        /// <param name="op">The spatial predicate operation</param>
        /// <param name="a">The prepared geometry to test</param>
        /// <param name="b">The geometry to test</param>
        /// <returns>The result of the spatial predicate</returns>
        public static bool IsMatch(SpatialOperation op, IPreparedGeometry a, IGeometry b)
        {
            switch (op)
            {
                case SpatialOperation.None:
                    return false;
                case SpatialOperation.Crosses:
                    return a.Crosses(b);
                case SpatialOperation.Contains:
                    return a.Contains(b);
                case SpatialOperation.ContainsProperly:
                    return a.ContainsProperly(b);
                case SpatialOperation.Disjoint:
                    return a.Disjoint(b);
                case SpatialOperation.Equals:
                    return a.Geometry.Equals(b);
                case SpatialOperation.Intersects:
                    return a.Intersects(b);
                case SpatialOperation.Overlaps:
                    return a.Overlaps(b);
                case SpatialOperation.Touches:
                    return a.Touches(b);
                case SpatialOperation.Within:
                    return a.Within(b);
                default:
                    return false;
            }
        }

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