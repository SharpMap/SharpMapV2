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

		[Test]
		public void GetPixelSize_FixedZoom_Return8_75()
		{
			MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 500);

			mapPresenter.ZoomToWorldWidth(3500);
			Assert.AreEqual(8.75, mapPresenter.WorldUnitsPerPixel, _e);
		}

		[Test]
		public void GetWorldHeight_FixedZoom_Return1750()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.ZoomToWorldWidth(3500);
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
			mapPresenter.ZoomToWorldWidth(150);
			Assert.AreEqual(100, mapPresenter.WorldWidth);
		}

		[Test]
		public void SetZoom_ValueBelowMin()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

			mapPresenter.MinimumWorldWidth = 100;
			mapPresenter.ZoomToWorldWidth(50);
			Assert.AreEqual(100, mapPresenter.WorldWidth);
        }

        [Test]
        public void ZoomToViewBounds_NoAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

            mapPresenter.ZoomToViewBounds(new Rectangle2D(300, 300, 900, 900));
            Assert.AreEqual(new Point(60, 40), mapPresenter.GeoCenter);
            Assert.AreEqual(60, mapPresenter.WorldWidth);
            Assert.AreEqual(60, mapPresenter.WorldHeight);
        }

        [Test]
        public void ZoomToViewBounds_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);
            mapPresenter.WorldAspectRatio = 2;
            mapPresenter.ZoomToViewBounds(new Rectangle2D(100, 50, 300, 150));
            Assert.AreEqual(new Point(50, 50), mapPresenter.GeoCenter);
            Assert.AreEqual(50, mapPresenter.WorldWidth);
            Assert.AreEqual(50, mapPresenter.WorldHeight);
        }

		[Test]
        public void ZoomToWorldBounds_NoAspectCorrection()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

			mapPresenter.ZoomToWorldBounds(new BoundingBox(20, 50, 100, 80));
            Assert.AreEqual(new Point(60, 65), mapPresenter.GeoCenter);
			Assert.AreEqual(80, mapPresenter.WorldWidth);
		}

		[Test]
        public void ZoomToWorldBounds_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 400, 200);
            mapPresenter.WorldAspectRatio = 2;
			mapPresenter.ZoomToWorldBounds(new BoundingBox(20, 10, 100, 180));
			Assert.AreEqual(new Point(60, 95), mapPresenter.GeoCenter);
            Assert.AreEqual(170, mapPresenter.WorldWidth);
            Assert.AreEqual(170, mapPresenter.WorldHeight);
		}

		[Test]
		public void WorldToViewTests()
        {
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

			mapPresenter.GeoCenter = new Point(23, 34);
			mapPresenter.ZoomToWorldWidth(2500);

			Point2D p1 = mapPresenter.ToView(8, 50);
			Point2D p2 = mapPresenter.ToView(new Point(8, 50));
			Assert.AreEqual(new Point2D(494, 493.6), p1);
			Assert.AreEqual(p1, p2);
		}

		[Test]
		public void ViewToWorldTests()
		{
            MockRepository mocks = new MockRepository();

            MapPresenter2D mapPresenter = createPresenter(mocks, 500, 200);

			mapPresenter.GeoCenter = new Point(23, 34);
			mapPresenter.ZoomToWorldWidth(1000);

            Point p1 = mapPresenter.ToWorld(242.5f, 92);
            Point p2 = mapPresenter.ToWorld(new Point2D(242.5f, 92));
            Assert.AreEqual(new Point(8, 50), p1);
            Assert.AreEqual(p1, p2);
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

			map.Dispose();
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
