using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NPack;
using NPack.Matrix;
using NUnit.Framework;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Coordinate2D = NetTopologySuite.Coordinates.BufferedCoordinate;
using Coordinate2DFactory = NetTopologySuite.Coordinates.BufferedCoordinateFactory;
using Coordinate2DSequenceFactory = NetTopologySuite.Coordinates.BufferedCoordinateSequenceFactory;

namespace ProjNet.Tests
{
    [TestFixture]
    public class CoordinateTransformTests
    {
        [Test]
        public void TestAlbersProjection()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6378206.4,
                                                              294.9786982138982,
                                                              LinearUnit.USSurveyFoot,
                                                              "Clarke 1866");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Clarke 1866");

            IExtents<Coordinate2D> extents = gf.CreateExtents();

            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Clarke 1866");

            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("central_meridian", -96));
            parameters.Add(new ProjectionParameter("latitude_of_center", 23));
            parameters.Add(new ProjectionParameter("standard_parallel_1", 29.5));
            parameters.Add(new ProjectionParameter("standard_parallel_2", 45.5));
            parameters.Add(new ProjectionParameter("false_easting", 0));
            parameters.Add(new ProjectionParameter("false_northing", 0));

            IProjection projection = cFac.CreateProjection("albers",
                                                           parameters,
                                                           "Albers Conical Equal Area");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "Albers Conical Equal Area");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(-75, 35).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected = gf.CreatePoint2D(1885472.7, 1535925).Coordinate;

            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.05),
                          String.Format(
                              "Albers forward transformation outside tolerance, " +
                              "Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              pUtm[0],
                              pUtm[1]));

            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "Albers reverse transformation outside tolerance, " +
                              "Expected [{0},{1}], got [{2},{3}]",
                              pGeo[0],
                              pGeo[1],
                              pGeo2[0],
                              pGeo2[1]));
        }

        [Test]
        public void TestAlbersProjectionFeet()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6378206.4,
                                                              294.9786982138982,
                                                              LinearUnit.USSurveyFoot,
                                                              "Clarke 1866");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Clarke 1866");
            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Clarke 1866");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("central_meridian", -96));
            parameters.Add(new ProjectionParameter("latitude_of_center", 23));
            parameters.Add(new ProjectionParameter("standard_parallel_1", 29.5));
            parameters.Add(new ProjectionParameter("standard_parallel_2", 45.5));
            parameters.Add(new ProjectionParameter("false_easting", 0));
            parameters.Add(new ProjectionParameter("false_northing", 0));
            IProjection projection = cFac.CreateProjection("albers",
                                                           parameters,
                                                           "Albers Conical Equal Area");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Foot,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "Albers Conical Equal Area");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(-75, 35).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected =
                gf.CreatePoint2D(1885472.7 / LinearUnit.Foot.MetersPerUnit,
                                 1535925 / LinearUnit.Foot.MetersPerUnit).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.1),
                          String.Format(
                              "Albers forward transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              pUtm[0],
                              pUtm[1]));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "Albers reverse transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              pGeo[0],
                              pGeo[1],
                              pGeo2[0],
                              pGeo2[1]));
        }

        [Test]
        public void TestMercator_1SP_Projection()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6377397.155,
                                                              299.15281,
                                                              LinearUnit.Meter,
                                                              "Bessel 1840");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Bessel 1840");

            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Bessel 1840");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 0));
            parameters.Add(new ProjectionParameter("central_meridian", 110));
            parameters.Add(new ProjectionParameter("scale_factor", 0.997));
            parameters.Add(new ProjectionParameter("false_easting", 3900000));
            parameters.Add(new ProjectionParameter("false_northing", 900000));
            IProjection projection = cFac.CreateProjection("Mercator_1SP",
                                                           parameters,
                                                           "Mercator_1SP");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "Makassar / NEIEZ");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(120, -3).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected = gf.CreatePoint2D(5009726.58, 569150.82).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.02),
                          String.Format(
                              "Mercator_1SP forward transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              pUtm[0],
                              pUtm[1]));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "Mercator_1SP reverse transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              pGeo[0],
                              pGeo[1],
                              pGeo2[0],
                              pGeo2[1]));
        }

        [Test]
        public void TestMercator_1SP_Projection_Feet()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6377397.155,
                                                              299.15281,
                                                              LinearUnit.Meter,
                                                              "Bessel 1840");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Bessel 1840");
            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Bessel 1840");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 0));
            parameters.Add(new ProjectionParameter("central_meridian", 110));
            parameters.Add(new ProjectionParameter("scale_factor", 0.997));
            parameters.Add(new ProjectionParameter("false_easting",
                                                   3900000 / LinearUnit.Foot.MetersPerUnit));
            parameters.Add(new ProjectionParameter("false_northing",
                                                   900000 / LinearUnit.Foot.MetersPerUnit));
            IProjection projection = cFac.CreateProjection("Mercator_1SP",
                                                           parameters,
                                                           "Mercator_1SP");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Foot,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "Makassar / NEIEZ");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(120, -3).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected =
                gf.CreatePoint2D(5009726.58 / LinearUnit.Foot.MetersPerUnit,
                                 569150.82 / LinearUnit.Foot.MetersPerUnit).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.02),
                          String.Format(
                              "Mercator_1SP forward transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              pUtm[0],
                              pUtm[1]));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "Mercator_1SP reverse transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              pGeo[0],
                              pGeo[1],
                              pGeo2[0],
                              pGeo2[1]));
        }

        [Test]
        public void TestMercator_2SP_Projection()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6378245.0,
                                                              298.3,
                                                              LinearUnit.Meter,
                                                              "Krassowski 1940");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Krassowski 1940");
            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Krassowski 1940");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 42));
            parameters.Add(new ProjectionParameter("central_meridian", 51));
            parameters.Add(new ProjectionParameter("false_easting", 0));
            parameters.Add(new ProjectionParameter("false_northing", 0));
            IProjection projection = cFac.CreateProjection("Mercator_2SP",
                                                           parameters,
                                                           "Mercator_2SP");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "Pulkovo 1942 / Mercator Caspian Sea");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(53, 53).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected = gf.CreatePoint2D(165704.29, 5171848.07).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.02),
                          String.Format(
                              "Mercator_2SP forward transformation outside tolerance, Expected {0}, got {1}",
                              expected,
                              pUtm));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "Mercator_2SP reverse transformation outside tolerance, Expected {0}, got {1}",
                              pGeo,
                              pGeo2));
        }

        [Test]
        public void TestTransverseMercator_Projection()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(6377563.396,
                                                              299.32496,
                                                              LinearUnit.Meter,
                                                              "Airy 1830");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Airy 1830");
            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Airy 1830");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 49));
            parameters.Add(new ProjectionParameter("central_meridian", -2));
            parameters.Add(new ProjectionParameter("scale_factor", 0.9996012717));
            parameters.Add(new ProjectionParameter("false_easting", 400000));
            parameters.Add(new ProjectionParameter("false_northing", -100000));
            IProjection projection = cFac.CreateProjection("Transverse_Mercator",
                                                           parameters,
                                                           "Transverse Mercator");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "OSGB 1936 / British National Grid");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(0.5, 50.5).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected = gf.CreatePoint2D(577274.99, 69740.50).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.02),
                          String.Format(
                              "TransverseMercator forward transformation outside tolerance, Expected {0}, got {1}",
                              expected,
                              pUtm));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "TransverseMercator reverse transformation outside tolerance, Expected {0}, got {1}",
                              pGeo,
                              pGeo2));
        }

        [Test]
        public void TestLambertConicConformal2SP_Projection()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere(20925832.16,
                                                              294.97470,
                                                              LinearUnit.USSurveyFoot,
                                                              "Clarke 1866");

            IHorizontalDatum datum = cFac.CreateHorizontalDatum(DatumType.HorizontalGeocentric,
                                                                ellipsoid,
                                                                null,
                                                                "Clarke 1866");
            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      datum,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "Lon"),
                                                      new AxisInfo(AxisOrientation.North, "Lat"),
                                                      "Clarke 1866");
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 27.833333333));
            parameters.Add(new ProjectionParameter("central_meridian", -99));
            parameters.Add(new ProjectionParameter("standard_parallel_1", 28.3833333333));
            parameters.Add(new ProjectionParameter("standard_parallel_2", 30.2833333333));
            parameters.Add(new ProjectionParameter("false_easting",
                                                   2000000 / LinearUnit.USSurveyFoot.MetersPerUnit));
            parameters.Add(new ProjectionParameter("false_northing", 0));
            IProjection projection = cFac.CreateProjection("lambert_conformal_conic_2sp",
                                                           parameters,
                                                           "Lambert Conic Conformal (2SP)");

            IProjectedCoordinateSystem<Coordinate2D> coordsys =
                cFac.CreateProjectedCoordinateSystem(gcs,
                                                     projection,
                                                     LinearUnit.USSurveyFoot,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "NAD27 / Texas South Central");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(gcs, coordsys);

            ICoordinate pGeo = gf.CreatePoint2D(-96, 28.5).Coordinate;
            ICoordinate pUtm = trans.MathTransform.Transform(pGeo);
            ICoordinate pGeo2 = trans.MathTransform.Inverse.Transform(pUtm);

            ICoordinate expected =
                gf.CreatePoint2D(2963503.91 / LinearUnit.USSurveyFoot.MetersPerUnit,
                                 254759.80 / LinearUnit.USSurveyFoot.MetersPerUnit).Coordinate;
            Assert.IsTrue(ToleranceLessThan(pUtm, expected, 0.05),
                          String.Format(
                              "LambertConicConformal2SP forward transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              pUtm[0],
                              pUtm[1]));
            Assert.IsTrue(ToleranceLessThan(pGeo, pGeo2, 0.0000001),
                          String.Format(
                              "LambertConicConformal2SP reverse transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              pGeo[0],
                              pGeo[1],
                              pGeo2[0],
                              pGeo2[1]));
        }

        [Test]
        public void TestGeocentric()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            IExtents<Coordinate2D> extents = gf.CreateExtents();
            IGeographicCoordinateSystem<Coordinate2D> gcs =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      HorizontalDatum.Etrf89,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "East"),
                                                      new AxisInfo(AxisOrientation.North, "North"),
                                                      "ETRF89 Geographic");
            IGeocentricCoordinateSystem gcenCs
                = cFac.CreateGeocentricCoordinateSystem(extents,
                                                        HorizontalDatum.Etrf89,
                                                        LinearUnit.Meter,
                                                        PrimeMeridian.Greenwich,
                                                        "ETRF89 Geographic");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            CoordinateTransformationFactory<Coordinate2D> gtFac =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory);

            ICoordinateTransformation ct = gtFac.CreateFromCoordinateSystems(gcs,
                                                                             gcenCs as
                                                                             ICoordinateSystem
                                                                                 <Coordinate2D>);
            IPoint2D pExpected = gf.CreatePoint2D(2 + 7.0 / 60 + 46.38 / 3600, 53 + 48.0 / 60 + 33.82 / 3600);
            // Point.FromDMS(2, 7, 46.38, 53, 48, 33.82);
            ICoordinate pExpected3D = gf.CreatePoint3D(pExpected, 73.0).Coordinate;
            ICoordinate p0 = gf.CreatePoint3D(3771793.97, 140253.34, 5124304.35).Coordinate;
            ICoordinate p1 = ct.MathTransform.Transform(pExpected3D);
            ICoordinate p2 = ct.MathTransform.Inverse.Transform(p1);
            Assert.IsTrue(ToleranceLessThan(p1, p0, 0.01));
            Assert.IsTrue(ToleranceLessThan(p2, pExpected.Coordinate, 0.00001));
        }

        [Test]
        public void TestDatumTransform()
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> cFac =
                new CoordinateSystemFactory<Coordinate2D>(cf, gf);

            //Define datums
            IHorizontalDatum wgs72 = HorizontalDatum.Wgs72;
            //IHorizontalDatum ed50 = HorizontalDatum.ED50;
            IHorizontalDatum ed50 = cFac.CreateHorizontalDatum(
                DatumType.HorizontalGeocentric,
                Ellipsoid.International1924,
                new Wgs84ConversionInfo( //Parameters for Denmark
                    -81.0703,
                    -89.3603,
                    -115.7526,
                    -0.48488,
                    -0.02436,
                    -0.41321,
                    -0.540645
                    ),
                "European Datum 1950",
                "EPSG",
                6230,
                "ED50",
                String.Empty,
                String.Empty
                );

            IExtents<Coordinate2D> extents = gf.CreateExtents();

            //Define geographic coordinate systems
            IGeographicCoordinateSystem<Coordinate2D> gcsWGS72 =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      wgs72,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "East"),
                                                      new AxisInfo(AxisOrientation.North, "North"),
                                                      "WGS72 Geographic");

            IGeographicCoordinateSystem<Coordinate2D> gcsWGS84 =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      HorizontalDatum.Wgs84,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "East"),
                                                      new AxisInfo(AxisOrientation.North, "North"),
                                                      "WGS84 Geographic");

            IGeographicCoordinateSystem<Coordinate2D> gcsED50 =
                cFac.CreateGeographicCoordinateSystem(extents,
                                                      AngularUnit.Degrees,
                                                      ed50,
                                                      PrimeMeridian.Greenwich,
                                                      new AxisInfo(AxisOrientation.East, "East"),
                                                      new AxisInfo(AxisOrientation.North, "North"),
                                                      "ED50 Geographic");

            //Define geocentric coordinate systems
            IGeocentricCoordinateSystem<Coordinate2D> gcenCsWGS72 =
                cFac.CreateGeocentricCoordinateSystem(extents,
                                                      wgs72,
                                                      LinearUnit.Meter,
                                                      PrimeMeridian.Greenwich,
                                                      "WGS72 Geocentric");
            IGeocentricCoordinateSystem<Coordinate2D> gcenCsWGS84 =
                cFac.CreateGeocentricCoordinateSystem(extents,
                                                      HorizontalDatum.Wgs84,
                                                      LinearUnit.Meter,
                                                      PrimeMeridian.Greenwich,
                                                      "WGS84 Geocentric");
            IGeocentricCoordinateSystem<Coordinate2D> gcenCsED50 =
                cFac.CreateGeocentricCoordinateSystem(extents,
                                                      ed50,
                                                      LinearUnit.Meter,
                                                      PrimeMeridian.Greenwich,
                                                      "ED50 Geocentric");

            //Define projections
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 0));
            parameters.Add(new ProjectionParameter("central_meridian", 9));
            parameters.Add(new ProjectionParameter("scale_factor", 0.9996));
            parameters.Add(new ProjectionParameter("false_easting", 500000));
            parameters.Add(new ProjectionParameter("false_northing", 0));
            IProjection projection = cFac.CreateProjection("Transverse_Mercator",
                                                           parameters,
                                                           "Transverse Mercator");
            IProjectedCoordinateSystem<Coordinate2D> utmED50 =
                cFac.CreateProjectedCoordinateSystem(gcsED50,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "ED50 UTM Zone 32N");
            IProjectedCoordinateSystem<Coordinate2D> utmWGS84 =
                cFac.CreateProjectedCoordinateSystem(gcsWGS84,
                                                     projection,
                                                     LinearUnit.Meter,
                                                     new AxisInfo(AxisOrientation.East, "East"),
                                                     new AxisInfo(AxisOrientation.North, "North"),
                                                     "WGS84 UTM Zone 32N");

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            //Set up coordinate transformations
            CoordinateTransformationFactory<Coordinate2D> ctFac =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory);
            ICoordinateTransformation ctForw = ctFac.CreateFromCoordinateSystems(gcsWGS72,
                                                                                 gcenCsWGS72);
            //Geographic->Geocentric (WGS72)
            ICoordinateTransformation ctWGS84_Gcen2Geo =
                ctFac.CreateFromCoordinateSystems(gcenCsWGS84, gcsWGS84);
            //Geocentric->Geographic (WGS84)
            ICoordinateTransformation ctWGS84_Geo2UTM = ctFac.CreateFromCoordinateSystems(gcsWGS84,
                                                                                          utmWGS84);
            //UTM ->Geographic (WGS84)
            ICoordinateTransformation ctED50_UTM2Geo = ctFac.CreateFromCoordinateSystems(utmED50,
                                                                                         gcsED50);
            //UTM ->Geographic (ED50)
            ICoordinateTransformation ctED50_Geo2Gcen = ctFac.CreateFromCoordinateSystems(gcsED50,
                                                                                          gcenCsED50);
            //Geographic->Geocentric (ED50)

            //Test datum-shift from WGS72 to WGS84
            //Point3D pGeoCenWGS72 = ctForw.MathTransform.Transform(pLongLatWGS72) as Point3D;
            ICoordinate pGeoCenWGS72 =
                gf.CreatePoint3D(3657660.66, 255768.55, 5201382.11).Coordinate;
            ICoordinateTransformation geocen_ed50_2_Wgs84 =
                ctFac.CreateFromCoordinateSystems(gcenCsWGS72, gcenCsWGS84);
            ICoordinate pGeoCenWGS84 = geocen_ed50_2_Wgs84.MathTransform.Transform(pGeoCenWGS72);
            //Point3D pGeoCenWGS84 = wgs72.Wgs84Parameters.Apply(pGeoCenWGS72);

            Assert.IsTrue(
                ToleranceLessThan(gf.CreatePoint3D(3657660.78, 255778.43, 5201387.75).Coordinate,
                                  pGeoCenWGS84,
                                  0.01));

            ICoordinateTransformation utm_ed50_2_Wgs84 = ctFac.CreateFromCoordinateSystems(utmED50,
                                                                                           utmWGS84);
            ICoordinate pUTMED50 = gf.CreatePoint2D(600000, 6100000).Coordinate;
            ICoordinate pUTMWGS84 = utm_ed50_2_Wgs84.MathTransform.Transform(pUTMED50);
            Assert.IsTrue(ToleranceLessThan(gf.CreatePoint2D(599928.6, 6099790.2).Coordinate,
                                            pUTMWGS84,
                                            0.1));
            //Perform reverse
            ICoordinateTransformation utm_Wgs84_2_Ed50 = ctFac.CreateFromCoordinateSystems(
                utmWGS84, utmED50);
            pUTMED50 = utm_Wgs84_2_Ed50.MathTransform.Transform(pUTMWGS84);
            Assert.IsTrue(ToleranceLessThan(gf.CreatePoint2D(600000, 6100000).Coordinate,
                                            pUTMED50,
                                            0.1));
            //Assert.IsTrue(Math.Abs((pUTMWGS84 as Point3D).Z - 36.35) < 0.5);
            //Point pExpected = Point.FromDMS(2, 7, 46.38, 53, 48, 33.82);
            //ED50_to_WGS84_Denmark: datum.Wgs84Parameters = new Wgs84ConversionInfo(-89.5, -93.8, 127.6, 0, 0, 4.5, 1.2);
        }

        private bool ToleranceLessThan(ICoordinate p1, ICoordinate p2, Double tolerance)
        {
            if (p1.ComponentCount > 2 && p2.ComponentCount > 2)
            {
                return
                    p1.Components[0].Subtract(p2.Components[0]).Abs().LessThan(tolerance) &&
                    p1.Components[1].Subtract(p2.Components[1]).Abs().LessThan(tolerance) &&
                    p1.Components[2].Subtract(p2.Components[2]).Abs().LessThan(tolerance);
            }
            else
            {
                return
                    p1.Components[0].Subtract(p2.Components[0]).Abs().LessThan(tolerance) &&
                    p1.Components[1].Subtract(p2.Components[1]).Abs().LessThan(tolerance);
            }
        }

        [Test]
        public void TestUnitTransforms()
        {
            ICoordinateSystem<Coordinate2D> nadUTM = SridReader.GetCSbyID(2868);
            //UTM Arizona Central State Plane using Feet as units
            ICoordinateSystem<Coordinate2D> wgs84GCS = SridReader.GetCSbyID(4326); //GCS WGS84


            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());

            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();

            ICoordinateTransformation trans =
                new CoordinateTransformationFactory<Coordinate2D>(cf, gf, factory)
                    .CreateFromCoordinateSystems(wgs84GCS, nadUTM);

            ICoordinate p0 = gf.CreatePoint2D(-111.89, 34.165).Coordinate;
            ICoordinate expected = gf.CreatePoint2D(708066.190579, 1151426.44638).Coordinate;

            ICoordinate p1 = trans.MathTransform.Transform(p0);
            ICoordinate p2 = trans.MathTransform.Inverse.Transform(p1);

            Assert.IsTrue(ToleranceLessThan(p1, expected, 0.013),
                          String.Format(
                              "Transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              expected[0],
                              expected[1],
                              p1[0],
                              p1[1]));
            //WARNING: This accuracy is too poor!
            Assert.IsTrue(ToleranceLessThan(p0, p2, 0.0000001),
                          String.Format(
                              "Transformation outside tolerance, Expected [{0},{1}], got [{2},{3}]",
                              p0[0],
                              p0[1],
                              p2[0],
                              p2[1]));
        }
    }
}