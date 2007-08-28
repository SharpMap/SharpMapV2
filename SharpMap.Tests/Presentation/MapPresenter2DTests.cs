using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;

namespace SharpMap.Tests.Presentation
{
    [TestFixture]
    public class MapPresenter2DTests
    {
        private static readonly double _e = 0.0005;

        #region Manual fakes
        private class ViewEvents
        {
            public IEventRaiser Hover;
            public IEventRaiser Begin;
            public IEventRaiser MoveTo;
            public IEventRaiser End;
        }

        private class TestVectorRenderer2D : VectorRenderer2D<object>
        {
            public override object RenderPath(GraphicsPath2D path, StylePen outline,
                                              StylePen highlightOutline, StylePen selectOutline)
            {
                throw new NotImplementedException();
            }

            public override object RenderPath(GraphicsPath2D path, StyleBrush fill,
                                              StyleBrush highlightFill, StyleBrush selectFill, StylePen outline,
                                              StylePen highlightOutline, StylePen selectOutline)
            {
                throw new NotImplementedException();
            }

            public override object RenderSymbol(Point2D location, Symbol2D symbolData)
            {
                throw new NotImplementedException();
            }

            public override object RenderSymbol(Point2D location, Symbol2D symbolData,
                                                ColorMatrix highlight, ColorMatrix select)
            {
                throw new NotImplementedException();
            }

            public override object RenderSymbol(Point2D location, Symbol2D symbolData,
                                                Symbol2D highlightSymbolData, Symbol2D selectSymbolData)
            {
                throw new NotImplementedException();
            }
        }

        private class TestPresenter2D : MapPresenter2D
        {
            public TestPresenter2D(Map map, IMapView2D mapView)
                : base(map, mapView)
            {
            }

            protected override IRenderer CreateVectorRenderer()
            {
                return new TestVectorRenderer2D();
            }

            protected override IRenderer CreateRasterRenderer()
            {
                return null;
            }

            #region Test accessible members

            internal StyleColor BackgroundColor
            {
                get { return BackgroundColorInternal; }
            }

            internal Point GeoCenter
            {
                get { return GeoCenterInternal; }
                set { GeoCenterInternal = value; }
            }

            internal double MaximumWorldWidth
            {
                get { return MaximumWorldWidthInternal; }
                set { MaximumWorldWidthInternal = value; }
            }

            internal double MinimumWorldWidth
            {
                get { return MinimumWorldWidthInternal; }
                set { MinimumWorldWidthInternal = value; }
            }

            internal double PixelWorldWidth
            {
                get { return PixelWorldWidthInternal; }
            }

            internal double PixelWorldHeight
            {
                get { return PixelWorldHeightInternal; }
            }

            internal ViewSelection2D Selection
            {
                get { return SelectionInternal; }
            }

            internal Matrix2D ToViewTransform
            {
                get { return ToViewTransformInternal; }
            }

            internal Matrix2D ToWorldTransform
            {
                get { return ToWorldTransformInternal; }
            }

            internal BoundingBox ViewEnvelope
            {
                get { return ViewEnvelopeInternal; }
            }

            internal double WorldAspectRatio
            {
                get { return WorldAspectRatioInternal; }
                set { WorldAspectRatioInternal = value; }
            }

            internal double WorldHeight
            {
                get { return WorldHeightInternal; }
            }

            internal double WorldWidth
            {
                get { return WorldWidthInternal; }
            }

            internal double WorldUnitsPerPixel
            {
                get { return WorldUnitsPerPixelInternal; }
            }

            internal void ZoomToExtents()
            {
                ZoomToExtentsInternal();
            }

            internal void ZoomToViewBounds(Rectangle2D viewBounds)
            {
                ZoomToViewBoundsInternal(viewBounds);
            }

            internal void ZoomToWorldBounds(BoundingBox zoomBox)
            {
                ZoomToWorldBoundsInternal(zoomBox);
            }

            internal void ZoomToWorldWidth(double newWorldWidth)
            {
                ZoomToWorldWidthInternal(newWorldWidth);
            }

            internal new Point2D ToView(Point point)
            {
                return base.ToView(point);
            }

            internal new Point2D ToView(double x, double y)
            {
                return base.ToView(x, y);
            }

            internal new Point ToWorld(Point2D point)
            {
                return base.ToWorld(point);
            }

            internal new Point ToWorld(double x, double y)
            {
                return base.ToWorld(x, y);
            }

            #endregion

            protected override void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewGeoCenter(Point fromGeoPoint, Point toGeoPoint)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewMaximumWorldWidth(double fromMaxWidth, double toMaxWidth)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewMinimumWorldWidth(double fromMinWidth, double toMinWidth)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewEnvelope(BoundingBox fromEnvelope, BoundingBox toEnvelope)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewSize(Size2D fromSize, Size2D toSize)
            {
                throw new NotImplementedException();
            }

            protected override void SetViewWorldAspectRatio(double fromRatio, double toRatio)
            {
                throw new NotImplementedException();
            }
        } 
        #endregion

        [Test]
        public void Initalize_MapPresenter()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            IMapView2D mapView = mocks.Stub<IMapView2D>();

            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(200, 400);

