
using System;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Tools;

namespace SharpMap.Tests
{
	[TestFixture]
	public class MapTests
	{
		[Test]
		public void GetLayerByNameReturnsCorrectLayer()
		{
            MockRepository mocks = new MockRepository();

			Map map = new Map();
            IFeatureLayerProvider dataSource = mocks.Stub<IFeatureLayerProvider>();

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));

			ILayer layer = map.GetLayerByName("Layer 2");
			Assert.IsNotNull(layer);
			Assert.AreEqual("Layer 2", layer.LayerName);
		}

		[Test]
        [ExpectedException(typeof(DuplicateLayerException))]
		public void DuplicateLayerNamesThrowsException()
        {
            Map map = new Map();
            IFeatureLayerProvider dataSource = DataSourceHelper.CreateFeatureDatasource();

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3", dataSource));
		}

		[Test]
		public void FindLayerByPredicateReturnsMatchingLayers()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            IFeatureLayerProvider dataSource = mocks.Stub<IFeatureLayerProvider>();

            map.AddLayer(new GeometryLayer("Layer 1", dataSource));
            map.AddLayer(new GeometryLayer("Layer 3a", dataSource));
            map.AddLayer(new GeometryLayer("Layer 2", dataSource));
            map.AddLayer(new GeometryLayer("layer 3b", dataSource));

			int count = 0;

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
			Map map = new Map();
			Assert.AreEqual(map.GetExtents(), BoundingBox.Empty);
			Assert.AreEqual(map.Center, Point.Empty);
			Assert.IsNotNull(map.Layers);
			Assert.AreEqual(map.Layers.Count, 0);
			Assert.AreEqual(map.ActiveTool, StandardMapTools2D.None);
			Assert.IsNotNull(map.SelectedLayers);
		}

		[Test]
		public void GetExtents_ValidDatasource()
		{
			Map map = new Map();

			GeometryLayer vLayer = new GeometryLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());

			map.AddLayer(vLayer);
			BoundingBox box = map.GetExtents();
			Assert.AreEqual(new BoundingBox(0, 0, 100, 100), box);
		}
	}
}
