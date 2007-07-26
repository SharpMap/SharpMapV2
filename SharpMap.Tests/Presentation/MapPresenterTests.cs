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
		private static readonly double _e = 0.0005;

		[Test]
		public void Initalize_MapPresenter()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            IMapView2D mapView = mocks.Stub<IMapView2D>();

            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(200, 400);

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

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 500);

			mapPresenter.ZoomToWidth(3500);
			Assert.AreEqual(8.75, mapPresenter.WorldUnitsPerPixel);
		}

		[Test]
		public void GetWorldHeight_FixedZoom_Return1750()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.ZoomToWidth(3500);
			Assert.AreEqual(1750, mapPresenter.WorldHeight);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SetMinimumZoom_NegativeValue_ThrowException()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MinimumWorldWidth = -1;
		}

		[Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SetMaximumZoom_NegativeValue_ThrowException()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MaximumWorldWidth = -1;
		}

		[Test]
		public void SetMaximumZoom_OKValue()
        {
			MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MaximumWorldWidth = 100.3;
			Assert.AreEqual(100.3, mapPresenter.MaximumWorldWidth);
		}

		[Test]
		public void SetMinimumZoom_OKValue()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MinimumWorldWidth = 100.3;
			Assert.AreEqual(100.3, mapPresenter.MinimumWorldWidth);
		}

		[Test]
		public void SetZoom_ValueOutsideMax()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MaximumWorldWidth = 100;
			mapPresenter.ZoomToWidth(150);
			Assert.AreEqual(100, mapPresenter.WorldWidth);
		}

		[Test]
		public void SetZoom_ValueBelowMin()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MinimumWorldWidth = 100;
			mapPresenter.ZoomToWidth(50);
			Assert.AreEqual(100, mapPresenter.WorldWidth);
		}

		[Test]
		public void ZoomToBox_NoAspectCorrection()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 400);

			mapPresenter.ZoomToBox(new BoundingBox(20, 50, 100, 80));
            Assert.AreEqual(new Point(60, 65), mapPresenter.GeoCenter);
			Assert.AreEqual(80, mapPresenter.WorldWidth);
		}

		[Test]
		public void ZoomToBox_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.ZoomToBox(new BoundingBox(20, 10, 100, 180));
			Assert.AreEqual(new Point(60, 95), mapPresenter.GeoCenter);
			Assert.AreEqual(340, mapPresenter.WorldWidth);
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
		public void WorldToViewTests()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.GeoCenter = new Point(23, 34);
			mapPresenter.ZoomToWidth(1000);

			Point2D p1 = mapPresenter.ToView(8, 50);
			Point2D p2 = mapPresenter.ToView(new Point(8, 50));
			Assert.AreEqual(new Point2D(242.5f, 92), p1);
			Assert.AreEqual(p1, p2);
		}

		[Test]
		public void ViewToWorldTests()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 500, 200);

			mapPresenter.GeoCenter = new Point(23, 34);
			mapPresenter.ZoomToWidth(1000);

			Point p = mapPresenter.ToWorld(242.5f, 92);
            Assert.AreEqual(new Point(8, 50), p);
		}

		[Test]
		public void GetMap_GeometryProvider_ReturnImage()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            Map map = mapPresenter.Map;

			VectorLayer vLayer = new VectorLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());
			vLayer.Style.Outline = new StylePen(StyleColor.Red, 2f);
			vLayer.Style.EnableOutline = true;
			vLayer.Style.Line = new StylePen(StyleColor.Green, 2f);
			vLayer.Style.Fill = new SolidStyleBrush(StyleColor.Yellow);
			map.Layers.Add(vLayer);

			VectorLayer vLayer2 = new VectorLayer("Geom layer 2", vLayer.DataSource);
			vLayer2.Style.Symbol.Offset = new Point2D(3, 4);
			vLayer2.Style.Symbol.Rotation = 45;
            vLayer2.Style.Symbol.Scale = 0.4f;
			map.Layers.Add(vLayer2);

			VectorLayer vLayer3 = new VectorLayer("Geom layer 3", vLayer.DataSource);
			vLayer3.Style.Symbol.Offset = new Point2D(3, 4);
			vLayer3.Style.Symbol.Rotation = 45;
			map.Layers.Add(vLayer3);

			VectorLayer vLayer4 = new VectorLayer("Geom layer 4", vLayer.DataSource);
            vLayer4.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer2.Style.Symbol.Scale = 0.4f;
			map.Layers.Add(vLayer4);

			mapPresenter.ZoomToExtents();

			//System.Drawing.Image img = map.GetMap();
			//Assert.IsNotNull(img);
			map.Dispose();
			//img.Dispose();
        }

        private static MapPresenter2D createPresenter(MockRepository mocks, double width, double height)
        {
            Map map = new Map();
            map.Layers.Add(DataSourceHelper.CreateVectorLayer());

            IMapView2D mapView = mocks.Stub<IMapView2D>();
            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(width, height);

            MapPresenter2D mapPresenter = new MapPresenter2D(map, mapView);
            return mapPresenter;
        }
	}
}
