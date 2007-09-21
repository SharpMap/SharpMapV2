using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using System.ComponentModel;
using SharpMap.Geometries;

namespace SharpMap.Tests.Features
{
    [TestFixture]
    public class FeatureDataViewTests
    {
        [Test]
        public void CreatingDataView()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException), ExpectedMessage = "RowFilter expressions not supported at this time.")]
        public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            int expectedRowCount = 0;

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

        [Test]
        [ExpectedException(typeof(NotSupportedException), ExpectedMessage = "RowFilter expressions not supported at this time.")]
        public void ChangeViewAttributeFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            bool resetNotificationOccured = false;

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

        [Test]
        public void NullSpatialFilterReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));
            FeatureDataView view = new FeatureDataView(table);

            Assert.AreEqual(table.Rows.Count, view.Count);
        }

        [Test]
        public void DefaultViewReturnsAllRows()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            table.Load(data.ExecuteIntersectionQuery(data.GetExtents()));

            Assert.AreEqual(table.Rows.Count, table.DefaultView.Count);
        }

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
            view.GeometryIntersectionFilter = queryExtents.ToGeometry();

            Assert.AreEqual(expectedRows.Count, view.Count);
        }

        [Test]
        public void ChangeViewSpatialFilterTriggersNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            data.ExecuteIntersectionQuery(data.GetExtents(), table);
            FeatureDataView view = new FeatureDataView(table);

            bool resetNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
            {
                if (e.ListChangedType == ListChangedType.Reset)
                {
                    resetNotificationOccured = true;
                }
            };

            Assert.IsFalse(resetNotificationOccured);

            BoundingBox queryExtents = new BoundingBox(0, 0, 10, 10);
            view.GeometryIntersectionFilter = queryExtents.ToGeometry();

            Assert.IsTrue(resetNotificationOccured);
        }

        [Test]
        public void ExecutingQueryOnTableTriggersViewListChangedResetNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            bool addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
            {
                if (e.ListChangedType == ListChangedType.Reset)
                {
                    addNotificationOccured = true;
                }
            };

            BoundingBox otherHalfBounds = new BoundingBox(
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2,
                dataExtents.Right, dataExtents.Top);

            data.ExecuteIntersectionQuery(otherHalfBounds, table);

            Assert.IsTrue(addNotificationOccured);
        }

        [Test]
        public void AddingRowToTableTriggersViewListChangedItemAddedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            bool addNotificationOccured = false;

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

        [Test]
        public void RemovingRowFromTableTriggersViewListChangedItemDeletedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            bool addNotificationOccured = false;

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

        [Test]
        public void ChangingRowTriggersViewListChangedItemChangedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            bool addNotificationOccured = false;

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

        [Test]
        [Ignore("Not sure how to generate this notification yet.")]
        public void MovingRowTriggersViewListChangedItemMovedNotification()
        {
            FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
            FeatureDataTable table = new FeatureDataTable();
            BoundingBox dataExtents = data.GetExtents();
            BoundingBox halfBounds = new BoundingBox(dataExtents.Left, dataExtents.Bottom,
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2);

            data.ExecuteIntersectionQuery(halfBounds, table);
            FeatureDataView view = new FeatureDataView(table);

            bool addNotificationOccured = false;

            view.ListChanged += delegate(object sender, ListChangedEventArgs e)
            {
                if (e.ListChangedType == ListChangedType.ItemMoved)
                {
                    addNotificationOccured = true;
                }
            };

            Assert.IsTrue(addNotificationOccured);
        }
    }
}
