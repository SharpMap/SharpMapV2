// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;

using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides the input-handling and view-updating logic for a 2D map view.
    /// </summary>
    public class MapPresenter2D : BasePresenter<IMapView2D>
    {
        private ViewSelection2D _selection;

        private Point2D? _beginActionLocation;
        private Point2D _lastMoveLocation;

        //private readonly double _viewDpi;
        //private double _worldUnitsPerInch;
        //private ViewSize2D _viewSize;
        //private GeoPoint _center;
        private double _maximumWorldWidth;
        private double _minimumWorldWidth;
        private Matrix2D _toViewTransform;
        private Matrix2D _toWorldTransform;
        private StyleColor _backgroundColor;
        
        #region Object Construction/Destruction
        public MapPresenter2D(Map map, IMapView2D mapView)
            : base(map, mapView)
        {
            Map.LayersCollectionChanged += Map_LayersChanged;

            View.SizeChangeRequested += View_SizeChangeRequested;
            View.Hover += View_Hover;
            View.BeginAction += View_BeginAction;
            View.MoveTo += View_MoveTo;
            View.EndAction += View_EndAction;
            //_viewDpi = View.Dpi;

            BoundingBox extents = map.Envelope;

            GeoPoint geoCenter = extents.GetCentroid();

            double initialScale = extents.Width / View.ViewSize.Width;

            if (View.ViewSize.Height / extents.Height < initialScale)
            {
                initialScale = extents.Height / View.ViewSize.Height;
            }

            if (geoCenter != GeoPoint.Empty)
            {
                double widthScale = 1 / initialScale;
                double xCenterTranslation = View.ViewSize.Width / 2 - geoCenter.X;
                
                double heightScale = 1 / initialScale;
                double yCenterTranslation = View.ViewSize.Height / 2 - geoCenter.Y;

                _toViewTransform = new Matrix2D(widthScale, 0, xCenterTranslation,
                    0, heightScale, yCenterTranslation);

                _toViewTransform.Translate(0, -View.ViewSize.Height);
                _toViewTransform.Scale(1, -1);

                _toWorldTransform = _toViewTransform.Inverse as Matrix2D;
            }
            else
            {
                _toViewTransform = new Matrix2D();
                _toWorldTransform = new Matrix2D();
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        /// <remarks>
        /// Defaults to transparent.
        /// </remarks>
        public StyleColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    StyleColor oldValue = _backgroundColor;
                    _backgroundColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        public GeoPoint GeoCenter
        {
            get 
            {
                Point2D values = ToWorldTransform.TransformVector(View.ViewSize.Width / 2, View.ViewSize.Height / 2);
                return new GeoPoint(values.X, values.Y);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                setViewMetricsInternal(View.ViewSize, value, WorldUnitsPerPixel);
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        public double MaximumWorldWidth
        {
            get { return _maximumWorldWidth; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Maximum world width must greater than 0.");
                }

                if (_maximumWorldWidth != value)
                {
                    double oldValue = _maximumWorldWidth;
                    _maximumWorldWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        public double MinimumWorldWidth
        {
            get { return _minimumWorldWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Minimum world width must be 0 or greater.");
                }

                if (_minimumWorldWidth != value)
                {
                    double oldValue = _minimumWorldWidth;
                    _minimumWorldWidth = value;
                }
            }
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
            get { return WorldUnitsPerPixel * PixelAspectRatio; }
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the <see cref="ViewSize"/> 
        /// height to the <see cref="ViewSize"/> width.
        /// </summary>
        /// <remarks> 
        /// A value less than 1 will make the map stretch upwards 
        /// (the view will cover less world distance vertically), 
        /// and greater than 1 will make it more squat (the view will 
        /// cover more world distance vertically).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws an argument exception when value is 0 or less.
        /// </exception>
        public double PixelAspectRatio
        {
#warning Is this division the correct order for the PixelAspectRatio property?
            get { return Math.Abs(_toViewTransform.X1 / _toViewTransform.Y2); }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Invalid pixel aspect ratio.");
                }

                double currentRatio = Math.Abs(_toViewTransform.X1 / _toViewTransform.Y2);

                if (currentRatio != value)
                {
                    double ratioModifier = value / currentRatio;
                    ToViewTransform.Y2 *= ratioModifier;
                    ToWorldTransform = ToViewTransform.Inverse as Matrix2D;
                }
            }
        }

        /// <summary>
        /// A selection on a view.
        /// </summary>
        public ViewSelection2D Selection
        {
            get { return _selection; }
            private set { _selection = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="ViewMatrix2D"/> used to project the world
        /// coordinate system into the view coordinate system.
        /// The inverse of the <see cref="ToWorldTransform"/> matrix.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// </code>
        /// </example>
        public Matrix2D ToViewTransform
        {
            get { return _toViewTransform; }
            set
            {
                if (_toViewTransform != value)
                {
                    Matrix2D oldValue = _toViewTransform;
                    _toViewTransform = value;
                    ToWorldTransform = _toViewTransform.Inverse as Matrix2D;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="ViewMatrix2D"/> used to reverse the view projection
        /// transform to get world coordinates.
        /// The inverse of the <see cref="ToViewTransform"/> matrix.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public Matrix2D ToWorldTransform
        {
            get { return _toWorldTransform; }
            private set { _toWorldTransform = value; }
        }

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        public BoundingBox ViewEnvelope
        {
            get 
            {
                Point2D lowerLeft = ToWorldTransform.TransformVector(0, View.ViewSize.Height);
                Point2D upperRight = ToWorldTransform.TransformVector(View.ViewSize.Width, 0);
                return new BoundingBox(lowerLeft.X, lowerLeft.Y, upperRight.X, upperRight.Y);
            }
            set
            {
                setViewEnvelopeInternal(value);
            }
        }

        /// <summary>
        /// Gets the height of view in world units.
        /// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="PixelAspectRatio"/> 
        /// (<see cref="WorldWidth"/> * <see cref="PixelAspectRatio"/> * 
        /// <see cref="ViewSize"/> height / <see cref="ViewSize"/> width).
        /// </returns>
        public double WorldHeight
        {
            get { return WorldWidth * PixelAspectRatio * View.ViewSize.Height / View.ViewSize.Width; }
        }

        /// <summary>
        /// Gets the width of view in world units.
        /// </summary>
        /// <returns>The width of the view in world units (<see cref="ViewSize" /> 
        /// height * <see cref="WorldUnitsPerPixel"/>).</returns>
        public double WorldWidth
        {
            get { return View.ViewSize.Width * WorldUnitsPerPixel; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        public double WorldUnitsPerPixel
        {
            get { return ToWorldTransform.X1; }
        }
        #endregion

		public Point2D ToView(Point point)
		{
			return worldToView(point);
		}

		public Point2D ToView(double x, double y)
		{
			return ToViewTransform.TransformVector(x, y);
		}

		public Point ToWorld(Point2D point)
		{
			return viewToWorld(point);
		}

		public Point ToWorld(double x, double y)
		{
			Point2D values = ToWorldTransform.TransformVector(x, y);
			return new GeoPoint(values.X, values.Y);
		}

        /// <summary>
        /// Zooms the view to the given width.
        /// </summary>
        /// <remarks>
        /// View modifiers <see cref="MinimumWorldWidth"/>, 
        /// <see cref="MaximumWorldWidth"/> and <see cref="PixelAspectRatio"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        public void ZoomToWidth(double worldWidth)
        {
            double newHeight = worldWidth * (WorldHeight / WorldWidth);
            double halfWidth = worldWidth * 0.5;
            double halfHeight = newHeight * 0.5;

            BoundingBox widthWorldBounds = new BoundingBox(
                GeoCenter.X - halfWidth, 
                GeoCenter.Y - halfHeight, 
                GeoCenter.X + halfWidth, 
                GeoCenter.Y + halfHeight);

            ZoomToBox(widthWorldBounds);
        }

        /// <summary>
        /// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
        /// </summary>
        public void ZoomToExtents()
        {
            ZoomToBox(Map.GetExtents());
        }

        /// <summary>
        /// Zooms the map to fit a bounding box.
        /// </summary>
        /// <remarks>
        /// If the ratio of either the width of the current 
        /// map <see cref="ViewEnvelope">envelope</see> 
        /// to the width of <paramref name="zoomBox"/> or 
        /// of the height of the current map <see cref="ViewEnvelope">envelope</see> 
        /// to the height of <paramref name="zoomBox"/> is 
        /// greater, the map envelope will be enlarged to contain the 
        /// <paramref name="zoomBox"/> parameter.
        /// </remarks>
        /// <param name="zoomBox"><see cref="BoundingBox"/> to set zoom to.</param>
        public void ZoomToBox(BoundingBox zoomBox)
        {
            setViewEnvelopeInternal(zoomBox);
        }

        public IRenderer GetRendererForLayer<TRenderObject>(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            IRenderer renderer = null;

            //_layerRendererCatalog.TryGetValue(layer.GetType().TypeHandle, out renderer);
            renderer = LayerRendererCatalog.Instance.Get<IRenderer>(layer.GetType());
            return renderer;
        }

        protected void RegisterRenderer<TLayerType, TRenderObject>(IRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            LayerRendererCatalog.Instance.Register<Point2D, Size2D, Rectangle2D, TRenderObject>(
                typeof(TLayerType), renderer);
        }

        protected TRenderer GetRenderer<TRenderer>(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            Type layerType = layer.GetType();
            return LayerRendererCatalog.Instance.Get<TRenderer>(layerType);
        }

        protected void CreateSelection(Point2D location)
        {
            _selection = ViewSelection2D.CreateRectangluarSelection(location, Size2D.Zero);
        }

        protected void ModifySelection(Point2D location, Size2D size)
        {
            _selection.MoveBy(location);
            _selection.Expand(size);
        }

        protected void DestroySelection()
        {
            _selection = null;
        }

        protected virtual void OnMapViewHover(Point2D viewPoint2D)
        {
        }

        protected virtual void OnMapViewBeginAction(Point2D location)
        {
            _beginActionLocation = location;
        }

        protected virtual void OnMapViewMoveTo(Point2D location)
        {
            double xDelta = location.X - _lastMoveLocation.X;
            double yDelta = location.Y - _lastMoveLocation.Y;

            if (Selection == null)
            {
                CreateSelection(location);
            }

            _lastMoveLocation = location;
        }

        protected virtual void OnMapViewEndAction(Point2D location)
        {
            _beginActionLocation = null;
        }

        protected virtual void OnSelectedFeaturesChanged()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLayersChanged()
        {
            throw new NotImplementedException();
        }

        #region Event handlers
        private void Map_SelectedFeaturesChanged(object sender, EventArgs e)
        {
            OnSelectedFeaturesChanged();
        }

        private void Map_LayersChanged(object sender, ModelCollectionChangedEventArgs<ILayer> e)
        {
            OnLayersChanged();
        }

        private void View_SizeChangeRequested(object sender, SizeChangeEventArgs<Size2D> e)
        {
        }

        private void View_Hover(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewHover(e.ActionPoint);
        }

        private void View_BeginAction(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewBeginAction(e.ActionPoint);
        }

        private void View_MoveTo(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewMoveTo(e.ActionPoint);
        }

        private void View_EndAction(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewEndAction(e.ActionPoint);
        }
        #endregion

        #region Private helper methods
        private void modifySelection(double xDelta, double yDelta)
        {
            Selection.Expand(new Size2D(xDelta, yDelta));
        }

        private void setCenterInternal(GeoPoint newCenter)
        {
            if (GeoCenter == newCenter)
            {
                return;
            }

            setViewMetricsInternal(View.ViewSize, newCenter, WorldUnitsPerPixel);
        }

        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            BoundingBox oldEnvelope = ViewEnvelope;

            if (oldEnvelope == newEnvelope)
            {
                return;
            }

            double widthZoomRatio = newEnvelope.Width / oldEnvelope.Width;
            double heightZoomRatio = newEnvelope.Height / oldEnvelope.Height;

            double newWidth = widthZoomRatio > heightZoomRatio ? newEnvelope.Width : newEnvelope.Width * heightZoomRatio;

            if (newWidth < _minimumWorldWidth)
            {
                newWidth = _minimumWorldWidth;
            }

            if (newWidth > _maximumWorldWidth)
            {
                newWidth = _maximumWorldWidth;
            }

            setViewMetricsInternal(View.ViewSize, newEnvelope.GetCentroid(), newWidth / View.ViewSize.Width);
        }

        private void setViewMetricsInternal(Size2D newViewSize, GeoPoint newCenter, double newWorldUnitsPerPixel)
        {
            Size2D oldViewSize = View.ViewSize;
            GeoPoint oldCenter = GeoCenter;
            double oldWorldWidth = WorldWidth;
            double oldWorldHeight = WorldHeight;

            if (newViewSize == View.ViewSize && GeoCenter.Equals(newCenter) && newWorldUnitsPerPixel == WorldUnitsPerPixel)
            {
                return;
            }

            View.ViewSize = newViewSize;
        }

        private Point2D worldToView(GeoPoint geoPoint)
        {
            return ToViewTransform.TransformVector(geoPoint.X, geoPoint.Y);
        }

        private GeoPoint viewToWorld(Point2D viewPoint)
        {
            Point2D values = ToWorldTransform.TransformVector(viewPoint.X, viewPoint.Y);
            return new GeoPoint(values.X, values.Y);
        }
        #endregion
	}
}
