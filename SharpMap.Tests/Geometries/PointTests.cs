using System;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NUnit.Framework;

namespace SharpMap.Tests.Geometries
{
    [TestFixture]
    public class PointTests
    {
        private static readonly IGeometryFactory _geoFactory
            = new SharpMap.SimpleGeometries.GeometryFactory();

        [Test]
        public void IPoint()
        {
            //Do various IPoint method calls to cover the point class with sufficient testing
            IPoint2D p0 = _geoFactory.CreatePoint() as IPoint2D;
            IPoint2D p1 = _geoFactory.CreatePoint2D(0, 0);
            IPoint2D p2 = _geoFactory.CreatePoint2D(450, 120);

            Assert.IsTrue(p0.IsEmpty);
            Assert.IsFalse(p1.IsEmpty);
            Assert.AreNotEqual(p0, p1);
            Assert.AreEqual(450, p2.X);
            Assert.AreEqual(120, p2.Y);
            Assert.AreNotSame(p2.Clone(), p2);
            p0 = _geoFactory.CreatePoint2D(p2.X + 100, 150);
            p0 = _geoFactory.CreatePoint2D(p0.X + p0.Y, p0.Y);
            Assert.AreEqual(_geoFactory.CreatePoint2D(700, 150), p0);
            Assert.AreEqual(p2.Coordinate, p2.Extents.Min);
            Assert.AreEqual(p2.Coordinate, p2.Extents.Max);
            Assert.IsTrue(p2.IsSimple);
            Assert.IsFalse(p2.IsEmpty);
            Assert.AreEqual(2, p2.OrdinateCount);
            Assert.AreEqual(_geoFactory.CreatePoint2D(400, 100), p2.Add(_geoFactory.CreatePoint2D(-50, -20)));
            Assert.AreEqual(_geoFactory.CreatePoint2D(500, 100), p2.Subtract(_geoFactory.CreatePoint2D(-50, 20)));
            Assert.AreEqual(_geoFactory.CreatePoint2D(900, 240), p2.Multiply(2));
            Assert.AreEqual(0, p2.Dimension);
            Assert.AreEqual(450, p2[Ordinates.X]);
            Assert.AreEqual(120, p2[Ordinates.Y]);
            Assert.IsNull(p2.Boundary);
            Assert.AreEqual(p2.X.GetHashCode() ^ p2.Y.GetHashCode() ^ p2.IsEmpty.GetHashCode(), p2.GetHashCode());
            Assert.Greater(p2.CompareTo(p1), 0);
            Assert.Less(p1.CompareTo(p2), 0);
            Assert.AreEqual(p2.CompareTo(_geoFactory.CreatePoint2D(450, 120)), 0);
        }

        [Test]
        public void Point3D()
        {
            //Do various IPoint method calls to cover the point class with sufficient testing
            IPoint3D p0 = _geoFactory.CreatePoint3D();
            IPoint2D p = _geoFactory.CreatePoint2D(23, 21);
            IPoint3D p1 = _geoFactory.CreatePoint3D(450, 120, 34);
            IPoint3D p2 = _geoFactory.CreatePoint3D(p, 94);
            Assert.IsTrue(p0.IsEmpty);
            Assert.IsFalse(p1.IsEmpty);
            Assert.IsFalse(p2.IsEmpty);

            Assert.AreNotEqual(p, p2);
            Assert.AreEqual(94, p2.Z);

            Assert.AreNotSame(p1.Clone(), p1);
            //p0 = p1.Clone();
            //p0.X += 100;
            //p0.Y = 150;
            //p0.Z += 499;
            p0 = _geoFactory.CreatePoint3D(p0, (p0[Ordinates.Z]) + (p0[Ordinates.Z]));
            Assert.AreEqual(_geoFactory.CreatePoint3D(550, 150, 1066), p0);
            Assert.AreEqual(p2.Coordinate, p2.Extents.Min);
            Assert.AreEqual(p2.Coordinate, p2.Extents.Max);
            Assert.AreEqual(3, p2.OrdinateCount);
            Assert.AreEqual(_geoFactory.CreatePoint3D(-27, 1, 123), p2.Add(_geoFactory.CreatePoint3D(-50, -20, 29)));
            Assert.AreEqual(_geoFactory.CreatePoint3D(73, 1, 0), p2.Subtract(_geoFactory.CreatePoint2D(-50, 20)));
            Assert.AreEqual(_geoFactory.CreatePoint3D(46, 42, 188), p2.Multiply(2));
            Assert.AreEqual(0, p2.Dimension);
            Assert.AreEqual(23, p2.X, TestConstants.Epsilon);
            Assert.AreEqual(21, p2.Y, TestConstants.Epsilon);
            Assert.AreEqual(94, p2.Z, TestConstants.Epsilon);
            Assert.IsNull(p2.Boundary);
            Assert.AreEqual(p2.X.GetHashCode() ^ p2.Y.GetHashCode() ^ p2.Z.GetHashCode() ^ p2.IsEmpty.GetHashCode(),
                            p2.GetHashCode());
            Assert.Less(p2.CompareTo(p1), 0);
            Assert.Greater(p1.CompareTo(p2), 0);
            Assert.AreEqual(0, p2.CompareTo(_geoFactory.CreatePoint3D(23, 21, 94)));
            Assert.AreEqual(0, p2.CompareTo(_geoFactory.CreatePoint2D(23, 21)));
        }
    }
}