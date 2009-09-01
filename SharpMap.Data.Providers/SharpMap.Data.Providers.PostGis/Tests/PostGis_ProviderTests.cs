using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.PostGis;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;
using DBFile = System.IO.File;

/*
 *
 * These tests will not work with the current version of SharpMap 2.0
 * Felix Obermaier
 *
 */

namespace SharpMap.Tests.Data.Providers.PostGis
{
    [TestClass]
    public class PostGis_ProviderTests
    {
        private const String connectionString =
            "Server=127.0.0.1;Port=5432;" +
            "Userid=obe;Password=obe;database=obe;" +
            "Protocol=3;SSL=false;" +
            "Pooling=true;MinPoolSize=1;MaxPoolSize=20;" +
            "Encoding=UNICODE;Timeout=15;SslMode=Disable;" +
            "Enlist=true;";

        private static IGeometryFactory _geometryFactory;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void PostGis_ProviderTestsInitialize(TestContext testContext)
        {
            try
            {
                //Delete working table
                using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
                {
                    cn.Open();
                    new NpgsqlCommand("DROP TABLE IF EXISTS public.\"TestFeatureDataTable\";", cn).ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch
            {
            }
            SridMap.DefaultInstance =
                new SridMap(new[] {new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});
            _geometryFactory = new GeometryServices().DefaultGeometryFactory;
        }

        [ClassCleanup]
        public static void PostGis_ProviderTestCleanup()
        {
            _geometryFactory = null;
        }

        [TestMethod]
        public void RunTests()
        {
            T11_JD_Test();

            T01_CreateTableFromFeatureDataTable();
            T02_CreateGeometryLayerFromPostGis();
            T03_SomeSelects();
            T04_InsertInDataSource();
            T05_DeleteFromDataSource();
            T06_UpdateDataSource();
        }

        [TestMethod]
        public void T11_JD_Test()
        {
            PostGisProvider<int> search = new PostGisProvider<Int32>(_geometryFactory,
                                                                     connectionString, "public",
                                                                     "vw_osm_germany_line", "osm_id", "way");

            BinaryExpression binaryExpression =
                new BinaryExpression(new PropertyNameExpression("z_order"),
                                     BinaryOperator.Equals, new LiteralExpression<int>(0));

            ProviderPropertiesExpression providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[] {});


            ProviderQueryExpression prov = new ProviderQueryExpression(null as ProviderPropertiesExpression,
                                                                       new AllAttributesExpression(), binaryExpression);

            object obj = search.ExecuteQuery(prov);

            Assert.IsNotNull(obj);
            PostGisFeatureDataReader sfdr = obj as PostGisFeatureDataReader;
            Assert.IsNotNull(sfdr);
            Int64 numRows = 0;
            while (sfdr.Read())
                numRows++;
            Assert.IsTrue(numRows > 1);

            providerProps =
                new ProviderPropertiesExpression(
                    new ProviderPropertyExpression[]
                        {
                            //new OrderByCollectionExpression(
                            //    new OrderByExpression("z_order", SortOrder.Descending)),
                            new DataPageSizeExpression(10),
                            new DataPageNumberExpression(5)
                        });
            AttributesProjectionExpression ape =
                new AttributesProjectionExpression(new[] {"osm_id", "name", "z_order", "way"});

            prov = new ProviderQueryExpression(providerProps, ape, null);

            sfdr = search.ExecuteQuery(prov) as PostGisFeatureDataReader;
            Assert.IsNotNull(sfdr);
            numRows = 0;
            while (sfdr.Read()) numRows++;
            Assert.AreEqual(10, numRows);
            Debug.WriteLine("\r\n*** T11 passed!");
        }

        [TestMethod]
        public void T01_CreateTableFromFeatureDataTable()
        {
            FeatureDataTable fdt = createFeatureDataTable();
            PostGisProviderStatic.CreateDataTable<Int64>(fdt, connectionString);

            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString,
                "public", fdt.TableName, fdt.PrimaryKey[0].ColumnName,
                PostGisProviderStatic.DefaultGeometryColumnName);
            Assert.IsNotNull(prov);
            Assert.AreEqual(connectionString, prov.ConnectionString);
            Assert.AreEqual("public", prov.TableSchema);
            Assert.AreEqual(fdt.TableName, prov.Table);
            Assert.AreEqual(PostGisProviderStatic.DefaultSrid, prov.Srid);
            Assert.AreEqual(fdt.PrimaryKey[0].ColumnName.ToLower(), prov.OidColumn);
            Assert.AreEqual(PostGisProviderStatic.DefaultGeometryColumnName, prov.GeometryColumn);
        }

        [TestMethod]
        public void T02_CreateGeometryLayerFromPostGis()
        {
            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");

            GeometryLayer gl = new GeometryLayer("test", prov);
            Assert.IsNotNull(gl);
            Assert.AreEqual("test", gl.LayerName);
            Assert.AreEqual(prov.Srid, gl.Srid);
        }

