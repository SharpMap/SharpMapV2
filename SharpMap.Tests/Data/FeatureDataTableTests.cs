using System;
using System.Data;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;

namespace SharpMap.Tests.Data
{
    [TestFixture]
    public class FeatureDataTableTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

        [Test]
        public void CreateTable()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            Assert.IsNotNull(table);
            Assert.IsNull(table.DataSet);
        }

        [Test]
        public void NewRowReturnsDetachedFeatureDataRow()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            Assert.IsNotNull(row);
            Assert.AreEqual(0, table.Rows.Count);
            Assert.AreEqual(DataRowState.Detached, row.RowState);
            Assert.AreSame(table, row.Table);
        }

        [Test]
        public void AddedRowChangesRowState()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.AreEqual(DataRowState.Added, row.RowState);
        }

        [Test]
        public void AddedRowIncreasesRowCount()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(1, table.FeatureCount);
        }

        [Test]
        public void AddedRowAppearsAsChange()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            FeatureDataTable changes = table.GetChanges();
            Assert.AreEqual(1, changes.FeatureCount);
        }

        [Test]
        public void AcceptChangesAppearAsUnchanged()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            Assert.AreEqual(DataRowState.Unchanged, row.RowState);
        }

        [Test]
        public void AcceptChangesReturnsNullChangesTable()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            FeatureDataTable changes = table.GetChanges();
            Assert.IsNull(changes);
        }

        [Test]
        public void DefaultViewIsFeatureDataView()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            DataView view = table.DefaultView;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType(typeof (FeatureDataView), view);
            Assert.IsNull(view.DataViewManager);
            Assert.AreEqual(String.Empty, view.Sort);
            Assert.AreEqual(String.Empty, view.RowFilter);
            Assert.AreEqual(0, view.Count);
            Assert.AreSame(table, view.Table);
            FeatureDataView featureView = view as FeatureDataView;
            Assert.IsNotNull(featureView);
            IGeometry g = _geoFactory.CreatePoint();
            Assert.AreEqual(g, featureView.GeometryFilter);
            Assert.AreEqual(DataViewRowState.CurrentRows, view.RowStateFilter);
        }

        [Test]
        public void LoadingTableFromReader()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            table.Load(provider.ExecuteIntersectionQuery(provider.GetExtents()));
        }

        [Test]
        public void CloneToCopiesTableStructureAndNoData()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            IFeatureDataReader reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable clone = new FeatureDataTable(_geoFactory);
            table.CloneTo(clone);
            DataTableHelper.AssertTableStructureIdentical(table, clone);

            Assert.AreEqual(0, clone.Rows.Count);
        }

        [Test]
        public void MergeSchemaToSchemalessTargetShouldCreateIdenticalTable()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            IFeatureDataReader reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable target = new FeatureDataTable(_geoFactory);
            table.MergeSchema(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        public void MergeSchemaToIdenticalTableShouldRemainIdentical()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            IFeatureDataReader reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable target = new FeatureDataTable(_geoFactory);
            reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            target.Load(reader, LoadOption.OverwriteChanges, null);

            table.MergeSchema(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        public void MergeSchemaToKeyedTableShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            IFeatureDataReader reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("Oid", _geoFactory);

            table.MergeSchema(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        [Ignore("This functionality isn't implemented yet")]
        public void MergeSchemaToKeyedTableWithDifferentKeyNameButSameTypeShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            IFeatureDataReader reader = provider.ExecuteIntersectionQuery(provider.GetExtents());
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("GID", _geoFactory);

            table.MergeSchema(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }
    }
}