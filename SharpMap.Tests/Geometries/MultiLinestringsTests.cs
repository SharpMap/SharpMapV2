using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NUnit.Framework;

namespace SharpMap.Tests.Geometries
{
    [TestFixture]
    public class MultiLinestringsTests
    {
        private static readonly IGeometryFactory _geoFactory 
            = new SharpMap.SimpleGeometries.GeometryFactory();

        [Test]
        public void MultiLinestring()
        {
            IMultiLineString mls = _geoFactory.CreateMultiLineString();
            Assert.IsTrue(mls.IsEmpty);
            mls.Add(_geoFactory.CreateLineString());
            Assert.IsTrue(mls.IsEmpty);
            mls[0].Coordinates.Add(_geoFactory.CreatePoint2D(45, 68));
            mls[0].Coordinates.Add(_geoFactory.CreatePoint2D(82, 44));
            mls.Add(createLineString());

            foreach (ILineString ls in (IEnumerable<ILineString>)mls)
            {
                Assert.IsFalse(ls.IsEmpty);
            }

            Assert.IsFalse(mls.IsEmpty);

            foreach (ILineString ls in (IEnumerable<ILineString>)mls)
            {
                Assert.IsFalse(ls.IsClosed);
            }

            Assert.IsFalse(mls.IsClosed);

            //Close linestrings
            foreach (ILineString ls in (IEnumerable<ILineString>)mls)
            {
                ls.Coordinates.Add((ls.StartPoint.Clone() as IPoint).Coordinate);
            }

            foreach (ILineString ls in (IEnumerable<ILineString>)mls)
            {
                Assert.IsTrue(ls.IsClosed);
            }

            Assert.IsTrue(mls.IsClosed);
            Assert.AreEqual(_geoFactory.CreateExtents2D(1, 2, 930, 123), mls.Extents);
        }

        private ILineString createLineString()
        {
            ICoordinateSequence coordinates
                = _geoFactory.CoordinateSequenceFactory.Create(generateCoordinates());
            ILineString ls = _geoFactory.CreateLineString(coordinates);
            return ls;
        }

        private static IEnumerable<ICoordinate> generateCoordinates()
        {
            ICoordinateFactory coordFactory = _geoFactory.CoordinateFactory;
            yield return coordFactory.Create(1, 2);
            yield return coordFactory.Create(10, 22);
            yield return coordFactory.Create(930, 123);
        }
    }
}