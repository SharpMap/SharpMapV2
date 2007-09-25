using System;
using System.Data;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;

namespace SharpMap.Tests.Features
{
    [TestFixture]
    public class FeatureDataTableTests
    {
        [Test]
        public void CreateTable()
        {
            FeatureDataTable table = new FeatureDataTable();
            Assert.IsNotNull(table);
            Assert.IsNull(table.DataSet);
        }

        [Test]
        public void NewRowReturnsDetachedFeatureDataRow()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            Assert.IsNotNull(row);
            Assert.AreEqual(0, table.Rows.Count);
            Assert.AreEqual(DataRowState.Detached, row.RowState);
            Assert.AreSame(table, row.Table);
        }

        [Test]
        public void AddedRowChangesRowState()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.AreEqual(DataRowState.Added, row.RowState);
        }

        [Test]
        public void AddedRowIncreasesRowCount()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(1, table.FeatureCount);
        }

        [Test]
        public void AddedRowAppearsAsChange()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            FeatureDataTable changes = table.GetChanges();
            Assert.AreEqual(1, changes.FeatureCount);
        }

        [Test]
        public void AcceptChangesAppearAsUnchanged()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            Assert.AreEqual(DataRowState.Unchanged, row.RowState);
        }

        [Test]
        public void AcceptChangesReturnsNullChangesTable()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            table.AddRow(row);
            table.AcceptChanges();
            FeatureDataTable changes = table.GetChanges();
            Assert.IsNull(changes);
        }

        [Test]
        public void DefaultViewIsFeatureDataView()
        {
            FeatureDataTable table = new FeatureDataTable();
            DataView view = table.DefaultView;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType(typeof(FeatureDataView), view);
            Assert.IsNull(view.DataViewManager);
            Assert.AreEqual(String.Empty, view.Sort);
            Assert.AreEqual(String.Empty, view.RowFilter);
            Assert.AreEqual(0, view.Count);
            Assert.AreSame(table, view.Table);
            FeatureDataView featureView = view as FeatureDataView;
            Assert.IsNotNull(featureView);
            Assert.IsNull(featureView.GeometryFilter);
            Assert.AreEqual(DataViewRowState.CurrentRows, view.RowStateFilter);
        }

        [Test]
        public void LoadingTableFromReader()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            table.Load(provider.ExecuteIntersectionQuery(provider.GetExtents()));
        }

        [Test]
        public void CloneToCopiesTableStructureAndNoData()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable clone = new FeatureDataTable();
            table.CloneTo(clone);
			DataTableHelper.AssertTableStructureIdentical(table, clone);

            Assert.AreEqual(0, clone.Rows.Count);
        }

        [Test]
        public void MergeSchemaToSchemalessTargetShouldCreateIdenticalTable()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable target = new FeatureDataTable();
            table.MergeSchema(target);

			DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        public void MergeSchemaToIdenticalTableShouldRemainIdentical()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable target = new FeatureDataTable();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), target);

            table.MergeSchema(target);

			DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        public void MergeSchemaToKeyedTableShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("Oid");

            table.MergeSchema(target);

			DataTableHelper.AssertTableStructureIdentical(table, target);
        }

        [Test]
        [Ignore("This functionality isn't implemented yet")]
        public void MergeSchemaToKeyedTableWithDifferentKeyNameButSameTypeShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("GID");

            table.MergeSchema(target);

            DataTableHelper.AssertTableStructureIdentical(table, target);
		}
    }
}
