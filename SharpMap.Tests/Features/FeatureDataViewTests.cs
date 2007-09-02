using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Features;

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
    	public void ChangeViewAttributeFilterReturnsOnlyFilteredRows()
		{
			FeatureProvider data = DataSourceHelper.CreateFeatureDatasource();
			FeatureDataTable table = new FeatureDataTable();
			data.ExecuteIntersectionQuery(data.GetExtents(), table);
			FeatureDataView view = new FeatureDataView(table);

			view.RowFilter = "FeatureName LIKE 'A m*'";
		}

		[Test]
		public void ChangeViewAttributeFilterTriggersNotification()
		{

		}

		[Test]
		public void ChangeViewSpatialFilterReturnsOnlyFilteredRows()
		{

		}

		[Test]
		public void ChangeViewSpatialFilterTriggersNotification()
		{

		}
    }
}
