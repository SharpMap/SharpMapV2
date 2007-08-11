using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

using SharpMap;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Tools;
using SharpMap.Data;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Model
{
	[TestFixture]
	public class MapTests
	{
		[Test]
		public void GetLayerByName_ReturnCorrectLayer()
		{
            MockRepository mocks = new MockRepository();

			Map map = new Map();
            ILayerProvider dataSource = mocks.Stub<ILayerProvider>();

            map.AddLayer(new VectorLayer("Layer 1", dataSource));
            map.AddLayer(new VectorLayer("Layer 3", dataSource));
            map.AddLayer(new VectorLayer("Layer 2", dataSource));

			ILayer layer = map.GetLayerByName("Layer 2");
			Assert.IsNotNull(layer);
			Assert.AreEqual("Layer 2", layer.LayerName);
		}

		[Test]
        [ExpectedException(typeof(DuplicateLayerException))]
		public void DuplicateLayerNamesThrowsException()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            ILayerProvider dataSource = mocks.Stub<ILayerProvider>();

            map.AddLayer(new VectorLayer("Layer 1", dataSource));
            map.AddLayer(new VectorLayer("Layer 3", dataSource));
            map.AddLayer(new VectorLayer("Layer 2", dataSource));
            map.AddLayer(new VectorLayer("Layer 3", dataSource));
		}

		[Test]
		public void FindLayerByPredicate()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            ILayerProvider dataSource = mocks.Stub<ILayerProvider>();

            map.AddLayer(new VectorLayer("Layer 1", dataSource));
            map.AddLayer(new VectorLayer("Layer 3", dataSource));
            map.AddLayer(new VectorLayer("Layer 2", dataSource));
            map.AddLayer(new VectorLayer("Layer 4", dataSource));

			int count = 0;

            foreach (ILayer layer in map.FindLayers("Layer 3"))
			{
				Assert.AreEqual("Layer 3", layer.LayerName);
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
			Assert.AreEqual(map.ActiveTool, MapTool.None);
			Assert.IsNotNull(map.SelectedLayers);
		}

		[Test]
		public void GetExtents_ValidDatasource()
		{
			Map map = new Map();

			VectorLayer vLayer = new VectorLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());

			map.AddLayer(vLayer);
			BoundingBox box = map.GetExtents();
			Assert.AreEqual(new BoundingBox(0, 0, 100, 100), box);
		}
	}
}
