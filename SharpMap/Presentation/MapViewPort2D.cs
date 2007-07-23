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
using NPack.Interfaces;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IAffineMatrixD = NPack.Interfaces.IAffineTransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
	public class MapViewPort2D
	{
		private readonly Map _map;
        private IMapView2D _view;

        #region Constructor

        public MapViewPort2D(Map map, IMapView2D view)
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the center of the view has changed relative to the <see cref="Map"/>.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewPoint2D, GeoPoint>> CenterChanged;

        /// <summary>
        /// Fired when MapViewPort2D is resized.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>> SizeChanged;

        /// <summary>
        /// Fired when the <see cref="ViewEnvelope"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>> ViewEnvelopeChanged;

        /// <summary>
        /// Fired when the <see cref="MinimumWorldWidth"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MinimumWorldWidthChanged;

        /// <summary>
        /// Fired when the <see cref="MaximumWorldWidth"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MaximumWorldWidthChanged;

        /// <summary>
        /// Fired when the <see cref="PixelAspectRatio"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> PixelAspectRatioChanged;

        /// <summary>
        /// Fired when the <see cref="MapViewTransform"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<IMatrixD>> MapViewTransformChanged;

        /// <summary>
        /// Fired when the <see cref="BackgroundColor"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleColor>> BackgroundColorChanged;

        /// <summary>
        /// Fired when the <see cref="StyleRenderingMode"/> has changed.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>> StyleRenderingModeChanged;

        #endregion


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
			EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>> @event = ViewEnvelopeChanged;

			if (@event != null)
			{
				@event(this, new MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>(
					oldBounds, currentBounds, oldViewRectangle, currentViewRectangle));
			}
		}

		protected virtual void OnMapTransformChanged(IMatrixD oldTransform, IMatrixD newTransform)
		{
			EventHandler<MapPresentationPropertyChangedEventArgs<IMatrixD>> @event = MapViewTransformChanged;

			if (@event != null)
			{
                @event(this, new MapPresentationPropertyChangedEventArgs<IMatrixD>(oldTransform, newTransform));
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
					previousCenter, currentCenter, 
                    worldToView(previousCenter), 
                    worldToView(currentCenter)));
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

		#endregion
	}
}
