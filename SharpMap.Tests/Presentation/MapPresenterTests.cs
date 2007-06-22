using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Tests.Presentation
{
	[TestFixture]
	public class MapPresenterTests
	{
		[Test]
		public void Initalize_MapPresenter()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            IMapView2D mapView = mocks.Stub<IMapView2D>();

            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);

            mapView.BeginAction += null;
            IEventRaiser beginAction = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.EndAction += null;
            IEventRaiser endAction = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.Hover += null;
            IEventRaiser hoverAction = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.MoveTo += null;
            IEventRaiser moveToAction = LastCall.IgnoreArguments().GetEventRaiser();

            mocks.ReplayAll();

            MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);
            mapView.ViewPort.ViewSize = new ViewSize2D(100, 100);

			Assert.IsNotNull(map);
			Assert.IsNotNull(map.Layers);
            Assert.AreEqual(100f, mapView.ViewPort.ViewSize.Width);
            Assert.AreEqual(100f, mapView.ViewPort.ViewSize.Height);
            Assert.AreEqual(StyleColor.Transparent, mapView.ViewPort.BackColor);
            Assert.AreEqual(double.MaxValue, mapView.ViewPort.MaximumWorldWidth);
            Assert.AreEqual(0, mapView.ViewPort.MinimumWorldWidth);
            Assert.AreEqual(new Point(0, 0), map.Center, "map.Center should be initialized to (0,0)");
            Assert.AreEqual(new Point(0, 0), mapView.ViewPort.GeoCenter, "mapView.ViewPort.GeoCenter should be initialized to (0,0)");
			Assert.AreEqual(1, mapView.ViewPort.WorldUnitsPerPixel, "World units per pixel should be initialized to 1.0");
		}

		//[Test]
		//public void ImageToWorld()
		//{
		//    Map map = new Map(new System.Drawing.Size(1000, 500));
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
		//    Map map = new Map(new System.Drawing.Size(1000, 500));
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
		//    Map map = new Map(new System.Drawing.Size(2, 1));
		//    map.GetMap();
		//}

		[Test]
		public void GetPixelSize_FixedZoom_Return8_75()
		{
			MockRepository mocks = new MockRepository();

            Map map = new Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

            MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new ViewSize2D(200, 400);

			mapView.ViewPort.ZoomToWidth(3500);
			Assert.AreEqual(8.75, mapView.ViewPort.WorldUnitsPerPixel);
		}

		[Test]
		public void GetMapHeight_FixedZoom_Return1750()
		{
			MockRepository mocks = new MockRepository();

            Map map = new Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);

            MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			Expect.On(mapView).Call(mapView.ViewPort).Return(viewPort);

			mapView.ViewPort.ViewSize = new ViewSize2D(200, 400);

			mapView.ViewPort.ZoomToWidth(3500);
			Assert.AreEqual(1750, mapView.ViewPort.WorldHeight);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetMinimumZoom_NegativeValue_ThrowException()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
            MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MinimumWorldWidth = -1;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void SetMaximumZoom_NegativeValue_ThrowException()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
            MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MaximumWorldWidth = -1;
		}

		[Test]
		public void SetMaximumZoom_OKValue()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MaximumWorldWidth = 100.3;
			Assert.AreEqual(100.3, viewPort.MaximumWorldWidth);
		}

		[Test]
		public void SetMinimumZoom_OKValue()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MinimumWorldWidth = 100.3;
			Assert.AreEqual(100.3, viewPort.MinimumWorldWidth);
		}

		[Test]
		public void SetZoom_ValueOutsideMax()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MaximumWorldWidth = 100;
			viewPort.ZoomToWidth(150);
			Assert.AreEqual(100, viewPort.MaximumWorldWidth);
		}

		[Test]
		public void SetZoom_ValueBelowMin()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);

			viewPort.MaximumWorldWidth = 100;
			viewPort.ZoomToWidth(50);
			Assert.AreEqual(100, viewPort.MinimumWorldWidth);
		}

		[Test]
		public void ZoomToBox_NoAspectCorrection()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
			IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			viewPort.ViewSize = new ViewSize2D(400, 200);

			viewPort.ZoomToBox(new SharpMap.Geometries.BoundingBox(20, 50, 100, 80));
			Assert.AreEqual(new SharpMap.Geometries.Point(60, 65), map.Center);
			Assert.AreEqual(80, viewPort.WorldWidth);
		}

		[Test]
		public void ZoomToBox_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			viewPort.ViewSize = new ViewSize2D(400, 200);

			viewPort.ZoomToBox(new SharpMap.Geometries.BoundingBox(20, 10, 100, 180));
			Assert.AreEqual(new SharpMap.Geometries.Point(60, 95), viewPort.GeoCenter);
			Assert.AreEqual(340, viewPort.WorldWidth);
		}

		//[Test]
		//[ExpectedException(typeof(ApplicationException))]
		//public void GetMap_RenderLayerWithoutDatasource_ThrowException()
		//{
		//    Map map = new Map();
		//    map.Layers.Add(new SharpMap.Layers.VectorLayer("Layer 1"));
		//    map.GetMap();
		//}

		[Test]
		public void WorldToMap_DefaultMap_ReturnValue()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			viewPort.ViewSize = new ViewSize2D(500, 200);

			viewPort.GeoCenter = new SharpMap.Geometries.Point(23, 34);
			viewPort.ZoomToWidth(1000);

            ViewPoint2D p = Transform2D.WorldToView(new SharpMap.Geometries.Point(8, 50), viewPort);
			Assert.AreEqual(new ViewPoint2D(242.5f, 92), p);
		}

		[Test]
		public void ImageToWorld_DefaultMap_ReturnValue()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			viewPort.ViewSize = new ViewSize2D(500, 200);

            viewPort.GeoCenter = new SharpMap.Geometries.Point(23, 34);
            viewPort.ZoomToWidth(1000);

            Point p = Transform2D.ViewToWorld(new ViewPoint2D(242.5f, 92), viewPort);
            Assert.AreEqual(new Point(8, 50), p);
		}

		[Test]
		public void GetMap_GeometryProvider_ReturnImage()
        {
            MockRepository mocks = new MockRepository();
            Map map = new Map();
            IMapView2D mapView = mocks.CreateMock<IMapView2D>();
			MapViewPort2D viewPort = new MapViewPort2D(map, mapView);
			viewPort.ViewSize = new ViewSize2D(400, 200);

			SharpMap.Layers.VectorLayer vLayer = new SharpMap.Layers.VectorLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());
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
	}
}
