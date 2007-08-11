using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap;
using System.Data;
using SharpMap.Geometries;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Data.Providers.FeatureProvider;

namespace SharpMap.Tests.Model
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
            Assert.AreEqual(1, table.FeatureCount);
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
            Assert.AreEqual(BoundingBox.Empty, featureView.VisibleRegion);
            Assert.AreEqual(DataViewRowState.CurrentRows, view.RowStateFilter);
        }

        [Test]
        public void LoadTableFromReader()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            table.Load(provider.ExecuteIntersectionQuery(provider.GetExtents()));
        }
    }
}
