using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SharpMap.Data.Providers;
using System.IO;
using System.Data;
using SharpMap.CoordinateSystems;
using SharpMap.Utilities;
using SharpMap.Converters.WellKnownText;
using SharpMap.Geometries;
using System.Threading;

namespace SharpMap.Tests.Provider
{
	[TestFixture]
	public class ShapeFileTests
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Directory.CreateDirectory("UnitTestData");
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
        public void CreatNeweNoAttributesTest()
        {
			ShapeFile shapeFile = ShapeFile.Create(".", "Test1", ShapeType.Polygon);
			Assert.IsTrue(File.Exists("Test1.shp"));
			Assert.IsTrue(File.Exists("Test1.shx"));
			Assert.IsFalse(File.Exists("Test1.dbf"));
			shapeFile.Close();
			File.Delete("Test1.shp");
			File.Delete("Test1.shx");
		}

		[Test]
		[ExpectedException(typeof(ShapeFileInvalidOperationException))]
		public void CreatNeweNoAttributesNullShapeTypeFailsTest()
		{
			ShapeFile.Create(".", "Test1", ShapeType.Null);
		}

        [Test]
        public void CreateNewWithAttributesSchemaTest()
		{
			FeatureDataTable schema = new FeatureDataTable();
			schema.Columns.AddRange( new DataColumn[] {
				new DataColumn("Name", typeof(String)),
				new DataColumn("Date_Created", typeof(DateTime)),
				new DataColumn("Visits", typeof(int)),
				new DataColumn("Weight", typeof(float))
			});
			ShapeFile shapeFile = ShapeFile.Create("UnitTestData", "Test1", ShapeType.Point, schema);
			Assert.IsTrue(File.Exists("Test1.shp"));
			Assert.IsTrue(File.Exists("Test1.shx"));
			Assert.IsTrue(File.Exists("Test1.dbf"));
			Assert.IsTrue(shapeFile.HasDbf);
			shapeFile.Close();
			File.Delete("Test1.shp");
			File.Delete("Test1.shx");
			File.Delete("Test1.dbf");
        }

