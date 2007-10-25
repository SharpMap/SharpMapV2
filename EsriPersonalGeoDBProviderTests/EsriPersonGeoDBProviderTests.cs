using DigitalInspections.Mapping.Data;
using DigitalInspections.Mapping.Data.Testing.Properties;
using DigitalInspections.Mapping.SharpMap.Data;

using NUnit.Framework;

using SharpMap;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.CoordinateSystems;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;
using SharpMap.Geometries;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DigitalInspections.Mapping.Data.Testing
{
    [TestFixture]
    public class EsriPersonalGeoDatabaseProviderTests
    {
		//private readonly static string TestLayerName = "EsriPersonalGeoDatabaseProvider Unit Tests";
		//private readonly static string TestLayerViewName = "casspatial";
		//private readonly static int    TestSrid = 26910;
        //private readonly static LayerPropertiesType TestStyle;

        private string _connectionString;
        private string _provider;
		private string _testLayerName;

        //private DbProviderFactory _dbFact;
        //private CascadeInternalLayer _workingLayer;
        //private int _existingLayerId;

        static EsriPersonalGeoDatabaseProviderTests()
        {
			//TestStyle = new LayerPropertiesType();
			//TestStyle.Item = new VectorStyleStorageType();
        }


        [TestFixtureSetUp]
        public void EsriPersonalGeoDatabaseProviderTestFixtureSetup()
        {
			_connectionString = Settings.Default.Connection;
			_provider = Settings.Default.Provider;
			_testLayerName = Settings.Default.TestLayer;
        }

        [TestFixtureTearDown]
        public void EsriPersonalGeoDatabaseProviderTestTearDown()
        {
        }

        [Test]
        public void OpenCloseTest()
        {
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);
			provider.Open();
			Assert.IsTrue(provider.IsOpen);
			provider.Close();
			Assert.IsFalse(provider.IsOpen);
			provider.Open();
			Assert.IsTrue(provider.IsOpen);
			provider.Close();
			Assert.IsFalse(provider.IsOpen);
        }

		[Test]
		public void CreateNewTableTest()
		{
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			provider.Open();
			FeatureDataTable table = provider.CreateNewTable();
			provider.Close();
		}

		[Test]
		public void GetFeatureCountTest()
		{
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			provider.Open();
			FeatureDataTable table = provider.CreateNewTable();
			int count = provider.GetFeatureCount();
			Assert.Greater(count, 0);
			provider.Close();
		}

		[Test]
        public void SimpleMetadataTest()
        {
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			provider.Open();

			// required
			//provider.Locale;

			Assert.IsTrue(provider.IsOpen);

			StringAssert.Contains(_provider, provider.ConnectionId);
			StringAssert.Contains(_connectionString, provider.ConnectionId);

			provider.Close();

			Assert.IsFalse(provider.IsOpen);

			StringAssert.Contains(_provider, provider.ConnectionId);
			StringAssert.Contains(_connectionString, provider.ConnectionId);
        }

        [Test]
        public void TableMetaDataTest()
        {
 			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			DataTable schemaTable;
			FeatureDataTable genericTable;
			FeatureDataTable<uint> idTable;

			try
			{
				schemaTable = provider.GetSchemaTable();
				Assert.Fail("accessing GetSchemaTable() before open should throw exception");
			}
			catch (Exception) { }
			try
			{
				genericTable = new FeatureDataTable();
				provider.SetTableSchema(genericTable);
				Assert.Fail("accessing SetTableSchema() before open should throw exception");
			}
			catch (Exception) { }
			try
			{
				idTable = new FeatureDataTable<uint>("fred");
				provider.SetTableSchema(idTable);
				Assert.Fail("accessing SetTableSchema() before open should throw exception");
			}
			catch (Exception) { }
			try
			{
				idTable = new FeatureDataTable<uint>("fred");
				provider.SetTableSchema(idTable, SchemaMergeAction.AddWithKey);
				Assert.Fail("accessing SetTableSchema( w/ merge option ) before open should throw exception");
			}
			catch (Exception) { }


			provider.Open();

			List<string> columns = new List<string>();

			schemaTable = provider.GetSchemaTable();
			foreach (DataRow row in schemaTable.Rows)
				columns.Add(row["ColumnName"] as string);

			Assert.Contains("Shape", columns);

			//genericTable = new FeatureDataTable();
			//provider.SetTableSchema(genericTable);
			//foreach (DataColumn col in genericTable.Columns)
			//    Assert.Contains(col.ColumnName, columns);

			idTable = new FeatureDataTable<uint>("fred");
			provider.SetTableSchema(idTable);
			bool fredFound = false;
			foreach (DataColumn col in idTable.Columns)
			{
				if (col.ColumnName == "fred")
				{
					Assert.IsFalse(fredFound, "initial column should be there just once");
					fredFound = true;
				}
				else
				{
					Assert.Contains(col.ColumnName, columns);
				}
			}
			Assert.IsTrue(fredFound);

			idTable = new FeatureDataTable<uint>("fred");
			provider.SetTableSchema(idTable, SchemaMergeAction.AddWithKey);
			fredFound = false;
			foreach (DataColumn col in idTable.Columns)
			{
				if (col.ColumnName == "fred")
				{
					Assert.IsFalse(fredFound, "initial column should be there just once");
					fredFound = true;
				}
				else
				{
					Assert.Contains(col.ColumnName, columns);
				}
			}
			Assert.IsTrue(fredFound);
        }

        [Test]
		public void LocaleTest()
        {
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			Assert.AreEqual(System.Threading.Thread.CurrentThread.CurrentCulture, provider.Locale);
        }

		[Test]
		public void FeatureQueryTest()
		{
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			provider.Open();
			FeatureDataTable table = provider.CreateNewTable();
			Assert.AreEqual(table.Rows.Count, 0);
			FeatureSpatialExpression queryExpr = new FeatureSpatialExpression(new BoundingBox(1397577.4098136294576, 474904.945082301272, 1414538.3007094259316, 508826.72687389422).ToGeometry(), SpatialExpressionType.Intersects, null);

			provider.ExecuteFeatureQuery(queryExpr, table);

			Assert.Greater(table.Rows.Count, 0);
		}

		[Test]
		public void IntersectionQueryTest()
		{
			EsriPersonalGeoDatabaseProvider provider = new EsriPersonalGeoDatabaseProvider(_connectionString, _testLayerName);

			provider.Open();
			FeatureDataTable table = provider.CreateNewTable();
			Assert.AreEqual(table.Rows.Count, 0);

			provider.ExecuteIntersectionQuery(new BoundingBox(1397577.4098136294576, 474904.945082301272, 1414538.3007094259316, 508826.72687389422),
			                                  table,
											  QueryExecutionOptions.FullFeature);

			Assert.Greater(table.Rows.Count, 0);
		}
	}
}