        [TestMethod]
        public void T03_SomeSelects()
        {
            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");
            GeometryLayer gl = new GeometryLayer("test", prov);

            gl.Select(new AllAttributesExpression());
            gl.Features.AcceptChanges();
            Assert.AreEqual(4, gl.Features.FeatureCount);
            Assert.AreEqual(_geometryFactory.CreateExtents2D(0, -3, 15, 20), gl.Features.Extents);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new ExtentsExpression(prov.GeometryFactory.CreateExtents2D(-1, -1, 1, 1)),
                    SpatialOperation.Contains,
                    new ThisExpression())
                );
            Assert.AreEqual(1, gl.Features.Rows.Count);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new GeometryExpression(prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, 0 1, 1 -1, -1 -1))"))),
                    SpatialOperation.Contains,
                    new ThisExpression())
                );
            //SELECT testfeaturedatatable.poid,testfeaturedatatable.label,ST_AsBinary(xgeometryx)::bytea AS xgeometryx FROM public.testfeaturedatatable   WHERE  ( ST_Contains( ST_GeomFromWKB(:iparam0, -1), xgeometryx ))
            //SELECT testfeaturedatatable.poid,testfeaturedatatable.label,ST_AsBinary(xgeometryx)::bytea AS xgeometryx FROM public.testfeaturedatatable   WHERE  ( ST_Contains( ST_GeomFromText('POLYGON((-1 -1, 0 1, 1 -1, -1 -1))', -1), xgeometryx ))
            Assert.AreEqual(1, gl.Features.Rows.Count);

            gl = new GeometryLayer(prov);
            gl.Select(
                new SpatialBinaryExpression(
                    new GeometryExpression(
                        prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, -1 1, 0 1, 0 -1, -1 -1))"))),
                    SpatialOperation.Touches,
                    new ThisExpression())
                );
            Assert.AreEqual(1, gl.Features.Rows.Count);

            //gl = new GeometryLayer(prov);
            //gl.Select(
            //    new SpatialBinaryExpression(
            //        new ThisExpression(),
            //        SpatialOperation.Within,
            //        new GeometryExpression(prov.GeometryFactory.WktReader.Read("POLYGON((-1 -1, 0 1, 1 -1, -1 -1))")))
            //        );
            //Assert.AreEqual(0, gl.Features.Rows.TotalItemCount);
        }

        [TestMethod]
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

        [TestMethod]
        public void T05_DeleteFromDataSource()
        {
            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");

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
            //Assert.AreEqual(4, gl.Features.Rows.TotalItemCount);
        }

        [TestMethod]
        public void T04_InsertInDataSource()
        {
            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");

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
            //Assert.AreEqual(5, gl.SelectedFeatures.TotalItemCount);

            fdr = gl.Features.Find(999);
            Assert.IsNotNull(fdr);
        }

        private static FeatureDataTable createFeatureDataTable()
        {
            FeatureDataTable<Int64> fdt = new FeatureDataTable<Int64>("TestFeatureDataTable", "poid", _geometryFactory);
            DataColumn dc = fdt.Columns["poid"];
            dc.AutoIncrementSeed = 1001;
            dc.AutoIncrementStep = 1;
            dc.AutoIncrement = true;
            fdt.Columns.Add("LABEL", typeof (String));

            FeatureDataRow fdr = null;
            IWktGeometryReader wktReader = fdt.GeometryFactory.WktReader;

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
            IWktGeometryReader wktReader = featureTable.GeometryFactory.WktReader;

            FeatureDataRow fdr = (FeatureDataRow) featureTable.Rows[1];
            fdr.Geometry = wktReader.Read("LINESTRING(0 0, 10 20, 15 7, 7 -3)");
            fdr[1] = fdrLabel(fdr);
            (featureTable.Rows[1])[1] = "modified";
        }

        private static String fdrLabel(FeatureDataRow fdr)
        {
            return String.Format("Feature {0} with GeometryType {1}: {2}",
                                 fdr.GetOid(), fdr.Geometry.GeometryTypeName, fdr.Geometry);
        }

        [TestMethod]
        public void T99_GeometryFromBinary()
        {
            PostGisProvider<Int64> prov = new PostGisProvider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");
            GeometryLayer gl = new GeometryLayer("test", prov);

            gl.Select(new AllAttributesExpression());
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                NpgsqlCommand cm = new NpgsqlCommand(
                    "SELECT ST_SRID( ST_GeomFromWKB(:p0::bytea, -1) )", cn);
                NpgsqlParameter par = new NpgsqlParameter(":p0", DbType.Binary);
                par.Value = prov.GeometryFactory.WktReader.Read("POLYGON((-1 -1, 0 1, 1 -1, -1 -1))").AsBinary();
                cm.Parameters.Add(par);
                Debug.Write(cm.ExecuteScalar());
            }
        }
    }
}