using System.Collections.ObjectModel;
using NUnit.Framework;

namespace SharpMap.Tests.Geometries
{
    [TestFixture]
    public class LinestringTests
    {
        [Test]
        public void Linestring()
        {
            LineString l = new LineString();
            Assert.IsTrue(l.IsEmpty());
            Assert.AreEqual(BoundingBox.Empty, l.GetBoundingBox());
            Assert.AreEqual(0, l.Length);
            Assert.IsFalse(l.Equals(null));
            Assert.IsTrue(l.Equals(new LineString()));

            Collection<Point> vertices = new Collection<Point>();
            vertices.Add(new Point(54, 23));
            vertices.Add(new Point(93, 12));
            vertices.Add(new Point(104, 32));

            foreach (Point point in vertices)
            {
                l.Vertices.Add(point);
            }

            Assert.IsFalse(l.IsEmpty());
            Assert.IsFalse(l.IsClosed);
            Assert.AreEqual(3, l.NumPoints);
            Assert.AreEqual(new Point(54, 23), l.StartPoint);
            Assert.AreEqual(new Point(104, 32), l.EndPoint);
            l.Vertices.Add(new Point(54, 23));
            Assert.IsTrue(l.IsClosed);
            Assert.AreEqual(114.15056678325843, l.Length);
            Assert.AreNotSame(l.Clone(), l);
            Assert.AreNotSame((l.Clone() as LineString).Vertices[0], l.Vertices[0]);
            Assert.AreEqual(l.Clone(), l);
            LineString l2 = l.Clone() as LineString;
            l2.Vertices[2] = l2.Vertices[2] + new Point(1, 1);
            Assert.AreNotEqual(l2, l);
            l2 = l.Clone() as LineString;
            l2.Vertices.Add(new Point(34, 23));
            Assert.AreNotEqual(l2, l);
        }
    }
}