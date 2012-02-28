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
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;

namespace SharpMap.Expressions
{
    /// <summary>
    /// An expression which represents an <see cref="IGeometry"/> in a
    /// compound expression or an expression tree.
    /// </summary>
    public class GeometryExpression : SpatialExpression
    {
        private readonly IGeometry _geometry;

        /// <summary>
        /// Creates a geometry expression
        /// </summary>
        /// <param name="geometry">The geometry</param>
        public GeometryExpression(IGeometry geometry)
        {
            _geometry = geometry;
        }

        public override string ToString()
        {
            return "GeometryExpression: " + Geometry;
        }

        public override IExtents Extents
        {
            get { return _geometry == null ? null : _geometry.Extents; }
        }

        public override ICoordinateSystem SpatialReference
        {
            get { return _geometry == null ? null : _geometry.SpatialReference; }
        }

        /// <summary>
        /// Gets the geometry
        /// </summary>
        public IGeometry Geometry
        {
            get { return _geometry; }
        }

        public override Boolean Contains(Expression other)
        {
            return Equals(other);
        }

        public override Boolean Equals(Expression other)
        {
            return Equals(other as GeometryExpression);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (_geometry != null
                            ? _geometry.GetHashCode()
                            : 0x1fd3b) ^ 29;
            }
        }

        public override Expression Clone()
        {
            return new GeometryExpression(_geometry.Clone());
        }

        public override Boolean Equals(SpatialExpression other)
        {
            var geometryExpression = other as GeometryExpression;

            return geometryExpression != null && Equals(_geometry, geometryExpression._geometry);
        }

        public override bool IsNull
        {
            get { return _geometry == null; }
        }

        public override bool IsEmpty
        {
            get { return _geometry != null && _geometry.IsEmpty; }
        }
    }
}