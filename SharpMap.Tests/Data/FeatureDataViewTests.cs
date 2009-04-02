using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;

using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;
using Xunit;

namespace SharpMap.Tests.Data
{
    
    public class FeatureDataViewTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

        #region IUseFixture<FixtureFactories> Members

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        #endregion

        #region CreatingDataViewReturnsValidDataView

        [Fact]
        public void CreatingDataViewReturnsValidDataView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            FeatureDataView view = new FeatureDataView(table);
            Assert.IsType(typeof (FeatureDataTable), view.Table);
            Assert.Same(table, view.Table);
        }

        #endregion

        [Fact]
        public void CreatingDataViewWithCrossesSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Crosses, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithContainsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Contains, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithEqualsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Equals, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithNoneSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.None, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithOverlapsSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Overlaps, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithTouchesSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Touches, "", DataViewRowState.CurrentRows);
        }

        [Fact]
        public void CreatingDataViewWithWithinSpatialExpressionSucceeds()
        {
            // This test is here so that when it is supported, the test breaks and is rewritten

            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Point<BufferedCoordinate> empty = _factories.GeoFactory.CreatePoint() as Point<BufferedCoordinate>;
            new FeatureDataView(table, empty, SpatialOperation.Within, "", DataViewRowState.CurrentRows);
        }

        #region ChangeViewAttributeFilterReturnsOnlyFilteredRows

        [Fact]
        public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
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

            Assert.Throws<NotSupportedException>(delegate { view.RowFilter = "FeatureName LIKE 'A m*'"; });
            Assert.Equal(expectedRowCount, view.Count); 
        }

        #endregion

        #region ChangeViewAttributeFilterTriggersNotification

        [Fact]
        public void ChangeViewAttributeFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
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

            Assert.False(resetNotificationOccured);

            Assert.Throws<NotSupportedException>(delegate { view.RowFilter = "FeatureName LIKE 'A m*'"; });
            Assert.True(resetNotificationOccured);
        }

        #endregion

        #region NullSpatialFilterReturnsAllRows

        [Fact]
        public void NullSpatialFilterReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            table.Load(data.ExecuteFeatureQuery(query));
            FeatureDataView view = new FeatureDataView(table);

            Assert.Equal(table.Rows.Count, view.Count);
        }

        #endregion

        #region DefaultViewReturnsAllRows

        [Fact]
        public void DefaultViewReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader);

            Assert.Equal(table.Rows.Count, table.DefaultView.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterReturnsOnlyFilteredRows

        [Fact]
        public void ChangeViewSpatialFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader);
            IGeometry queryExtents = _factories.GeoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();

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

            Assert.Equal(expectedRows.Count, view.Count);
        }

        #endregion

        #region ChangeViewSpatialFilterTriggersNotification

        [Fact]
        public void ChangeViewSpatialFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
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

            Assert.False(resetNotificationOccured);

            IExtents queryExtents = _factories.GeoFactory.CreateExtents2D(0, 0, 10, 10);
            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(queryExtents);
            view.SpatialFilter = expression;

            Assert.True(resetNotificationOccured);
        }

        #endregion

        #region ExecutingQueryOnTableTriggersViewListChangedResetNotification

        [Fact]
        public void ExecutingQueryOnTableTriggersViewListChangedResetNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            IExtents dataExtents = data.GetExtents();
            IExtents halfBounds = _factories.GeoFactory.CreateExtents(
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

            IExtents otherHalfBounds = _factories.GeoFactory.CreateExtents(
                dataExtents.Min, dataExtents.Max);
            otherHalfBounds.TranslateRelativeToWidth(0.5, 0.5);
            otherHalfBounds.Scale(0.5);

            query = FeatureQueryExpression.Intersects(otherHalfBounds);
            reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);

            Assert.True(addNotificationOccured);
        }

        #endregion

        #region AddingRowToTableTriggersViewListChangedItemAddedNotification

        [Fact]
        public void AddingRowToTableTriggersViewListChangedItemAddedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);

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
            row.Geometry = _factories.GeoFactory.CreatePoint2D(44, 44);
            table.AddRow(row);

            Assert.True(addNotificationOccured);
        }

        #endregion

        #region RemovingRowFromTableTriggersViewListChangedItemDeletedNotification

        [Fact]
        public void RemovingRowFromTableTriggersViewListChangedItemDeletedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);

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

            Assert.True(addNotificationOccured);
        }

        #endregion

        #region ChangingRowTriggersViewListChangedItemChangedNotification

        [Fact]
        public void ChangingRowTriggersViewListChangedItemChangedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);

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

            Assert.True(addNotificationOccured);
        }

        #endregion

        #region MovingRowTriggersViewListChangedItemMovedNotification

        [Fact(Skip = "Not sure how to generate this notification yet.")]
        public void MovingRowTriggersViewListChangedItemMovedNotification()
        {
            // TODO: implement MovingRowTriggersViewListChangedItemMovedNotification
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);

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

            Assert.True(addNotificationOccured);
        }

        #endregion

        #region SettingViewFilterToIncludeRowsNotPresentInTableCausesFeaturesRequestNotification

        [Fact(Skip = "Notifications now happen at a Layer level.")]
        public void SettingViewFilterToIncludeRowsNotPresentInTableCausesFeaturesRequestNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);

            Boolean featuresRequested = false;
            //table.FeaturesNotFound += delegate { featuresRequested = true; };

            FeatureDataView view = new FeatureDataView(table);

            IExtents otherHalfBounds = data.GetExtents();
            otherHalfBounds.TranslateRelativeToWidth(0.5, 0.5);
            otherHalfBounds.Scale(0.5);

            Assert.False(featuresRequested);

            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(otherHalfBounds);
            view.SpatialFilter = expression;

            Assert.True(featuresRequested);
        }

        #endregion

        #region SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView

        [Fact]
        public void SettingOidFilterAllowsOnlyFeaturesContainingFilteredOidsInView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable<Guid> table = new FeatureDataTable<Guid>("Oid", _factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            Guid[] ids = new Guid[3];
            ids[0] = (table.Rows[1] as FeatureDataRow<Guid>).Id;
            ids[1] = (table.Rows[4] as FeatureDataRow<Guid>).Id;
            ids[2] = (table.Rows[6] as FeatureDataRow<Guid>).Id;
            FeatureDataView view = new FeatureDataView(table);
            view.OidFilter = new OidCollectionExpression(ids);

            Assert.Equal(3, view.Count);
            Assert.True(view.Find(ids[0]) > -1);
            Assert.True(view.Find(ids[1]) > -1);
            Assert.True(view.Find(ids[2]) > -1);
            Assert.Equal(-1, view.Find((table.Rows[0] as FeatureDataRow<Guid>).Id));
            Assert.Equal(-1, view.Find((table.Rows[2] as FeatureDataRow<Guid>).Id));
            Assert.Equal(-1, view.Find((table.Rows[3] as FeatureDataRow<Guid>).Id));
            Assert.Equal(-1, view.Find((table.Rows[5] as FeatureDataRow<Guid>).Id));
        }

        #endregion

        #region ChangingOidFilterTriggersNotification

        [Fact]
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

            Assert.False(resetNotificationOccured);

            Guid[] ids = new Guid[3];
            ids[0] = (table.Rows[1] as FeatureDataRow<Guid>).Id;
            ids[1] = (table.Rows[4] as FeatureDataRow<Guid>).Id;
            ids[2] = (table.Rows[6] as FeatureDataRow<Guid>).Id;
            view.OidFilter = new OidCollectionExpression(ids);

            Assert.True(resetNotificationOccured);
        }

        #endregion

        [Fact]
        public void SettingSpatialPredicateOfFilterChangesViewToMatchFilter()
        {
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.Equal(table.Rows.Count, view.Count);

            view.SpatialFilter = SpatialBinaryExpression.Intersects(table.Extents);
                
            Assert.Equal(table.Rows.Count, view.Count);

            view.SpatialFilter = new SpatialBinaryExpression(new ThisExpression(), 
                                                             SpatialOperation.Disjoint, 
                                                             new ExtentsExpression(table.Extents));

            Assert.Equal(0, view.Count);

            view.SpatialFilter = SpatialBinaryExpression.Intersects(_factories.GeoFactory.CreatePoint2D(0, 0));

            Assert.Equal(1, view.Count);
        }


        [Fact]
        public void ChangingFilterIsExclusivePropertyInvertsFilterMatch()
        {
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.Equal(table.Rows.Count, view.Count);
            Assert.False(view.IsViewDefinitionExclusive);

            view.IsViewDefinitionExclusive = !view.IsViewDefinitionExclusive;

            Assert.True(view.IsViewDefinitionExclusive);
            Assert.Equal(0, view.Count); 
        }


        [Fact]
        public void VariousFilteringCombinationTestSequence()
        {
            // this horrendous sequence is what I had time alloted for
            // takes some better setup to factor into tests that test 1 thing
            FeatureDataTable<Guid> table;
            FeatureDataView view;
            createDataViewOnNewTable(out view, out table, false);

            Assert.Equal(table.Rows.Count, view.Count);

            SpatialBinaryExpression spatialFilter = 
                SpatialBinaryExpression.Intersects(_factories.GeoFactory.WktReader.Read("POLYGON ((19 19, 21 19, 20 21, 19 19))"));

            // test basic spatial filter
            view.SpatialFilter = spatialFilter;

            Assert.Equal(3, view.Count);

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

            Assert.Equal(2, view.Count);

            // test clearing spatial component
            view.SpatialFilter = null;

            Assert.Equal(4, view.Count);

            // test inverting oid filter
            view.IsViewDefinitionExclusive = true;

            Assert.Equal(table.Rows.Count - 4, view.Count);

            // test both components with inversion
            view.SpatialFilter = spatialFilter;

            Assert.Equal(table.Rows.Count - 2, view.Count);

            // test clearing OidFilter
            view.OidFilter = null;

            Assert.Equal(table.Rows.Count - 3, view.Count);

            // test clearing inversion
            view.IsViewDefinitionExclusive = false;

            Assert.Equal(3, view.Count);

            // test clearing everything
            view.SpatialFilter = null;

            Assert.Equal(table.Rows.Count, view.Count);
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

        [Fact(Skip = "Incomplete")]
        public void SettingTablePropertyImposesFiltersOnNewTable()
        {
        }

        #region AddNewRowThrowsNotSupported

        [Fact]
        public void AddNewRowThrowsNotSupported()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            FeatureDataView view = new FeatureDataView(table);
            Assert.Throws<NotSupportedException>(delegate { view.AddNew(); });
        }

        #endregion

        #region SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject

        [Fact]
        public void SettingGeometryFilterToIdenticalGeometryDoesntChangeFilterObject()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            IGeometry filter = _factories.GeoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            IGeometry filterCopy = filter.Clone();
            Assert.NotSame(filter, filterCopy);
            Assert.Equal(filter, filterCopy);
            SpatialBinaryExpression expression = SpatialBinaryExpression.Intersects(filterCopy);
            view.SpatialFilter = expression;
            Assert.NotSame(expression, view.SpatialFilter);
        }

        #endregion

        #region SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime

        [Fact]
        public void SpatialQueryTypeIsWhatIsSpecifiedAtCreateTime()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            IGeometry filter = _factories.GeoFactory.CreateExtents2D(0, 0, 10, 10).ToGeometry();
            FeatureDataView view = new FeatureDataView(table, filter, "", DataViewRowState.CurrentRows);
            Assert.Equal(SpatialOperation.Intersects, view.SpatialFilter.Op);
        }

        #endregion

        #region DataViewManagerIsAFeatureDataViewManager

        [Fact]
        //[ExpectedException(typeof (NotImplementedException))]
        public void DataViewManagerIsAFeatureDataViewManager()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);
            FeatureDataTable table = new FeatureDataTable(_factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            FeatureDataSet dataSet = new FeatureDataSet(_factories.GeoFactory);
            dataSet.Tables.Add(table);
            FeatureDataView view = dataSet.DefaultViewManager.CreateDataView(table);

            Assert.NotNull(view.DataViewManager);
            Assert.IsType(typeof (FeatureDataViewManager), view.DataViewManager);
        }

        #endregion

        private void createDataViewOnNewTable(out FeatureDataView view, 
                                              out FeatureDataTable<Guid> table,
                                              Boolean includeGeometryCollections)
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory,
                                                                            includeGeometryCollections);
            table = new FeatureDataTable<Guid>("Oid", _factories.GeoFactory);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(data.GetExtents());
            IFeatureDataReader reader = data.ExecuteFeatureQuery(query);
            table.Load(reader, LoadOption.OverwriteChanges, null);
            view = new FeatureDataView(table);
        }
    }
}
