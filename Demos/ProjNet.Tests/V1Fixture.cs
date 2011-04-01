namespace ProjNet.Tests
{
    using System;
    using System.Collections.Generic;
    using GeoAPI.CoordinateSystems;
    using GeoAPI.CoordinateSystems.Transformations;
    using NUnit.Framework;
    using SharpMap.Converters.WellKnownText;
    using SharpMap.CoordinateSystems;
    using SharpMap.CoordinateSystems.Transformations;

    [TestFixture]
    public class V1Fixture
    {
        [Test]
        public void LatLonToGoogle()
        {
            double[] data = new[] { -74.008573, 40.711946 };
            ICoordinateSystem source = CrsFor(4326);
            ICoordinateSystem target = CrsFor(900913);
            Assert.That(source, Is.Not.Null);
            Assert.That(target, Is.Not.Null);

            ICoordinateTransformationFactory factory = new CoordinateTransformationFactory();
            ICoordinateTransformation transformation = factory.CreateFromCoordinateSystems(source, target);
            Assert.That(transformation, Is.Not.Null);
            IMathTransform mathTransform = transformation.MathTransform;
            Assert.That(mathTransform, Is.Not.Null);

            double[] converted = mathTransform.Transform(data);
            double x = converted[0];
            double y = converted[1];
            Console.WriteLine("x: {0}, y: {1}", x, y);
            Assert.That(x, Is.EqualTo(-8238596.6606968148d));            
            Assert.That(y, Is.EqualTo(4969946.166007298d));
        }

        private static ICoordinateSystem CrsFor(int srid)
        {
            switch (srid)
            {
                case 4326:
                    {
                        const string source =
                            "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
                        return (ICoordinateSystem)CoordinateSystemWktReader.Parse(source);
                    }

                case 32632:
                    {
                        const string source =
                            "PROJCS[\"WGS 84 / UTM zone 32N\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",9],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"32632\"]]";
                        return (ICoordinateSystem)CoordinateSystemWktReader.Parse(source);
                    }

                case 3857:
                case 900913:
                    return GetMercatorProjection();

                default:
                    var format = String.Format("SRID unmanaged: {0}", srid);
                    throw new ArgumentOutOfRangeException("srid", format);
            }
        }

        private static ICoordinateSystem GetMercatorProjection()
        {
            var factory = new CoordinateSystemFactory();
            var parameters = new List<ProjectionParameter> {
                new ProjectionParameter("semi_major", 6378137),
                new ProjectionParameter("semi_minor", 6378137),
                new ProjectionParameter("latitude_of_origin", 0.0),
                new ProjectionParameter("central_meridian", 0.0),
                new ProjectionParameter("scale_factor", 1.0),
                new ProjectionParameter("false_easting", 0.0),
                new ProjectionParameter("false_northing", 0.0)
            };
            var projection = factory.CreateProjection("Mercator", "mercator_1sp", parameters);
            var gcs = factory.CreateGeographicCoordinateSystem(
                "WGS 84",
                AngularUnit.Degrees,
                HorizontalDatum.WGS84,
                PrimeMeridian.Greenwich,
                new AxisInfo("north", AxisOrientationEnum.North),
                new AxisInfo("east", AxisOrientationEnum.East));
            var mercator = factory.CreateProjectedCoordinateSystem(
                "Mercator",
                gcs,
                projection,
                LinearUnit.Metre,
                new AxisInfo("East", AxisOrientationEnum.East),
                new AxisInfo("North", AxisOrientationEnum.North));
            return mercator;
        }     
    }
}
