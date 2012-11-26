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
    /// An expression which represents an <see cref="IExtents"/> in a
    /// compound expression or an expression tree.
    /// </summary>
    [Serializable]
    public class ExtentsExpression : SpatialExpression
    {
        private readonly IExtents _extents;

        public ExtentsExpression(IExtents extents)
        {
            if (extents == null) throw new ArgumentNullException("extents");

            _extents = extents;
        }

        public override string ToString()
        {
            return Extents.ToString();
        }

        public override IExtents Extents
        {
            get { return _extents; }
        }

        public override ICoordinateSystem SpatialReference
        {
            get { return _extents == null ? null : _extents.SpatialReference; }
        }

        public override Boolean Contains(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new ExtentsExpression(_extents.Clone() as IExtents);
        }

        public override Boolean Equals(Expression other)
        {
            return other != null && Equals(other as SpatialExpression);
        }

        public override Boolean Equals(SpatialExpression other)
        {
            ExtentsExpression extentsExpression = other as ExtentsExpression;

            return extentsExpression != null && Equals(_extents, extentsExpression._extents);
        }

        public override Boolean IsNull
        {
            get { return _extents == null; }
        }

        public override Boolean IsEmpty
        {
            get { return _extents != null && _extents.IsEmpty; }
        }
    }
}
