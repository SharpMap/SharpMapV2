using System;
using NUnit.Framework;
using SharpMap.Geometries;
using SharpMap.Utilities;

namespace SharpMap.Tests.Geometries
{
    [TestFixture]
    public class BoundingBoxTests
    {
        [Test]
        [Ignore("Need to implement test")]
        public void IntersectionTest()
        {
        }

        [Test]
        public void JoinTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 50);
            BoundingBox b2 = new BoundingBox(-20, 56, 70, 75);
            BoundingBox joined = new BoundingBox(-20, 30, 70, 75);
            Assert.AreEqual(joined, b1.Join(b2));
            Assert.AreEqual(b1, b1.Join(BoundingBox.Empty));
            Assert.AreEqual(joined, BoundingBox.Join(b1, b2));
            Assert.AreEqual(b2, BoundingBox.Join(BoundingBox.Empty, b2));
            Assert.AreEqual(b1, BoundingBox.Join(b1, BoundingBox.Empty));
            Assert.AreEqual(joined, BoundingBox.Join(b2, b1));
            Assert.AreEqual(joined, new BoundingBox(b1, b2));
        }

        [Test]
        public void ExpandToIncludeTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 50);
            BoundingBox b2 = new BoundingBox(-20, 56, 70, 75);
            BoundingBox joined = new BoundingBox(-20, 30, 70, 75);

            BoundingBox composite = BoundingBox.Empty;
            composite.ExpandToInclude(b1);
            Assert.AreEqual(composite, b1);
            composite.ExpandToInclude(b2);
            Assert.AreEqual(composite, joined);
        }

        [Test]
        public void ExpandingAnEmptyBoundingBoxByAnEmptyBoundingBoxShouldBeEmpty()
        {
            BoundingBox b1 = BoundingBox.Empty;
            BoundingBox b2 = BoundingBox.Empty;

            b1.ExpandToInclude(b2);

            Assert.IsTrue(b1.IsEmpty);
            Assert.AreEqual(b1, b2);
        }

        [Test]
        [Ignore("Need to implement test")]
        public void OffsetTest()
        {
        }

        [Test]
        public void MetricsPropertiesTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.IsFalse(b1.IsEmpty);
            Assert.AreEqual(20, b1.Left);
            Assert.AreEqual(20, b1.Min.X);
            Assert.AreEqual(40, b1.Right);
            Assert.AreEqual(40, b1.Max.X);
            Assert.AreEqual(30, b1.Bottom);
            Assert.AreEqual(30, b1.Min.Y);
            Assert.AreEqual(55, b1.Top);
            Assert.AreEqual(55, b1.Max.Y);
            Assert.AreEqual(20, b1.Width);
            Assert.AreEqual(25, b1.Height);

            Assert.AreEqual(new Point(40, 55), b1.UpperRight);
            Assert.AreEqual(new Point(20, 55), b1.UpperLeft);
            Assert.AreEqual(new Point(40, 30), b1.LowerRight);
            Assert.AreEqual(new Point(20, 30), b1.LowerLeft);

            Assert.AreEqual(b1, new BoundingBox(40, 55, 20, 30));
        }

        [Test]
        public void ToStringTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual("[BoundingBox] Lower Left: (20.00, 30.00) Upper Right: (40.00, 55.00)", b1.ToString());
            Assert.AreEqual("[BoundingBox] Empty", BoundingBox.Empty.ToString());
        }

        [Test]
        public void GetHashCodeTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual(b1.GetHashCode(), (Int32) (b1.Left + b1.Bottom + b1.Right + b1.Top));
        }

        [Test]
        public void GetAreaTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual(b1.GetArea(), 20*25);
        }

        [Test]
        public void GetIntersectingAreaTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b2 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b3 = new BoundingBox(10, 20, 30, 45);

            Assert.AreEqual(b1.GetIntersectingArea(b2), 20*25);
            Assert.AreEqual(b1.GetIntersectingArea(b3), 10*15);
            Assert.AreEqual(b1.GetIntersectingArea(BoundingBox.Empty), 0);
        }

        [Test]
        public void DistanceTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual(Math.Sqrt(200), b1.Distance(new BoundingBox(50, 65, 60, 75)));
        }

        [Test]
        public void GetCentroidTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual(new Point(30, 42.5), b1.GetCentroid());
        }

        [Test]
        public void GrowAndShrinkTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.AreEqual(new BoundingBox(19, 29, 41, 56), b1.Grow(1));
        }

        [Test]
        public void ValueOperatorsTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b2 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b3 = new BoundingBox(21, 31, 39, 54);

            Assert.IsTrue(b1 == b2);
            Assert.IsTrue(b1 != b3);
            Assert.IsFalse(b1 != b2);
            Assert.IsFalse(b1 == b3);
        }

        [Test]
        public void EqualityTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b2 = new BoundingBox(20, 30, 40, 55);
            BoundingBox b3 = new BoundingBox(21, 31, 39, 54);
            Assert.IsFalse(b1.Equals(null));
            Assert.IsFalse(b1.Equals(BoundingBox.Empty));
            Assert.IsFalse(b1.Equals(new object()));
            Assert.IsTrue(b1.Equals(b2));

            Assert.IsTrue(b1.Equals(b3, new Tolerance(1.1)));
            Assert.IsFalse(b1.Equals(b3, new Tolerance(0.9)));
        }

        [Test]
        [Ignore("Need to implement test")]
        public void SplitTest()
        {
        }

        [Test]
        [Ignore("Need to implement test")]
        public void BordersTest()
        {
        }

        [Test]
        public void ContainsTest()
        {
            BoundingBox b1 = new BoundingBox(20, 30, 40, 55);
            Assert.IsTrue(b1.Contains(new Point(30, 40)));
            Assert.IsTrue(b1.Contains(new Point(20, 40)));
            Assert.IsFalse(b1.Contains(new Point(10, 10)));
            Assert.IsFalse(b1.Contains(new Point(50, 60)));
            Assert.IsFalse(b1.Contains(new Point(30, 60)));
            Assert.IsFalse(b1.Contains(new Point(50, 40)));
            Assert.IsFalse(b1.Contains(new Point(30, 15)));
        }

        [Test]
        public void IntersectTest()
        {
            //Test disjoint
            BoundingBox b1 = new BoundingBox(0, 0, 10, 10);
            BoundingBox b2 = new BoundingBox(20, 20, 30, 30);
            Assert.IsFalse(b1.Intersects(b2), "Bounding box intersect test 1a failed");
            Assert.IsFalse(b2.Intersects(b1), "Bounding box intersect test 1a failed");
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(0, 20, 10, 30);
            Assert.IsFalse(b1.Intersects(b2), "Bounding box intersect test 1b failed");
            Assert.IsFalse(b2.Intersects(b1), "Bounding box intersect test 1b failed");
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(20, 0, 30, 10);
            Assert.IsFalse(b1.Intersects(b2), "Bounding box intersect test 1c failed");
            Assert.IsFalse(b2.Intersects(b1), "Bounding box intersect test 1c failed");

            //Test intersects
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(5, 5, 15, 15);
            Assert.IsTrue(b1.Intersects(b2), "Bounding box intersect test 2 failed");
            Assert.IsTrue(b2.Intersects(b1), "Bounding box intersect test 2 failed");

            //Test overlaps
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(-5, -5, 15, 15);
            Assert.IsTrue(b1.Intersects(b2), "Bounding box intersect test 3 failed");
            Assert.IsTrue(b2.Intersects(b1), "Bounding box intersect test 3 failed");

            //Test touches
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(10, 0, 20, 10);
            Assert.IsTrue(b1.Intersects(b2), "Bounding box intersect test 4a failed");
            Assert.IsTrue(b2.Intersects(b1), "Bounding box intersect test 4a failed");

            //Test touches 2
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(10, 10, 20, 20);
            Assert.IsTrue(b1.Intersects(b2), "Bounding box intersect test 4b failed");
            Assert.IsTrue(b2.Intersects(b1), "Bounding box intersect test 4b failed");

            //Test equal
            b1 = new BoundingBox(0, 0, 10, 10);
            b2 = new BoundingBox(0, 0, 10, 10);
            Assert.IsTrue(b1.Intersects(b2), "Bounding box intersect test 5 failed");
            Assert.IsTrue(b2.Intersects(b1), "Bounding box intersect test 5 failed");
        }

        [Test]
        [Ignore("Need to implement test")]
        public void OverlapsTest()
        {
        }

        [Test]
        [Ignore("Need to implement test")]
        public void TouchesTest()
        {
        }

        [Test]
        [Ignore("Need to implement test")]
        public void WithinTest()
        {
        }
    }
}