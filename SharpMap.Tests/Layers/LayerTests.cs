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
        public void SettingVisibleRegionToEmptyShouldMakeAllFeaturesInvisible()
        {
            VectorLayer layer = DataSourceHelper.CreateFeatureVectorLayer();

            layer.AsyncQuery = false;
            layer.VisibleRegion = layer.Envelope;

            // There are only 7 non-empty geometries
            Assert.AreEqual(7, layer.VisibleFeatures.Count, "Count tests probably not valid");

            layer.VisibleRegion = BoundingBox.Empty;

            Assert.AreEqual(0, layer.VisibleFeatures.Count);
        }

        [Test]
        public void VisibleRegionChangedTest()
        {
            VectorLayer layer = DataSourceHelper.CreateFeatureVectorLayer();

            layer.AsyncQuery = false;
            layer.VisibleRegion = layer.Envelope;

            // There are only 7 non-empty geometries
            Assert.AreEqual(7, layer.Features.Rows.Count, "Count tests probably not valid");

            layer.VisibleRegion = new BoundingBox(0, 0, 10, 10);

            List<string> featureNames = new List<string>();
            featureNames.Add("A geometry collection");
            featureNames.Add("A multipolygon");
            featureNames.Add("A multilinestring");

            Assert.AreEqual(featureNames.Count, layer.VisibleFeatures.Count, "Expected features in visible region");

            foreach (FeatureDataRow row in ((IEnumerable<FeatureDataRow>)layer.VisibleFeatures))
            {
                Assert.IsTrue(layer.VisibleRegion.Intersects(row.Geometry.GetBoundingBox()));
                Assert.Contains(row["FeatureName"], featureNames, "Unexpected visible feature");
            }

            layer.VisibleRegion = new BoundingBox(35, 25, 45, 35);
            featureNames.Clear();
#warning The linestring doesn't actually intersect the visible region, but it's bounding box does. This should break when NTS is integrated.
            featureNames.Add("A linestring");
            featureNames.Add("A multilinestring");
            featureNames.Add("A multipoint");

            Assert.AreEqual(featureNames.Count, layer.VisibleFeatures.Count, "Expected features in visible region");

            foreach (FeatureDataRow row in ((IEnumerable<FeatureDataRow>)layer.VisibleFeatures))
            {
                Assert.IsTrue(layer.VisibleRegion.Intersects(row.Geometry.GetBoundingBox()));
                Assert.Contains(row["FeatureName"], featureNames, "Unexpected visible feature");
            }
        }


        [Test]
        [Ignore("Test deferred until caching detection improved")]
        public void LayerCachingTest()
        {
            VectorLayer layer1 = DataSourceHelper.CreateFeatureVectorLayer();
            VectorLayer layer2 = DataSourceHelper.CreateFeatureVectorLayer();

            BoundingBox box1 = new BoundingBox(0, 0, 10, 10);
            BoundingBox box2 = new BoundingBox(35, 25, 45, 35);

            layer1.AsyncQuery = false;
            layer1.VisibleRegion = box1;

            layer2.AsyncQuery = false;
            layer2.VisibleRegion = box2;

            Assert.IsTrue(layer1.Features.Envelope.Contains(box2), "Test validity check");
            // features intersecting box1 must have extents that contain box2
            Assert.IsFalse(dataViewEquivalent(layer1.VisibleFeatures, layer2.VisibleFeatures), "Different regions, same return set");

            layer1.VisibleRegion = box2;
            Assert.IsTrue(dataViewEquivalent(layer1.VisibleFeatures, layer2.VisibleFeatures), "Same region, different return sets");
        }

        static bool dataViewEquivalent(FeatureDataView view1, FeatureDataView view2)
        {
            if (view1.Count != view2.Count)
                return false;

            foreach (FeatureDataRow row1 in ((IEnumerable<FeatureDataRow>)view1))
            {
                bool notFound = true;
                foreach (FeatureDataRow row2 in ((IEnumerable<FeatureDataRow>)view2))
                {
                    if (row1["FeatureName"] == row2["FeatureName"])
                    {
                        notFound = false;
                        if (!row1.Geometry.Equals(row2.Geometry))
                            return false;
                    }
                }
                if (notFound)
                    return false;
            }
            return true;
        }
    }
}
