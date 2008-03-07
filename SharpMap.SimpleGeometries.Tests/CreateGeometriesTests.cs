using GeoAPI.Geometries;
using NetTopologySuite.Coordinates;
using SharpMap.SimpleGeometries;
using NUnit.Framework;

namespace SharpMap.SimpleGeometries.Tests
{
    [TestFixture]
    public class CreateGeometriesTests
    {
        [Test]
        public void CreateEmptyPointSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IPoint p = factory.CreatePoint();
            Assert.IsNotNull(p);
            Assert.IsTrue(p.IsEmpty);
        }

        [Test]
        public void CreateEmptyLineStringSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            ILineString l = factory.CreateLineString();
            Assert.IsNotNull(l);
            Assert.IsTrue(l.IsEmpty);
        }

        [Test]
        public void CreateEmptyLinearRingSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            ILinearRing l = factory.CreateLinearRing();
            Assert.IsNotNull(l);
            Assert.IsTrue(l.IsEmpty);
        }

        [Test]
        public void CreateEmptyPolygonSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IPolygon p = factory.CreatePolygon();
            Assert.IsNotNull(p);
            Assert.IsTrue(p.IsEmpty);
        }

        [Test]
        public void CreateEmptyMultiPointSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IMultiPoint p = factory.CreateMultiPoint();
            Assert.IsNotNull(p);
            Assert.IsTrue(p.IsEmpty);
        }

        [Test]
        public void CreateEmptyMultiLineStringSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IMultiLineString l = factory.CreateMultiLineString();
            Assert.IsNotNull(l);
            Assert.IsTrue(l.IsEmpty);
        }

        [Test]
        public void CreateEmptyMultiPolygonSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IMultiPolygon p = factory.CreateMultiPolygon();
            Assert.IsNotNull(p);
            Assert.IsTrue(p.IsEmpty);
        }

        [Test]
        public void CreateEmptyGeometryCollectionSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IGeometryCollection g = factory.CreateGeometryCollection();
            Assert.IsNotNull(g);
            Assert.IsTrue(g.IsEmpty);
        }
    }
}
