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
            //map.AddLayer(DataSourceHelper.CreateGeometryVectorLayer());
            layer.VisibleRegion = new BoundingBox(0, 0, 10, 10);

            BoundingBox actual = BoundingBox.Empty;
            foreach (FeatureDataRow row in layer.VisibleFeatures)
            {
                Assert.IsTrue(layer.VisibleRegion.Intersects(row.Geometry.GetBoundingBox()));
                
            }
            // also look for features in layer not in visible features collection but do intersect visible region

            // looking at dropping OnVisibleFeaturesChanged in lieu of OnPropertyChanged



            //layer.VisibleRegion = new BoundingBox(0, 0, 10, 10 etc.);
           
        }
	}
}
