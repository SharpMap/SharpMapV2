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
            Assert.AreEqual(7, layer.Features.Rows.Count, "Count tests probably not valid");

            layer.VisibleRegion = new BoundingBox(0, 0, 10, 10);

            Assert.AreEqual(0, layer.Features.Rows.Count);
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
    }
}
