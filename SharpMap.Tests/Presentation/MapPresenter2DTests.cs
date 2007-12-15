using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;

namespace SharpMap.Tests.Presentation
{
    [TestFixture]
    public class MapPresenter2DTests
    {
        #region Manual fakes

        #region ViewEvents

        private class ViewEvents
        {
            public IMapView2D View;
            public IEventRaiser Hover;
            public IEventRaiser Begin;
            public IEventRaiser MoveTo;
            public IEventRaiser End;
        }

        #endregion

        #region TestTextRenderer2D
        private class TestTextRenderer2D : TextRenderer2D<object>
        {
            public override IEnumerable<object> RenderText(String text, StyleFont font, Rectangle2D layoutRectangle, Path2D flowPath, StyleBrush fontBrush, Matrix2D transform)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override Size2D MeasureString(String text, StyleFont font)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
        #endregion

        #region TestVectorRenderer2D

        private class TestVectorRenderer2D : VectorRenderer2D<object>
        {
            public override IEnumerable<object> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                            StylePen highlightOutline, StylePen selectOutline,
                                                            RenderState renderState)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<object> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                            StyleBrush highlightFill, StyleBrush selectFill,
                                                            StylePen outline,
                                                            StylePen highlightOutline, StylePen selectOutline,
                                                            RenderState renderState)
            {
                yield break;
            }

