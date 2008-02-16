using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.Utilities;
using SharpMap.SimpleGeometries.Geometries3D;

namespace SharpMap.SimpleGeometries
{
    public class GeometryFactory : IGeometryFactory
    {
        private readonly ICoordinateFactory _coordFactory;
        private readonly ICoordinateSequenceFactory _sequenceFactory;
        private Int32? _srid;
        private ICoordinateSystem _spatialReference;

        public GeometryFactory(ICoordinateFactory coordFactory, ICoordinateSequenceFactory sequenceFactory)
            : this(coordFactory, sequenceFactory, null, null) { }

        public GeometryFactory(ICoordinateFactory coordFactory, ICoordinateSequenceFactory sequenceFactory, Int32? srid)
            : this(coordFactory, sequenceFactory, srid, null) { }

        public GeometryFactory(ICoordinateFactory coordFactory, ICoordinateSequenceFactory sequenceFactory, Int32? srid, ICoordinateSystem spatialReference)
        {
            _coordFactory = coordFactory;
            _sequenceFactory = sequenceFactory;
            _srid = srid;
            _spatialReference = spatialReference;
        }

        #region IGeometryFactory Members

        public ICoordinateFactory CoordinateFactory
        {
            get { return _coordFactory; }
        }

        public ICoordinateSequenceFactory CoordinateSequenceFactory
        {
            get { return _sequenceFactory; }
        }

        public Int32? Srid
        {
            get { return _srid; }
            set { _srid = value; }
        }

