using System;
using GeoAPI.Geometries;
using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Tools;
using Xunit;

namespace SharpMap.Tests
{
    
    public class MapTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        [Fact]
        public void GetLayerByNameReturnsCorrectLayer()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map(_factories.GeoFactory);
            IFeatureProvider dataSource = mocks.Stub<IFeatureProvider>();
            dataSource.GeometryFactory = _factories.GeoFactory;

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));

            ILayer layer = map.GetLayerByName("Layer 2");
            Assert.NotNull(layer);
            Assert.Equal("Layer 2", layer.LayerName);
        }

        [Fact]
        public void DuplicateLayerNamesThrowsException()
        {
            Map map = new Map(_factories.GeoFactory);
            IFeatureProvider dataSource = DataSourceHelper.CreateFeatureDatasource(_factories.GeoFactory);

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            Assert.Throws<DuplicateLayerException>(delegate { map.AddLayer(new GeometryLayer("Layer 3", dataSource)); });
        }

        [Fact]
        public void FindLayerByPredicateReturnsMatchingLayers()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map(_factories.GeoFactory);
            IFeatureProvider dataSource = mocks.Stub<IFeatureProvider>();
            dataSource.GeometryFactory = _factories.GeoFactory;

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3a", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            map.AddLayer(new GeometryLayer("layer 3b", dataSource));

            Int32 count = 0;

            foreach (ILayer layer in map.FindLayers("Layer 3"))
            {
                Assert.True(layer.LayerName.StartsWith("Layer 3", StringComparison.CurrentCultureIgnoreCase));
                count++;
            }

            Assert.Equal(2, count);
        }

        [Fact]
        public void EmptyMapDefaultsAreCorrect()
        {
            Map map = new Map(_factories.GeoFactory);
            IExtents emptyExtents = _factories.GeoFactory.CreateExtents();
            IPoint emptyPoint = _factories.GeoFactory.CreatePoint();
            Assert.Equal(emptyExtents, map.Extents);
            // changed to null from Point.Empty
            Assert.Equal(emptyPoint.Coordinate, map.Center);
            Assert.NotNull(map.Layers);
            Assert.Equal(0, map.Layers.Count);
            Assert.Equal(StandardMapView2DMapTools.None, map.ActiveTool);
            Assert.NotNull(map.SelectedLayers);
        }

        [Fact]
        public void MapExtentsReflectTheUnderlyingLayersCorrectlyWhenUsingAddLayer()
        {
            Map map = new Map(_factories.GeoFactory);

            GeometryLayer layer = new GeometryLayer("Geom layer", 
                                                     DataSourceHelper.CreateGeometryDatasource(_factories.GeoFactory));

            map.AddLayer(layer);
            IExtents box = map.Extents;
            Assert.Equal(_factories.GeoFactory.CreateExtents2D(0, 0, 120, 100), box);
        }

        [Fact]
        public void MapExtentsReflectTheUnderlyingLayersCorrectlyWhenUsingLayersAdd()
        {
            Map map = new Map(_factories.GeoFactory);

            GeometryLayer layer = new GeometryLayer("Geom layer",
                                                     DataSourceHelper.CreateGeometryDatasource(_factories.GeoFactory));

            map.Layers.Add(layer);
            IExtents box = map.Extents;
            Assert.Equal(_factories.GeoFactory.CreateExtents2D(0, 0, 120, 100), box);   
        }
    }
}