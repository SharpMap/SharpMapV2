using System;
using GeoAPI.Geometries;
using NetTopologySuite.Coordinates;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.SimpleGeometries;
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
            BufferedCoordinate2DFactory coordFactory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2DSequenceFactory sequenceFactory = new BufferedCoordinate2DSequenceFactory();
            _geoFactory = new GeometryFactory(coordFactory, sequenceFactory);
        }

        [Test]
        public void GetLayerByNameReturnsCorrectLayer()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map(_geoFactory);
            IFeatureLayerProvider dataSource = mocks.Stub<IFeatureLayerProvider>();

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
            IFeatureLayerProvider dataSource = DataSourceHelper.CreateFeatureDatasource(_geoFactory);

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
            IFeatureLayerProvider dataSource = mocks.Stub<IFeatureLayerProvider>();

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
        public void EmptyMap_Defaults()
        {
            Map map = new Map(_geoFactory);
            IExtents emptyExtents = _geoFactory.CreateExtents();
            Assert.AreEqual(map.Extents, emptyExtents);
            // changed to null from Point.Empty
            Assert.AreEqual(map.Center, null);
            Assert.IsNotNull(map.Layers);
            Assert.AreEqual(map.Layers.Count, 0);
            Assert.AreEqual(map.ActiveTool, StandardMapTools2D.None);
            Assert.IsNotNull(map.SelectedLayers);
        }

        [Test]
        public void GetExtents_ValidDatasource()
        {
            Map map = new Map(_geoFactory);

            GeometryLayer vLayer = new GeometryLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource(_geoFactory));

            map.AddLayer(vLayer);
            IExtents box = map.Extents;
            Assert.AreEqual(_geoFactory.CreateExtents2D(0, 0, 120, 100), box);
        }
    }
}