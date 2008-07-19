using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Expressions;

namespace SharpMap.Tests.Data
{
    [TestFixture]
    public class FeatureDataViewTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate2D>(sequenceFactory);
        }

        #region CreatingDataViewReturnsValidDataView

        [Test]
        public void CreatingDataViewReturnsValidDataView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            FeatureDataView view = new FeatureDataView(table);
            Assert.IsInstanceOfType(typeof (FeatureDataTable), view.Table);
            Assert.AreSame(table, view.Table);
        }

        #endregion

        [Test]
        public void CreatingDataViewWithCrossesSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Crosses, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithContainsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Contains, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithEqualsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Equals, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithNoneSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.None, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithOverlapsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Overlaps, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithTouchesSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Touches, "", DataViewRowState.CurrentRows);
        }

        [Test]
        public void CreatingDataViewWithWithinSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate2D> empty = _geoFactory.CreatePoint() as Point<BufferedCoordinate2D>;
            new FeatureDataView(table, empty, SpatialOperation.Within, "", DataViewRowState.CurrentRows);
        }

        #region ChangeViewAttributeFilterReturnsOnlyFilteredRows

        [Test]
        [ExpectedException(typeof (NotSupportedException),
            ExpectedMessage = "RowFilter expressions not supported at this time. Use the ViewDefinition property instead.")]
        public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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
            ExpectedMessage = "RowFilter expressions not supported at this time. Use the ViewDefinition property instead.")]
        public void ChangeViewAttributeFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            table.Load(data.ExecuteFeatureQuery(query));
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
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader);

            Assert.AreEqual(table.Rows.Count, table.DefaultView.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterReturnsOnlyFilteredRows

        [Test]
        public void ChangeViewSpatialFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader);
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
            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(queryExtents);
            view.SpatialFilter = expression;

            Assert.AreEqual(expectedRows.Count, view.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterTriggersNotification

        [Test]
        public void ChangeViewSpatialFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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
            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(queryExtents);
            view.SpatialFilter = expression;

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

            FeatureQueryExpression query = FeatureQueryExpression.Intersects(halfBounds);
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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

            query = FeatureQueryExpression.Intersects(otherHalfBounds);
            reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

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

            FeatureQueryExpression query = FeatureQueryExpression.Intersects(halfBounds);
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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

            FeatureQueryExpression query = FeatureQueryExpression.Intersects(halfBounds);
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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

            FeatureQueryExpression query = FeatureQueryExpression.Intersects(halfBounds);
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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

            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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
        [Ignore("Notifications now happen at a Layer level.")]
        public void SettingViewFilterToIncludeRowsNotPresentInTableCausesFeaturesRequestNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);

            Boolean featuresRequested = false;
            //table.FeaturesNotFound += delegate { featuresRequested = true; };

            FeatureDataView view = new FeatureDataView(table);

            IExtents otherHalfBounds = data.GetExtents();
            otherHalfBounds.TranslateRelativeToWidth(0.5, 0.5);
            otherHalfBounds.Scale(0.5);

            Assert.IsFalse(featuresRequested);

            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(otherHalfBounds);
            view.SpatialFilter = expression;

            Assert.IsTrue(featuresRequested);
        }

        #endregion

        #region SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView

        [Test]
        public void SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid", _geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Guid[] ids = new Guid[3];
            ids[0] = (table.Rows[1] as FeatureDataRow<Guid>).Id;
            ids[1] = (table.Rows[4] as FeatureDataRow<Guid>).Id;
            ids[2] = (table.Rows[6] as FeatureDataRow<Guid>).Id;
            FeatureDataView view = new FeatureDataView(table);
            view.OidFilter = new OidCollectionExpression(ids);

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
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

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
            view.OidFilter = new OidCollectionExpression(ids);

            Assert.IsTrue(resetNotificationOccured);
        }

        #endregion

        [Test]
        public void SettingSpatialPredicateOfFilterChangesViewToMatchFilter()
        {
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.AreEqual(table.Rows.Count, view.Count);

            view.SpatialFilter = SpatialBinaryExpression.Intersects(table.Extents);
                
            Assert.AreEqual(table.Rows.Count, view.Count);

            view.SpatialFilter = new SpatialBinaryExpression(new ThisExpression(), 
                                                             SpatialOperation.Disjoint, 
                                                             new ExtentsExpression(table.Extents));

            Assert.AreEqual(0, view.Count);

            view.SpatialFilter = SpatialBinaryExpression.Intersects(_geoFactory.CreatePoint2D(0, 0));

            Assert.AreEqual(1, view.Count);
        }


        [Test]
        public void ChangingFilterIsExclusivePropertyInvertsFilterMatch()
        {
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.AreEqual(table.Rows.Count, view.Count);
            Assert.IsFalse(view.IsViewDefinitionExclusive);

            view.IsViewDefinitionExclusive = !view.IsViewDefinitionExclusive;

            Assert.IsTrue(view.IsViewDefinitionExclusive);
            Assert.AreEqual(0, view.Count); 
        }


        [Test]
        public void VariousFilteringCombinationTestSequence()
        {
            // this horrendous sequence is what I had time alloted for
            // takes some better setup to factor into tests that test 1 thing
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.AreEqual(table.Rows.Count, view.Count);

            SpatialBinaryExpression spatialFilter = 
                SpatialBinaryExpression.Intersects(_geoFactory.WktReader.Read("POLYGON ((19 19, 21 19, 20 21, 19 19))"));

            // test basic spatial filter
            view.SpatialFilter = spatialFilter;

            Assert.AreEqual(3, view.Count);

            // setup for combination tests
            List<Guid> oidsInSpatialFilter = new List<Guid>(3);
            foreach (FeatureDataRow row in (IEnumerable<FeatureDataRow>)view)
            {
                oidsInSpatialFilter.Add((Guid)row.GetOid());
            }

            List<Guid> oidsNotInSpatialFilter = new List<Guid>(table.Rows.Count - 3);
            foreach (FeatureDataRow<Guid> row in table)
            {
                if (!oidsNotInSpatialFilter.Contains(row.Id))
                {
                    oidsNotInSpatialFilter.Add(row.Id);
                }
            }

            List<Guid> twoOidsFromEachList = new List<Guid>(4);
            twoOidsFromEachList.AddRange(oidFilterSource(oidsInSpatialFilter, 2));
            twoOidsFromEachList.AddRange(oidFilterSource(oidsNotInSpatialFilter, 2));

            // test spatial filter and oid filter
            OidCollectionExpression spanningOids = new OidCollectionExpression(twoOidsFromEachList);
            view.OidFilter = spanningOids;

            Assert.AreEqual(2, view.Count);

            // test clearing spatial component
            view.SpatialFilter = null;

            Assert.AreEqual(4, view.Count);

            // test inverting oid filter
            view.IsViewDefinitionExclusive = true;

            Assert.AreEqual(table.Rows.Count - 4, view.Count);

            // test both components with inversion
            view.SpatialFilter = spatialFilter;

            Assert.AreEqual(table.Rows.Count - 2, view.Count);

            // test clearing OidFilter
            view.OidFilter = null;

            Assert.AreEqual(table.Rows.Count - 3, view.Count);

            // test clearing inversion
            view.IsViewDefinitionExclusive = false;

            Assert.AreEqual(3, view.Count);

            // test clearing everything
            view.SpatialFilter = null;

            Assert.AreEqual(table.Rows.Count, view.Count);
        }

        private IEnumerable<Guid> oidFilterSource(IEnumerable<Guid> source, Int32 maxCount)
        {
            Int32 count = 0;

            foreach (Guid guid in source)
            {
                if (count >= maxCount)
                {
                    yield break;
                }

                count ++;
                yield return guid;
            }
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
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
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
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            IGeometry filter = _geoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            IGeometry filterCopy = filter.Clone();
            Assert.AreNotSame(filter, filterCopy);
            Assert.AreEqual(filter, filterCopy);
            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(filterCopy);
            view.SpatialFilter = expression;
            Assert.AreNotSame(expression, view.SpatialFilter);
        }

        #endregion

        #region SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime

        [Test]
        public void SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            IGeometry filter = _geoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            Assert.AreEqual(SpatialOperation.Intersects, view.SpatialFilter.Op);
        }

        #endregion

        #region DataViewManagerIsAFeatureDataViewManager

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void DataViewManagerIsAFeatureDataViewManager()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory);
            FeatureDataTable table = new FeatureDataTable(_geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            FeatureDataSet dataSet = new FeatureDataSet(_geoFactory);
            dataSet.Tables.Add(table);
            FeatureDataView view = dataSet.DefaultViewManager.CreateDataView(table);

            Assert.IsNotNull(view.DataViewManager);
            Assert.IsInstanceOfType(typeof (FeatureDataViewManager), view.DataViewManager);
        }

        #endregion

        private void createDataViewOnNewTable(out FeatureDataView view, 
                                              out FeatureDataTable<Guid> table,
                                              Boolean includeGeometryCollections)
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_geoFactory,
                                                                            includeGeometryCollections);
            table = new FeatureDataTable<Guid>("Oid", _geoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            view = new FeatureDataView(table);
        }
    }
}