        public ICoordinateSystem SpatialReference
        {
            get
            {
                return _spatialReference;
            }
            set
            {
                _spatialReference = value;
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
            return new Extents();
        }

        public IExtents CreateExtents(ICoordinate min, ICoordinate max)
        {
            return new Extents(min[Ordinates.X], min[Ordinates.Y], max[Ordinates.X], max[Ordinates.Y]);
        }

        public IGeometry CreateGeometry(IGeometry g)
        {
            throw new NotImplementedException();
        }

        public IGeometry CreateGeometry(ICoordinateSequence coordinates, OgcGeometryType type)
        {
            switch (type)
            {
                case OgcGeometryType.Point:
                    return CreatePoint(coordinates);
                case OgcGeometryType.LineString:
                    return CreateLineString(coordinates);
                case OgcGeometryType.Polygon:
                    return CreatePolygon(coordinates);
                case OgcGeometryType.MultiPoint:
                    return CreateMultiPoint(coordinates);
                case OgcGeometryType.MultiPolygon:
                    return CreateMultiPolygon(coordinates);
                case OgcGeometryType.MultiLineString:
                case OgcGeometryType.GeometryCollection:
                case OgcGeometryType.Geometry:
                case OgcGeometryType.Curve:
                case OgcGeometryType.Surface:
                case OgcGeometryType.MultiSurface:
                case OgcGeometryType.MultiCurve:
                default:
                    throw new NotSupportedException("Geometry type not supported: " + type);
            }
        }

        public IPoint CreatePoint()
        {
            Point p = new Point();
            p.Factory = this;
            return p;
        }

        public IPoint CreatePoint(ICoordinate coordinate)
        {
            Point p = new Point(coordinate);
            p.Factory = this;
            return p;
        }

        public IPoint CreatePoint(ICoordinateSequence coordinates)
        {
            Point p = new Point(coordinates[0]);
            p.Factory = this;
            return p;
        }

        public IPoint2D CreatePoint2D()
        {
            Point p = new Point();
            p.Factory = this;
            return p;
        }

        public IPoint2D CreatePoint2D(Double x, Double y)
        {
            Point p = new Point(x, y);
            p.Factory = this;
            return p;
        }

        public IPoint2DM CreatePoint2DM(Double x, Double y, Double m)
        {
            throw new NotImplementedException();
        }

        public IPoint3D CreatePoint3D()
        {
            Point3D p = new Point3D();
            p.Factory = this;
            return p;
        }

        public IPoint3D CreatePoint3D(Double x, Double y, Double z)
        {
            Point3D p = new Point3D(x, y, z);
            p.Factory = this;
            return p;
        }

        public IPoint3D CreatePoint3D(IPoint2D point2D, Double z)
        {
            Point3D p = new Point3D(point2D.X, point2D.Y, z);
            p.Factory = this;
            return p;
        }

        public IPoint3DM CreatePoint3DM(Double x, Double y, Double z, Double m)
        {
            throw new NotImplementedException();
        }

        public ILineString CreateLineString()
        {
            LineString l = new LineString();
            l.Factory = this;
            return l;
        }

        public ILineString CreateLineString(IEnumerable<ICoordinate> coordinates)
        {
            LineString l = new LineString(coordinates);
            l.Factory = this;
            return l;
        }

        public ILineString CreateLineString(ICoordinateSequence coordinates)
        {
            return CreateLineString((IEnumerable<ICoordinate>)coordinates);
        }

        public ILinearRing CreateLinearRing()
        {
            LinearRing l = new LinearRing();
            l.Factory = this;
            return l;
        }

        public ILinearRing CreateLinearRing(IEnumerable<ICoordinate> coordinates)
        {
            LinearRing l = new LinearRing(coordinates);
            l.Factory = this;
            return l;
        }

        public ILinearRing CreateLinearRing(ICoordinateSequence coordinates)
        {
            return CreateLinearRing((IEnumerable<ICoordinate>)coordinates);
        }

        public IPolygon CreatePolygon()
        {
            Polygon p = new Polygon();
            p.Factory = this;
            return p;
        }

        public IPolygon CreatePolygon(IEnumerable<ICoordinate> shell)
        {
            Polygon p = new Polygon(shell);
            p.Factory = this;
            return p;
        }

        public IPolygon CreatePolygon(ILinearRing shell)
        {
            LinearRing l = shell as LinearRing;

            if(l == null)
            {
                throw new ArgumentException("Parameter must be a non-null LinearRing instance");
            }

            Polygon p = new Polygon(l);
            p.Factory = this;
            return p;
        }

        public IPolygon CreatePolygon(ILinearRing shell, IEnumerable<ILinearRing> holes)
        {
            LinearRing l = shell as LinearRing;

            if (l == null)
            {
                throw new ArgumentException("Parameter must be a non-null LinearRing instance");
            }

            Polygon p = new Polygon(l, Enumerable.Downcast<LinearRing, ILinearRing>(holes));
            p.Factory = this;
            return p;
        }

        public IMultiPoint CreateMultiPoint()
        {
            MultiPoint p = new MultiPoint();
            p.Factory = this;
            return p;
        }

        public IMultiPoint CreateMultiPoint(IEnumerable<ICoordinate> coordinates)
        {
            throw new NotImplementedException();
            //MultiPoint p = new MultiPoint(coordinates);
            //p.Factory = this;
            //return p;
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
            MultiLineString l = new MultiLineString();
            l.Factory = this;
            return l;
        }

        public IMultiLineString CreateMultiLineString(IEnumerable<ILineString> lineStrings)
        {
            throw new NotImplementedException();
        }

        public IMultiPolygon CreateMultiPolygon()
        {
            MultiPolygon p = new MultiPolygon();
            p.Factory = this;
            return p;
        }

        public IMultiPolygon CreateMultiPolygon(IEnumerable<IPolygon> polygons)
        {
            throw new NotImplementedException();
        }

        public IGeometryCollection CreateGeometryCollection()
        {
            return new GeometryCollection(4);
        }

        public IGeometryCollection CreateGeometryCollection(IEnumerable<IGeometry> geometries)
        {
            GeometryCollection collection = new GeometryCollection(16);

            foreach (IGeometry geometry in geometries)
            {
                collection.Add(geometry);
            }

            return collection;
        }

        public IGeometry ToGeometry(IExtents envelopeInternal)
        {
            ICoordinate[] verticies = new ICoordinate[5];

            ICoordinate min = envelopeInternal.Min;
            ICoordinate max = envelopeInternal.Max;

            verticies[0] = max;
            verticies[1] = CoordinateFactory.Create(min[Ordinates.X], max[Ordinates.Y]);
            verticies[2] = min;
            verticies[3] = CoordinateFactory.Create(max[Ordinates.X], min[Ordinates.Y]);
            verticies[4] = max;

            LinearRing ring = new LinearRing(verticies);
            return new Polygon(ring);
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

        public IExtents2D CreateExtents2D(Double left, Double bottom, Double right, Double top)
        {
            throw new NotImplementedException();
        }

        public IExtents2D CreateExtents2D(GeoAPI.DataStructures.Pair<Double> lowerLeft, GeoAPI.DataStructures.Pair<Double> upperRight)
        {
            throw new NotImplementedException();
        }

        public IExtents3D CreateExtents3D(Double left, Double bottom, Double front, Double right, Double top, Double back)
        {
            throw new NotImplementedException();
        }

        public IExtents3D CreateExtents3D(GeoAPI.DataStructures.Triple<Double> lowerLeft, GeoAPI.DataStructures.Triple<Double> upperRight)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGeometryFactory Members


        public IPolygon CreatePolygon(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        public IMultiPolygon CreateMultiPolygon(ICoordinateSequence coordinates)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}