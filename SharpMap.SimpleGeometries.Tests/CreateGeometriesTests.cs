using System;
using System.Collections.Generic;
using System.Text;
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
        public void CreateEmptyGeometryCollectionSucceeds()
        {
            GeometryFactory factory = new GeometryFactory(
                new BufferedCoordinate2DFactory(),
                new BufferedCoordinate2DSequenceFactory());

            IGeometryCollection g;// = factory.CreateGeometryCollection();
            g = new GeometryCollection<Point>();
            Assert.IsNotNull(g);
            Assert.IsTrue(g.IsEmpty);
        }
    }
}
