using System;
using System.Collections;
using System.Collections.Generic;
//using System.Windows;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Wpf;
using SharpMap.Styles;
using SharpMap.Tools;
using WpfVerticalAlignment = System.Windows.VerticalAlignment;
using WpfPoint = System.Windows.Point;
using WpfMatrix = System.Windows.Media.Matrix;
using WpfRect = System.Windows.Rect;

namespace SharpMap.Presentation.Windows
{
    public class MapViewControl : UserControl, IMapView2D
    {

        private IMapTool _selectedTool;
        private readonly Double _dpi;
        private Boolean _dragging;
        private static readonly WpfPoint Empty = new WpfPoint(Double.MinValue, Double.MinValue);
        private WpfPoint _mouseDownLocation = Empty;
        private WpfPoint _mouseRelativeLocation = Empty;
        private WpfPoint _mousePreviousLocation = Empty;
        private readonly List<WpfRenderObject> _renderObjectQueue = new List<WpfRenderObject>();
        private readonly WpfMapActionEventArgs _globalActionArgs = new WpfMapActionEventArgs();
        private MapToolSet _tools;
        private MapPresenter _presenter;
        private WpfMatrix _gdiViewMatrix;
        //private readonly StringFormat _format;
        private readonly WpfPoint[] _symbolTargetPointsTransfer = new WpfPoint[3];
        private Boolean _backgroundBeingSet;
        private readonly Label _infoLabel;
        private BitmapSource _bufferedMapImage;

        public MapViewControl()
        {
            _dpi = 96d;

            Visual v = Application.Current.MainWindow;
            if (v != null)
            {
                var t = PresentationSource.FromVisual(v);
                if (t != null && t.CompositionTarget != null)
                {
                    WpfMatrix m = t.CompositionTarget.TransformToDevice;
                    _dpi = m.M11;
                }
            }

            _infoLabel = new Label
                              {
                                  VerticalAlignment = WpfVerticalAlignment.Bottom,
                              };
            AddChild(_infoLabel);
            Information = "No Map has been set up!";
        }
        #region Static DependencyProperties

        public static DependencyProperty MapProperty =
            DependencyProperty.Register("MapProperty", typeof(Map), typeof(MapViewControl),
                                        new UIPropertyMetadata(default(Map), MapPropertyChanged));


        #endregion

        /// <summary>
        /// Sets the map to display.
        /// </summary>
        public Map Map
        {
            private get { return _presenter == null ? null : _presenter.Map; }
            set { SetValue(MapProperty, value); }
        }

        private MapPresenter MapPresenter
        { 
            set { _presenter = value; }
        }

        internal Boolean BackgroundBeingSet
        {
            get { return _backgroundBeingSet; }
            set { _backgroundBeingSet = value; }
        }

        internal String Information
        {
            get { return (string)_infoLabel.Content; }
            set { _infoLabel.Content = value; }
        }