        [Test]
        public void CreateWithoutFileBasedSpatialIndexTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			Assert.IsNotNull(shapeFile);
			shapeFile.Close();
        }

        [Test]
        public void CreateWithFileBasedSpatialIndexTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP", true);
            Assert.IsNotNull(shapeFile);
            shapeFile.Open();
            Assert.IsTrue(File.Exists(@"..\TestData\BCROADS.shp.sidx"));
            shapeFile.Close();
            File.Delete(@"..\TestData\BCROADS.shp.sidx");
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void RebuildSpatialIndexWhenClosedThrowsExceptionTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.RebuildSpatialIndex();
			shapeFile.Close();
        }

        [Test]
        public void RebuildSpatialIndexTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP", true);
            shapeFile.Open();
            File.Delete(@"..\TestData\BCROADS.shp.sidx");
            shapeFile.RebuildSpatialIndex();
            Assert.IsTrue(File.Exists(@"..\TestData\BCROADS.shp.sidx"));
            shapeFile.Close();
            File.Delete(@"..\TestData\BCROADS.shp.sidx");
        }

        [Test]
        public void GetCoordinateSystemHavingPrjFileTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open();
			Assert.IsNotNull(shapeFile.SpatialReference);
			Assert.IsInstanceOfType(typeof(IProjectedCoordinateSystem), shapeFile.SpatialReference);
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
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
			shapeFile.Open();
			Assert.IsNull(shapeFile.SpatialReference);
			shapeFile.Close();
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void SetCoordinateSystemWithPrjFileThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open();
			IProjectedCoordinateSystem cs = createExpectedCoordinateSystem();
			shapeFile.SpatialReference = cs;
        }

        [Test]
        public void NoPrjFileSetCoordinateSystemTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
			shapeFile.Open();
			string wkt = File.ReadAllText(@"..\TestData\BCROADS.prj");
			IProjectedCoordinateSystem cs = CoordinateSystemWktReader.Parse(wkt) as IProjectedCoordinateSystem;
			shapeFile.SpatialReference = cs;
			Assert.IsNotNull(shapeFile.SpatialReference);

			Geometry g = shapeFile.GetGeometryById(1);

			Assert.IsTrue(g.SpatialReference.EqualParams(createExpectedCoordinateSystem()));

			shapeFile.Close();
        }

        [Test]
        public void GetShapeTypeTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
			shapeFile.Open();
			Assert.AreEqual(ShapeType.PolyLine, shapeFile.ShapeType);
			shapeFile.Close();
        }

		[Test]
		[ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void GetShapeTypeWhenClosedThrowsExceptionTest()
        {
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
			Assert.AreEqual(ShapeType.PolyLine, shapeFile.ShapeType);
        }

        [Test]
        public void GetFilenameTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
			Assert.AreEqual(@"..\TestData\BCROADSWithoutDbf.SHP", shapeFile.Filename);
        }

        [Test]
        public void SetFilenameTest()
		{
			if (File.Exists(@"UnitTestData\NewBCROADS.SHP")) File.Delete(@"UnitTestData\NewBCROADS.SHP");
			if (File.Exists(@"UnitTestData\NewBCROADS.SHX")) File.Delete(@"UnitTestData\NewBCROADS.SHX");
			if (File.Exists(@"UnitTestData\NewBCROADS.DBF")) File.Delete(@"UnitTestData\NewBCROADS.DBF");
			if (File.Exists(@"UnitTestData\NewBCROADS.SBN")) File.Delete(@"UnitTestData\NewBCROADS.SBN");
			if (File.Exists(@"UnitTestData\NewBCROADS.SBX")) File.Delete(@"UnitTestData\NewBCROADS.SBX");
			if (File.Exists(@"UnitTestData\NewBCROADS.PRJ")) File.Delete(@"UnitTestData\NewBCROADS.PRJ");

			File.Copy(@"..\TestData\BCROADS.SHP", @"UnitTestData\BCROADSCopy.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"UnitTestData\BCROADSCopy.SHX", true);
			File.Copy(@"..\TestData\BCROADS.DBF", @"UnitTestData\BCROADSCopy.DBF", true);
			File.Copy(@"..\TestData\BCROADS.SBN", @"UnitTestData\BCROADSCopy.SBN", true);
			File.Copy(@"..\TestData\BCROADS.SBX", @"UnitTestData\BCROADSCopy.SBX", true);
			File.Copy(@"..\TestData\BCROADS.PRJ", @"UnitTestData\BCROADSCopy.PRJ", true);

			ShapeFile shapeFile = new ShapeFile(@"UnitTestData\BCROADSCopy.SHP");
			shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";

			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SHP"));
			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SHX"));
			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.DBF"));
			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SBN"));
			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.SBX"));
			Assert.IsTrue(File.Exists(@"UnitTestData\NewBCROADS.PRJ"));

			shapeFile.Open();
			Assert.AreEqual(@"UnitTestData\NewBCROADS.SHP", shapeFile.Filename);
			shapeFile.Close();

			File.Delete(@"UnitTestData\BCROADSCopy.SHP");
			File.Delete(@"UnitTestData\BCROADSCopy.SHX");
			File.Delete(@"UnitTestData\BCROADSCopy.DBF");
			File.Delete(@"UnitTestData\BCROADSCopy.SBN");
			File.Delete(@"UnitTestData\BCROADSCopy.SBX");
			File.Delete(@"UnitTestData\BCROADSCopy.PRJ");

			File.Delete(@"UnitTestData\NewBCROADS.SHP");
			File.Delete(@"UnitTestData\NewBCROADS.SHX");
			File.Delete(@"UnitTestData\NewBCROADS.DBF");
			File.Delete(@"UnitTestData\NewBCROADS.SBN");
			File.Delete(@"UnitTestData\NewBCROADS.SBX");
			File.Delete(@"UnitTestData\NewBCROADS.PRJ");
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void SetFilenameWhenOpenThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open();
			shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";
        }

        [Test]
		[ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void SetFilenameWhenFileExistsThrowsExceptionTest()
		{
			File.Copy(@"..\TestData\BCROADS.SHP", @"UnitTestData\BCROADSCopy.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"UnitTestData\BCROADSCopy.SHX", true);
			File.Copy(@"..\TestData\BCROADS.SHP", @"UnitTestData\NewBCROADS.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"UnitTestData\NewBCROADS.SHX", true);

			ShapeFile shapeFile = new ShapeFile(@"UnitTestData\BCROADSCopy.SHP");
			shapeFile.Filename = @"UnitTestData\NewBCROADS.SHP";
        }

        [Test]
        [ExpectedException(typeof(ShapeFileIsInvalidException))]
        public void SetFilenameToNonShpExtensionThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Filename = @"UnitTestData\NewBCROADS.abc";
        }

        [Test]
        public void GetIndexFilenameTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            Assert.AreEqual(0, String.Compare(
                Path.Combine(new DirectoryInfo(@"..\TestData").FullName, "BCROADS.SHX"), shapeFile.IndexFilename, true));
            shapeFile.Close();
        }

        [Test]
        public void GetDbfFilenameTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            Assert.AreEqual(0, String.Compare(
                Path.Combine(new DirectoryInfo(@"..\TestData").FullName, "BCROADS.DBF"), shapeFile.DbfFilename, true));
            shapeFile.Close();
        }

        [Test]
        public void HasDbfWithDbfFileIsTrueTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            Assert.IsTrue(shapeFile.HasDbf);
            shapeFile.Close();
        }

        [Test]
        public void HasDbfWithoutDbfFileIsFalseTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADSWithoutDbf.SHP");
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
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			Assert.IsFalse(shapeFile.IsOpen);
			shapeFile.Open();
			Assert.IsTrue(shapeFile.IsOpen);
			shapeFile.Close();
			Assert.IsFalse(shapeFile.IsOpen);
        }

        [Test]
		[ExpectedException(typeof(IOException))]
        public void OpenExclusiveTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open(true);
			File.OpenRead(@"..\TestData\BCROADS.SHP");
        }

        [Test]
        public void CloseExclusiveTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open(true);
			shapeFile.Close();
			File.OpenRead(@"..\TestData\BCROADS.SHP").Close();
        }

        [Test]
        public void GetGeometriesInViewTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open();
			List<Geometry> geometries = new List<Geometry>();

			geometries.AddRange(shapeFile.GetGeometriesInView(shapeFile.GetExtents()));
			Assert.AreEqual(shapeFile.GetFeatureCount(), geometries.Count);
			geometries.Clear();

			geometries.AddRange(shapeFile.GetGeometriesInView(BoundingBox.Empty));
			Assert.AreEqual(0, geometries.Count);
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void GetGeometriesInViewWhenClosedThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			List<Geometry> geometries = new List<Geometry>(shapeFile.GetGeometriesInView(BoundingBox.Empty));
        }

        [Test]
        public void ExecuteIntersectionQueryByBoundingBoxTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            shapeFile.Open();
            FeatureDataSet data = new FeatureDataSet();
            shapeFile.ExecuteIntersectionQuery(shapeFile.GetExtents(), data);
            Assert.AreEqual(1, data.Tables.Count);
            Assert.AreEqual(shapeFile.GetFeatureCount(), data.Tables[0].Rows.Count);
            shapeFile.Close();
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void ExecuteIntersectionQueryByBoundingBoxWhenClosedThrowsExceptionTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            FeatureDataSet data = new FeatureDataSet();
            shapeFile.ExecuteIntersectionQuery(shapeFile.GetExtents(), data);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ExecuteIntersectionQueryByGeometryTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
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
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
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
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void GetGeometryByIdWhenClosedThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetFeatureCountTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetFeatureTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetExtentsTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ConnectionIdTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SridTest()
        {
        }

        [Test]
        public void SaveRowTest()
		{
			FeatureDataTable<uint> schema = new FeatureDataTable<uint>("oid");
			schema.Columns.AddRange(new DataColumn[] {
				new DataColumn("Name", typeof(String)),
				new DataColumn("Date_Created", typeof(DateTime)),
				new DataColumn("Visits", typeof(int)),
				new DataColumn("Weight", typeof(float))
			});

			ShapeFile shapeFile = ShapeFile.Create("UnitTestData", "Test2", ShapeType.Point, schema);
			shapeFile.Open();

			DateTime dateCreated = DateTime.Now;
			FeatureDataRow<uint> feature = schema.NewRow(1);
			feature["Name"] = "Test feature";
			feature["Date_Created"] = dateCreated;
			feature["Visits"] = 0;
			feature["Weight"] = 100.0f;
			feature.Geometry = new Point(0, 0);

			shapeFile.Save(feature);

			shapeFile.Close();

			shapeFile = new ShapeFile(@"UnitTestData\Test2");
			shapeFile.Open();

			Assert.AreEqual(1, shapeFile.GetFeatureCount());
			FeatureDataSet dataSet = new FeatureDataSet();

			shapeFile.ExecuteIntersectionQuery(new BoundingBox(0, 0, 0, 0), dataSet);

			Assert.AreEqual(1, dataSet.Tables.Count);
			Assert.AreEqual(1, dataSet.Tables[0].Rows.Count);

			FeatureDataRow<uint> newFeature = dataSet.Tables[0].Rows[0] as FeatureDataRow<uint>;
			Assert.AreEqual(new Point(0, 0), newFeature.Geometry);
			Assert.AreEqual(newFeature["Name"], "Test feature");
			Assert.AreEqual(newFeature["Date_Created"], dateCreated);
			Assert.AreEqual(newFeature["Visits"], 0);
			Assert.AreEqual(newFeature["Weight"], 100.0f);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SaveRowsTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SaveTableTest()
        {
        }

        [Test]
        [Ignore("Delete method not yet implemented")]
        public void DeleteRowTest()
        {
		}

		private static IProjectedCoordinateSystem createExpectedCoordinateSystem()
		{
			ICoordinateSystemFactory factory = new CoordinateSystemFactory();

			IEllipsoid grs80 = Ellipsoid.GRS80;

			IHorizontalDatum harn = factory.CreateHorizontalDatum("D_North_American_1983_HARN",
				DatumType.HD_Classic, grs80, null);

			IPrimeMeridian greenwich = PrimeMeridian.Greenwich;

			AxisInfo axis0 = new AxisInfo("Lon", AxisOrientationEnum.East);
			AxisInfo axis1 = new AxisInfo("Lat", AxisOrientationEnum.North);

			IGeographicCoordinateSystem gcs = factory.CreateGeographicCoordinateSystem("GCS_North_American_1983_HARN",
				AngularUnit.Degrees, harn, greenwich, axis0, axis1);

			IProjection prj = factory.CreateProjection("Lambert_Conformal_Conic", "Lambert_Conformal_Conic", new ProjectionParameter[] 
					{	new ProjectionParameter("False_Easting", 8202099.737532808), 
						new ProjectionParameter("False_Northing", 0),
						new ProjectionParameter("Central_Meridian", -120.5),
						new ProjectionParameter("Standard_Parallel_1", 44.33333333333334),
						new ProjectionParameter("Standard_Parallel_2", 46.0),
						new ProjectionParameter("Latitude_Of_Origin", 43.66666666666666),
					});

			IProjectedCoordinateSystem expected = factory.CreateProjectedCoordinateSystem("NAD_1983_HARN_StatePlane_Oregon_North_FIPS_3601",
				gcs, prj, LinearUnit.Foot, axis0, axis1);

			// TODO: Check if this is correct, since on line 184 of CoorindateSystemFactory.cs, HorizontalDatum is passed in as null
			expected.HorizontalDatum = harn;
			return expected;
		}
	}
}
