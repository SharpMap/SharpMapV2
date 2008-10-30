using System;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Tools;

namespace SharpMap.Tests
{
    [TestFixture]
    public class MapTests
    {
        private IGeometryFactory _geoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            BufferedCoordinateSequenceFactory sequenceFactory = new BufferedCoordinateSequenceFactory();
            _geoFactory = new GeometryFactory<BufferedCoordinate>(sequenceFactory);
        }

        [Test]
        public void GetLayerByNameReturnsCorrectLayer()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map(_geoFactory);
            IFeatureProvider dataSource = mocks.Stub<IFeatureProvider>();
            dataSource.GeometryFactory = _geoFactory;

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));

            ILayer layer = map.GetLayerByName("Layer 2");
            Assert.IsNotNull(layer);
            Assert.AreEqual("Layer 2", layer.LayerName);
        }

        [Test]
        [ExpectedException(typeof (DuplicateLayerException))]
        public void DuplicateLayerNamesThrowsException()
        {
            Map map = new Map(_geoFactory);
            IFeatureProvider dataSource = DataSourceHelper.CreateFeatureDatasource(_geoFactory);

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
        }

        [Test]
        public void FindLayerByPredicateReturnsMatchingLayers()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map(_geoFactory);
            IFeatureProvider dataSource = mocks.Stub<IFeatureProvider>();
            dataSource.GeometryFactory = _geoFactory;

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3a", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            map.AddLayer(new GeometryLayer("layer 3b", dataSource));

            Int32 count = 0;

            foreach (ILayer layer in map.FindLayers("Layer 3"))
            {
                Assert.IsTrue(layer.LayerName.StartsWith("Layer 3", StringComparison.CurrentCultureIgnoreCase));
                count++;
            }

            Assert.AreEqual(2, count);
        }

        [Test]
        public void EmptyMapDefaultsAreCorrect()
        {
            Map map = new Map(_geoFactory);
            IExtents emptyExtents = _geoFactory.CreateExtents();
            IPoint emptyPoint = _geoFactory.CreatePoint();
            Assert.AreEqual(emptyExtents, map.Extents);
            // changed to null from Point.Empty
            Assert.AreEqual(emptyPoint.Coordinate, map.Center);
            Assert.IsNotNull(map.Layers);
            Assert.AreEqual(0, map.Layers.Count);
            Assert.AreEqual(StandardMapView2DMapTools.None, map.ActiveTool);
            Assert.IsNotNull(map.SelectedLayers);
        }

        [Test]
        public void MapExtentsReflectTheUnderlyingLayersCorrectlyWhenUsingAddLayer()
        {
            Map map = new Map(_geoFactory);

            GeometryLayer layer = new GeometryLayer("Geom layer", 
                                                     DataSourceHelper.CreateGeometryDatasource(_geoFactory));

            map.AddLayer(layer);
            IExtents box = map.Extents;
            Assert.AreEqual(_geoFactory.CreateExtents2D(0, 0, 120, 100), box);
        }

        [Test]
        public void MapExtentsReflectTheUnderlyingLayersCorrectlyWhenUsingLayersAdd()
        {
            Map map = new Map(_geoFactory);

            GeometryLayer layer = new GeometryLayer("Geom layer",
                                                     DataSourceHelper.CreateGeometryDatasource(_geoFactory));

            map.Layers.Add(layer);
            IExtents box = map.Extents;
            Assert.AreEqual(_geoFactory.CreateExtents2D(0, 0, 120, 100), box);   
        }
    }
}