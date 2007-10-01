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
using SharpMap.Geometries;

namespace SharpMap.Query
{
    public class SpatialQuery : IEquatable<SpatialQuery>
    {
        private readonly Geometry _queryRegion;
        private readonly SpatialQueryType _queryType;

        public SpatialQuery(Geometry queryRegion, SpatialQueryType queryType)
        {
            _queryRegion = queryRegion;
            _queryType = queryType;
        }

        public Geometry QueryRegion
        {
            get { return _queryRegion ?? Point.Empty; }
        }

        public SpatialQueryType QueryType
        {
            get { return _queryType; }
        }

        public static bool operator !=(SpatialQuery lhs, SpatialQuery rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(SpatialQuery lhs, SpatialQuery rhs)
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

        public bool Equals(SpatialQuery other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return _queryRegion == other._queryRegion
                && _queryType == other._queryType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as SpatialQuery);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_queryRegion != null ? _queryRegion.GetHashCode() : 472054336)
                       ^ 29*_queryType.GetHashCode();
            }
        }

        public SpatialQuery Clone()
        {
            SpatialQuery clone = new SpatialQuery(
                _queryRegion == null ? null : _queryRegion.Clone(), QueryType);

            return clone;
        }
    }
}
