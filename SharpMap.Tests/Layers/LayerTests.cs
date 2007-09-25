using System.Collections.Generic;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;

namespace SharpMap.Tests.Layers
{
    [TestFixture]
    public class LayerTests
    {
        [Test]
        [Ignore("Test deferred until caching detection improved")]
        public void LayerCachingTest()
        {
            GeometryLayer layer1 = DataSourceHelper.CreateFeatureFeatureLayer();
            GeometryLayer layer2 = DataSourceHelper.CreateFeatureFeatureLayer();

            BoundingBox box1 = new BoundingBox(0, 0, 10, 10);
            BoundingBox box2 = new BoundingBox(35, 25, 45, 35);

            layer1.AsyncQuery = false;
            // Do query

            layer2.AsyncQuery = false;
            // Do query

            Assert.IsTrue(layer1.Features.Extents.Contains(box2), "Test validity check");
            // features intersecting box1 must have extents that contain box2

            // Do query
        }

        private static bool dataViewEquivalent(FeatureDataView view1, FeatureDataView view2)
        {
            if (view1.Count != view2.Count)
            {
                return false;
            }

            foreach (FeatureDataRow row1 in ((IEnumerable<FeatureDataRow>)view1))
            {
                bool notFound = true;

                foreach (FeatureDataRow row2 in ((IEnumerable<FeatureDataRow>)view2))
                {
                    if (row1["FeatureName"] == row2["FeatureName"])
                    {
                        notFound = false;

                        if (!row1.Geometry.Equals(row2.Geometry))
                        {
                            return false;
                        }
                    }
                }

                if (notFound)
                {
                    return false;
                }
            }
            return true;
        }
    }
}