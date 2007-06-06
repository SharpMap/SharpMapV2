using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Map;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Presentation
{
    public class MapViewPort2D
    {
        private SharpMap.Map.Map _map;
        //private ViewRectangle2D _viewRectangle;
        //private double _pixelAspectRatio = 1.0;
        private double _viewDpi;
        private double _worldUnitsPerInch;
        private ViewSize2D _viewSize;
        private GeoPoint _center;
        private double _maximumZoom;
        private double _minimumZoom;
        private IViewMatrix _viewTransform = new ViewMatrix2D();
        private IViewMatrix _viewTransformInverted = new ViewMatrix2D();
        private StyleColor _backgroundColor;

        public MapViewPort2D(SharpMap.Map.Map map, double viewDpi)
        {
            _map = map;
            _viewDpi = viewDpi;
        }

        /// <summary>
        /// Fired when the center of the view has changed relative to the <see cref="Map"/>.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewPoint2D, GeoPoint>> CenterChanged;

        /// <summary>
        /// Fired when MapViewPort2D is resized.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>> SizeChanged;

        /// <summary>
        /// Fired when the <see cref="ViewRectangle"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>> ViewRectangleChanged;

        /// <summary>
        /// Fired when the <see cref="MinimumZoom"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MinimumZoomChanged;

        /// <summary>
        /// Fired when the <see cref="MaximumZoom"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MaximumZoomChanged;

        /// <summary>
        /// Fired when the <see cref="PixelAspectRatio"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> PixelAspectRatioChanged;

        /// <summary>
        /// Fired when the <see cref="MapTransform"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<IViewMatrix>> MapTransformChanged;

        /// <summary>
        /// Fired when the <see cref="MaximumZoom"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleColor>> BackgroundColorChanged;

        /// <summary>
        /// Fired when the <see cref="StyleRenderingMode"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>> StyleRenderingModeChanged;

        /// <summary>
        /// Event fired when the <see cref="Zoom"/> value has changed.
        /// </summary>
        public event EventHandler<MapZoomChangedEventArgs> ZoomChanged;

        /// <summary>
        /// Height of view in world units.
        /// </summary>
        /// <returns></returns>
        public double WorldHeight
        {
            get { return (Zoom * ViewSize.Height) / ViewSize.Width * PixelAspectRatio; }
        }

        /// <summary>
        /// Width of view in world units.
        /// </summary>
        /// <returns></returns>
        public double WorldWidth
        {
            get { return (Zoom * ViewSize.Height) / ViewSize.Width * PixelAspectRatio; }
        }

        /// <summary>
        /// Gets or sets the size of the view in display units.
        /// </summary>
        public ViewSize2D ViewSize
        {
            get { return ViewRectangle.Size; }
            set
            {
                if (value != ViewRectangle.Size)
                {
                    setViewMetricsInternal(new ViewRectangle2D(ViewRectangle.Location, ViewRectangle.Size), _center, _widthInWorldUnits);
                }
            }
        }

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        public BoundingBox ViewEnvelope
        {
            get { return Transform2D.ViewToWorld(ViewRectangle, this); }
            set
            {
                setViewEnvelopeInternal(value);
            }
        }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        public GeoPoint GeoCenter
        {
            get { return _center.Clone(); }
            set { setViewMetricsInternal(ViewRectangle, value, _widthInWorldUnits); }
        }

        /// <summary>
        /// Gets or sets the zoom level of map.
        /// </summary>
        /// <remarks>
        /// <para>The zoom level corresponds to the <see cref="Map.Width"/> of the map in world coordinate units.</para>
        /// <para>A zoom level &lt;= 0 will result in an empty map being rendered, but will not throw an exception.</para>
        /// </remarks>
        public double Zoom
        {
            get { return _widthInWorldUnits; }
            set { setViewMetricsInternal(ViewRectangle, GeoCenter, value); }
        }

        /// <summary>
        /// Minimum zoom amount allowed.
        /// </summary>
        public double MinimumZoom
        {
            get { return _minimumZoom; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Minimum zoom must be 0 or more");
                }

                if (_minimumZoom != value)
                {
                    double oldValue = _minimumZoom;
                    _minimumZoom = value;
                    OnMinimumZoomChanged(oldValue, _minimumZoom);
                }
            }
        }

        /// <summary>
        /// Maximum zoom amount allowed.
        /// </summary>
        public double MaximumZoom
        {
            get { return _maximumZoom; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Maximum zoom must larger than 0");
                }

                if (_maximumZoom != value)
                {
                    double oldValue = _maximumZoom;
                    _maximumZoom = value;
                    OnMaximumZoomChanged(oldValue, _maximumZoom);
                }
            }
        }

        /// <summary>
        /// Gets the size of a pixel in world coordinate units.
        /// </summary>
        public double WorldUnitsPerPixel
        {
            get { return _worldUnitsPerInch / _viewDpi; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="WorldUnitsPerPixel"/>.</remarks>
        public double PixelWidth
        {
            get { return WorldUnitsPerPixel; }
        }

        /// <summary>
        /// Gets the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="WorldUnitsPerPixel"/> 
        /// unless <see cref="PixelAspectRatio"/> is different from 1.</remarks>
        public double PixelHeight
        {
            get { return WorldUnitsPerPixel * _pixelAspectRatio; }
        }

        /// <summary>
        /// Gets or sets the aspect-ratio of the pixel scales. A value less than 
        /// 1 will make the map streach upwards, and larger than 1 will make it smaller.
        /// </summary>
        /// <exception cref="ArgumentException">Throws an argument exception when value 
        /// is 0 or less.</exception>
        public double PixelAspectRatio
        {
            get { return _pixelAspectRatio; }
            set
            {
                if (_pixelAspectRatio <= 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid pixel aspect ratio");
                }

                if (_pixelAspectRatio != value)
                {
                    double oldValue = _pixelAspectRatio;
                    _pixelAspectRatio = value;
                    OnPixelAspectRatioChanged(oldValue, _pixelAspectRatio);
                }
            }
        }

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        /// <remarks>
        /// Defaults to transparent.
        /// </remarks>
        public StyleColor BackColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    StyleColor oldValue = _backgroundColor;
                    _backgroundColor = value;
                    OnBackgroundColorChanged(oldValue, _backgroundColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Matrix"/> used to transform the map image.
        /// </summary>
        /// <remarks>
        /// Using the <see cref="MapTransform"/> you can alter the coordinate system of the map rendering.
        /// This makes it possible to rotate or rescale the image, for instance to have another direction 
        /// than north upwards.
        /// </remarks>
        /// <example>
        /// Rotate the map output 45 degrees around its center:
        /// <code lang="C#">
        /// IViewMatrix mapTransform = new SharpMap.Rendering.ViewMatrix2D(); //Create transformation matrix
        ///	mapTransform.RotateAt(45, myMap.ViewCenter);    //Apply 45 degrees rotation around the center of the map
        ///	myMap.MapTransform = mapTransform;              //Apply transformation to map view
        /// </code>
        /// </example>
        public IViewMatrix MapViewTransform
        {
            get { return _viewTransform; }
            set
            {
                if (_viewTransform != value)
                {
                    IViewMatrix oldValue = _viewTransform;
                    _viewTransform = value;

                    if (_viewTransform.IsInvertible)
                    {
                        MapViewTransformInverted = _viewTransform.Clone() as IViewMatrix;
                        MapViewTransformInverted.Invert();
                    }
                    else
                    {
                        MapViewTransformInverted.Reset();
                    }

                    OnMapTransformChanged(oldValue, _viewTransform);
                }
            }
        }

        public IViewMatrix MapViewTransformInverted
        {
            get { return _viewTransformInverted; }
            private set { _viewTransformInverted = value; }
        }

        /// <summary>
        /// Zooms to the extents of all layers
        /// </summary>
        public void ZoomToExtents()
        {
            this.ZoomToBox(_map.GetExtents());
        }

        /// <summary>
        /// Zooms the map to fit a bounding box.
        /// </summary>
        /// <remarks>
        /// If the ratio of either the width of the current map <see cref="Envelope">envelope</see> 
        /// to the width of <paramref name="zoomBox"/> or of the height of the current map <see cref="Envelope">envelope</see> 
        /// to the height of <paramref name="zoomBox"/> is greater, the map envelope will be enlarged to contain the 
        /// <paramref name="zoomBox"/> parameter.
        /// </remarks>
        /// <param name="zoomBox"><see cref="BoundingBox"/> to set zoom to.</param>
        public void ZoomToBox(BoundingBox zoomBox)
        {
            setViewEnvelopeInternal(zoomBox);
        }

        #region Event Generators

        protected virtual void OnMinimumZoomChanged(double oldMinimumZoom, double newMinimumZoom)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MinimumZoomChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldMinimumZoom, newMinimumZoom));
            }
        }

        protected virtual void OnMaximumZoomChanged(double oldMaximumZoom, double newMaximumZoom)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MaximumZoomChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldMaximumZoom, newMaximumZoom));
            }
        }

        protected virtual void OnPixelAspectRatioChanged(double oldPixelAspectRatio, double newPixelAspectRatio)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = PixelAspectRatioChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldPixelAspectRatio, newPixelAspectRatio));
            }
        }

        protected virtual void OnViewRectangleChanged(ViewRectangle2D oldViewRectangle, ViewRectangle2D currentViewRectangle)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>> @event = ViewRectangleChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>(
                    _viewTransform.ViewToWorld(oldViewRectangle), _viewTransform.ViewToWorld(currentViewRectangle), 
                    oldViewRectangle, currentViewRectangle));
            }
        }

        protected virtual void OnMapTransformChanged(IViewMatrix oldTransform, IViewMatrix newTransform)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<IViewMatrix>> @event = MapTransformChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<IViewMatrix>(oldTransform, newTransform));
            }
        }

        protected virtual void OnBackgroundColorChanged(StyleColor oldColor, StyleColor newColor)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<StyleColor>> @event = BackgroundColorChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<StyleColor>(oldColor, newColor));
            }
        }

        protected virtual void OnStyleRenderingModeChanged(StyleRenderingMode oldValue, StyleRenderingMode newValue)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>> @event = StyleRenderingModeChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<StyleRenderingMode>(oldValue, newValue));
            }
        }

        protected virtual void OnZoomChanged(double previousZoom, double currentZoom, GeoPoint previousCenter, GeoPoint currentCenter)
        {
            EventHandler<MapZoomChangedEventArgs> @event = ZoomChanged;

            if (@event != null)
            {
                @event(this, new MapZoomChangedEventArgs(previousZoom, currentZoom, previousCenter, currentCenter));
            }
        }

        protected virtual void OnMapCenterChanged(GeoPoint previousCenter, GeoPoint currentCenter)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<ViewPoint2D, GeoPoint>> @event = CenterChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<ViewPoint2D, GeoPoint>(
                    previousCenter, currentCenter, Transform2D.WorldToView(previousCenter, this), Transform2D.WorldToView(currentCenter, this)));
            }
        }

        protected virtual void OnSizeChanged(ViewSize2D oldSize, ViewSize2D newSize)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>> @event = SizeChanged;

            if (@event != null)
            {
                @event(this, new MapPresentationPropertyChangedEventArgs<ViewSize2D>(oldSize, newSize));
            }
        }

        #endregion

        #region Helper methods
        private void setCenterInternal(GeoPoint newCenter)
        {
            if (_center == newCenter)
            {
                return;
            }

            setViewMetricsInternal(ViewRectangle, newCenter, _widthInWorldUnits);
        }

        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            BoundingBox currentEnvelope = ViewEnvelope;

            if (currentEnvelope == newEnvelope)
            {
                return;
            }

            double widthZoomRatio = newEnvelope.Width / currentEnvelope.Width;
            double heightZoomRatio = newEnvelope.Height / currentEnvelope.Height;
            double newWidth = widthZoomRatio > heightZoomRatio ? newEnvelope.Width : newEnvelope.Width * heightZoomRatio;

            if (newWidth < _minimumZoom)
            {
                newWidth = _minimumZoom;
            }

            if (newWidth > _maximumZoom)
            {
                newWidth = _maximumZoom;
            }

            setViewMetricsInternal(ViewRectangle, newEnvelope.GetCentroid(), newWidth);
        }

        private void setViewMetricsInternal(ViewRectangle2D viewRectangle, GeoPoint center, double widthInWorldUnits)
        {
            if (viewRectangle == _viewRectangle && center == _center && widthInWorldUnits == _widthInWorldUnits)
            {
                return;
            }

            ViewRectangle2D oldViewRectangle = _viewRectangle;
            double oldWidthInWorldUnits = _widthInWorldUnits;
            GeoPoint oldCenter = _center;

            _viewRectangle = viewRectangle;
            _widthInWorldUnits = widthInWorldUnits;
            _center = center;

            if (_center != oldCenter)
            {
                OnMapCenterChanged(oldCenter, _center);
            }

            if (_widthInWorldUnits != oldWidthInWorldUnits)
            {
                OnZoomChanged(oldWidthInWorldUnits, _widthInWorldUnits, oldCenter, _center);
            }

            if (_viewRectangle != oldViewRectangle)
            {
                OnViewRectangleChanged(oldViewRectangle, _viewRectangle);

                if (_viewRectangle.Size != oldViewRectangle.Size)
                {
                    OnSizeChanged(oldViewRectangle.Size, _viewRectangle.Size);
                }
            }
        }
        #endregion
    }
}
