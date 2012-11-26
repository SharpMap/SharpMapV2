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
using System.Runtime.Serialization;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;

namespace SharpMap.Expressions
{
    /// <summary>
    /// An expression which represents an <see cref="IGeometry"/> in a
    /// compound expression or an expression tree.
    /// </summary>
    [Serializable]
    public class GeometryExpression : SpatialExpression, ISerializable
    {
        private readonly IGeometry _geometry;

        public GeometryExpression(IGeometry geometry)
        {
            _geometry = geometry;
        }

        protected GeometryExpression(SerializationInfo info, StreamingContext context)
        {
            if (!info.GetBoolean("isnull"))
            {
                var gs = context.Context as Utilities.IGeometryServices;
                if (gs == null)
                    throw new ArgumentException("The context does not provide access to a 'IGeometryServices' class", "context");
                var reader = gs[info.GetString("srid")];
                _geometry = reader.WkbReader.Read((byte[]) info.GetValue("wkb", typeof (byte[])));
            }
        }

        public override string ToString()
        {
            return _geometry != null ? _geometry.ToString() : "<null>";
        }

        public override IExtents Extents
        {
            get { return _geometry == null ? null : _geometry.Extents; }
        }

        public override ICoordinateSystem SpatialReference
        {
            get { return _geometry == null ? null : _geometry.SpatialReference; }
        }

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("isnull", IsNull);
            info.AddValue("srid", _geometry.Srid);
            info.AddValue("wkb", _geometry.AsBinary());
        }

        public override Expression Clone()
        {
            return new GeometryExpression(_geometry.Clone());
        }

        public override Boolean Equals(SpatialExpression other)
        {
            GeometryExpression geometryExpression = other as GeometryExpression;

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
