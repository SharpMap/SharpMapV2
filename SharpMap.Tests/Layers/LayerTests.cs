using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SharpMap.Layers;
using SharpMap.Geometries;
using SharpMap.Features;

namespace SharpMap.Tests.Layers
{
    [TestFixture]
	public class LayerTests
	{

        [Test]
        public void VisibleRegionChangedTest()
        {
            VectorLayer layer = DataSourceHelper.CreateFeatureVectorLayer();
            Assert.IsTrue(layer.Features.Rows.Count == 12, "Count tests probably not valid");

            layer.AsyncQuery = false;
            layer.VisibleRegion = new BoundingBox(0, 0, 10, 10);

            List<string> featureNames = new List<string>();
            featureNames.Add("A geometry collection");
            featureNames.Add("A multipolygon");
            featureNames.Add("A multilinestring");

            Assert.IsTrue(layer.VisibleFeatures.Count == 3, "Expected features in visible region");
            foreach (FeatureDataRow row in layer.VisibleFeatures)
            {
                Assert.IsTrue(layer.VisibleRegion.Intersects(row.Geometry.GetBoundingBox()));
                Assert.Contains(row["FeatureName"], featureNames, "Unexpected visible feature");
            }


            layer.VisibleRegion = new BoundingBox(35, 25, 45, 35);
            featureNames.Clear();
            featureNames.Add("A linestring");
            featureNames.Add("A multilinestring");
            featureNames.Add("A polygon");
            featureNames.Add("A multipoint");

            Assert.IsTrue(layer.VisibleFeatures.Count == 4, "Expected features in visible region");
            foreach (FeatureDataRow row in layer.VisibleFeatures)
            {
                Assert.IsTrue(layer.VisibleRegion.Intersects(row.Geometry.GetBoundingBox()));
                Assert.Contains(row["FeatureName"], featureNames, "Unexpected visible feature");
            }

            // looking at dropping OnVisibleFeaturesChanged in lieu of OnPropertyChanged

        }
	}
}
