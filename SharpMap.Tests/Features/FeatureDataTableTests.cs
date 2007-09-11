using System;
using System.Data;
using NUnit.Framework;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Features;

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
            Assert.IsNull(featureView.GeometryIntersectionFilter);
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
            assertTableStructureIdentical(table, clone);

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

            assertTableStructureIdentical(table, target);
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

            assertTableStructureIdentical(table, target);
        }

        [Test]
        public void MergeSchemaToKeyedTableShouldKeepKeyButAddOtherColumns()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureProvider provider = DataSourceHelper.CreateFeatureDatasource();
            provider.ExecuteIntersectionQuery(provider.GetExtents(), table);

            FeatureDataTable<Guid> target = new FeatureDataTable<Guid>("Oid");

            table.MergeSchema(target);

            assertTableStructureIdentical(table, target);
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

            assertTableStructureIdentical(table, target);
        }

        private static void assertTableStructureIdentical(FeatureDataTable lhs, FeatureDataTable rhs)
        {
            Assert.AreNotSame(lhs, rhs);
            Assert.Greater(lhs.Columns.Count, 0);
            Assert.AreEqual(lhs.Columns.Count, rhs.Columns.Count);

            for (int i = 0; i < lhs.Columns.Count; i++)
            {
                Assert.AreEqual(lhs.Columns[i].AllowDBNull, rhs.Columns[i].AllowDBNull);
                Assert.AreEqual(lhs.Columns[i].AutoIncrement, rhs.Columns[i].AutoIncrement);
                Assert.AreEqual(lhs.Columns[i].AutoIncrementSeed, rhs.Columns[i].AutoIncrementSeed);
                Assert.AreEqual(lhs.Columns[i].AutoIncrementStep, rhs.Columns[i].AutoIncrementStep);
                Assert.AreEqual(lhs.Columns[i].Caption, rhs.Columns[i].Caption);
                Assert.AreEqual(lhs.Columns[i].ColumnMapping, rhs.Columns[i].ColumnMapping);
                Assert.AreEqual(lhs.Columns[i].ColumnName, rhs.Columns[i].ColumnName);
                Assert.AreEqual(lhs.Columns[i].DataType, rhs.Columns[i].DataType);
                Assert.AreEqual(lhs.Columns[i].DateTimeMode, rhs.Columns[i].DateTimeMode);
                Assert.AreEqual(lhs.Columns[i].DefaultValue, rhs.Columns[i].DefaultValue);
                Assert.AreEqual(lhs.Columns[i].Expression, rhs.Columns[i].Expression);
                Assert.AreEqual(lhs.Columns[i].MaxLength, rhs.Columns[i].MaxLength);
                Assert.AreEqual(lhs.Columns[i].Namespace, rhs.Columns[i].Namespace);
                Assert.AreEqual(lhs.Columns[i].Ordinal, rhs.Columns[i].Ordinal);
                Assert.AreEqual(lhs.Columns[i].Prefix, rhs.Columns[i].Prefix);
                Assert.AreEqual(lhs.Columns[i].ReadOnly, rhs.Columns[i].ReadOnly);
                Assert.AreEqual(lhs.Columns[i].Unique, rhs.Columns[i].Unique);
                Assert.AreEqual(lhs.Columns[i].ExtendedProperties.Count, lhs.Columns[i].ExtendedProperties.Count);

                object[] tableProperties = new object[lhs.Columns[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(tableProperties, tableProperties.Length);

                object[] cloneProperties = new object[rhs.Columns[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(cloneProperties, cloneProperties.Length);

                for (int epIndex = 0; epIndex < tableProperties.Length; epIndex++)
                {
                    Assert.AreEqual(tableProperties[i], cloneProperties[i]);
                }
            }

            Assert.AreEqual(lhs.PrimaryKey.Length, rhs.PrimaryKey.Length);

            if (lhs.PrimaryKey.Length > 0)
            {
                for (int i = 0; i < lhs.PrimaryKey.Length; i++)
                {
                    Assert.AreEqual(lhs.PrimaryKey[i].AllowDBNull, rhs.PrimaryKey[i].AllowDBNull);
                    Assert.AreEqual(lhs.PrimaryKey[i].AutoIncrement, rhs.PrimaryKey[i].AutoIncrement);
                    Assert.AreEqual(lhs.PrimaryKey[i].AutoIncrementSeed, rhs.PrimaryKey[i].AutoIncrementSeed);
                    Assert.AreEqual(lhs.PrimaryKey[i].AutoIncrementStep, rhs.PrimaryKey[i].AutoIncrementStep);
                    Assert.AreEqual(lhs.PrimaryKey[i].Caption, rhs.PrimaryKey[i].Caption);
                    Assert.AreEqual(lhs.PrimaryKey[i].ColumnMapping, rhs.PrimaryKey[i].ColumnMapping);
                    Assert.AreEqual(lhs.PrimaryKey[i].ColumnName, rhs.PrimaryKey[i].ColumnName);
                    Assert.AreEqual(lhs.PrimaryKey[i].DataType, rhs.PrimaryKey[i].DataType);
                    Assert.AreEqual(lhs.PrimaryKey[i].DateTimeMode, rhs.PrimaryKey[i].DateTimeMode);
                    Assert.AreEqual(lhs.PrimaryKey[i].DefaultValue, rhs.PrimaryKey[i].DefaultValue);
                    Assert.AreEqual(lhs.PrimaryKey[i].Expression, rhs.PrimaryKey[i].Expression);
                    Assert.AreEqual(lhs.PrimaryKey[i].MaxLength, rhs.PrimaryKey[i].MaxLength);
                    Assert.AreEqual(lhs.PrimaryKey[i].Namespace, rhs.PrimaryKey[i].Namespace);
                    Assert.AreEqual(lhs.PrimaryKey[i].Ordinal, rhs.PrimaryKey[i].Ordinal);
                    Assert.AreEqual(lhs.PrimaryKey[i].Prefix, rhs.PrimaryKey[i].Prefix);
                    Assert.AreEqual(lhs.PrimaryKey[i].ReadOnly, rhs.PrimaryKey[i].ReadOnly);
                    Assert.AreEqual(lhs.PrimaryKey[i].Unique, rhs.PrimaryKey[i].Unique);
                    Assert.AreEqual(lhs.PrimaryKey[i].ExtendedProperties.Count, lhs.PrimaryKey[i].ExtendedProperties.Count);

                    object[] tableProperties = new object[lhs.PrimaryKey[i].ExtendedProperties.Count];
                    lhs.PrimaryKey[i].ExtendedProperties.CopyTo(tableProperties, tableProperties.Length);

                    object[] cloneProperties = new object[rhs.PrimaryKey[i].ExtendedProperties.Count];
                    lhs.PrimaryKey[i].ExtendedProperties.CopyTo(cloneProperties, cloneProperties.Length);

                    for (int epIndex = 0; epIndex < tableProperties.Length; epIndex++)
                    {
                        Assert.AreEqual(tableProperties[i], cloneProperties[i]);
                    }
                }
            }

            for (int i = 0; i < lhs.Constraints.Count; i++)
            {
                Assert.AreEqual(lhs.Constraints[i].ConstraintName, rhs.Constraints[i].ConstraintName);

                object[] tableProperties = new object[lhs.Constraints[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(tableProperties, tableProperties.Length);

                object[] cloneProperties = new object[rhs.Constraints[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(cloneProperties, cloneProperties.Length);

                for (int epIndex = 0; epIndex < tableProperties.Length; epIndex++)
                {
                    Assert.AreEqual(tableProperties[i], cloneProperties[i]);
                }
            }
        }
    }
}
