using System.Collections.ObjectModel;
using NUnit.Framework;
using SharpMap.Geometries;
using GeoProvider = SharpMap.Data.Providers.GeometryProvider.GeometryProvider;

namespace SharpMap.Tests.Data.Providers.GeometryProvider
{
	[TestFixture]
	public class GeometryProviderTests
	{
        [Test]
        public void CreatingGeometryProviderWithWktShouldContainSpecifiedGeometries()
        {
            GeoProvider provider = new GeoProvider("POINT EMPTY");
            Assert.AreEqual(1, provider.Geometries.Count);
            Assert.AreEqual(Point.Empty, provider.Geometries[0]);
        }

        [Test]
        public void CreatingGeometryProviderWithWkbShouldContainSpecifiedGeometries()
        {
            Point[] points = new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(0, 0) };
            Polygon p = new Polygon(new LinearRing(points));
            byte[] wkb = p.AsBinary();
            GeoProvider provider = new GeoProvider(wkb);
            Assert.AreEqual(1, provider.Geometries.Count);
            Assert.AreEqual(p, provider.Geometries[0]);
        }
	}
}