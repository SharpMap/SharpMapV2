using System;
using System.Data;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;

using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Expressions;
using Xunit;

namespace SharpMap.Tests.Data
{
    public class FeatureDataTableTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        [Fact]
        public void CreateTable()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            Assert.NotNull(table);
            Assert.Null(table.DataSet);
        }

        [Fact]
        public void NewRowReturnsDetachedFeatureDataRow()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            Assert.NotNull(row);
            Assert.Equal(0, table.Rows.Count);
            Assert.Equal(DataRowState.Detached, row.RowState);
            Assert.Same(table, row.Table);
        }

        [Fact]
        public void AddedRowChangesRowState()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.Equal(DataRowState.Added, row.RowState);
        }

        [Fact]
        public void AddedRowIncreasesRowCount()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal(1, table.FeatureCount);
        }

        [Fact]
        public void AddedRowAppearsAsChange()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            FeatureDataTable changes = table.GetChanges();
            Assert.Equal(1, changes.FeatureCount);
        }

        [Fact]
        public void AcceptChangesAppearAsUnchanged()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            Assert.Equal(DataRowState.Unchanged, row.RowState);
        }

        [Fact]
        public void AcceptChangesReturnsNullChangesTable()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            FeatureDataTable changes = table.GetChanges();
            Assert.Null(changes);
        }

        [Fact]
        public void DefaultViewIsFeatureDataView()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            DataView view = table.DefaultView;
            Assert.NotNull(view);
            Assert.IsType(typeof (FeatureDataView), view);
            Assert.Null(view.DataViewManager);
            Assert.Equal(String.Empty, view.Sort);
            Assert.Equal(String.Empty, view.RowFilter);
            Assert.Equal(0, view.Count);
            Assert.Same(table, view.Table);
            FeatureDataView featureView = view as FeatureDataView;
            Assert.NotNull(featureView);
            Assert.Null(featureView.SpatialFilter);
            Assert.Equal(DataViewRowState.CurrentRows, view.RowStateFilter);
        }

        [Fact]
        public void LoadingTableFromReader()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            table.Load(provider.ExecuteFeatureQuery(query));
        }

        [Fact]
        public void CloneToCopiesTableStructureAndNoData()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            IFeatureDataReader reader = provider.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable clone = new FeatureDataTable(_factories.GeoFactory);
            table.CloneTo(clone);
            DataTableHelper.AssertTableStructureIdentical(table, clone);

            Assert.Equal(0, clone.Rows.Count);
        }

        [Fact]
        public void MergeSchemaToSchemalessTargetShouldCreateIdenticalTable()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            IFeatureDataReader reader = provider.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable target = new FeatureDataTable(_factories.GeoFactory);
            table.MergeSchemaTo(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Fact]
        public void MergeSchemaToIdenticalTableShouldRemainIdentical()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            IFeatureDataReader reader = provider.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable target = new FeatureDataTable(_factories.GeoFactory);
            reader = provider.ExecuteFeatureQuery(query);
            target.Load(reader, LoadOption.OverwriteChanges, null);

            table.MergeSchemaTo(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Fact]
        public void MergeSchemaToKeyedTableShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            IFeatureDataReader reader = provider.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("Oid", _factories.GeoFactory);

            table.MergeSchemaTo(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Fact(Skip = "Tested functionality incomplete")]
        public void MergeSchemaToKeyedTableWithDifferentKeyNameButSameTypeShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(provider.GetExtents());
            IFeatureDataReader reader = provider.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("GID", _factories.GeoFactory);

            table.MergeSchemaTo(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
        }
    }
}