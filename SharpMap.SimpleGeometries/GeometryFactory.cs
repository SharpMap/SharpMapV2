using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;

namespace SharpMap.SimpleGeometries
{
    public class GeometryFactory : IGeometryFactory
    {
        #region IGeometryFactory Members

        public ICoordinateFactory CoordinateFactory
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinateSequenceFactory CoordinateSequenceFactory
        {
            get { throw new NotImplementedException(); }
        }

        public int? Srid
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinateSystem SpatialReference
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IPrecisionModel PrecisionModel
        {
            get { throw new NotImplementedException(); }
        }

        public IGeometry BuildGeometry(IEnumerable<IGeometry> geometryList)
        {
            throw new NotImplementedException();
        }

        public IExtents CreateExtents()
        {
            throw new NotImplementedException();
        }

        public IExtents CreateExtents(ICoordinate min, ICoordinate max)
        {
            throw new NotImplementedException();
        }

        public IGeometry CreateGeometry(IGeometry g)
        {
            throw new NotImplementedException();
        }

        public IGeometry CreateGeometry(ICoordinateSequence coordinates, OgcGeometryType type)
        {
            throw new NotImplementedException();
        }

        public IPoint CreatePoint()
        {
            throw new NotImplementedException();
        }

        public IPoint CreatePoint(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public IPoint CreatePoint(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        public IPoint2D CreatePoint2D()
        {
            throw new NotImplementedException();
        }

        public IPoint2D CreatePoint2D(double x, double y)
        {
            throw new NotImplementedException();
        }

        public IPoint2DM CreatePoint2DM(double x, double y, double m)
        {
            throw new NotImplementedException();
        }

        public IPoint3D CreatePoint3D()
        {
            throw new NotImplementedException();
        }

        public IPoint3D CreatePoint3D(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        public IPoint3D CreatePoint3D(IPoint2D point2D, double z)
        {
            throw new NotImplementedException();
        }

        public IPoint3DM CreatePoint3DM(double x, double y, double z, double m)
        {
            throw new NotImplementedException();
        }

        public ILineString CreateLineString()
        {
            throw new NotImplementedException();
        }

        public ILineString CreateLineString(IEnumerable<ICoordinate> coordinates)
        {
            throw new NotImplementedException();
        }

        public ILineString CreateLineString(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        public ILinearRing CreateLinearRing()
        {
            throw new NotImplementedException();
        }

        public ILinearRing CreateLinearRing(IEnumerable<ICoordinate> coordinates)
        {
            throw new NotImplementedException();
        }

        public ILinearRing CreateLinearRing(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        public IPolygon CreatePolygon()
        {
            throw new NotImplementedException();
        }

        public IPolygon CreatePolygon(IEnumerable<ICoordinate> shell)
        {
            throw new NotImplementedException();
        }

        public IPolygon CreatePolygon(ILinearRing shell)
        {
            throw new NotImplementedException();
        }

        public IPolygon CreatePolygon(ILinearRing shell, IEnumerable<ILinearRing> holes)
        {
            throw new NotImplementedException();
        }

        public IMultiPoint CreateMultiPoint()
        {
            throw new NotImplementedException();
        }

        public IMultiPoint CreateMultiPoint(IEnumerable<ICoordinate> coordinates)
        {
            throw new NotImplementedException();
        }

        public IMultiPoint CreateMultiPoint(IEnumerable<IPoint> point)
        {
            throw new NotImplementedException();
        }

        public IMultiPoint CreateMultiPoint(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        public IMultiLineString CreateMultiLineString()
        {
            throw new NotImplementedException();
        }

        public IMultiLineString CreateMultiLineString(IEnumerable<ILineString> lineStrings)
        {
            throw new NotImplementedException();
        }

        public IMultiPolygon CreateMultiPolygon()
        {
            throw new NotImplementedException();
        }

        public IMultiPolygon CreateMultiPolygon(IEnumerable<IPolygon> polygons)
        {
            throw new NotImplementedException();
        }

        public IGeometryCollection CreateGeometryCollection()
        {
            throw new NotImplementedException();
        }

        public IGeometryCollection CreateGeometryCollection(IEnumerable<IGeometry> geometries)
        {
            throw new NotImplementedException();
        }

        public IGeometry ToGeometry(IExtents envelopeInternal)
        {
            throw new NotImplementedException();
        }

        public IExtents CreateExtents(IExtents first, IExtents second)
        {
            throw new NotImplementedException();
        }

        public IExtents CreateExtents(IExtents first, IExtents second, IExtents third)
        {
            throw new NotImplementedException();
        }

        public IExtents CreateExtents(params IExtents[] extents)
        {
            throw new NotImplementedException();
        }

        public IExtents2D CreateExtents2D(double left, double bottom, double right, double top)
        {
            throw new NotImplementedException();
        }

        public IExtents2D CreateExtents2D(GeoAPI.DataStructures.Pair<double> lowerLeft, GeoAPI.DataStructures.Pair<double> upperRight)
        {
            throw new NotImplementedException();
        }

        public IExtents3D CreateExtents3D(double left, double bottom, double front, double right, double top, double back)
        {
            throw new NotImplementedException();
        }

        public IExtents3D CreateExtents3D(GeoAPI.DataStructures.Triple<double> lowerLeft, GeoAPI.DataStructures.Triple<double> upperRight)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}