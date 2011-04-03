using System;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NUnit.Framework;
using SharpMap.Utilities;

namespace ProjNet.Tests.V2
{
    [TestFixture]
    public class V2Fixture : AbstractFixture
    {
        [Test]
        public void LatLonToGoogle()
        {
            IGeometryServices services = new GeometryServices();
            double[] data = new[] { -74.008573, 40.711946 };

            ICoordinateSystemFactory coordinateSystemFactory = services.CoordinateSystemFactory;
            ICoordinateSystem source = CrsFor(4326, coordinateSystemFactory);
            ICoordinateSystem target = CrsFor(900913, coordinateSystemFactory);
            Assert.That(source, Is.Not.Null);
            Assert.That(target, Is.Not.Null);

            ICoordinateTransformationFactory transformationFactory = services.CoordinateTransformationFactory;
            ICoordinateTransformation transformation = transformationFactory.CreateFromCoordinateSystems(source, target);
            Assert.That(transformation, Is.Not.Null);
            IMathTransform mathTransform = transformation.MathTransform;
            Assert.That(mathTransform, Is.Not.Null);

            ICoordinateFactory coordinateFactory = services.CoordinateFactory;
            ICoordinate coordinate = coordinateFactory.Create(data);
            Assert.That(coordinate, Is.Not.Null);

            ICoordinate converted = mathTransform.Transform(coordinate);
            Assert.That(converted, Is.Not.Null);

            double x = converted[Ordinates.X];
            double y = converted[Ordinates.Y];
            Console.WriteLine("x: {0}, y: {1}", x, y);

            const double ex = -8238596.6606968148d;
            const double ey = 4969946.1600651201d;                              
            Assert.That(x, Is.EqualTo(ex), String.Format("XConv error: {0}", (int)(ex - x)));
            Assert.That(y, Is.EqualTo(ey), String.Format("YConv error: {0}", (int)(ey - y)));
        }
    }
}
