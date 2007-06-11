using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

using SharpMap.Map;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Tests
{
	[TestFixture]
	public class MapTest
	{
		[Test]
		public void Initalize_MapInstance()
		{
            MockRepository mocks = new MockRepository();

            SharpMap.Map.Map map = new SharpMap.Map.Map();
            MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

            IMapView2D mapView = mocks.CreateMock<IMapView2D>();

            mapView.BeginAction += null;
            LastCall.IgnoreArguments();
            IEventRaiser beginAction = LastCall.GetEventRaiser();

            mapView.EndAction += null;
            LastCall.IgnoreArguments();
            IEventRaiser endAction = LastCall.GetEventRaiser();

            mapView.Hover += null;
            LastCall.IgnoreArguments();
            IEventRaiser hoverAction = LastCall.GetEventRaiser();

            mapView.MoveTo += null;
            LastCall.IgnoreArguments();
            IEventRaiser moveToAction = LastCall.GetEventRaiser();

            Expect.On(mapView).Call(mapView.ViewPort).Repeat.Twice().Return(viewPort);

            mocks.ReplayAll();

            MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(100, 100);

			Assert.IsNotNull(map);
			Assert.IsNotNull(map.Layers);
            Assert.AreEqual(100f, viewPort.ViewSize.Width);
            Assert.AreEqual(100f, viewPort.ViewSize.Height);
            Assert.AreEqual(StyleColor.Transparent, viewPort.BackColor);
            Assert.AreEqual(double.MaxValue, viewPort.MaximumZoom);
            Assert.AreEqual(0, viewPort.MinimumZoom);
            Assert.AreEqual(new Point(0, 0), map.Center, "map.Center should be initialized to (0,0)");
            Assert.AreEqual(new Point(0, 0), mapView.ViewPort.GeoCenter, "mapView.ViewPort.GeoCenter should be initialized to (0,0)");
			Assert.AreEqual(1, mapView.ViewPort.Zoom, "Map zoom should be initialized to 1.0");
		}

		//[Test]
		//public void ImageToWorld()
		//{
		//    SharpMap.Map.Map map = new SharpMap.Map.Map(new System.Drawing.Size(1000, 500));
		//    map.Zoom = 360;
		//    map.Center = new SharpMap.Geometries.Point(0, 0);
		//    Assert.AreEqual(new SharpMap.Geometries.Point(0, 0), map.ImageToWorld(new System.Drawing.PointF(500, 250)));
		//    Assert.AreEqual(new SharpMap.Geometries.Point(-180, 90), map.ImageToWorld(new System.Drawing.PointF(0, 0)));
		//    Assert.AreEqual(new SharpMap.Geometries.Point(-180, -90), map.ImageToWorld(new System.Drawing.PointF(0, 500)));
		//    Assert.AreEqual(new SharpMap.Geometries.Point(180, 90), map.ImageToWorld(new System.Drawing.PointF(1000, 0)));
		//    Assert.AreEqual(new SharpMap.Geometries.Point(180, -90), map.ImageToWorld(new System.Drawing.PointF(1000, 500)));
		//}

		//[Test]
		//public void WorldToImage()
		//{
		//    SharpMap.Map.Map map = new SharpMap.Map.Map(new System.Drawing.Size(1000, 500));
		//    map.Zoom = 360;
		//    map.Center = new SharpMap.Geometries.Point(0, 0);
		//    Assert.AreEqual(new System.Drawing.PointF(500, 250), map.WorldToImage(new SharpMap.Geometries.Point(0, 0)));
		//    Assert.AreEqual(new System.Drawing.PointF(0, 0), map.WorldToImage(new SharpMap.Geometries.Point(-180, 90)));
		//    Assert.AreEqual(new System.Drawing.PointF(0, 500), map.WorldToImage(new SharpMap.Geometries.Point(-180, -90)));
		//    Assert.AreEqual(new System.Drawing.PointF(1000, 0), map.WorldToImage(new SharpMap.Geometries.Point(180, 90)));
		//    Assert.AreEqual(new System.Drawing.PointF(1000, 500), map.WorldToImage(new SharpMap.Geometries.Point(180, -90)));
		//}

		//[Test]
		//[ExpectedException(typeof(InvalidOperationException))]
		//public void GetMap_RenderEmptyMap_ThrowInvalidOperationException()
		//{
		//    SharpMap.Map.Map map = new SharpMap.Map.Map(new System.Drawing.Size(2, 1));
		//    map.GetMap();
		//}

		[Test]
		public void GetLayerByName_ReturnCorrectLayer()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 1"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 3"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 2"));

			SharpMap.Layers.ILayer layer = map.GetLayerByName("Layer 2");
			Assert.IsNotNull(layer);
			Assert.AreEqual("Layer 2", layer.LayerName);
		}

		[Test]
		public void GetLayerByName()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 1"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 3"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 2"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 3"));

			ILayer layer = map.Layers["Layer 3"];
			Assert.AreEqual("Layer 3", layer.LayerName);
		}

		[Test]
		public void FindLayerByPredicate()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 1"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 3"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 2"));
			map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 3"));

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
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetExtents_EmptyMap_ThrowInvalidOperationException()
		{
			MockRepository mocks = new MockRepository();

            SharpMap.Map.Map map = new SharpMap.Map.Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(100, 100);

			mapView.ViewPort.ZoomToExtents();
		}

		[Test]
		public void GetExtents_ValidDatasource()
		{
			MockRepository mocks = new MockRepository();

            SharpMap.Map.Map map = new SharpMap.Map.Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(100, 100);

			SharpMap.Layers.VectorLayer vLayer = new SharpMap.Layers.VectorLayer("Geom layer", CreateDatasource());

			map.Layers.Add(vLayer);
			SharpMap.Geometries.BoundingBox box = map.GetExtents();
			Assert.AreEqual(new SharpMap.Geometries.BoundingBox(0, 0, 50, 346.3493254), box);
		}

		[Test]
		public void GetPixelSize_FixedZoom_Return8_75()
		{
			MockRepository mocks = new MockRepository();

            SharpMap.Map.Map map = new SharpMap.Map.Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(200, 400);

			mapView.ViewPort.Zoom = 3500;
			Assert.AreEqual(8.75, mapView.ViewPort.WorldUnitsPerPixel);
		}

		[Test]
		public void GetMapHeight_FixedZoom_Return1750()
		{
			MockRepository mocks = new MockRepository();

            SharpMap.Map.Map map = new SharpMap.Map.Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(200, 400);

			mapView.ViewPort.Zoom = 3500;
			Assert.AreEqual(1750, mapView.ViewPort.WorldHeight);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetMinimumZoom_NegativeValue_ThrowException()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MinimumZoom = -1;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetMaximumZoom_NegativeValue_ThrowException()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MaximumZoom = -1;
		}

		[Test]
		public void SetMaximumZoom_OKValue()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MaximumZoom = 100.3;
			Assert.AreEqual(100.3, viewPort.MaximumZoom);
		}

		[Test]
		public void SetMinimumZoom_OKValue()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MinimumZoom = 100.3;
			Assert.AreEqual(100.3, viewPort.MinimumZoom);
		}

		[Test]
		public void SetZoom_ValueOutsideMax()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MaximumZoom = 100;
			viewPort.Zoom = 150;
			Assert.AreEqual(100, viewPort.MaximumZoom);
		}

		[Test]
		public void SetZoom_ValueBelowMin()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);

			viewPort.MaximumZoom = 100;
			viewPort.Zoom = 50;
			Assert.AreEqual(100, viewPort.MinimumZoom);
		}

		[Test]
		public void ZoomToBox_NoAspectCorrection()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(400, 200);

			viewPort.ZoomToBox(new SharpMap.Geometries.BoundingBox(20, 50, 100, 80));
			Assert.AreEqual(new SharpMap.Geometries.Point(60, 65), map.Center);
			Assert.AreEqual(80, viewPort.Zoom);
		}

		[Test]
		public void ZoomToBox_WithAspectCorrection()
		{
            SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(400, 200);

			viewPort.ZoomToBox(new SharpMap.Geometries.BoundingBox(20, 10, 100, 180));
			Assert.AreEqual(new SharpMap.Geometries.Point(60, 95), viewPort.GeoCenter);
			Assert.AreEqual(340, viewPort.Zoom);
		}

		//[Test]
		//[ExpectedException(typeof(ApplicationException))]
		//public void GetMap_RenderLayerWithoutDatasource_ThrowException()
		//{
		//    Map map = new SharpMap.Map.Map();
		//    map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 1"));
		//    map.GetMap();
		//}

		[Test]
		public void WorldToMap_DefaultMap_ReturnValue()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(500, 200);

			viewPort.GeoCenter = new SharpMap.Geometries.Point(23, 34);
			viewPort.Zoom = 1000;

			//System.Drawing.PointF p = map.WorldToImage(new SharpMap.Geometries.Point(8, 50));
			//Assert.AreEqual(new System.Drawing.PointF(242.5f, 92), p);
		}

		[Test]
		public void ImageToWorld_DefaultMap_ReturnValue()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(500, 200);

			viewPort.GeoCenter = new SharpMap.Geometries.Point(23, 34);
			viewPort.Zoom = 1000;
			
			//SharpMap.Geometries.Point p = map.ImageToWorld(new System.Drawing.PointF(242.5f, 92));
			//Assert.AreEqual(new SharpMap.Geometries.Point(8, 50), p);
		}

		[Test]
		public void GetMap_GeometryProvider_ReturnImage()
		{
			SharpMap.Map.Map map = new SharpMap.Map.Map();
			MapViewPort2D viewPort = new MapViewPort2D(map, ScreenHelper.Dpi);
			viewPort.ViewSize = new SharpMap.Rendering.ViewSize2D(400, 200);

			SharpMap.Layers.VectorLayer vLayer = new SharpMap.Layers.VectorLayer("Geom layer", CreateDatasource());
			vLayer.Style.Outline = new StylePen(StyleColor.Red, 2f);
			vLayer.Style.EnableOutline = true;
			vLayer.Style.Line = new StylePen(StyleColor.Green, 2f);
			vLayer.Style.Fill = new SolidStyleBrush(StyleColor.Yellow);
			map.Layers.Add(vLayer);

			SharpMap.Layers.VectorLayer vLayer2 = new SharpMap.Layers.VectorLayer("Geom layer 2", vLayer.DataSource);
			vLayer2.Style.Symbol.Offset = new ViewPoint2D(3, 4);
			vLayer2.Style.Symbol.Rotation = 45;
			vLayer2.Style.Symbol.Scale = 0.4f;
			map.Layers.Add(vLayer2);

			SharpMap.Layers.VectorLayer vLayer3 = new SharpMap.Layers.VectorLayer("Geom layer 3", vLayer.DataSource);
			vLayer3.Style.Symbol.Offset = new ViewPoint2D(3, 4);
			vLayer3.Style.Symbol.Rotation = 45;
			map.Layers.Add(vLayer3);

			SharpMap.Layers.VectorLayer vLayer4 = new SharpMap.Layers.VectorLayer("Geom layer 4", vLayer.DataSource);
			vLayer4.Style.Symbol.Offset = new ViewPoint2D(3, 4);
			vLayer4.Style.Symbol.Scale = 0.4f;
			map.Layers.Add(vLayer4);

			viewPort.ZoomToExtents();

			//System.Drawing.Image img = map.GetMap();
			//Assert.IsNotNull(img);
			map.Dispose();
			//img.Dispose();
		}

		private SharpMap.Data.Providers.IProvider CreateDatasource()
		{
			Collection<SharpMap.Geometries.Geometry> geoms = new Collection<SharpMap.Geometries.Geometry>();
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("POINT EMPTY"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("POINT (20.564 346.3493254)"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTIPOINT (20.564 346.3493254, 45 32, 23 54)"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTIPOLYGON EMPTY"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTILINESTRING EMPTY"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("MULTIPOINT EMPTY"));
			geoms.Add(SharpMap.Geometries.Geometry.GeomFromText("LINESTRING EMPTY"));
			return new SharpMap.Data.Providers.GeometryProvider(geoms);
		}
	}

	public static class ScreenHelper
	{
		public static readonly int Dpi;

		static ScreenHelper()
		{
            IntPtr dc = IntPtr.Zero;

            try
            {
                dc = CreateDC("DISPLAY", null, null, IntPtr.Zero);

                int dpi = GetDeviceCaps(dc, LOGPIXELSX);

                if (dpi == 0)
                {
                    // Can't query the DPI - just guess the most likely
                    Dpi = 96;
                }
                else
                {
                    Dpi = dpi;
                }
            }
            finally
            {
                if (dc != IntPtr.Zero)
                {
                    DeleteDC(dc);
                }
            }
		}

		[DllImport("Gdi32")]
		private static extern IntPtr CreateDC(string driver, string device, string output, IntPtr initData);

		[DllImport("Gdi32")]
		private static extern void DeleteDC(IntPtr dc);

        [DllImport("Gdi32")]
        static extern int GetDeviceCaps(IntPtr hdc, int index);

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
	}
}