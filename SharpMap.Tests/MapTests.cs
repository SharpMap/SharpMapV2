using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Tools;

namespace SharpMap.Tests
{
	[TestFixture]
	public class MapTests
	{
		[Test]
		public void GetLayerByName_ReturnCorrectLayer()
		{
			Map map = new Map();
			map.Layers.Add(new VectorLayer("Layer 1"));
			map.Layers.Add(new VectorLayer("Layer 3"));
			map.Layers.Add(new VectorLayer("Layer 2"));

			ILayer layer = map.GetLayerByName("Layer 2");
			Assert.IsNotNull(layer);
			Assert.AreEqual("Layer 2", layer.LayerName);
		}

		[Test]
		public void GetLayerByName()
		{
			Map map = new Map();
			map.Layers.Add(new VectorLayer("Layer 1"));
			map.Layers.Add(new VectorLayer("Layer 3"));
			map.Layers.Add(new VectorLayer("Layer 2"));
			map.Layers.Add(new VectorLayer("Layer 3"));

			ILayer layer = map.Layers["Layer 3"];
			Assert.AreEqual("Layer 3", layer.LayerName);
		}

		[Test]
		public void FindLayerByPredicate()
		{
			Map map = new Map();
			map.Layers.Add(new VectorLayer("Layer 1"));
			map.Layers.Add(new VectorLayer("Layer 3"));
			map.Layers.Add(new VectorLayer("Layer 2"));
			map.Layers.Add(new VectorLayer("Layer 3"));

			int count = 0;

			foreach (ILayer layer in map.Layers.FindAll(delegate(ILayer match)
			{
				return String.Compare(match.LayerName, "Layer 3", StringComparison.CurrentCultureIgnoreCase) == 0;
			}))
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
			Assert.AreEqual(map.SelectedTool, MapTool.None);
			Assert.IsNotNull(map.SelectedLayers);
		}

		[Test]
		public void GetExtents_ValidDatasource()
		{
			Map map = new Map();

			VectorLayer vLayer = new VectorLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());

			map.Layers.Add(vLayer);
			BoundingBox box = map.GetExtents();
			Assert.AreEqual(new BoundingBox(0, 0, 50, 346.3493254), box);
		}
	}
}
