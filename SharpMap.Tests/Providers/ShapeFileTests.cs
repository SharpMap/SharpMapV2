using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SharpMap.Data.Providers;
using System.IO;

namespace SharpMap.Tests.Provider
{
	[TestFixture]
	public class ShapeFileTests
    {
        [Test]
        [Ignore("Test not yet implemented")]
        public void CreatNeweNoAttributesTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void CreateNewWithAttributesSchemaTest()
        {
        }

        [Test]
        public void CreateWithoutFileBasedSpatialIndexTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP");
            Assert.IsNotNull(shapeFile);
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
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
        public void RebuildSpatialIndexWhenClosedThrowsExceptionTest()
        {
            ShapeFile shapeFile = new ShapeFile(@"..\TestData\BCROADS.SHP", true);
            shapeFile.RebuildSpatialIndex();
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
        [Ignore("Test not yet implemented")]
        public void GetCoordinateSystemHavingPrjFileTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void NoPrjFileImpliesCoordinateSystemIsNullTest()
        {
        }

        [Test]
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
        [Ignore("Test not yet implemented")]
        public void SetCoordinateSystemWithPrjFileThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void NoPrjFileSetCoordinateSystemTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetShapeTypeTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetShapeTypeWhenClosedThrowsExceptionTest()
        {

        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetFilenameTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void SetFilenameTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
        public void SetFilenameWhenOpenThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SetFilenameWhenFileMissingThrowsExceptionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(InvalidShapeFileException))]
        public void SetFilenameToNonShpExtensionThrowsExceptionTest()
        {
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
        [Ignore("Test not yet implemented")]
        public void IsOpenTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void OpenTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void OpenExclusiveTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void CloseTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void GetGeometriesInViewTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
        public void GetGeometriesInViewWhenClosedThrowsExceptionTest()
        {
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
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
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
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
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
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
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
        [ExpectedException(typeof(InvalidShapeFileOperationException))]
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
	}
}
