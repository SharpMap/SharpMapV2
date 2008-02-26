using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Expressions;
using SharpMap.SimpleGeometries;

namespace SharpMap.Tests.Data
{
    [TestFixture]
    public class FeatureDataViewTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DFactory coordFactory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory(coordFactory, sequenceFactory);
        }

        #region CreatingDataViewReturnsValidDataView

        [Test]
        public void CreatingDataViewReturnsValidDataView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
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

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Crosses, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithContainsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Contains, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithEqualsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Equals, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithNoneSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.None, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithOverlapsSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Overlaps, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithTouchesSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Touches, "", DataViewRowState.CurrentRows);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreatingDataViewWithWithinSpatialExpressionTypeNotSupported()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            Point empty = _geoFactory.CreatePoint() as Point;
            new FeatureDataView(table, empty, SpatialExpressionType.Within, "", DataViewRowState.CurrentRows);
        }

        #endregion

        #region ChangeViewAttributeFilterReturnsOnlyFilteredRows

        [Test]
        [ExpectedException(typeof (NotSupportedException),
            ExpectedMessage = "RowFilter expressions not supported at this time.")]
        public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));
            FeatureDataView view = new FeatureDataView(table);

            Assert.AreEqual(table.Rows.Count, view.Count);
        }

        #endregion

        #region DefaultViewReturnsAllRows

        [Test]
        public void DefaultViewReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));

            Assert.AreEqual(table.Rows.Count, table.DefaultView.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterReturnsOnlyFilteredRows

        [Test]
        public void ChangeViewSpatialFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));
            IGeometry queryExtents = _geoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();

            List<FeatureDataRow> expectedRows = new List<FeatureDataRow>();

            foreach (FeatureDataRow row in table)
            {
                IGeometry g = row.Geometry;

                if (queryExtents.Intersects(g))
                {
                    expectedRows.Add(row);
                }
            }

            FeatureDataView view = new FeatureDataView(table);
            view.GeometryFilter = queryExtents;

            Assert.AreEqual(expectedRows.Count, view.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterTriggersNotification

        [Test]
        public void ChangeViewSpatialFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            resetNotificationOccured = true;
                                        }
                                    };

            Assert.IsFalse(resetNotificationOccured);

            IExtents queryExtents = _geoFactory.CreateExtents2D(0, 0, 10, 10);
            view.GeometryFilter = queryExtents.ToGeometry();

            Assert.IsTrue(resetNotificationOccured);
        }

        #endregion

        #region ExecutingQueryOnTableTriggersViewListChangedResetNotification

        [Test]
        public void ExecutingQueryOnTableTriggersViewListChangedResetNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            IExtents dataExtents = data.GetExtents();
            IExtents halfBounds = _geoFactory.CreateExtents(
                dataExtents.Min, dataExtents.Max);
            halfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.Reset)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            IExtents otherHalfBounds = _geoFactory.CreateExtents(
                dataExtents.Min, dataExtents.Max);
            otherHalfBounds.TranslateRelativeToWidth(0.5, 0.5);
            otherHalfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(otherHalfBounds, table);

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region AddingRowToTableTriggersViewListChangedItemAddedNotification

        [Test]
        public void AddingRowToTableTriggersViewListChangedItemAddedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            IExtents halfBounds = data.GetExtents();
            halfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
                                    {
                                        if (e.ListChangedType == ListChangedType.ItemAdded)
                                        {
                                            addNotificationOccured = true;
                                        }
                                    };

            FeatureDataRow row = table.NewRow();
            row["Oid"] = Guid.NewGuid();
            row["FeatureName"] = "New row";
            row.Geometry = _geoFactory.CreatePoint2D(44, 44);
            table.AddRow(row);

            Assert.IsTrue(addNotificationOccured);
        }

        #endregion

        #region RemovingRowFromTableTriggersViewListChangedItemDeletedNotification

        [Test]
        public void RemovingRowFromTableTriggersViewListChangedItemDeletedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            IExtents halfBounds = data.GetExtents();
            halfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            IExtents halfBounds = data.GetExtents();
            halfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            IExtents halfBounds = data.GetExtents();
            halfBounds.Scale(0.5);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean addNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            Boolean featuresRequested = false;
            table.FeaturesNotFound += delegate { featuresRequested = true; };

            FeatureDataView view = new FeatureDataView(table);

            IExtents otherHalfBounds = data.GetExtents();
            otherHalfBounds.TranslateRelativeToWidth(0.5, 0.5);
            otherHalfBounds.Scale(0.5);

            Assert.IsFalse(featuresRequested);

            view.GeometryFilter = otherHalfBounds.ToGeometry();

            Assert.IsTrue(featuresRequested);
        }

        #endregion

        #region SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView

        [Test]
        public void SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid", _geoFactory);
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid", _geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            Boolean resetNotificationOccured = false;

            view.ListChanged += delegate(Object sender, ListChangedEventArgs e)
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);
            view.AddNew();
        }

        #endregion

        #region SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject

        [Test]
        public void SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            IGeometry filter = _geoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            IGeometry filterCopy = filter.Clone();
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
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            IGeometry filter = _geoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            Assert.AreEqual(SpatialExpressionType.Intersects, view.GeometryFilterType);
        }

        #endregion

        #region DataViewManagerIsAFeatureDataViewManager

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void DataViewManagerIsAFeatureDataViewManager()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataSet dataSet = new FeatureDataSet(_geoFactory);
            dataSet.Tables.Add(table);
            FeatureDataView view = dataSet.DefaultViewManager.CreateDataView(table);

            Assert.IsNotNull(view.DataViewManager);
            Assert.IsInstanceOfType(typeof (FeatureDataViewManager), view.DataViewManager);
        }

        #endregion
    }
}