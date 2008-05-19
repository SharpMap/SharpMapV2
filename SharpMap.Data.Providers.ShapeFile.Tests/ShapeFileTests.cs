using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownText;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using ProjNet.CoordinateSystems;
using SharpMap.Data;
using SharpMap.Expressions;
using SharpMap.Tests;

namespace SharpMap.Data.Providers.ShapeFile.Tests
{
    [TestFixture]
    public class ShapeFileTests
    {
        private static readonly Random _rnd = new Random();

        private IGeometryFactory<BufferedCoordinate2D> _geoFactory;
        private ICoordinateSystemFactory<BufferedCoordinate2D> _coordSysFactory;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Directory.CreateDirectory("UnitTestData");

            ICoordinateSequenceFactory<BufferedCoordinate2D> sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);

            _coordSysFactory = new CoordinateSystemFactory<BufferedCoordinate2D>(
                sequenceFactory.CoordinateFactory, _geoFactory);
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            Directory.Delete("UnitTestData", true);
        }

        [TearDown]
        public void Teardown()
        {
            GC.Collect();
        }

        [Test]
        public void CreateNewNoAttributesTest()
        {
            if (File.Exists(@"UnitTestData\Test1.shp")) File.Delete(@"UnitTestData\Test1.shp");
            if (File.Exists(@"UnitTestData\Test1.shx")) File.Delete(@"UnitTestData\Test1.shx");
            if (File.Exists(@"UnitTestData\Test1.dbf")) File.Delete(@"UnitTestData\Test1.dbf");

            ShapeFileProvider shapeFile = ShapeFileProvider.Create("UnitTestData", "Test1", ShapeType.Polygon, _geoFactory);
            Assert.IsTrue(File.Exists(@"UnitTestData\Test1.shp"));
            Assert.IsTrue(File.Exists(@"UnitTestData\Test1.shx"));
            Assert.IsFalse(File.Exists(@"UnitTestData\Test1.dbf"));
            shapeFile.Close();
            File.Delete(@"UnitTestData\Test1.shp");
            File.Delete(@"UnitTestData\Test1.shx");
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void CreateNewNoAttributesNullShapeTypeFailsTest()
        {
            ShapeFileProvider.Create("UnitTestData", "Test1", ShapeType.Null, _geoFactory);
        }

        [Test]
        public void CreateNewWithAttributesSchemaTest()
        {
            FeatureDataTable schema = new FeatureDataTable(_geoFactory);
            schema.Columns.AddRange(new DataColumn[]
                                        {
                                            new DataColumn("Name", typeof (String)),
                                            new DataColumn("Date_Created", typeof (DateTime)),
                                            new DataColumn("Visits", typeof (Int32)),
                                            new DataColumn("Weight", typeof (Single))
                                        });
            ShapeFileProvider shapeFile = ShapeFileProvider.Create("UnitTestData", "Test1", ShapeType.Point, schema, _geoFactory);
            Assert.IsTrue(File.Exists(@"UnitTestData\Test1.shp"));
            Assert.IsTrue(File.Exists(@"UnitTestData\Test1.shx"));
            Assert.IsTrue(File.Exists(@"UnitTestData\Test1.dbf"));
            Assert.IsTrue(shapeFile.HasDbf);
            shapeFile.Close();
            File.Delete(@"UnitTestData\Test1.shp");
            File.Delete(@"UnitTestData\Test1.shx");
            File.Delete(@"UnitTestData\Test1.dbf");
        }

        [Test]
        public void NewWithoutFileBasedSpatialIndexTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Assert.IsNotNull(shapeFile);
            shapeFile.Close();
        }

        [Test]
        [Ignore("File based spatial indexing not working.")]
        public void NewWithFileBasedSpatialIndexTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory, true);
            Assert.IsNotNull(shapeFile);
            shapeFile.Open();
            Assert.IsTrue(File.Exists(@"..\..\..\TestData\BCROADS.shp.sidx"));
            shapeFile.Close();
            File.Delete(@"..\..\..\TestData\BCROADS.shp.sidx");
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void RebuildSpatialIndexWhenClosedThrowsExceptionTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            shapeFile.RebuildSpatialIndex();
            shapeFile.Close();
        }

        [Test]
        [Ignore]
        public void RebuildSpatialIndexTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory, false);
            shapeFile.Open();
            File.Delete(@"..\..\..\TestData\BCROADS.shp.sidx");
            shapeFile.RebuildSpatialIndex();
            Assert.IsTrue(File.Exists(@"..\..\..\TestData\BCROADS.shp.sidx"));
            shapeFile.Close();
            File.Delete(@"..\..\..\TestData\BCROADS.shp.sidx");
        }

        [Test]
        public void GetCoordinateSystemHavingPrjFileTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            Assert.IsNotNull(shapeFile.SpatialReference);
            Assert.IsInstanceOfType(typeof (IProjectedCoordinateSystem), shapeFile.SpatialReference);
            IProjectedCoordinateSystem cs = shapeFile.SpatialReference as IProjectedCoordinateSystem;

            Assert.IsNotNull(cs);
            Assert.AreEqual("NAD_1983_HARN_StatePlane_Oregon_North_FIPS_3601", cs.Name);

            IProjectedCoordinateSystem expected = createExpectedCoordinateSystem();

            Assert.IsTrue(expected.EqualParams(cs));
            shapeFile.Close();
        }

        [Test]
        public void NoPrjFileImpliesCoordinateSystemIsNullTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADSWithoutDbf.SHP", _geoFactory);
            shapeFile.Open();
            Assert.IsNull(shapeFile.SpatialReference);
            shapeFile.Close();
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void SetCoordinateSystemWithPrjFileThrowsExceptionTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            IProjectedCoordinateSystem cs = createExpectedCoordinateSystem();
            shapeFile.SpatialReference = cs;
        }

        [Test]
        public void NoPrjFileThenSetCoordinateSystemGivesSpatialRefToGeometries()
        {
            //Layer name: BCROADSWithoutDbf
            //Geometry: Line String
            //Feature Count: 7291
            //Extent: (7332083.212797, 236823.718672) - (7538428.618000, 405610.346926)
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                                                @"..\..\..\TestData\BCROADSWithoutDbf.SHP", 
                                                _geoFactory);
            shapeFile.Open();
            ICoordinateSystemFactory<BufferedCoordinate2D> coordSysFactory 
                = _coordSysFactory;
            String wkt = File.ReadAllText(@"..\..\..\TestData\BCROADS.prj");
            IProjectedCoordinateSystem cs = WktReader<BufferedCoordinate2D>
                .ToCoordinateSystemInfo(wkt, coordSysFactory) as IProjectedCoordinateSystem;
            shapeFile.SpatialReference = cs;
            Assert.IsNotNull(shapeFile.SpatialReference);

            IGeometry g = shapeFile.GetGeometryByOid(0);

            Assert.IsTrue(g.SpatialReference.EqualParams(createExpectedCoordinateSystem()));

            shapeFile.Close();
        }

        [Test]
        public void GetShapeTypeTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                                                @"..\..\..\TestData\BCROADSWithoutDbf.SHP", 
                                                _geoFactory);
            shapeFile.Open();
            Assert.AreEqual(ShapeType.PolyLine, shapeFile.ShapeType);
            shapeFile.Close();
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void GetShapeTypeWhenClosedThrowsExceptionTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                                                @"..\..\..\TestData\BCROADSWithoutDbf.SHP",
                                                _geoFactory);
            Assert.AreEqual(ShapeType.PolyLine, shapeFile.ShapeType);
        }

        [Test]
        public void GetFilenameTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                                                @"..\..\..\TestData\BCROADSWithoutDbf.SHP", 
                                                _geoFactory);
            Assert.AreEqual(@"..\..\..\TestData\BCROADSWithoutDbf.SHP", shapeFile.Filename);
        }

        //[Test]
        //public void SetFilenameTest()
        //{
        //    if (File.Exists(@"UnitTestData\NewBCROADS.SHP")) File.Delete(@"UnitTestData\NewBCROADS.SHP");
        //    if (File.Exists(@"UnitTestData\NewBCROADS.SHX")) File.Delete(@"UnitTestData\NewBCROADS.SHX");
        //    if (File.Exists(@"UnitTestData\NewBCROADS.DBF")) File.Delete(@"UnitTestData\NewBCROADS.DBF");
        //    if (File.Exists(@"UnitTestData\NewBCROADS.SBN")) File.Delete(@"UnitTestData\NewBCROADS.SBN");
        //    if (File.Exists(@"UnitTestData\NewBCROADS.SBX")) File.Delete(@"UnitTestData\NewBCROADS.SBX");
        //    if (File.Exists(@"UnitTestData\NewBCROADS.PRJ")) File.Delete(@"UnitTestData\NewBCROADS.PRJ");

        //    File.Copy(@"..\..\..\TestData\BCROADS.SHP", @"UnitTestData\BCROADSCopy.SHP", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SHX", @"UnitTestData\BCROADSCopy.SHX", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.DBF", @"UnitTestData\BCROADSCopy.DBF", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SBN", @"UnitTestData\BCROADSCopy.SBN", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SBX", @"UnitTestData\BCROADSCopy.SBX", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.PRJ", @"UnitTestData\BCROADSCopy.PRJ", true);

        //    ShapeFileProvider shapeFile = new ShapeFileProvider(@"UnitTestData\BCROADSCopy.SHP");
        //    shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";

        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SHP"));
        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SHX"));
        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.DBF"));
        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SBN"));
        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SBX"));
        //    Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.PRJ"));

        //    shapeFile.Open();
        //    Assert.AreEqual(@"UnitTestData\NewBCROADS.SHP", shapeFile.Filename);
        //    shapeFile.Close();

        //    File.Delete(@"UnitTestData\BCROADSCopy.SHP");
        //    File.Delete(@"UnitTestData\BCROADSCopy.SHX");
        //    File.Delete(@"UnitTestData\BCROADSCopy.DBF");
        //    File.Delete(@"UnitTestData\BCROADSCopy.SBN");
        //    File.Delete(@"UnitTestData\BCROADSCopy.SBX");
        //    File.Delete(@"UnitTestData\BCROADSCopy.PRJ");

        //    File.Delete(@"UnitTestData\NewBCROADS.SHP");
        //    File.Delete(@"UnitTestData\NewBCROADS.SHX");
        //    File.Delete(@"UnitTestData\NewBCROADS.DBF");
        //    File.Delete(@"UnitTestData\NewBCROADS.SBN");
        //    File.Delete(@"UnitTestData\NewBCROADS.SBX");
        //    File.Delete(@"UnitTestData\NewBCROADS.PRJ");
        //}

        //[Test]
        //[ExpectedException(typeof (ShapeFileInvalidOperationException))]
        //public void SetFilenameWhenOpenThrowsExceptionTest()
        //{
        //    ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
        //    shapeFile.Open();
        //    shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";
        //}

        //[Test]
        //[ExpectedException(typeof (ShapeFileInvalidOperationException))]
        //public void SetFilenameWhenFileExistsThrowsExceptionTest()
        //{
        //    File.Copy(@"..\..\..\TestData\BCROADS.SHP", @"UnitTestData\BCROADSCopy.SHP", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SHX", @"UnitTestData\BCROADSCopy.SHX", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SHP", @"UnitTestData\NewBCROADS.SHP", true);
        //    File.Copy(@"..\..\..\TestData\BCROADS.SHX", @"UnitTestData\NewBCROADS.SHX", true);

        //    ShapeFileProvider shapeFile = new ShapeFileProvider(@"UnitTestData\BCROADSCopy.SHP");
        //    shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";
        //}

        //[Test]
        //[ExpectedException(typeof (ShapeFileIsInvalidException))]
        //public void SetFilenameToNonShpExtensionThrowsExceptionTest()
        //{
        //    ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
        //    shapeFile.Filename = @"UnitTestData\NewBCROADS.abc";
        //}

        [Test]
        public void GetIndexFilenameTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Assert.AreEqual(0, String.Compare(
                                   Path.Combine(new DirectoryInfo(@"..\..\..\TestData").FullName, "BCROADS.SHX"),
                                   shapeFile.IndexFilename,
                                   true));
            shapeFile.Close();
        }

        [Test]
        public void GetDbfFilenameTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Assert.AreEqual(0, String.Compare(
                                   Path.Combine(new DirectoryInfo(@"..\..\..\TestData").FullName, "BCROADS.DBF"),
                                   shapeFile.DbfFilename,
                                   true));
            shapeFile.Close();
        }

        [Test]
        public void HasDbfWithDbfFileIsTrueTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Assert.IsTrue(shapeFile.HasDbf);
            shapeFile.Close();
        }

        [Test]
        public void HasDbfWithoutDbfFileIsFalseTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADSWithoutDbf.SHP", _geoFactory);
            Assert.IsFalse(shapeFile.HasDbf);
            shapeFile.Close();
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetEncodingTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetEncodingWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SetEncodingTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SetEncodingWithNoDbfThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SetEncodingWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        public void IsOpenTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            Assert.IsFalse(shapeFile.IsOpen);
            shapeFile.Open();
            Assert.IsTrue(shapeFile.IsOpen);
            shapeFile.Close();
            Assert.IsFalse(shapeFile.IsOpen);
        }

        [Test]
        [ExpectedException(typeof (IOException))]
        public void OpenExclusiveTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open(true);
            File.OpenRead(@"..\..\..\TestData\BCROADS.SHP");
        }

        [Test]
        public void CloseExclusiveTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open(true);
            shapeFile.Close();
            File.OpenRead(@"..\..\..\TestData\BCROADS.SHP").Close();
        }

        [Test]
        [Ignore("Retarget or delete test; IEnumerable<IGeometry> method variations obsoleted in favor of returning IFeatureDataReader")]
        public void GetGeometriesInViewTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            List<IGeometry> geometries = new List<IGeometry>();

            //geometries.AddRange(shapeFile.ExecuteGeometryIntersectionQuery(shapeFile.GetExtents()));
            Assert.AreEqual(shapeFile.GetFeatureCount(), geometries.Count);
            geometries.Clear();

            IGeometry empty = _geoFactory.CreatePoint();
            //geometries.AddRange(shapeFile.ExecuteGeometryIntersectionQuery(empty));
            Assert.AreEqual(0, geometries.Count);
        }

        [Test]
        [Ignore("Retarget or delete test; IEnumerable<IGeometry> method variations obsoleted in favor of returning IFeatureDataReader")]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void GetGeometriesInViewWhenClosedThrowsExceptionTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);

            IGeometry empty = _geoFactory.CreatePoint();
            //new List<IGeometry>(shapeFile.ExecuteGeometryIntersectionQuery(empty));
        }

        [Test]
        public void ExecuteIntersectionQueryByBoundingBoxTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            FeatureDataTable data = new FeatureDataTable("ShapeFile test", _geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(shapeFile.GetExtents());
            IFeatureDataReader reader = shapeFile.ExecuteFeatureQuery(query);
            data.Load(reader, LoadOption.OverwriteChanges, null);
            Assert.AreEqual(shapeFile.GetFeatureCount(), data.Rows.Count);
            shapeFile.Close();
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void ExecuteIntersectionQueryByBoundingBoxWhenClosedThrowsExceptionTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(shapeFile.GetExtents());
            IFeatureDataReader reader = shapeFile.ExecuteFeatureQuery(query);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ExecuteIntersectionQueryByGeometryTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void ExecuteIntersectionQueryByGeometryWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetObjectIdsInViewTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void GetObjectIdsInViewWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetGeometryByIdTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void GetGeometryByIdWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        public void GetFeatureCountTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Int32 expected = 7291;
            Int32 actual = shapeFile.GetFeatureCount();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetFeatureTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            FeatureDataRow<UInt32> feature = shapeFile.GetFeatureByOid(0) as FeatureDataRow<UInt32>;
            Assert.IsNotNull(feature);
            Assert.AreEqual(0, feature.Id);
            shapeFile.Close();
        }

        [Test]
        public void GetExtentsTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            IExtents expected =_geoFactory.CreateExtents2D(
                7332083.2127965018, 236823.71867240831, 
                7538428.618, 405610.34692560317);
            IExtents actual = shapeFile.GetExtents();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConnectionIdTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            Assert.AreEqual(@"..\..\..\TestData\BCROADS.SHP", shapeFile.ConnectionId);
        }

        [Test]
        [Ignore("Srid is broken and not well thought out.")]
        public void SridTest()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            shapeFile.Open();
            Assert.AreEqual(0, shapeFile.Srid);
            shapeFile.Close();

            shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADSWithoutDbf.SHP", _geoFactory);
            shapeFile.Open();
            Assert.AreEqual(-1, shapeFile.Srid);
            shapeFile.Close();
        }

        [Test]
        public void InsertFeatureTest()
        {
            FeatureDataTable<UInt32> schema = new FeatureDataTable<UInt32>("oid", _geoFactory);
            schema.Columns.AddRange(new DataColumn[]
                                        {
                                            new DataColumn("Name", typeof (String)),
                                            new DataColumn("DateCreated", typeof (DateTime)),
                                            new DataColumn("Visits", typeof (Int32)),
                                            new DataColumn("Weight", typeof (Single))
                                        });

            ShapeFileProvider shapeFile = ShapeFileProvider.Create("UnitTestData", "Test2", ShapeType.Point, schema, _geoFactory);
            shapeFile.Open();

            DateTime dateCreated = DateTime.Now;
            FeatureDataRow<UInt32> feature = schema.NewRow(1);
            feature["Name"] = "Test feature";
            feature["DateCreated"] = dateCreated;
            feature["Visits"] = 0;
            feature["Weight"] = 100.0f;
            feature.Geometry = _geoFactory.CreatePoint2D(1, 1);

            shapeFile.Insert(feature);
            shapeFile.Close();

            shapeFile = new ShapeFileProvider(@"UnitTestData\Test2.shp", _geoFactory);
            shapeFile.Open();

            Assert.AreEqual(1, shapeFile.GetFeatureCount());

            FeatureDataTable dataTable = new FeatureDataTable("ShapeFile test", _geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(_geoFactory.CreateExtents2D(0.9, 0.9, 1, 1));
            IFeatureDataReader reader = shapeFile.ExecuteFeatureQuery(query);
            dataTable.Load(reader, LoadOption.OverwriteChanges, null);

            Assert.AreEqual(1, dataTable.Rows.Count);

            FeatureDataRow newFeature = dataTable.Rows[0] as FeatureDataRow;
            Assert.AreEqual(_geoFactory.CreatePoint2D(1, 1), newFeature.Geometry);
            Assert.AreEqual(newFeature["Name"], "Test feature");
            DateTime dateCreatedActual = (DateTime) newFeature["DateCreated"];
            Assert.AreEqual(dateCreatedActual.Year, dateCreated.Year);
            Assert.AreEqual(dateCreatedActual.Month, dateCreated.Month);
            Assert.AreEqual(dateCreatedActual.Day, dateCreated.Day);
            Assert.AreEqual(newFeature["Visits"], 0);
            Assert.AreEqual(newFeature["Weight"], 100.0f);
            shapeFile.Close();
        }

        [Test]
        public void InsertFeaturesTest()
        {
            FeatureDataTable<UInt32> schema = new FeatureDataTable<UInt32>("OID", _geoFactory);
            schema.Columns.AddRange(new DataColumn[]
                                        {
                                            new DataColumn("Name", typeof (String)),
                                            new DataColumn("DateCreated", typeof (DateTime)),
                                            new DataColumn("Visits", typeof (Int64)),
                                            new DataColumn("Weight", typeof (Double))
                                        });

            ShapeFileProvider shapeFile = ShapeFileProvider.Create("UnitTestData", "Test3", ShapeType.PolyLine, schema, _geoFactory);
            shapeFile.Open();

            IExtents computedBounds = _geoFactory.CreateExtents();

            List<FeatureDataRow<UInt32>> rows = new List<FeatureDataRow<UInt32>>();

            for (Int32 i = 0; i < 10000; i++)
            {
                DateTime dateCreated = new DateTime(_rnd.Next(1900, 2155), _rnd.Next(1, 12), _rnd.Next(1, 28));
                FeatureDataRow<UInt32> feature = schema.NewRow((UInt32) i);

                Char[] chars = new Char[_rnd.Next(0, 254)];
                for (Int32 charIndex = 0; charIndex < chars.Length; charIndex++)
                {
                    chars[charIndex] = (Char) (Byte) _rnd.Next(32, 126);
                }

                feature["Name"] = new String(chars);
                feature["DateCreated"] = dateCreated;
                feature["Visits"] = _rnd.Next(0, Int32.MaxValue) << _rnd.Next(0, 32);
                feature["Weight"] = _rnd.NextDouble()*_rnd.Next(0, 100000);

                ICoordinateSequence coordinates
                    = _geoFactory.CoordinateSequenceFactory.Create(generateCoordinates());
                
                ILineString line = _geoFactory.CreateLineString(coordinates);

                computedBounds.ExpandToInclude(line.Extents);

                feature.Geometry = line;

                rows.Add(feature);
            }

            shapeFile.Insert(rows);
            shapeFile.Close();

            shapeFile = new ShapeFileProvider(@"UnitTestData\Test3.shp", _geoFactory, _coordSysFactory, false);
            shapeFile.Open();

            Assert.AreEqual(10000, shapeFile.GetFeatureCount());
            Assert.AreEqual(computedBounds, shapeFile.GetExtents());

            FeatureDataTable dataTable = new FeatureDataTable("ShapeFile test", _geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(shapeFile.GetExtents());
            IFeatureDataReader reader = shapeFile.ExecuteFeatureQuery(query);
            dataTable.Load(reader, LoadOption.OverwriteChanges, null);

            Assert.AreEqual(10000, dataTable.Rows.Count);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void UpdateFeatureTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void UpdateFeaturesTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DeleteFeatureTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DeleteFeaturesTest()
        {
        }

        private IProjectedCoordinateSystem createExpectedCoordinateSystem()
        {
            ICoordinateSystemFactory factory = new CoordinateSystemFactory<BufferedCoordinate2D>(
                _geoFactory.CoordinateFactory, _geoFactory);

            IEllipsoid grs80 = Ellipsoid.Grs80;

            IHorizontalDatum harn = factory.CreateHorizontalDatum(
                DatumType.HorizontalGeocentric, grs80, null, 
                "D_North_American_1983_HARN");

            IPrimeMeridian greenwich = PrimeMeridian.Greenwich;

            IAxisInfo axis0 = new AxisInfo(AxisOrientation.East, "Lon");
            IAxisInfo axis1 = new AxisInfo(AxisOrientation.North, "Lat");

            // Made the first parameter - the IExtents - null, which should be improved
            IGeographicCoordinateSystem gcs = factory.CreateGeographicCoordinateSystem(null,
                AngularUnit.Degrees, harn, greenwich, axis0, axis1, "GCS_North_American_1983_HARN");

            IProjection prj = factory.CreateProjection(
                "Lambert_Conformal_Conic",
                new ProjectionParameter[]
                    {
                        new ProjectionParameter("False_Easting", 8202099.737532808),
                        new ProjectionParameter("False_Northing", 0),
                        new ProjectionParameter("Central_Meridian", -120.5),
                        new ProjectionParameter("Standard_Parallel_1", 44.33333333333334),
                        new ProjectionParameter("Standard_Parallel_2", 46.0),
                        new ProjectionParameter("Latitude_Of_Origin", 43.66666666666666)
                    },
                "Lambert_Conformal_Conic");

            IProjectedCoordinateSystem expected = factory.CreateProjectedCoordinateSystem(
                gcs, prj, LinearUnit.Foot, axis0, axis1, "NAD_1983_HARN_StatePlane_Oregon_North_FIPS_3601");

            // TODO: Check if this is correct, since on line 184 of CoorindateSystemFactory.cs, 
            // HorizontalDatum is passed in as null
            return expected;
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void GetSchemaTableFailsifShapeFileNotOpen()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);

            DataTable schemaTable = shapeFile.GetSchemaTable();
        }

        [Test]
        public void GetSchemaTableReturnsOid()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();

            DataTable schemaTable = shapeFile.GetSchemaTable();

            Assert.AreEqual(34, schemaTable.Rows.Count);
            Assert.AreEqual("OID", schemaTable.Rows[0][ProviderSchemaHelper.ColumnNameColumn]);
        }

        [Test]
        [ExpectedException(typeof (ShapeFileInvalidOperationException))]
        public void SetTableSchemaShouldFailIfShapeFileNotOpen()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);

            FeatureDataTable nonKeyedTable = new FeatureDataTable(_geoFactory);
            shapeFile.SetTableSchema(nonKeyedTable);
        }

        [Test]
        public void SetTableSchemaShouldMatchShapeFileSchema()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            IGeometry empty = _geoFactory.CreatePoint();
            FeatureDataTable<UInt32> queryTable = shapeFile.CreateNewTable() as FeatureDataTable<UInt32>;

            FeatureDataTable nonKeyedTable = new FeatureDataTable(_geoFactory);
            shapeFile.SetTableSchema(nonKeyedTable);
            DataTableHelper.AssertTableStructureIdentical(nonKeyedTable, queryTable);

            FeatureDataTable<UInt32> keyedTable = new FeatureDataTable<UInt32>("OID", _geoFactory);
            shapeFile.SetTableSchema(keyedTable);
            DataTableHelper.AssertTableStructureIdentical(keyedTable, queryTable);
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void SetTableSchemaWithDifferentKeyCase()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(@"..\..\..\TestData\BCROADS.SHP", _geoFactory);
            shapeFile.Open();
            IGeometry empty = _geoFactory.CreatePoint();

            FeatureDataTable<UInt32> keyedTable = new FeatureDataTable<UInt32>("oid", _geoFactory);
            shapeFile.SetTableSchema(keyedTable);
        }

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void SetTableSchemaWithDifferentKeyCaseAndSchemaMergeAction()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            IGeometry empty = _geoFactory.CreatePoint();
            FeatureDataTable<UInt32> queryTable = shapeFile.CreateNewTable() as FeatureDataTable<UInt32>;
            // expecting a new FeatureDataTable<UInt32>("OID", _geoFactory);

            FeatureDataTable<UInt32> keyedTable = new FeatureDataTable<UInt32>("oid", _geoFactory);
            shapeFile.SetTableSchema(keyedTable, SchemaMergeAction.CaseInsensitive);
            DataTableHelper.AssertTableStructureIdentical(keyedTable, queryTable);
        }

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void SetTableSchemaWithDifferentKeyNameAndSchemaMergeAction()
        {
            ShapeFileProvider shapeFile = new ShapeFileProvider(
                @"..\..\..\TestData\BCROADS.SHP", _geoFactory, _coordSysFactory);
            shapeFile.Open();
            IGeometry empty = _geoFactory.CreatePoint();
            FeatureDataTable<UInt32> queryTable = shapeFile.CreateNewTable() as FeatureDataTable<UInt32>;
            // expecting a new FeatureDataTable<UInt32>("OID", _geoFactory);

            FeatureDataTable<UInt32> keyedTable = new FeatureDataTable<UInt32>("FID", _geoFactory);
            shapeFile.SetTableSchema(keyedTable, SchemaMergeAction.KeyByType);
            DataTableHelper.AssertTableStructureIdentical(keyedTable, queryTable);
        }

        private IEnumerable<ICoordinate> generateCoordinates()
        {
            ICoordinateFactory coordFactory = _geoFactory.CoordinateFactory;

            Int32 pointCount = _rnd.Next(3, 100);

            for (Int32 pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                ICoordinate p = coordFactory.Create(
                    _rnd.NextDouble() * _rnd.Next(200000, 700000),
                    (_rnd.NextDouble() * _rnd.Next(1000000)) + 50000000);

                yield return p;
            }
        }
    }
}