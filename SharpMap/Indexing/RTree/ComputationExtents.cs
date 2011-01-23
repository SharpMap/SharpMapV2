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
using GeoAPI.Coordinates;
using GeoAPI.Geometries;

namespace SharpMap.Indexing.RTree
{
    class ComputationExtents
    {
        private readonly Int32 _hasValue;
        public readonly Double XMin;
        public readonly Double XMax;
        public readonly Double YMin;
        public readonly Double YMax;

        public ComputationExtents(IExtents extents)
        {
            if (extents == null) throw new ArgumentNullException("extents");

            ICoordinate min = extents.Min;
            ICoordinate max = extents.Max;

            XMin = min[Ordinates.X];
            YMin = min[Ordinates.Y];
            XMax = max[Ordinates.X];
            YMax = max[Ordinates.Y];

            _hasValue = 1;
        }

        public ComputationExtents(Double xMin, Double yMin, Double xMax, Double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            _hasValue = 1;
        }

        public ComputationExtents()
        {
            //throw new NotImplementedException();
        }

        public Boolean IsEmpty
        {
            get { return _hasValue == 0; }
        }

        public ComputationExtents Union(ComputationExtents other)
        {
            if (other.IsEmpty)
            {
                return this;
            }

            if (IsEmpty)
            {
                return other;
            }

            return new ComputationExtents(
                Math.Min(XMin, other.XMin),
                Math.Min(YMin, other.YMin),
                Math.Max(XMax, other.XMax),
                Math.Max(YMax, other.YMax));
        }

        public ComputationExtents Intersection(ComputationExtents other)
        {
            if (XMin > other.XMax || XMax < other.XMin
                || YMin > other.YMax || YMax < other.YMin)
            {
                return new ComputationExtents();
            }

            return new ComputationExtents(
                Math.Max(XMin, other.XMin),
                Math.Max(YMin, other.YMin),
                Math.Min(XMax, other.XMax),
                Math.Min(YMax, other.YMax));
        }

        public Double Area
        {
            get { return _hasValue * (XMax - XMin) * (YMax - YMin); }
        }
    }
}
