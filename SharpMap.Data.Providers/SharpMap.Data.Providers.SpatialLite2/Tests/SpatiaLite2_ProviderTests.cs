
using System;
using System.Collections.Generic;
using System.Data;
using DBFile = System.IO.File;
using System.Text;

using NUnit.Framework;

using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.SpatiaLite2;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Data.Providers.Db.Expressions;

using NetTopologySuite.Coordinates;
using GisSharpBlog.NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;

/*
 *
 * These tests will not work with the current version of SharpMap 2.0
 * Felix Obermaier
 *
 */
 
namespace SharpMap.Tests.Data.Providers.SpatiaLite2
{
    [TestFixture]
    public class SpatiaLite2_ProviderTests
    {
        private static IGeometryFactory _geometryFactory;
        
        //[TestFixtureSetUp]
        static SpatiaLite2_ProviderTests()
        {
            //Delete possibly existing database file
            if (DBFile.Exists(@"test.sqlite")) DBFile.Delete(@"test.sqlite");

            BufferedCoordinateSequenceFactory sequenceFactory = new BufferedCoordinateSequenceFactory();
            _geometryFactory = new GeometryFactory<BufferedCoordinate>(sequenceFactory);
        }
        
        //private static readonly BufferedCoordinate2DFactory _coordinateFactory;
        //private static readonly BufferedCoordinate2DSequenceFactory _coordinateSequenceFactory;
        //private static readonly GeometryFactory<BufferedCoordinate2D> _geometryFactory;
        //private static readonly CoordinateSystemFactory<BufferedCoordinate2D> _coordinateSystemFactory;

        public void RunTests()
        {
            T01_CreateTableFromFeatureDataTable();
            T02_CreateGeometryLayerFromSpatiaLite2();
            T03_MultipleSelects();
            T04_InsertInDataSource();
            T05_DeleteFromDataSource();
            T06_UpdateDataSource();

            T11_JD_Test();
        }
        [Test]
        private void T11_JD_Test()
        {
            return;
            var search = new SpatiaLite2_Provider(_geometryFactory,
                                                  @"Data Source=C:\VENUS\CodePlex\sharpMap\SharpMap\TestData\VRS2386_V11.sqlite", "main",
                                                  "regions", "OID", "XGeometryX");

            //search.SpatiaLiteIndexType = SpatiaLite2_IndexType.MBRCache;

            var binaryExpression =
                new BinaryExpression(new PropertyNameExpression("VHG5"),
                                     BinaryOperator.GreaterThan, new LiteralExpression<int>(6));

            var providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[] { });


            var prov = new ProviderQueryExpression(providerProps, new AllAttributesExpression(), binaryExpression);

            object obj = search.ExecuteQuery(prov);

            Assert.IsNotNull(obj);
            SharpMap.Data.Providers.Db.SpatialDbFeatureDataReader sfdr = obj as SharpMap.Data.Providers.Db.SpatialDbFeatureDataReader;
            Assert.IsNotNull(sfdr);
            Int64 numRows = 0;
            while (sfdr.Read())
                numRows++;
            Assert.GreaterOrEqual(numRows, 1);

            providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[]
                        {
                            new OrderByCollectionExpression(
                                new OrderByExpression("Einwohner", System.Data.SqlClient.SortOrder.Descending)),
                            new DataPageSizeExpression(10),
                            new DataPageNumberExpression(5) 

                        });
            var ape = new AttributesProjectionExpression(new String[]{"OID" ,"Einwohner", "Erwerbstätige", "Beschäftigte"});

            prov = new ProviderQueryExpression(providerProps, ape, null);

