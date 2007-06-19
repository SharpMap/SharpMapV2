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
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Presentation
{
	public class MapViewPort2D
	{
		private SharpMap.Map _map;
        private IMapView2D _view;
		private double _pixelAspectRatio = 1.0;
		private double _viewDpi;
		private double _worldUnitsPerInch;
		private ViewSize2D _viewSize;
		private GeoPoint _center = GeoPoint.Zero;
		private double _maximumWorldWidth;
		private double _minimumWorldWidth;
		private IViewMatrix _viewTransform = new ViewMatrix2D();
		private IViewMatrix _viewTransformInverted = new ViewMatrix2D();
		private StyleColor _backgroundColor;
         
		public MapViewPort2D(SharpMap.Map map, IMapView2D view)
		{
			_map = map;
            _view = view;
            _center = map.Envelope.GetCentroid();
			_viewDpi = view.Dpi;
            _worldUnitsPerInch = _viewDpi;
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
		public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MinimumWorldWidthChanged;

		/// <summary>
		/// Fired when the <see cref="MaximumZoom"/> has changed.
		/// </summary>
		public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MaximumWorldWidthChanged;

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
        /// Gets the height of view in world units.
		/// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="PixelAspectRatio"/> 
        /// (<see cref="WorldWidth"/> * <see cref="PixelAspectRatio"/> * <see cref="ViewSize.Height"/> / <see cref="ViewSize.Width"/>).
        /// </returns>
		public double WorldHeight
		{
            get { return WorldWidth * PixelAspectRatio * ViewSize.Height / ViewSize.Width; }
		}

		/// <summary>
		/// Gets the width of view in world units.
		/// </summary>
        /// <returns>The width of the view in world units (<see cref="ViewSize.Width" /> * <see cref="WorldUnitsPerPixel"/>).</returns>
		public double WorldWidth
		{
			get { return ViewSize.Width * WorldUnitsPerPixel; }
		}

		/// <summary>
		/// Gets or sets the size of the view in display units.
		/// </summary>
		public ViewSize2D ViewSize
		{
			get { return _viewSize; }
			set
			{
				if (value != _viewSize)
				{
					setViewMetricsInternal(value, _center, WorldUnitsPerPixel);
				}
			}
		}

		/// <summary>
		/// Gets or sets the extents of the current view in world units.
		/// </summary>
		public BoundingBox ViewEnvelope
		{
			get { return Transform2D.ViewToWorld(new ViewRectangle2D(ViewPoint2D.Zero, ViewSize), this); }
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
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("GeoCenter");
				}

				setViewMetricsInternal(ViewSize, value, WorldUnitsPerPixel);
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
                    throw new ArgumentOutOfRangeException("MinimumWorldWidth", value, "Minimum world width must be 0 or greater.");
				}

				if (_minimumWorldWidth != value)
				{
					double oldValue = _minimumWorldWidth;
					_minimumWorldWidth = value;
					OnMinimumZoomChanged(oldValue, _minimumWorldWidth);
				}
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
                    throw new ArgumentOutOfRangeException("MaximumWorldWidth", value, "Maximum world width must greater than 0.");
				}

				if (_maximumWorldWidth != value)
				{
					double oldValue = _maximumWorldWidth;
					_maximumWorldWidth = value;
					OnMaximumZoomChanged(oldValue, _maximumWorldWidth);
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
        /// Gets the dots per inch resolution of the view.
        /// </summary>
		public double ViewDpi
		{
			get { return _viewDpi; }
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
        /// Gets or sets the aspect ratio of the <see cref="ViewSize.Height"/> to the <see cref="ViewSize.Width"/>.
		/// </summary>
		/// <remarks> 
        /// A value less than 1 will make the map stretch upwards (the view will cover less world distance vertically), 
        /// and greater than 1 will make it more squat (the view will cover more world distance vertically).
		/// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws an argument exception when value is 0 or less.
        /// </exception>
		public double PixelAspectRatio
		{
			get { return _pixelAspectRatio; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("PixelAspectRatio", value, "Invalid pixel aspect ratio.");
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
		/// Gets or sets a <see cref="IViewMatrix"/> used to transform the map image.
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

		/// <summary>
		/// The invert of the <see cref="MapViewTransform"/> matrix.
		/// </summary>
		/// <remarks>
		/// An inverse matrix is used to reverse the transformation of a matrix.
		/// </remarks>
		public IViewMatrix MapViewTransformInverted
		{
			get { return _viewTransformInverted; }
			private set { _viewTransformInverted = value; }
        }

        /// <summary>
        /// Zooms the view to the given width.
        /// </summary>
        /// <remarks>
        /// View modifiers <see cref="MinimumWorldWidth"/>, <see cref="MaximumWorldWidth"/> and <see cref="PixelAspectRatio"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        public void ZoomToWidth(double worldWidth)
        {
            double newHeight = worldWidth * (WorldHeight / WorldWidth);
            double halfWidth = worldWidth * 0.5;
            double halfHeight = newHeight * 0.5;
            ZoomToBox(new BoundingBox(GeoCenter.X - halfWidth, GeoCenter.Y - halfHeight, GeoCenter.X + halfWidth, GeoCenter.Y + halfHeight));
        }

		/// <summary>
		/// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
		/// </summary>
		public void ZoomToExtents()
		{
			ZoomToBox(_map.GetExtents());
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
			EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MinimumWorldWidthChanged;

			if (@event != null)
			{
				@event(this, new MapPresentationPropertyChangedEventArgs<double>(oldMinimumZoom, newMinimumZoom));
			}
		}

		protected virtual void OnMaximumZoomChanged(double oldMaximumZoom, double newMaximumZoom)
		{
			EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MaximumWorldWidthChanged;

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

		protected virtual void OnViewRectangleChanged(ViewRectangle2D oldViewRectangle, ViewRectangle2D currentViewRectangle, BoundingBox oldBounds, BoundingBox currentBounds)
		{
			EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>> @event = ViewRectangleChanged;

			if (@event != null)
			{
				@event(this, new MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>(
					oldBounds, currentBounds, oldViewRectangle, currentViewRectangle));
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

			setViewMetricsInternal(ViewSize, newCenter, WorldUnitsPerPixel);
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

			setViewMetricsInternal(ViewSize, newEnvelope.GetCentroid(), newWidth / ViewSize.Width);
		}

		private void setViewMetricsInternal(ViewSize2D newViewSize, GeoPoint newCenter, double newWorldUnitsPerPixel)
        {
            ViewSize2D oldViewSize = ViewSize;
            GeoPoint oldCenter = GeoCenter;
            double oldWorldUnitsPerPixel = WorldUnitsPerPixel;
            double oldWorldWidth = WorldWidth;
            double oldWorldHeight = WorldHeight;

			if (newViewSize == _viewSize && _center.Equals(newCenter) && newWorldUnitsPerPixel == oldWorldUnitsPerPixel)
			{
				return;
			}

            _viewSize = newViewSize;
            _center = newCenter;
            _worldUnitsPerInch = newWorldUnitsPerPixel * _viewDpi;

			if (_viewSize != oldViewSize)
			{
				OnSizeChanged(oldViewSize, _viewSize);
			}

			if (!_center.Equals(oldCenter))
			{
				OnMapCenterChanged(oldCenter, _center);
			}

            OnViewRectangleChanged(new ViewRectangle2D(0, oldViewSize.Width, 0, oldViewSize.Height), new ViewRectangle2D(0, _viewSize.Width, 0, _viewSize.Height),
                new BoundingBox(oldCenter.X - (oldWorldWidth * 0.5), oldCenter.Y - (oldWorldHeight * 0.5), oldCenter.X + (oldWorldWidth * 0.5), oldCenter.Y + (oldWorldHeight * 0.5)),
                new BoundingBox(_center.X - (WorldWidth * 0.5), _center.Y - (WorldHeight * 0.5), _center.X + (WorldWidth * 0.5), _center.Y + (WorldHeight * 0.5)));
		}
		#endregion
	}
}
