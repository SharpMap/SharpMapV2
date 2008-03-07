using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.Utilities;
using SharpMap.SimpleGeometries.Geometries3D;
using GeoAPI.DataStructures;

namespace SharpMap.SimpleGeometries
{
    public class GeometryFactory : IGeometryFactory
    {
        private readonly ICoordinateFactory _coordFactory;
        private readonly ICoordinateSequenceFactory _sequenceFactory;
        private Int32? _srid;
        private ICoordinateSystem _spatialReference;
        private BoundingBoxSpatialOperations _spatialOps;

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
            _spatialOps = new BoundingBoxSpatialOperations(this);
        }

        internal BoundingBoxSpatialOperations SpatialOps
        {
            get { return _spatialOps; }
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
            get { throw new NotSupportedException(); }
        }

        public IGeometry BuildGeometry(IEnumerable<IGeometry> geometryList)
        {
            throw new NotSupportedException();
        }

        public IExtents CreateExtents()
        {
            return new Extents(this, new Extents[0]);
        }

        public IExtents CreateExtents(ICoordinate min, ICoordinate max)
        {
            return new Extents(this, min[Ordinates.X], min[Ordinates.Y], max[Ordinates.X], max[Ordinates.Y]);
        }

        public IGeometry CreateGeometry(IGeometry g)
        {
            throw new NotSupportedException();
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
            Point p = new Point(this, x, y);
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
            Point3D p = new Point3D(this, x, y, z);
            return p;
        }

        public IPoint3D CreatePoint3D(IPoint2D point2D, Double z)
        {
            Point3D p = new Point3D(this, point2D.X, point2D.Y, z);
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

        public ILinearRing CreateLinearRing(IEnumerable<Point> coordinates)
        {
            LinearRing l = new LinearRing(Enumerable.Upcast<IPoint, Point>(coordinates));
            l.Factory = this;
            return l;
        }

        public IPolygon CreatePolygon()
        {
            Polygon p = new Polygon();
            p.Factory = this;
            return p;
        }

        public IPolygon CreatePolygon(IEnumerable<ICoordinate> shell)
        {
            LinearRing ring = CreateLinearRing(shell) as LinearRing;
            ring.Factory = this;
            Polygon p = new Polygon(ring);
            p.Factory = this;
            return p;
        }

        public IPolygon CreatePolygon(ILinearRing shell)
        {
            LinearRing l = shell as LinearRing;

            if(l == null)
            {
                throw new ArgumentException(
                    "Parameter must be a non-null LinearRing instance");
            }

            //l.Factory = this;
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

        public IMultiPoint CreateMultiPoint(IEnumerable<IPoint> points)
        {
            MultiPoint mp = new MultiPoint();
            mp.Factory = this;

            foreach (Point point in points)
            {
                mp.Add(point);
            }

            return mp;
        }

        public IMultiPoint CreateMultiPoint(ICoordinateSequence coordinates)
        {
            MultiPoint mp = new MultiPoint();
            mp.Factory = this;
            mp.Coordinates = coordinates;
            return mp;
        }

        public IMultiLineString CreateMultiLineString()
        {
            MultiLineString mls = new MultiLineString();
            mls.Factory = this;
            return mls;
        }

        public IMultiLineString CreateMultiLineString(IEnumerable<ILineString> lineStrings)
        {
            MultiLineString mls = new MultiLineString();
            mls.Factory = this;

            foreach (LineString lineString in lineStrings)
            {
                mls.Add(lineString);
            }

            return mls;
        }

        public IMultiPolygon CreateMultiPolygon()
        {
            MultiPolygon mp = new MultiPolygon();
            mp.Factory = this;
            return mp;
        }

        public IMultiPolygon CreateMultiPolygon(IEnumerable<IPolygon> polygons)
        {
            MultiPolygon mp = new MultiPolygon();
            mp.Factory = this;

            foreach (Polygon polygon in polygons)
            {
                mp.Add(polygon);
            }

            return mp;
        }

        public IGeometryCollection CreateGeometryCollection()
        {
            GeometryCollection collection = new GeometryCollection(4);
            collection.Factory = this;
            return collection;
        }

        public IGeometryCollection CreateGeometryCollection(IEnumerable<IGeometry> geometries)
        {
            GeometryCollection collection = new GeometryCollection(16);
            collection.Factory = this;

            foreach (Geometry geometry in geometries)
            {
                collection.Add(geometry.Clone());
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
            ring.Factory = this;
            Polygon p = new Polygon(ring);
            p.Factory = this;
            return p;
        }

        public IExtents CreateExtents(IExtents first)
        {
            return new Extents(this, first);
        }

        public IExtents CreateExtents(IExtents first, IExtents second)
        {
            return new Extents(this, first, second);
        }

        public IExtents CreateExtents(IExtents first, IExtents second, IExtents third)
        {
            return new Extents(this, first, second, third);
        }

        public IExtents CreateExtents(params IExtents[] extents)
        {
            return new Extents(this, extents);
        }

        public IExtents2D CreateExtents2D(Double left, Double bottom, Double right, Double top)
        {
            return new Extents(this, left, bottom, right, top);
        }

        public IExtents2D CreateExtents2D(Pair<Double> lowerLeft, Pair<Double> upperRight)
        {
            throw new NotImplementedException();
        }

        public IExtents3D CreateExtents3D(Double left, Double bottom, Double front, Double right, Double top, Double back)
        {
            throw new NotSupportedException();
        }

        public IExtents3D CreateExtents3D(Triple<Double> lowerLeft, Triple<Double> upperRight)
        {
            throw new NotSupportedException();
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