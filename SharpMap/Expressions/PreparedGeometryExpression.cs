// Copyright 2012: Felix Obermaier
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

using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.Geometries.Prepared;

namespace SharpMap.Expressions
{
    /// <summary>
    /// An expression which represents an <see cref="IPreparedGeometry"/> in a
    /// compound expression or an expression tree.
    /// </summary>
    public class PreparedGeometryExpression : SpatialExpression
    {
        /// <summary>
        /// Creates a prepared geometry expression
        /// </summary>
        /// <param name="geometry">The geometry</param>
        public PreparedGeometryExpression(IGeometry geometry)
        {
            if (geometry != null)
                PreparedGeometry = geometry.ToPreparedGeometry();
        }

        /// <summary>
        /// Gets the prepared geometry
        /// </summary>
        public IPreparedGeometry PreparedGeometry { get; private set; }

        public override bool Contains(Expression other)
        {
            var se = other as SpatialExpression;
            if (se == null) return false;

            var ge = se as GeometryExpression;
            if (ge != null)
            {
                if (IsNullOrEmpty(this) && IsNullOrEmpty(ge))
                    return true;
                return (PreparedGeometry.Geometry.Equals(ge.Geometry));
            }
            return Equals(se);
        }

        public override Expression Clone()
        {
            return new PreparedGeometryExpression(PreparedGeometry.Geometry.Clone());
        }

        public override bool Equals(Expression other)
        {
            return Equals(other as PreparedGeometryExpression);
        }

        public override bool Equals(SpatialExpression other)
        {
            return Equals(other as PreparedGeometryExpression);
        }

        /// <summary>
        /// Returns
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PreparedGeometryExpression other)
        {
            if (other == null)
                return false;

            if (IsNullOrEmpty(this) && IsNullOrEmpty(other))
                return true;

            return PreparedGeometry.Geometry.Equals(other.PreparedGeometry.Geometry);
        }

        public override bool IsNull
        {
            get { return PreparedGeometry == null; }
        }

        public override bool IsEmpty
        {
            get
            {
                return PreparedGeometry == null || PreparedGeometry.Geometry.IsEmpty;
            }
        }

        public override IExtents Extents
        {
            get { return PreparedGeometry == null ? null : PreparedGeometry.Geometry.Extents; }
        }

        public override ICoordinateSystem SpatialReference
        {
            get { return PreparedGeometry == null ? null : PreparedGeometry.Geometry.SpatialReference; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PreparedGeometry != null
                            ? PreparedGeometry.Geometry.GetHashCode()
                            : 0x1f43b) ^ 17;
            }
        }

        public override string ToString()
        {
            return "PreparedGeometryExpression: " +
                (PreparedGeometry == null
                    ? "null"
                    : PreparedGeometry.Geometry.ToString()
                );
        }
    }
}