            public override IEnumerable<object> RenderSymbols(IEnumerable<Point2D> locatiosn, Symbol2D symbolData,
                                                              RenderState renderState)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<object> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                              ColorMatrix highlight, ColorMatrix select,
                                                              RenderState renderState)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<object> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                              Symbol2D highlightSymbolData, Symbol2D selectSymbolData,
                                                              RenderState renderState)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<object> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                            StylePen highlightLine, StylePen selectLine,
                                                            StylePen outline, StylePen highlightOutline,
                                                            StylePen selectOutline, RenderState renderState)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region TestPresenter2D

        private class TestPresenter2D : MapPresenter2D
        {
            public TestPresenter2D(Map map, IMapView2D mapView)
                : base(map, mapView)
            {
            }

            internal TestView2D TestView
            {
                get { return View as TestView2D; }
            }

            protected override IVectorRenderer2D CreateVectorRenderer()
            {
                return new TestVectorRenderer2D();
            }

            protected override IRasterRenderer2D CreateRasterRenderer()
            {
                return null;
            }

            protected override ITextRenderer2D CreateTextRenderer()
            {
                return new TestTextRenderer2D();
            }

            protected override Type GetRenderObjectType()
            {
                return typeof(object);
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

            internal Double MaximumWorldWidth
            {
                get { return MaximumWorldWidthInternal; }
                set { MaximumWorldWidthInternal = value; }
            }

            internal Double MinimumWorldWidth
            {
                get { return MinimumWorldWidthInternal; }
                set { MinimumWorldWidthInternal = value; }
            }

            internal Double PixelWorldWidth
            {
                get { return PixelWorldWidthInternal; }
            }

            internal Double PixelWorldHeight
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

            internal Double WorldAspectRatio
            {
                get { return WorldAspectRatioInternal; }
                set { WorldAspectRatioInternal = value; }
            }

            internal Double WorldHeight
            {
                get { return WorldHeightInternal; }
            }

            internal Double WorldWidth
            {
                get { return WorldWidthInternal; }
            }

            internal Double WorldUnitsPerPixel
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

            internal void ZoomToWorldWidth(Double newWorldWidth)
            {
                ZoomToWorldWidthInternal(newWorldWidth);
            }

            internal Point2D ToView(Point point)
            {
                return ToViewInternal(point);
            }

            internal Point2D ToView(Double x, Double y)
            {
                return ToViewInternal(x, y);
            }

            internal Point ToWorld(Point2D point)
            {
                return ToWorldInternal(point);
            }

            internal Point ToWorld(Double x, Double y)
            {
                return ToWorldInternal(x, y);
            }

            #endregion

            protected override void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor)
            {
            }

            protected override void SetViewGeoCenter(Point fromGeoPoint, Point toGeoPoint)
            {
            }

            protected override void SetViewMaximumWorldWidth(Double fromMaxWidth, Double toMaxWidth)
            {
            }

            protected override void SetViewMinimumWorldWidth(Double fromMinWidth, Double toMinWidth)
            {
            }

            protected override void SetViewEnvelope(BoundingBox fromEnvelope, BoundingBox toEnvelope)
            {
            }

            protected override void SetViewWorldAspectRatio(Double fromRatio, Double toRatio)
            {
            }
        }

        #endregion

        #region TestView2D

        private class TestView2D : IMapView2D
        {
            private readonly TestPresenter2D _presenter;
            internal Rectangle2D _bounds = new Rectangle2D(0, 0, 1000, 1000);

            public TestView2D(Map map)
            {
                _presenter = new TestPresenter2D(map, this);
            }

            public TestPresenter2D Presenter
            {
                get { return _presenter; }
            }

            public void RaiseBegin(Point2D point)
            {
                OnBeginAction(point);
            }

            public void RaiseEnd(Point2D point)
            {
                OnEndAction(point);
            }

            public void RaiseHover(Point2D point)
            {
                OnHover(point);
            }

            public void RaiseMoveTo(Point2D point)
            {
                OnMoveTo(point);
            }

            #region IMapView2D Members

            #region Events

            public event EventHandler<MapActionEventArgs<Point2D>> Hover;
            public event EventHandler<MapActionEventArgs<Point2D>> BeginAction;
            public event EventHandler<MapActionEventArgs<Point2D>> MoveTo;
            public event EventHandler<MapActionEventArgs<Point2D>> EndAction;
            public event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Point>> GeoCenterChangeRequested;
            public event EventHandler<LocationEventArgs> IdentifyLocationRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Double>> MaximumWorldWidthChangeRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Double>> MinimumWorldWidthChangeRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
            public event EventHandler SizeChanged;
            public event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ViewEnvelopeChangeRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Double>> WorldAspectRatioChangeRequested;
            public event EventHandler ZoomToExtentsRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ZoomToWorldBoundsRequested;
            public event EventHandler<MapViewPropertyChangeEventArgs<Double>> ZoomToWorldWidthRequested;

            #endregion

            #region Properties

            public StyleColor BackgroundColor
            {
                get { return _presenter.BackgroundColor; }
                set { OnRequestBackgroundColorChange(BackgroundColor, value); }
            }

            public Double Dpi
            {
                get { return ScreenHelper.Dpi; }
            }

            public Point GeoCenter
            {
                get { return _presenter.GeoCenter; }
                set { OnRequestGeoCenterChange(GeoCenter, value); }
            }

            public Double MaximumWorldWidth
            {
                get { return _presenter.MaximumWorldWidth; }
                set { OnRequestMaximumWorldWidthChange(MaximumWorldWidth, value); }
            }

            public Double MinimumWorldWidth
            {
                get { return _presenter.MinimumWorldWidth; }
                set { OnRequestMinimumWorldWidthChange(MinimumWorldWidth, value); }
            }

            public Double PixelWorldWidth
            {
                get { return _presenter.PixelWorldWidth; }
            }

            public Double PixelWorldHeight
            {
                get { return _presenter.PixelWorldHeight; }
            }

            public ViewSelection2D Selection
            {
                get { return _presenter.Selection; }
            }

            public Matrix2D ToViewTransform
            {
                get { return _presenter.ToViewTransform; }
            }

            public Matrix2D ToWorldTransform
            {
                get { return _presenter.ToWorldTransform; }
            }

            public Point2D ToView(Point point)
            {
                return _presenter.ToView(point);
            }

            public Point2D ToView(Double x, Double y)
            {
                return _presenter.ToView(x, y);
            }

            public Point ToWorld(Point2D point)
            {
                return _presenter.ToWorld(point);
            }

            public Point ToWorld(Double x, Double y)
            {
                return _presenter.ToWorld(x, y);
            }

            public BoundingBox ViewEnvelope
            {
                get { return _presenter.ViewEnvelope; }
                set
                {
                    OnRequestViewEnvelopeChange(ViewEnvelope, value);
                }
            }

            public Size2D ViewSize
            {
                get { return _bounds.Size; }
                set
                {
                    _bounds = new Rectangle2D(_bounds.Location, value);
                    OnViewSizeChanged(value);
                }
            }

            public Double WorldAspectRatio
            {
                get { return _presenter.WorldAspectRatio; }
                set { OnRequestWorldAspectRatioChange(WorldAspectRatio, value); }
            }

            public Double WorldHeight
            {
                get { return _presenter.WorldHeight; }
            }

            public Double WorldWidth
            {
                get { return _presenter.WorldWidth; }
            }

            public Double WorldUnitsPerPixel
            {
                get { return _presenter.WorldUnitsPerPixel; }
            }

            #endregion

            #region Methods

            public void IdentifyLocation(Point worldPoint)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Offset(Point2D offsetVector)
            {
                Point2D viewCenter = _bounds.Center;
                Point offsetGeoCenter = ToWorld(viewCenter + offsetVector);
                OnRequestGeoCenterChange(GeoCenter, offsetGeoCenter);
            }

            public void ShowRenderedObjects(IEnumerable renderedObjects)
            {
            }

            public void ZoomToExtents()
            {
                OnRequestZoomToExtents();
            }

            public void ZoomToViewBounds(Rectangle2D viewBounds)
            {
                OnRequestZoomToViewBounds(viewBounds);
            }

            public void ZoomToWorldBounds(BoundingBox zoomBox)
            {
                OnRequestZoomToWorldBounds(zoomBox);
            }

            public void ZoomToWorldWidth(Double newWorldWidth)
            {
                OnRequestZoomToWorldWidth(newWorldWidth);
            }

            #endregion

            #endregion

            #region IView Members

            public Boolean Visible
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Boolean Enabled
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public void Hide()
            {
                throw new NotImplementedException();
            }

            public void Show()
            {
                throw new NotImplementedException();
            }

            public String Title
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            #endregion

            protected virtual void OnViewSizeChanged(Size2D sizeRequested)
            {
                EventHandler e = SizeChanged;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }

            protected virtual void OnHover(Point2D actionLocation)
            {
                EventHandler<MapActionEventArgs<Point2D>> @event = Hover;

                if (@event != null)
                {
                    MapActionEventArgs<Point2D> args = new MapActionEventArgs<Point2D>(actionLocation);
                    @event(this, args);
                }
            }

            protected virtual void OnBeginAction(Point2D actionLocation)
            {
                EventHandler<MapActionEventArgs<Point2D>> @event = BeginAction;

                if (@event != null)
                {
                    MapActionEventArgs<Point2D> args = new MapActionEventArgs<Point2D>(actionLocation);
                    @event(this, args);
                }
            }

            protected virtual void OnMoveTo(Point2D actionLocation)
            {
                EventHandler<MapActionEventArgs<Point2D>> @event = MoveTo;

                if (@event != null)
                {
                    MapActionEventArgs<Point2D> args = new MapActionEventArgs<Point2D>(actionLocation);
                    @event(this, args);
                }
            }

            protected virtual void OnEndAction(Point2D actionLocation)
            {
                EventHandler<MapActionEventArgs<Point2D>> @event = EndAction;

                if (@event != null)
                {
                    MapActionEventArgs<Point2D> args = new MapActionEventArgs<Point2D>(actionLocation);
                    @event(this, args);
                }
            }

            protected virtual void OnRequestBackgroundColorChange(StyleColor current, StyleColor requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> e = BackgroundColorChangeRequested;

                if (e != null)
                {
                    e(this, new MapViewPropertyChangeEventArgs<StyleColor>(current, requested));
                }
            }

            protected virtual void OnRequestGeoCenterChange(Point current, Point requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Point>> e = GeoCenterChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Point> args =
                        new MapViewPropertyChangeEventArgs<Point>(current, requested);

                    e(this, args);
                }
            }

            private void OnRequestMaximumWorldWidthChange(Double current, Double requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Double>> e = MaximumWorldWidthChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Double> args =
                        new MapViewPropertyChangeEventArgs<Double>(current, requested);

                    e(this, args);
                }
            }

            private void OnRequestMinimumWorldWidthChange(Double current, Double requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Double>> e = MinimumWorldWidthChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Double> args =
                        new MapViewPropertyChangeEventArgs<Double>(current, requested);

                    e(this, args);
                }
            }

            private void OnRequestViewEnvelopeChange(BoundingBox current, BoundingBox requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ViewEnvelopeChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<BoundingBox> args =
                        new MapViewPropertyChangeEventArgs<BoundingBox>(current, requested);

                    e(this, args);
                }
            }

            private void OnRequestWorldAspectRatioChange(Double current, Double requested)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Double>> e = WorldAspectRatioChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Double> args =
                        new MapViewPropertyChangeEventArgs<Double>(current, requested);

                    e(this, args);
                }
            }

            private void OnRequestOffset(Point2D offset)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Point2D>> e = OffsetChangeRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Point2D> args =
                        new MapViewPropertyChangeEventArgs<Point2D>(Point2D.Zero, offset);

                    e(this, args);
                }
            }

            private void OnRequestZoomToExtents()
            {
                EventHandler e = ZoomToExtentsRequested;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }

            private void OnRequestZoomToViewBounds(Rectangle2D viewBounds)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> e = ZoomToViewBoundsRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Rectangle2D> args =
                        new MapViewPropertyChangeEventArgs<Rectangle2D>(_bounds, viewBounds);

                    e(this, args);
                }
            }

            private void OnRequestZoomToWorldBounds(BoundingBox zoomBox)
            {
                EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ZoomToWorldBoundsRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<BoundingBox> args =
                        new MapViewPropertyChangeEventArgs<BoundingBox>(ViewEnvelope, zoomBox);

                    e(this, args);
                }
            }

            private void OnRequestZoomToWorldWidth(Double newWorldWidth)
            {
                EventHandler<MapViewPropertyChangeEventArgs<Double>> e = ZoomToWorldWidthRequested;

                if (e != null)
                {
                    MapViewPropertyChangeEventArgs<Double> args =
                        new MapViewPropertyChangeEventArgs<Double>(WorldWidth, newWorldWidth);

                    e(this, args);
                }
            }
        }

        #endregion

        #endregion

        [Test]
        public void InitalizingMapPresenterWithEmptyMapHasUndefinedView()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            IMapView2D mapView = mocks.Stub<IMapView2D>();

            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(200, 400);

            mocks.ReplayAll();

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);
            Assert.AreEqual(Point.Empty, mapPresenter.GeoCenter);
            Assert.AreEqual(0, mapPresenter.WorldWidth);
            Assert.AreEqual(0, mapPresenter.WorldHeight);
            Assert.AreEqual(0, mapPresenter.WorldUnitsPerPixel);
            Assert.AreEqual(1, mapPresenter.WorldAspectRatio);
            Assert.AreEqual(0, mapPresenter.PixelWorldWidth);
            Assert.AreEqual(0, mapPresenter.PixelWorldHeight);
            Assert.AreEqual(null, mapPresenter.ToViewTransform);
            Assert.AreEqual(null, mapPresenter.ToWorldTransform);
            Assert.AreEqual(Point2D.Empty, mapPresenter.ToView(new Point(50, 50)));
            Assert.AreEqual(Point.Empty, mapPresenter.ToWorld(new Point2D(100, 100)));
        }

        [Test]
        public void InitalizingMapPresenterWithNonEmptyMapHasUndefinedView()
        {
            MockRepository mocks = new MockRepository();

            Map map = new Map();
            map.AddLayer(DataSourceHelper.CreateFeatureFeatureLayer());

            IMapView2D mapView = mocks.Stub<IMapView2D>();

            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(1000, 1000);

            mocks.ReplayAll();

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);

            Assert.AreEqual(map.Center, mapPresenter.GeoCenter);
            Assert.AreEqual(0, mapPresenter.WorldWidth);
            Assert.AreEqual(0, mapPresenter.WorldHeight);
            Assert.AreEqual(0, mapPresenter.WorldUnitsPerPixel);
            Assert.AreEqual(1, mapPresenter.WorldAspectRatio);
            Assert.AreEqual(0, mapPresenter.PixelWorldWidth);
            Assert.AreEqual(0, mapPresenter.PixelWorldHeight);
            Assert.AreEqual(null, mapPresenter.ToViewTransform);
            Assert.AreEqual(null, mapPresenter.ToWorldTransform);
            Assert.AreEqual(Point2D.Empty, mapPresenter.ToView(new Point(50, 50)));
            Assert.AreEqual(Point.Empty, mapPresenter.ToWorld(new Point2D(100, 100)));
        }

        [Test]
        public void DisposingPresenterMakesItDisposed()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(1000, 1000, out view);
            Assert.AreEqual(false, mapPresenter.IsDisposed);
            mapPresenter.Dispose();
            Assert.AreEqual(true, mapPresenter.IsDisposed);
        }

        [Test]
        public void DisposingPresenterRaisesDisposedEvent()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(1000, 1000, out view);
            Boolean disposedCalled = false;
            mapPresenter.Disposed += delegate { disposedCalled = true; };
            mapPresenter.Dispose();
            Assert.AreEqual(true, disposedCalled);
        }

        [Test]
        public void ViewEnvelopeMeasuresCorrectly()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(120, 100, out view);

            mapPresenter.ZoomToExtents();
            
            BoundingBox expected = new BoundingBox(0, 0, 120, 100);
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);
            Assert.AreEqual(1, mapPresenter.WorldUnitsPerPixel);
            Assert.AreEqual(120, mapPresenter.WorldWidth);
            Assert.AreEqual(100, mapPresenter.WorldHeight);
            Assert.AreEqual(expected, mapPresenter.ViewEnvelope);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PanWhenNoWorldBoundsThrowsException()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(400, 500, out view);

            Map map = mapPresenter.Map;
            map.ActiveTool = StandardMapTools2D.Pan;

            view.RaiseBegin(new Point2D(200, 250));
            view.RaiseMoveTo(new Point2D(250, 250));
            view.RaiseEnd(new Point2D(250, 250));
        }

        [Test]
        public void PanTest()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(400, 800, out view);

            mapPresenter.ZoomToExtents();


            Map map = mapPresenter.Map;
            map.ActiveTool = StandardMapTools2D.Pan;

            /*
             * 
             * |
             * |     1 2
             * |     * *
             * |     ->
             * |____________
             * 
             */

            view.RaiseBegin(new Point2D(0, 0));
            view.RaiseMoveTo(new Point2D(0, 0));
            view.RaiseEnd(new Point2D(0, 0));

            Assert.AreEqual(new Point(62.5, 50), mapPresenter.GeoCenter);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ZoomWhenNoWorldBoundsThrowsException()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(400, 500, out view);

            Map map = mapPresenter.Map;

            map.ActiveTool = StandardMapTools2D.ZoomIn;

            view.RaiseBegin(new Point2D(100, 125));
            view.RaiseMoveTo(new Point2D(300, 375));
            view.RaiseEnd(new Point2D(300, 375));
        }

        private void nullAction(ActionContext<IMapView2D, Point2D> context) { }

        private void testBeginZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;
            view.Selection.Clear();
            view.Selection.AddPoint(context.CurrentPoint);
        }

        private void testContinueZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            context.MapView.Selection.AddPoint(context.CurrentPoint);
        }

        private void testEndZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;

            if (view.Selection.Path.Points.Count == 1)
            {
				zoomByFactor(view, context.CurrentPoint, 1.2);	// 0.83333333333333337
            }
            else
            {
                view.Selection.Close();
                context.MapView.ZoomToViewBounds(view.Selection.Path.Bounds);
            }
        }

		private static void zoomByFactor(IMapView2D view, Point2D zoomCenter, Double zoomFactor)
        {
            zoomFactor = 1 / zoomFactor;

            Size2D viewSize = view.ViewSize;
            Point2D viewCenter = new Point2D((viewSize.Width / 2), (viewSize.Height / 2));
            Point2D viewDifference = zoomCenter - viewCenter;

			Size2D zoomBoundsSize = new Size2D(viewSize.Width * zoomFactor, viewSize.Height * zoomFactor);
			Double widthDifference = zoomBoundsSize.Width - viewSize.Width;
			Double heightDifference = zoomBoundsSize.Height - viewSize.Height;
			Point2D zoomUpperLeft = new Point2D(viewDifference.X * zoomFactor - widthDifference / 2, 
				viewDifference.Y * zoomFactor - heightDifference / 2);
            Rectangle2D zoomViewBounds = new Rectangle2D(zoomUpperLeft, zoomBoundsSize);

            view.ZoomToViewBounds(zoomViewBounds);
        }

        [Test]
        public void ZoomingInFromViewCalculatesCorrectViewMetrics()
        {
            TestView2D view;
            TestPresenter2D mapPresenter = createPresenter(400, 500, out view);

            mapPresenter.ZoomToExtents();

            Assert.AreEqual(120, mapPresenter.WorldWidth);
            Assert.AreEqual(150, mapPresenter.WorldHeight);
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);

            Map map = mapPresenter.Map;

            map.ActiveTool = new MapTool<IMapView2D, Point2D>("TestZoomIn", nullAction, testBeginZoomIn, testContinueZoomIn, testEndZoomIn);

            // Zoom using click
            view.RaiseBegin(new Point2D(200, 250));
            view.RaiseMoveTo(new Point2D(200, 250));
            view.RaiseEnd(new Point2D(200, 250));

            Assert.AreEqual(100, mapPresenter.WorldWidth);
            Assert.AreEqual(125, mapPresenter.WorldHeight);
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);

            mapPresenter.ZoomToExtents();

            Assert.AreEqual(120, mapPresenter.WorldWidth);
            Assert.AreEqual(150, mapPresenter.WorldHeight);
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);

            // Zoom using selection
            view.RaiseBegin(new Point2D(100, 125));
            view.RaiseMoveTo(new Point2D(300, 375));
            view.RaiseEnd(new Point2D(300, 375));

            Assert.AreEqual(60, mapPresenter.WorldWidth);
            Assert.AreEqual(75, mapPresenter.WorldHeight);
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);
        }

        [Test]
        public void QueryTest()
        {
        }

        [Test]
        public void AddFeatureTest()
        {
        }

        [Test]
        public void RemoveFeatureTest()
        {
        }

        [Test]
        public void ZoomingToExtentsCentersMap()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 500);
            mapPresenter.ZoomToExtents();

            Map map = mapPresenter.Map;

            Assert.AreEqual(map.Center, mapPresenter.GeoCenter);
        }

        [Test]
        public void GetPixelSize_FixedZoom_Return8_75()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 500);

            mapPresenter.ZoomToWorldWidth(3500);
            Assert.AreEqual(8.75, mapPresenter.WorldUnitsPerPixel, TestConstants.Epsilon);
        }

        [Test]
        public void GetWorldHeight_FixedZoom_Return1750()
        {
            MockRepository mocks = new MockRepository();

            // create a presenter with a view having Width = 400 and Height = 200
            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.ZoomToWorldWidth(3500);
            Assert.AreEqual(1750, mapPresenter.WorldHeight, TestConstants.Epsilon);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetMinimumZoom_NegativeValue_ThrowException()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);

            mapPresenter.MinimumWorldWidth = -1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void ZoomToViewBoundsWhenNoWorldBoundsSetThrowsException()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);
            mapPresenter.ZoomToViewBounds(new Rectangle2D(300, 300, 900, 900));
        }

        [Test]
        public void ZoomToViewBounds_NoAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 1000, 1000);
            mapPresenter.ZoomToExtents();

            mapPresenter.ZoomToViewBounds(new Rectangle2D(300, 300, 900, 900));
            Assert.AreEqual(new Point(72, 38), mapPresenter.GeoCenter);
            Assert.AreEqual(72, mapPresenter.WorldWidth);
            Assert.AreEqual(72, mapPresenter.WorldHeight);
        }

        [Test]
        public void ZoomToViewBounds_WithAspectCorrection()
        {
            MockRepository mocks = new MockRepository();

            TestPresenter2D mapPresenter = createPresenter(mocks, 400, 200);
            // Set the aspect ratio, which is the number of height units per width unit
            // to 2 height units to 1 width unit
            mapPresenter.WorldAspectRatio = 2;
            // Zooming to extents should 
            mapPresenter.ZoomToExtents();
            Assert.AreEqual(120, mapPresenter.WorldWidth);
            Assert.AreEqual(120, mapPresenter.WorldHeight);
            // Zoom to a 200x100 rectangle in view coordinates
            mapPresenter.ZoomToViewBounds(new Rectangle2D(100, 50, 300, 150));
            Assert.AreEqual(new Point(60, 50), mapPresenter.GeoCenter);
            Assert.AreEqual(60, mapPresenter.WorldWidth);
            Assert.AreEqual(60, mapPresenter.WorldHeight);
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

            GeometryLayer vLayer = new GeometryLayer("Geom layer", DataSourceHelper.CreateGeometryDatasource());
            vLayer.Style.Outline = new StylePen(StyleColor.Red, 2f);
            vLayer.Style.EnableOutline = true;
            vLayer.Style.Line = new StylePen(StyleColor.Green, 2f);
            vLayer.Style.Fill = new SolidStyleBrush(StyleColor.Yellow);
            map.AddLayer(vLayer);

            GeometryLayer vLayer2 = new GeometryLayer("Geom layer 2", vLayer.DataSource);
            Stream data = Assembly.GetAssembly(typeof(Map))
                .GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
            vLayer2.Style.Symbol = new Symbol2D(data, new Size2D(16, 16));
            vLayer2.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer2.Style.Symbol.Rotation = 45;
            vLayer2.Style.Symbol.Scale(0.4f);
            map.AddLayer(vLayer2);

            GeometryLayer vLayer3 = new GeometryLayer("Geom layer 3", vLayer.DataSource);
            vLayer3.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer3.Style.Symbol.Rotation = 45;
            map.AddLayer(vLayer3);

            GeometryLayer vLayer4 = new GeometryLayer("Geom layer 4", vLayer.DataSource);
            vLayer4.Style.Symbol.Offset = new Point2D(3, 4);
            vLayer2.Style.Symbol.Scale(0.4f);
            map.AddLayer(vLayer4);

            mapPresenter.ZoomToExtents();

            map.Dispose();
        }

        private static TestPresenter2D createPresenter(MockRepository mocks, Double width, Double height)
        {
            Map map = new Map();
            map.AddLayer(DataSourceHelper.CreateFeatureFeatureLayer());
            //map.AddLayer(DataSourceHelper.CreateGeometryFeatureLayer());

            IMapView2D mapView = mocks.Stub<IMapView2D>();
            SetupResult.For(mapView.Dpi).Return(ScreenHelper.Dpi);
            mapView.ViewSize = new Size2D(width, height);

            TestPresenter2D mapPresenter = new TestPresenter2D(map, mapView);
            return mapPresenter;
        }

        private static TestPresenter2D createPresenter(Double width, Double height, out TestView2D view)
        {
            Map map = new Map();
            map.AddLayer(DataSourceHelper.CreateFeatureFeatureLayer());
            //map.AddLayer(DataSourceHelper.CreateGeometryFeatureLayer());

            view = new TestView2D(map);

            view.ViewSize = new Size2D(width, height);

            TestPresenter2D mapPresenter = view.Presenter;
            return mapPresenter;
        }
    }
}