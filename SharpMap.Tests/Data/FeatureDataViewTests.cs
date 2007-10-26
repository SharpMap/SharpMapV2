using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Expressions;
using SharpMap.Geometries;

namespace SharpMap.Tests.Data
{
    [TestFixture]
    public class FeatureDataViewTests
    {
        #region CreatingDataViewReturnsValidDataView

        [Test]
        public void CreatingDataViewReturnsValidDataView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);
            Assert.IsInstanceOfType(typeof (FeatureDataTable), view.Table);
            Assert.AreSame(table, view.Table);
        }

        #endregion

        #region CreatingDataViewWithSpatialQueryTypeOtherThanIntersectsNotSupported

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void CreatingDataViewWithCrossesSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Crosses, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithContainsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Contains, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithEqualsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Equals, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithNoneSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.None, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithOverlapsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Overlaps, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithTouchesSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Touches, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithWithinSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            new FeatureDataView(table, Point.Empty, SpatialExpressionType.Within, "", DataViewRowState.CurrentRows);
        }

        #endregion

        #region ChangeViewAttributeFilterReturnsOnlyFilteredRows

        [Test]
        [ExpectedException(typeof (NotSupportedException),
            ExpectedMessage = "RowFilter expressions not supported at this time.")]
        public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Int32 expectedRowCount = 0;

            foreach (FeatureDataRow row in table.Rows)
            {
                if (row["FeatureName"].ToString().StartsWith("A m"))
                {
                    expectedRowCount++;
                }
            }

            view.RowFilter = "FeatureName LIKE 'A m*'";
            Assert.AreEqual(expectedRowCount, view.Count);
        }

        #endregion

        #region ChangeViewAttributeFilterTriggersNotification

        [Test]
        [ExpectedException(typeof (NotSupportedException),
            ExpectedMessage = "RowFilter expressions not supported at this time.")]
        public void ChangeViewAttributeFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            resetNotificationOccured = true;
                                        }
                                    };

            Assert.IsFalse(resetNotificationOccured);

            view.RowFilter = "FeatureName LIKE 'A m*'";

            Assert.IsTrue(resetNotificationOccured);
        }

        #endregion

        #region NullSpatialFilterReturnsAllRows

        [Test]
        public void NullSpatialFilterReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));
            FeatureDataView view = new FeatureDataView(table);

            Assert.AreEqual(table.Rows.Count, view.Count);
        }

        #endregion

        #region DefaultViewReturnsAllRows

        [Test]
        public void DefaultViewReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));

            Assert.AreEqual(table.Rows.Count, table.DefaultView.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterReturnsOnlyFilteredRows

        [Test]
        public void ChangeViewSpatialFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));
            BoundingBox queryExtents = new BoundingBox(0, 0, 10, 10);

            List<FeatureDataRow> expectedRows = new List<FeatureDataRow>();

            foreach (FeatureDataRow row in table)
            {
                Geometry g = row.Geometry;

                if (queryExtents.Intersects(g))
                {
                    expectedRows.Add(row);
                }
            }

            FeatureDataView view = new FeatureDataView(table);
            view.GeometryFilter = queryExtents.ToGeometry();

            Assert.AreEqual(expectedRows.Count, view.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterTriggersNotification

        [Test]
        public void ChangeViewSpatialFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            resetNotificationOccured = true;
                                        }
                                    };

            Assert.IsFalse(resetNotificationOccured);

            BoundingBox queryExtents = new BoundingBox(0, 0, 10, 10);
            view.GeometryFilter = queryExtents.ToGeometry();

            Assert.IsTrue(resetNotificationOccured);
        }

        #endregion

        #region ExecutingQueryOnTableTriggersViewListChangedResetNotification

        [Test]
        public void ExecutingQueryOnTableTriggersViewListChangedResetNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                                                     dataExtents.Left + dataExtents.Width/2,
                                                     dataExtents.Bottom + dataExtents.Height/2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            BoundingBox otherHalfBounds = new BoundingBox(
                dataExtents.Left + dataExtents.Width/2, dataExtents.Bottom + dataExtents.Height/2,
                dataExtents.Right, dataExtents.Top);

            data.ExecuteIntersectionQuery(otherHalfBounds, table);

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region AddingRowToTableTriggersViewListChangedItemAddedNotification

        [Test]
        public void AddingRowToTableTriggersViewListChangedItemAddedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                                                     dataExtents.Left + dataExtents.Width/2,
                                                     dataExtents.Bottom + dataExtents.Height/2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.ItemAdded)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            FeatureDataRow row = table.NewRow();
            row["Oid"] = Guid.NewGuid();
            row["FeatureName"] = "New row";
            row.Geometry = new Point(44, 44);
            table.AddRow(row);

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region RemovingRowFromTableTriggersViewListChangedItemDeletedNotification

        [Test]
        public void RemovingRowFromTableTriggersViewListChangedItemDeletedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                                                     dataExtents.Left + dataExtents.Width/2,
                                                     dataExtents.Bottom + dataExtents.Height/2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.ItemDeleted)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            FeatureDataRow row = table[0];
            table.RemoveRow(row);

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region ChangingRowTriggersViewListChangedItemChangedNotification

        [Test]
        public void ChangingRowTriggersViewListChangedItemChangedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                                                     dataExtents.Left + dataExtents.Width/2,
                                                     dataExtents.Bottom + dataExtents.Height/2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.ItemChanged)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            FeatureDataRow row = table[0];
            row["FeatureName"] = "Updated name";

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region MovingRowTriggersViewListChangedItemMovedNotification

        [Test]
        [Ignore("Not sure how to generate this notification yet.")]
        public void MovingRowTriggersViewListChangedItemMovedNotification()
        {
            // TODO: implement MovingRowTriggersViewListChangedItemMovedNotification
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                                                     dataExtents.Left + dataExtents.Width/2,
                                                     dataExtents.Bottom + dataExtents.Height/2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.ItemMoved)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region SettingViewFilterToIncludeRowsNotPresentInTableCausesFeaturesRequestNotification

        [Test]
        public void SettingViewFilterToIncludeRowsNotPresentInTableCausesFeaturesRequestNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();

            Boolean featuresRequested = false;
            table.FeaturesNotFound += delegate { featuresRequested = true; };

            BoundingBox dataExtents = data.GetExtents();
            FeatureDataView view = new FeatureDataView(table);

            BoundingBox otherHalfBounds = new BoundingBox(dataExtents.Left + dataExtents.Width/2,
                                                          dataExtents.Bottom + dataExtents.Height/2,
                                                          dataExtents.Right,
                                                          dataExtents.Top);

            Assert.IsFalse(featuresRequested);

            view.GeometryFilter = otherHalfBounds.ToGeometry();

            Assert.IsTrue(featuresRequested);
        }

        #endregion

        #region SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView

        [Test]
        public void SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid");
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Guid[] ids = new Guid[3];
            ids[0] = (table.Rows[1] as FeatureDataRow<Guid>).Id;
            ids[1] = (table.Rows[4] as FeatureDataRow<Guid>).Id;
            ids[2] = (table.Rows[6] as FeatureDataRow<Guid>).Id;
            FeatureDataView view = new FeatureDataView(table);
            view.OidFilter = ids;

            Assert.AreEqual(3, view.Count);
            Assert.Greater(view.Find(ids[0]), -1);
            Assert.Greater(view.Find(ids[1]), -1);
            Assert.Greater(view.Find(ids[2]), -1);
            Assert.AreEqual(-1, view.Find((table.Rows[0] as FeatureDataRow<Guid>).Id));
            Assert.AreEqual(-1, view.Find((table.Rows[2] as FeatureDataRow<Guid>).Id));
            Assert.AreEqual(-1, view.Find((table.Rows[3] as FeatureDataRow<Guid>).Id));
            Assert.AreEqual(-1, view.Find((table.Rows[5] as FeatureDataRow<Guid>).Id));
        }

        #endregion

        #region ChangingOidFilterTriggersNotification

        [Test]
        public void ChangingOidFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid");
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            resetNotificationOccured = true;
                                        }
                                    };

            Assert.IsFalse(resetNotificationOccured);

            Guid[] ids = new Guid[3];
            ids[0] = (table.Rows[1] as FeatureDataRow<Guid>).Id;
            ids[1] = (table.Rows[4] as FeatureDataRow<Guid>).Id;
            ids[2] = (table.Rows[6] as FeatureDataRow<Guid>).Id;
            view.OidFilter = ids;

            Assert.IsTrue(resetNotificationOccured);
        }

        #endregion

        [Test]
        [Ignore("Test not complete")]
        public void CombiningGeometryFilterAndOidFilterAllowsOnlyFeaturesMatchingBothFilters()
        {
        }

        [Test]
        [Ignore("Test not complete")]
        public void SettingTablePropertyImposesFiltersOnNewTable()
        {
        }

        #region AddNewRowThrowsNotSupported

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void AddNewRowThrowsNotSupported()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);
            view.AddNew();
        }

        #endregion

        #region SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject

        [Test]
        public void SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Geometry filter = new BoundingBox(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            Geometry filterCopy = filter.Clone();
            Assert.AreNotSame(filter, filterCopy);
            Assert.AreEqual(filter, filterCopy);
            view.GeometryFilter = filterCopy;
            Assert.AreNotSame(filterCopy, view.GeometryFilter);
        }

        #endregion

        #region SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime

        [Test]
        public void SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Geometry filter = new BoundingBox(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            Assert.AreEqual(SpatialExpressionType.Intersects, view.GeometryFilterType);
        }

        #endregion

        #region DataViewManagerIsAFeatureDataViewManager

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void DataViewManagerIsAFeatureDataViewManager()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataSet dataSet = new FeatureDataSet();
            dataSet.Tables.Add(table);
            FeatureDataView view = dataSet.DefaultViewManager.CreateDataView(table);

            Assert.IsNotNull(view.DataViewManager);
            Assert.IsInstanceOfType(typeof (FeatureDataViewManager), view.DataViewManager);
        }

        #endregion
    }
}