            mocks.ReplayAll();

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);
        }

        [Test]
        public void PanTest()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 500);
            Map map = mapPresenter.Map;
            map.ActiveTool = StandardMapTools2D.Pan;
        }

        [Test]
        public void GetPixelSize_FixedZoom_Return8_75()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 500);

            mapPresenter.ZoomToWorldWidth(3500);
            Assert.AreEqual(8.75, mapPresenter.WorldUnitsPerPixel, _e);
        }

        [Test]
        public void GetWorldHeight_FixedZoom_Return1750()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.ZoomToWorldWidth(3500);
            Assert.AreEqual(1750, mapPresenter.WorldHeight);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void SetMinimumZoom_NegativeValue_ThrowException()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MinimumWorldWidth = -1;
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void SetMaximumZoom_NegativeValue_ThrowException()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MaximumWorldWidth = -1;
        }

        [Test]
        public void SetMaximumZoom_OKValue()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MaximumWorldWidth = 100.3;
            Assert.AreEqual(100.3, mapPresenter.MaximumWorldWidth);
        }

        [Test]
        public void SetMinimumZoom_OKValue()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MinimumWorldWidth = 100.3;
            Assert.AreEqual(100.3, mapPresenter.MinimumWorldWidth);
        }

        [Test]
        public void SetZoom_ValueOutsideMax()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MaximumWorldWidth = 100;
            mapPresenter.ZoomToWorldWidth(150);
            Assert.AreEqual(100, mapPresenter.WorldWidth);
        }

        [Test]
        public void SetZoom_ValueBelowMin()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MinimumWorldWidth = 100;
            mapPresenter.ZoomToWorldWidth(50);
            Assert.AreEqual(100, mapPresenter.WorldWidth);
        }

        [Test]
        public void ZoomToViewBounds_NoAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

            mapPresenter.ZoomToViewBounds(new Rectangle2D(300, 300, 900, 900));
            Assert.AreEqual(new Point(60, 40), mapPresenter.GeoCenter);
            Assert.AreEqual(60, mapPresenter.WorldWidth);
            Assert.AreEqual(60, mapPresenter.WorldHeight);
        }

        [Test]
        public void ZoomToViewBounds_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);
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

            TestPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

            mapPresenter.ZoomToWorldBounds(new BoundingBox(20, 50, 100, 80));
            Assert.AreEqual(new Point(60, 65), mapPresenter.GeoCenter);
            Assert.AreEqual(80, mapPresenter.WorldWidth);
        }

        [Test]
        public void ZoomToWorldBounds_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);
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

            TestPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);

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

            TestPresenter2D mapPresenter = createPresenter(mocks, 500, 200);

            mapPresenter.GeoCenter = new Point(23, 34);
            mapPresenter.ZoomToWorldWidth(1000);

            Point p1 = mapPresenter.ToWorld(242.5f, 92);
            Point p2 = mapPresenter.ToWorld(new Point2D(242.5f, 92));
            Assert.AreEqual(new Point(8, 50), p1);
            Assert.AreEqual(p1, p2);
        }

        [Test]
        [Ignore("Test not yet completed")]
        public void GetMap_GeometryProvider_ReturnImage()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            Map map = mapPresenter.Map;

            VectorLayer vLayer = new VectorLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());
            vLayer.Style.Outline = new StylePen(StyleColor.Red, 2f);
            vLayer.Style.EnableOutline = true;
            vLayer.Style.Line = new StylePen(StyleColor.Green, 2f);
            vLayer.Style.Fill = new SolidStyleBrush(StyleColor.Yellow);
            map.AddLayer(vLayer);

            VectorLayer vLayer2 = new VectorLayer("Geom layer 2", vLayer.DataSource);
            Stream data = Assembly.GetAssembly(typeof (Map))
                .GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
            vLayer2.Style.Symbol = new Symbol2D(data, new Size2D(16, 16));
            vLayer2.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer2.Style.Symbol.Rotation = 45;
            vLayer2.Style.Symbol.Scale = 0.4f;
            map.AddLayer(vLayer2);

            VectorLayer vLayer3 = new VectorLayer("Geom layer 3", vLayer.DataSource);
            vLayer3.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer3.Style.Symbol.Rotation = 45;
            map.AddLayer(vLayer3);

            VectorLayer vLayer4 = new VectorLayer("Geom layer 4", vLayer.DataSource);
            vLayer4.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer2.Style.Symbol.Scale = 0.4f;
            map.AddLayer(vLayer4);

            mapPresenter.ZoomToExtents();

            map.Dispose();
        }

        private static TestPresenter2D createPresenter(MockRepository mocks, double width, double height)
        {
            Map map = new Map();
            map.AddLayer(DataSourceHelper.CreateFeatureVectorLayer());
            //map.AddLayer(DataSourceHelper.CreateGeometryVectorLayer());

            IMapView2D mapView = mocks.Stub<IMapView2D>();
            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(width, height);

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);
            return mapPresenter;
        }

        private static TestPresenter2D createPresenter(MockRepository mocks, double width, double height, out ViewEvents events)
        {
            Map map = new Map();
            map.AddLayer(DataSourceHelper.CreateFeatureVectorLayer());
            //map.AddLayer(DataSourceHelper.CreateGeometryVectorLayer());

            IMapView2D mapView = mocks.Stub<IMapView2D>();
            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(width, height);

            events = new ViewEvents();

            mapView.Hover += null;
            events.Hover = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.BeginAction += null;
            events.Begin = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.MoveTo += null;
            events.MoveTo = LastCall.IgnoreArguments().GetEventRaiser();

            mapView.EndAction += null;
            events.End = LastCall.IgnoreArguments().GetEventRaiser();

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);
            return mapPresenter;
        }
    }
}