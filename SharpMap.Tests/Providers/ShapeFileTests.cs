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
			ShapeFile shapeFile = ShapeFile.Create(".", "Test1", ShapeType.Point, schema);
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
			if (File.Exists(@"NewBCROADS.SHP")) File.Delete(@"NewBCROADS.SHP");
			if (File.Exists(@"NewBCROADS.SHX")) File.Delete(@"NewBCROADS.SHX");
			if (File.Exists(@"NewBCROADS.DBF")) File.Delete(@"NewBCROADS.DBF");
			if (File.Exists(@"NewBCROADS.SBN")) File.Delete(@"NewBCROADS.SBN");
			if (File.Exists(@"NewBCROADS.SBX")) File.Delete(@"NewBCROADS.SBX");
			if (File.Exists(@"NewBCROADS.PRJ")) File.Delete(@"NewBCROADS.PRJ");

			File.Copy(@"..\TestData\BCROADS.SHP", @"BCROADSCopy.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"BCROADSCopy.SHX", true);
			File.Copy(@"..\TestData\BCROADS.DBF", @"BCROADSCopy.DBF", true);
			File.Copy(@"..\TestData\BCROADS.SBN", @"BCROADSCopy.SBN", true);
			File.Copy(@"..\TestData\BCROADS.SBX", @"BCROADSCopy.SBX", true);
			File.Copy(@"..\TestData\BCROADS.PRJ", @"BCROADSCopy.PRJ", true);

			ShapeFile shapeFile = new ShapeFile(@"BCROADSCopy.SHP");
			shapeFile.Filename = @"NewBCROADS.SHP";

			Assert.IsTrue(File.Exists(@"NewBCROADS.SHP"));
			Assert.IsTrue(File.Exists(@"NewBCROADS.SHX"));
			Assert.IsTrue(File.Exists(@"NewBCROADS.DBF"));
			Assert.IsTrue(File.Exists(@"NewBCROADS.SBN"));
			Assert.IsTrue(File.Exists(@"NewBCROADS.SBX"));
			Assert.IsTrue(File.Exists(@"NewBCROADS.PRJ"));

			shapeFile.Open();
			Assert.AreEqual(@"NewBCROADS.SHP", shapeFile.Filename);
			shapeFile.Close();

			File.Delete(@"BCROADSCopy.SHP");
			File.Delete(@"BCROADSCopy.SHX");
			File.Delete(@"BCROADSCopy.DBF");
			File.Delete(@"BCROADSCopy.SBN");
			File.Delete(@"BCROADSCopy.SBX");
			File.Delete(@"BCROADSCopy.PRJ");

			File.Delete(@"NewBCROADS.SHP");
			File.Delete(@"NewBCROADS.SHX");
			File.Delete(@"NewBCROADS.DBF");
			File.Delete(@"NewBCROADS.SBN");
			File.Delete(@"NewBCROADS.SBX");
			File.Delete(@"NewBCROADS.PRJ");
        }

        [Test]
        [ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void SetFilenameWhenOpenThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Open();
			shapeFile.Filename = @"NewBCROADS.SHP";
        }

        [Test]
		[ExpectedException(typeof(ShapeFileInvalidOperationException))]
        public void SetFilenameWhenFileExistsThrowsExceptionTest()
		{
			File.Copy(@"..\TestData\BCROADS.SHP", @"BCROADSCopy.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"BCROADSCopy.SHX", true);
			File.Copy(@"..\TestData\BCROADS.SHP", @"NewBCROADS.SHP", true);
			File.Copy(@"..\TestData\BCROADS.SHX", @"NewBCROADS.SHX", true);

			ShapeFile shapeFile = new ShapeFile(@"BCROADSCopy.SHP");
			shapeFile.Filename = @"NewBCROADS.SHP";
        }

        [Test]
        [ExpectedException(typeof(ShapeFileIsInvalidException))]
        public void SetFilenameToNonShpExtensionThrowsExceptionTest()
		{
			ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
			shapeFile.Filename = @"NewBCROADS.abc";
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
        [Ignore("Test not yet implemented")]
        public void SaveRowTest()
        {
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
