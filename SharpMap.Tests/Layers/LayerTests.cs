using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems;

using SharpMap.Data;
using SharpMap.Layers;
using Xunit;

namespace SharpMap.Tests.Layers
{

    public class LayerTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        [Fact(Skip = "Test deferred until caching detection improved")]
        public void LayerCachingTest()
        {
            IGeometryFactory geoFactory = _factories.GeoFactory;
            GeometryLayer layer1 = DataSourceHelper.CreateFeatureFeatureLayer(geoFactory);
            GeometryLayer layer2 = DataSourceHelper.CreateFeatureFeatureLayer(geoFactory);

            IExtents box1 = geoFactory.CreateExtents2D(0, 0, 10, 10);
            IExtents box2 = geoFactory.CreateExtents2D(35, 25, 45, 35);

            layer1.AsyncQuery = false;
            // Do query

            layer2.AsyncQuery = false;
            // Do query

            Assert.True(layer1.Features.Extents.Contains(box2), "Test validity check");
            // features intersecting box1 must have extents that contain box2

            // Do query
        }

        private static Boolean dataViewEquivalent(FeatureDataView view1, FeatureDataView view2)
        {
            if (view1.Count != view2.Count)
            {
                return false;
            }

            foreach (FeatureDataRow row1 in ((IEnumerable<FeatureDataRow>)view1))
            {
                Boolean notFound = true;

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