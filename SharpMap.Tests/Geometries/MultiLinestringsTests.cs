using System.Collections.Generic;
using GeoAPI.Geometries;
using NUnit.Framework;

namespace SharpMap.Tests.Geometries
{
    [TestFixture]
    public class MultiLinestringsTests
    {
        [Test]
        public void MultiLinestring()
        {
            IMultiLineString mls = new IMultiLineString();
            Assert.IsTrue(mls.IsEmpty);
            mls.Add(new ILineString());
            Assert.IsTrue(mls.IsEmpty);
            mls[0].Coordinates.Add(new Point(45, 68));
            mls[0].Coordinates.Add(new Point(82, 44));
            mls.Add(CreateLineString());

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
            foreach (ILineString ls in mls)
            {
                ls.Coordinates.Add((ls.StartPoint.Clone() as IPoint).Coordinate);
            }

            foreach (ILineString ls in mls)
            {
                Assert.IsTrue(ls.IsClosed);
            }

            Assert.IsTrue(mls.IsClosed);
            Assert.AreEqual(new IExtents(1, 2, 930, 123), mls.Extents);
        }

        private ILineString CreateLineString()
        {
            ILineString ls = new ILineString();
            ls.Coordinates.Add(new Point(1, 2));
            ls.Coordinates.Add(new Point(10, 22));
            ls.Coordinates.Add(new Point(930, 123));
            return ls;
        }
    }
}