using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Features;
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
        [ExpectedException(typeof(NotSupportedException), "RowFilter expressions not supported at this time.")]
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
        [ExpectedException(typeof(NotSupportedException), "RowFilter expressions not supported at this time.")]
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
			view.VisibleRegion = queryExtents;

            Assert.AreEqual(expectedRows.Count, view.Count);
		}

		[Test]
		public void ChangeViewSpatialFilterTriggersNotification()
		{

		}

        [Test]
        public void AddingRowsToTableTriggersViewNotification()
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

            BoundingBox otherHalfBounds = new BoundingBox(
                dataExtents.Left + dataExtents.Width / 2, dataExtents.Bottom + dataExtents.Height / 2, 
                dataExtents.Right, dataExtents.Top);

            data.ExecuteIntersectionQuery(otherHalfBounds, table);

            Assert.IsTrue(addNotificationOccured);
        }
    }
}
