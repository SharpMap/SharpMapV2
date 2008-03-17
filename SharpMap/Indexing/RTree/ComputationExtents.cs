using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;

namespace SharpMap.Indexing.RTree
{
    struct ComputationExtents
    {
        private readonly Int32 _hasValue;
        public readonly Double XMin;
        public readonly Double XMax;
        public readonly Double YMin;
        public readonly Double YMax;

        public ComputationExtents(IExtents extents)
        {
            ICoordinate min = extents.Min;
            ICoordinate max = extents.Max;

            XMin = min[Ordinates.X];
            YMin = min[Ordinates.Y];
            XMax = max[Ordinates.X];
            YMax = max[Ordinates.Y];

            _hasValue = 1;
        }

        private ComputationExtents(Double xMin, Double yMin, Double xMax, Double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            _hasValue = 1;
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