        public bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        public bool Enabled
        {
            get { return IsEnabled; }
            set { IsEnabled = value; }
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public string Title
        {
            get { return Name; }
            set { Name = value; }
        }

        public event EventHandler<MapActionEventArgs<Point2D>> Hover;
        public event EventHandler<MapActionEventArgs<Point2D>> BeginAction;
        public event EventHandler<MapActionEventArgs<Point2D>> MoveTo;
        public event EventHandler<MapActionEventArgs<Point2D>> EndAction;
        public event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>> GeoCenterChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> MaximumWorldWidthChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> MinimumWorldWidthChangeRequested;
        public event EventHandler<LocationEventArgs> IdentifyLocationRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
        new public event EventHandler SizeChanged;
        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ViewEnvelopeChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> WorldAspectRatioChangeRequested;
        public event EventHandler ZoomToExtentsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ZoomToWorldBoundsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> ZoomToWorldWidthRequested;

        public StyleColor BackgroundColor
        {
            get { return _presenter.BackgroundColor; }
            set { _presenter.BackgroundColor = value; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //to avoid infinite view size, we set it to the screen size 
            //which is effectievly the maximal posible view size on the current system
            if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Width))
            {
                ViewSize = new Size2D(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            }
            else
            {
                ViewSize = ViewConverter.Convert(constraint);
            }

            constraint = ViewConverter.Convert(ViewSize);

            _infoLabel.Measure(constraint);

            return constraint;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _mouseDownLocation = e.GetPosition(this);
            _mousePreviousLocation = _mouseDownLocation;
            if (e.ChangedButton == MouseButton.Left) // dragging
            {
                onBeginAction(ViewConverter.Convert(_mouseDownLocation));
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var location = e.GetPosition(this);
            if (!_dragging && _mouseDownLocation != Empty && e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseRelativeLocation = new Point(location.X - _mouseDownLocation.X,
                                                   location.Y - _mouseDownLocation.Y);

                if (!WithinDragTolerance(location))
                {
                    _dragging = true;
                    _mousePreviousLocation = _mouseDownLocation;
                }
            }

            if (_dragging)
            {
                onMoveTo(ViewConverter.Convert(location));
                _mousePreviousLocation = location;
            }
            else
            {
                onHover(ViewConverter.Convert(location));
            }

            base.OnMouseMove(e);
        }

        private Boolean WithinDragTolerance(WpfPoint point)
        {
            return Math.Abs(_mouseDownLocation.X - point.X) <= 3
                && Math.Abs(_mouseDownLocation.Y - point.Y) <= 3;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            var location = e.GetPosition(this);
            if (e.ChangedButton == MouseButton.Left)
            {
                onEndAction(ViewConverter.Convert(location));
            }

            _mouseDownLocation = Empty;

            if (_dragging)
            {
                _dragging = false;
                _mouseRelativeLocation = Empty;
                _mousePreviousLocation = Empty;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void ParentLayoutInvalidated(UIElement child)
        {
            UpdateLayout();
            base.ParentLayoutInvalidated(child);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            EventHandler e = SizeChanged;
            if (e != null)
            {
                _infoLabel.Visibility = Visibility.Hidden;
                e(this, EventArgs.Empty);
                _infoLabel.Visibility = Visibility.Visible;
            }
        }

        protected override void  OnRender(DrawingContext drawingContext)
{
            if (_presenter == null)
            {
                drawingContext.DrawText(new FormattedText("No Map has been set up!", CultureInfo.CurrentUICulture,FlowDirection.LeftToRight, new Typeface("Arial"),24, Brushes.Black ), new Point(0,0) );
                return;
            }

            if (_bufferedMapImage != null)
            {
                drawingContext.DrawImage(_bufferedMapImage, new Rect(0,0, Width, Height));
                if (_renderObjectQueue.Count == 0) return;
            }

            foreach (WpfRenderObject ro in _renderObjectQueue)
            {
                WpfVectorRenderObject vro = ro as WpfVectorRenderObject;
                if (vro != null)
                {
                    if (vro.Outline != null)
                        drawingContext.DrawGeometry(null, vro.Outline, vro.Path);
                    drawingContext.DrawGeometry(vro.Fill, vro.Line, vro.Path);
                    continue;
                }
                WpfPointRenderObject pro = ro as WpfPointRenderObject;
                if (pro != null)
                {
                    drawingContext.DrawImage(pro.Symbol, pro.Bounds);
                    continue;
                }
                WpfTextRenderObject<Point> trop = ro as WpfTextRenderObject<Point>;
                if (trop != null)
                {
                    drawingContext.DrawText(trop.Text, trop.Location);
                    continue;
                }
                WpfTextRenderObject<StreamGeometry> tros = ro as WpfTextRenderObject<StreamGeometry>;
                if (tros != null)
                {
                    //drawingContext.DrawText(trops.Text, trop.Location);
                    continue;
                }

                WpfRasterRenderObject rro = ro as WpfRasterRenderObject;
                if (rro != null)
                {
                    drawingContext.DrawImage(rro.Raster, rro.Bounds);
                    continue;
                }
            }
        }

        public double Dpi
        {
            get { return _dpi; }
        }

        public static readonly DependencyProperty GeoCenterProperty =
            DependencyProperty.Register("GeoCenter", typeof(ICoordinate), typeof(MapViewControl));
        public ICoordinate GeoCenter
        {
            get { return _presenter.GeoCenter; }
            set { SetValue(GeoCenterProperty, value); }
        }

        public void IdentifyLocation(ICoordinate worldPoint)
        {
            onRequestIdentifyLocation(worldPoint);
        }

        public static readonly DependencyProperty MaximumWorldWidthProperty =
            DependencyProperty.Register("MaximumWorldWidth", typeof(double), typeof(MapViewControl), new UIPropertyMetadata(default(double)));

        public double MaximumWorldWidth
        {
            get { return _presenter.MaximumWorldWidth; }
            set { SetValue(MaximumWorldWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumWorldWidthProperty =
            DependencyProperty.Register("MinimumWorldProperty", typeof(double), typeof(MapViewControl), new UIPropertyMetadata(default(double)));
        public double MinimumWorldWidth
        {
            get { return _presenter.MinimumWorldWidth; }
            set { SetValue(MinimumWorldWidthProperty, value); }
        }

        public void Offset(Point2D offsetVector)
        {
            onRequestOffset(offsetVector);
        }

        public bool RequeryDatasources
        {
            get { return _presenter.RequeryDatasources; }
            set { _presenter.RequeryDatasources = value; }
        }

        public double PixelWorldHeight
        {
            get { return _presenter.PixelWorldHeight; }
        }

        public double PixelWorldWidth
        {
            get { return _presenter.PixelWorldWidth; }
        }

        public ViewSelection2D Selection
        {
            get { return _presenter.Selection; }
        }

        /// <summary>
        /// Draws the rendered object to the view.
        /// </summary>
        /// <param name="renderedObjects">The rendered objects to draw.</param>
        public void ShowRenderedObjects(IEnumerable<WpfRenderObject> renderedObjects)
        {
            if (renderedObjects == null)
            {
                throw new ArgumentNullException("renderedObjects");
            }

            _renderObjectQueue.Clear();
            _renderObjectQueue.AddRange(renderedObjects);
        }

        void IMapView2D.ShowRenderedObjects(IEnumerable renderedObjects)
        {
            if (renderedObjects is IEnumerable<WpfRenderObject>)
            {
                ShowRenderedObjects(renderedObjects as IEnumerable<WpfRenderObject>);
                return;
            }

            throw new ArgumentException("Rendered objects must be an IEnumerable<WpfRenderObject>.");
        }

        public Matrix2D ToViewTransform
        {
            get { return _presenter.ToViewTransform; }
        }

        public Matrix2D ToWorldTransform
        {
            get { return _presenter.ToWorldTransform; }
        }

        public Point2D ToView(ICoordinate point)
        {
            return _presenter.ToView(point);
        }

        public Point2D ToView(double x, double y)
        {
            return _presenter.ToView(x, y);
        }

        public ICoordinate ToWorld(Point2D point)
        {
            return _presenter.ToWorld(point);
        }

        public ICoordinate ToWorld(double x, double y)
        {
            return _presenter.ToWorld(x,y);
        }

        public IExtents2D ViewEnvelope
        {
            get { return _presenter.ViewEnvelope; }
            set { _presenter.ViewEnvelope = value; }
        }

        public Size2D ViewSize
        {
            get { return GetViewSize(); }
            set
            {
                Width = value.Width;
                Height = value.Height;
                //throw new NotImplementedException();
            }
        }

        private Size2D GetViewSize()
        {
            //FrameworkElement feParent = (FrameworkElement) Parent;
            if (double.IsNaN(Width))
                Width = ActualWidth;
            if (double.IsNaN(Height))
                Height = ActualHeight;
            return new Size2D(Width, Height);
        }

        public double WorldAspectRatio
        {
            get { return _presenter.WorldAspectRatio; }
            set { _presenter.WorldAspectRatio = value;}
        }

        public double WorldHeight
        {
            get { return _presenter.WorldHeight; }
        }

        public double WorldWidth
        {
            get { return _presenter.WorldWidth; }
        }

        public double WorldUnitsPerPixel
        {
            get { return _presenter.WorldUnitsPerPixel; }
        }

        public void ZoomToExtents()
        {
            onRequestZoomToExtents();
        }

        public void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            onRequestZoomToViewBounds(viewBounds);
        }

        public void ZoomToWorldBounds(IExtents2D zoomBox)
        {
            onRequestZoomToWorldBounds(zoomBox);
        }

        public void ZoomToWorldWidth(double newWorldWidth)
        {
            onRequestZoomToWorldWidth(newWorldWidth);
        }

        #region Event Invokers

        private void raiseSizeChanged()
        {
            EventHandler handler = null;// _sizeChangedHandler;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void onRequestOffset(Point2D offset)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Point2D>> e = OffsetChangeRequested;
            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Point2D> args =
                    new MapViewPropertyChangeEventArgs<Point2D>(Point2D.Zero, offset);
                e(this, args);
            }
        }

        private void onBeginAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = BeginAction;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onHover(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = Hover;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onEndAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = EndAction;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onMoveTo(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = MoveTo;
            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onRequestIdentifyLocation(ICoordinate location)
        {
            EventHandler<LocationEventArgs> e = IdentifyLocationRequested;

            if (e != null)
            {
                LocationEventArgs args = new LocationEventArgs(location);
                e(this, args);
            }
        }

        private void onRequestZoomToExtents()
        {
            EventHandler e = ZoomToExtentsRequested;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        private void onRequestZoomToViewBounds(Rectangle2D viewBounds)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> e = ZoomToViewBoundsRequested;

            if (e != null)
            {
                WpfRect rect = new WpfRect(ViewConverter.Convert(((IMapView2D)this).ViewSize));
                MapViewPropertyChangeEventArgs<Rectangle2D> args =
                    new MapViewPropertyChangeEventArgs<Rectangle2D>(ViewConverter.Convert(rect), viewBounds);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldBounds(IExtents2D zoomBox)
        {
            EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> e = ZoomToWorldBoundsRequested;
            if (e != null)
            {
                MapViewPropertyChangeEventArgs<IExtents2D> args =
                    new MapViewPropertyChangeEventArgs<IExtents2D>(ViewEnvelope, zoomBox);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldWidth(Double newWorldWidth)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = ZoomToWorldWidthRequested;
            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Double> args =
                    new MapViewPropertyChangeEventArgs<Double>(WorldWidth, newWorldWidth);

                e(this, args);
            }
        }

        private static void zoomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MapViewControl This = (MapViewControl)sender;
            if (e.Parameter is Double)
            {
                Double factor = (Double)e.Parameter;
                if (factor < 0)
                    NavigationCommands.IncreaseZoom.Execute(factor, This);
                else
                    NavigationCommands.DecreaseZoom.Execute(factor, This);
            }
        }

        //TODO: this handeling should be done in the Tool
        // will be implemnted when the tools support wheel gestures
        private static void increaseZoomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MapViewControl This = (MapViewControl)sender;
            double factor = 1.2;
            if ((e.Parameter != null) &&
                typeof(double).IsAssignableFrom(e.Parameter.GetType()))
            {
                factor = (double)e.Parameter;
            }
            This._presenter.ZoomByFactor(factor);
        }

        static private void decreaseZoomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            MapViewControl This = (MapViewControl)sender;
            double factor = 0.8;
            if ((e.Parameter != null) &&
                typeof(double).IsAssignableFrom(e.Parameter.GetType()))
            {
                factor = (double)e.Parameter;
            }
            This._presenter.ZoomByFactor(factor);
        }

        #endregion //private

        static void MapPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            MapViewControl mvcInstance = target as MapViewControl;
            if (mvcInstance != null && !ReferenceEquals(mvcInstance.Map, e.NewValue))
            {
                mvcInstance.MapPresenter = new MapPresenter((Map)e.NewValue, mvcInstance);
                mvcInstance.InvalidateVisual();
            }
        }


    }
    #region Event Arg Classes

    public class WpfMapActionEventArgs : MapActionEventArgs<Point2D>
    {
        public WpfMapActionEventArgs()
            : base(new Point2D()) { }

        public void SetActionPoint(Point2D actionLocation)
        {
            ActionPoint = actionLocation;
        }
    }

    #endregion
}