            sfdr = search.ExecuteQuery(prov) as SharpMap.Data.Providers.Db.SpatialDbFeatureDataReader;
            Assert.IsNotNull(sfdr);
            numRows = 0;
            while (sfdr.Read()) numRows++;
            Assert.AreEqual(10, numRows);
        }

        [Test]
        public void T01_CreateTableFromFeatureDataTable()
        {
            FeatureDataTable fdt = createFeatureDataTable();
            SpatiaLite2_Provider.CreateDataTable(fdt, @"Data Source=test.sqlite");

            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
                _geometryFactory, @"Data Source=test.sqlite", 
                "main", fdt.TableName, fdt.PrimaryKey[0].ColumnName, 
                SpatiaLite2_Provider.DefaultGeometryColumnName);
            Assert.IsNotNull( prov );
            Assert.AreEqual(@"Data Source=test.sqlite", prov.ConnectionString);
            Assert.AreEqual("main", prov.TableSchema);
            Assert.AreEqual(fdt.TableName, prov.Table);
            Assert.AreEqual(SpatiaLite2_Provider.DefaultSrid, prov.Srid);
            Assert.AreEqual(fdt.PrimaryKey[0].ColumnName, prov.OidColumn);
            Assert.AreEqual(SpatiaLite2_Provider.DefaultGeometryColumnName, prov.GeometryColumn);

            Assert.IsTrue(prov.SpatiaLiteIndexType != SpatiaLite2_IndexType.None);
            Assert.AreEqual(SpatiaLite2_Provider.DefaultSpatiaLiteIndexType, prov.SpatiaLiteIndexType);

        }

        [Test]
        public void T02_CreateGeometryLayerFromSpatiaLite2()
        {
            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
                _geometryFactory, @"Data Source=test.sqlite", "TestFeatureDataTable");

            SharpMap.Layers.GeometryLayer gl = new SharpMap.Layers.GeometryLayer("test", prov);
            Assert.IsNotNull( gl );
            Assert.AreEqual("test", gl.LayerName);
            Assert.AreEqual(prov.Srid, gl.Srid);
        }

        [Test]
        public void T03_MultipleSelects()
        {
            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
                _geometryFactory, @"Data Source=test.sqlite", "TestFeatureDataTable");
            GeometryLayer gl = new GeometryLayer("test", prov);

            gl.Select(new AllAttributesExpression());
            gl.Features.AcceptChanges();
            Assert.AreEqual(4, gl.Features.FeatureCount);
            Assert.AreEqual(_geometryFactory.CreateExtents2D(0,-3,15,20), gl.Features.Extents);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new ExtentsExpression(prov.GeometryFactory.CreateExtents2D(-1,-1,1,1)), 
                    SpatialOperation.Contains, 
                    new ThisExpression())
                    ); 
            Assert.AreEqual(1, gl.Features.Rows.Count);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new GeometryExpression(prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, 0 1, 1 1, -1 -1))"))), 
                    SpatialOperation.Contains, 
                    new ThisExpression())
                    );
            Assert.AreEqual(1, gl.Features.Rows.Count);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new GeometryExpression(prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, -1 1, 0 1, 0 -1, -1 -1))"))), 
                    SpatialOperation.Touches, 
                    new ThisExpression())
                    ); 
            Assert.AreEqual(1, gl.Features.Rows.Count, 1);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new ThisExpression(),
                    SpatialOperation.Within,
                    new GeometryExpression(prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, 0 1, 1 1, -1 -1))"))))
                    );
            Assert.AreEqual(0, gl.Features.Rows.Count);

        }

        [Test]
        public void T06_UpdateDataSource()
        {
            //try
            //{
            //    prov.Update(gl.Features.GetChanges(DataRowState.Modified).Rows);
            //}
            //catch
            //{ }
            //alterFeatureDataTable(gl.Features);
        }

        [Test]
        public void T05_DeleteFromDataSource()
        {
            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
                _geometryFactory, @"Data Source=test.sqlite", "TestFeatureDataTable");
            
            GeometryLayer gl = new GeometryLayer(prov);
            gl.Select(new SpatialBinaryExpression(
                new ExtentsExpression(prov.GetExtents()), SpatialOperation.Contains, new ThisExpression()));
            gl.Features.AcceptChanges();

            //gl.Features.RemoveRow(gl.Features[4]);
            FeatureDataRow fdr = gl.Features.Find(999);
            fdr.Delete();
            //prov.Delete(gl.Features.GetChanges(DataRowState.Deleted));
            gl.Features.AcceptChanges();

            gl = new GeometryLayer(prov);
            gl.Select(new SpatialBinaryExpression(
                new ExtentsExpression(prov.GetExtents()), SpatialOperation.Contains, new ThisExpression()));
            gl.Features.AcceptChanges();
            //Assert.AreEqual(4, gl.Features.Rows.Count);

        }

        [Test]
        
        public void T04_InsertInDataSource()
        {
            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
                _geometryFactory, @"Data Source=test.sqlite", "TestFeatureDataTable");

            GeometryLayer gl = new GeometryLayer("test", prov);
            gl.Select(new AllAttributesExpression());
            gl.Features.AcceptChanges();

            FeatureDataRow fdr = gl.Features.NewRow();

            fdr[0] = 999;
            fdr.Geometry = gl.GeometryFactory.WktReader.Read("POINT (-2 13)");
            fdr[1] = fdrLabel(fdr);
            gl.Features.AddRow(fdr);

            prov.Insert(gl.Features.GetChanges());

            //gl.SelectedFilter = new FeatureQueryExpression(
            //    new AttributeBinaryExpression(
            //        new PropertyNameExpression("OID"),
            //        BinaryOperator.Equals,
            //        (Int64)999));
            //Assert.AreEqual(5, gl.SelectedFeatures.Count);
            
            fdr = gl.Features.Find(999);
            Assert.IsNotNull(fdr);

        }

        private static FeatureDataTable createFeatureDataTable()
        {
            FeatureDataTable<Int64> fdt = new FeatureDataTable<Int64>("TestFeatureDataTable", "OID", _geometryFactory);
            DataColumn dc = fdt.Columns["OID"];
            dc.AutoIncrementSeed = 1001;
            dc.AutoIncrementStep = 1;
            dc.AutoIncrement = true;
            fdt.Columns.Add("LABEL", typeof(String));

            FeatureDataRow fdr = null;
            GeoAPI.IO.WellKnownText.IWktGeometryReader wktReader = fdt.GeometryFactory.WktReader;
            
            fdr = fdt.NewRow();
            fdr.Geometry = wktReader.Read("POINT (0 0)");
            fdr[1] = fdrLabel(fdr);
            fdt.AddRow(fdr);

            fdr = fdt.NewRow();
            fdr.Geometry = wktReader.Read("POINT (10 20)");
            fdr[1] = fdrLabel(fdr);
            fdt.AddRow(fdr);

            fdr = fdt.NewRow();
            fdr.Geometry = wktReader.Read("POINT (15 7)");
            fdr[1] = fdrLabel(fdr);
            fdt.AddRow(fdr);

            fdr = fdt.NewRow();
            fdr.Geometry = wktReader.Read("POINT (7 -3)");
            fdr[1] = fdrLabel(fdr);
            fdt.AddRow(fdr);

            return fdt;
        }

        private static void alterFeatureDataTable(FeatureDataTable featureTable)
        {
            GeoAPI.IO.WellKnownText.IWktGeometryReader wktReader = featureTable.GeometryFactory.WktReader;

            FeatureDataRow fdr = (FeatureDataRow)featureTable.Rows[1];
            fdr.Geometry = wktReader.Read("LINESTRING(0 0, 10 20, 15 7, 7 -3)");
            fdr[1] = fdrLabel(fdr);
            ((FeatureDataRow)featureTable.Rows[1])[1] = "modified";
        }

        private static String fdrLabel(FeatureDataRow fdr)
        {
            return String.Format("Feature {0} with GeometryType {1}: {2}",
              fdr.GetOid(), fdr.Geometry.GeometryTypeName, fdr.Geometry.ToString());
        }

}
}
