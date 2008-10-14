
using System;
using System.Collections.Generic;
using System.Data;
using DBFile = System.IO.File;
using System.Text;

using NUnit.Framework;

using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.PostGis;
using SharpMap.Expressions;
using SharpMap.Layers;

using NetTopologySuite.Coordinates;
using GisSharpBlog.NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;

using Npgsql;
using NpgsqlTypes;
/*
 *
 * These tests will not work with the current version of SharpMap 2.0
 * Felix Obermaier
 *
 */
 
namespace SharpMap.Tests.Data.Providers.PostGis
{
    [TestFixture]
    public class PostGis_ProviderTests
    {
        private static IGeometryFactory _geometryFactory;
        private static String connectionString = 
            "Server=127.0.0.1;Port=5432;" +
            "Userid=obe;Password=obe;database=obe;" +
            "Protocol=3;SSL=false;" +
            "Pooling=true;MinPoolSize=1;MaxPoolSize=20;"+
            "Encoding=UNICODE;Timeout=15;SslMode=Disable;" +
            "Enlist=true;";
        
        //[TestFixtureSetUp]
        static PostGis_ProviderTests()
        {
            try
            {
                using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
                {
                    cn.Open();
                    try
                    {
                        new NpgsqlCommand("DROP TABLE public.\"TestFeatureDataTable\";", cn).ExecuteNonQuery();
                        //System.Diagnostics.Debug.Write(
                        //    new NpgsqlCommand("SELECT ST_AsText(x.geom) FROM (SELECT ST_GeomFromText('POLYGON((-1 -1, 0 1, 1 -1, -1 -1))',-1) AS geom) AS x;", cn).ExecuteScalar());
                    }
                    catch (NpgsqlException ex)
                    {
                        System.Diagnostics.Trace.Write(ex.Message);
                    }
                }
            }
            catch
            { }
		

            BufferedCoordinateSequenceFactory sequenceFactory = new BufferedCoordinateSequenceFactory();
            _geometryFactory = new GeometryFactory<BufferedCoordinate>(sequenceFactory);
        }
        
        //private static readonly BufferedCoordinate2DFactory _coordinateFactory;
        //private static readonly BufferedCoordinate2DSequenceFactory _coordinateSequenceFactory;
        //private static readonly GeometryFactory<BufferedCoordinate2D> _geometryFactory;
        //private static readonly CoordinateSystemFactory<BufferedCoordinate2D> _coordinateSystemFactory;

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

        [Test]
        private void T11_JD_Test()
        {
            var search = new PostGis_Provider<Int32>(_geometryFactory,
                                                  connectionString, "public",
                                                  "vw_osm_germany_line", "osm_id", "way");

            var binaryExpression =
                new BinaryExpression(new PropertyNameExpression("z_order"),
                                     BinaryOperator.Equals, new LiteralExpression<int>(0));

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
                            new OrderByExpression(new[] {"z_order ASC"}),
                            new DataPageSizeExpression(10),
                            new DataPageNumberExpression(5) 

                        });
            var ape = new AttributesProjectionExpression(new String[] { "osm_id", "name", "z_order", "way" });

            prov = new ProviderQueryExpression(providerProps, ape, null);

            sfdr = search.ExecuteQuery(prov) as SharpMap.Data.Providers.Db.SpatialDbFeatureDataReader;
            Assert.IsNotNull(sfdr);
            numRows = 0;
            while (sfdr.Read()) numRows++;
            Assert.AreEqual(10, numRows);
            System.Diagnostics.Debug.WriteLine("\r\n*** T11 passed!");
        }

        [Test]
        public void T01_CreateTableFromFeatureDataTable()
        {
            FeatureDataTable fdt = createFeatureDataTable();
            PostGis_ProviderStatic.CreateDataTable<Int64>(fdt, connectionString);

            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
                _geometryFactory, connectionString, 
                "public", fdt.TableName, fdt.PrimaryKey[0].ColumnName, 
                PostGis_ProviderStatic.DefaultGeometryColumnName);
            Assert.IsNotNull( prov );
            Assert.AreEqual(connectionString, prov.ConnectionString);
            Assert.AreEqual("public", prov.TableSchema);
            Assert.AreEqual(fdt.TableName, prov.Table);
            Assert.AreEqual(PostGis_ProviderStatic.DefaultSrid, prov.Srid);
            Assert.AreEqual(fdt.PrimaryKey[0].ColumnName.ToLower(), prov.OidColumn);
            Assert.AreEqual(PostGis_ProviderStatic.DefaultGeometryColumnName, prov.GeometryColumn);

        }

        [Test]
        public void T02_CreateGeometryLayerFromPostGis()
        {
            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");

            SharpMap.Layers.GeometryLayer gl = new SharpMap.Layers.GeometryLayer("test", prov);
            Assert.IsNotNull( gl );
            Assert.AreEqual("test", gl.LayerName);
            Assert.AreEqual(prov.Srid, gl.Srid);
        }

        [Test]
        public void T03_SomeSelects()
        {
            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");
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
                    new GeometryExpression(prov.GeometryFactory.WktReader.Read(("POLYGON((-1 -1, -1 1, 0 1, 0 -1, -1 -1))"))), 
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
            //Assert.AreEqual(0, gl.Features.Rows.Count);

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
            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
                _geometryFactory, connectionString, "TestFeatureDataTable");
            
            GeometryLayer gl = new GeometryLayer(prov);
            gl.Select(new SpatialBinaryExpression(
                new ExtentsExpression(prov.GetExtents()), SpatialOperation.Contains, new ThisExpression()));
            gl.Features.AcceptChanges();

            //gl.Features.RemoveRow(gl.Features[4]);
            FeatureDataRow fdr = gl.Features.Find(999);
            fdr.Delete();
            prov.Delete(gl.Features.GetChanges(DataRowState.Deleted));
            gl.Features.AcceptChanges();

            gl = new GeometryLayer(prov);
            gl.Select(new SpatialBinaryExpression(
                new ExtentsExpression(prov.GetExtents()), SpatialOperation.Contains, new ThisExpression()));
            gl.Features.AcceptChanges();
            Assert.AreEqual(4, gl.Features.Rows.Count);

        }

        [Test]
        
        public void T04_InsertInDataSource()
        {
            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
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
            //Assert.AreEqual(5, gl.SelectedFeatures.Count);
            
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

        [Test]
        public void T99_GeometryFromBinary()
        {
            PostGis_Provider<Int64> prov = new PostGis_Provider<Int64>(
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
                System.Diagnostics.Debug.Write(cm.ExecuteScalar());
            }
            
            

        }

}
}
