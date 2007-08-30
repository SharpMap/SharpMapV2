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
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
	public interface IMapView2D : IView
	{
		event EventHandler<MapActionEventArgs<Point2D>> Hover;
		event EventHandler<MapActionEventArgs<Point2D>> BeginAction;
		event EventHandler<MapActionEventArgs<Point2D>> MoveTo;
		event EventHandler<MapActionEventArgs<Point2D>> EndAction;
		event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<Point>> GeoCenterChangeRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<double>> MaximumWorldWidthChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<double>> MinimumWorldWidthChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<Size2D>> SizeChangeRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ViewEnvelopeChangeRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<double>> WorldAspectRatioChangeRequested;
		event EventHandler ZoomToExtentsRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ZoomToWorldBoundsRequested;
		event EventHandler<MapViewPropertyChangeEventArgs<double>> ZoomToWorldWidthRequested;

		/// <summary>
		/// Gets or sets map background color.
		/// </summary>
		StyleColor BackgroundColor { get; set; }

		/// <summary>
		/// Gets the view resolution in dots per inch.
		/// </summary>
		double Dpi { get; }

		/// <summary>
		/// Gets or sets center of map in world coordinates.
		/// </summary>
		Point GeoCenter { get; set; }
		
		/// <summary>
		/// Gets or sets the minimum width in world units of the view.
		/// </summary>
		double MaximumWorldWidth { get; set; }
		
		/// <summary>
		/// Gets or sets the minimum width in world units of the view.
		/// </summary>
		double MinimumWorldWidth { get; set; }

	    void Offset(Point2D offsetVector);
		
		/// <summary>
		/// Gets the width of a pixel in world coordinate units.
		/// </summary>
		/// <remarks>
		/// The value returned is the same as <see cref="WorldUnitsPerPixel"/>.
		/// </remarks>
		double PixelWorldWidth { get; }

		/// <summary>
		/// Gets the height of a pixel in world coordinate units.
		/// </summary>
		/// <remarks>
		/// The value returned is the same as <see cref="WorldUnitsPerPixel"/> 
		/// unless <see cref="WorldAspectRatio"/> is different from 1.
		/// </remarks>
		double PixelWorldHeight { get; }
		
		/// <summary>
		/// Gets the selection on a view, if one exists.
		/// </summary>
		ViewSelection2D Selection { get; }

		/// <summary>
		/// Draws a rendered object at the given location.
		/// </summary>
		/// <param name="location">Location of the upper-left corner of the object.</param>
		/// <param name="renderedObject">The object to draw.</param>
		void ShowRenderedObject(Point2D location, object renderedObject);
		
		/// <summary>
		/// Gets a <see cref="Matrix2D"/> used to project the world
		/// coordinate system into the view coordinate system. 
		/// The inverse of the <see cref="ToWorldTransform"/> matrix.
		/// </summary>
		Matrix2D ToViewTransform { get; }
		
		/// <summary>
		/// Gets a <see cref="Matrix2D"/> used to reverse the view projection
		/// transform to get world coordinates.
		/// The inverse of the <see cref="ToViewTransform"/> matrix.
		/// </summary>
		Matrix2D ToWorldTransform { get; }
		
		/// <summary>
		/// Gets or sets the extents of the current view in world units.
		/// </summary>
		BoundingBox ViewEnvelope { get; set; }

		/// <summary>
		/// Gets or sets the size of the view.
		/// </summary>
		Size2D ViewSize { get; set; }

		/// <summary>
		/// Gets or sets the aspect ratio of the <see cref="WorldHeight"/> 
		/// to the <see cref="WorldWidth"/>.
		/// </summary>
		/// <remarks> 
		/// A value less than 1 will make the map stretch upwards 
		/// (the view will cover less world distance vertically), 
		/// and greater than 1 will make it more squat (the view will 
		/// cover more world distance vertically).
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws an exception when value is 0 or less.
		/// </exception>
		double WorldAspectRatio { get; set; }
		
		/// <summary>
		/// Gets the height of view in world units.
		/// </summary>
		/// <returns>
		/// The height of the view in world units, taking into account <see cref="WorldAspectRatio"/> 
		/// (<see cref="WorldWidth"/> * <see cref="WorldAspectRatio"/> * 
		/// <see cref="BasePresenter{IMapView2D}.View"/> ViewSize height 
		/// / <see cref="BasePresenter{IMapView2D}.View"/> ViewSize width).
		/// </returns>
		double WorldHeight { get; }

		/// <summary>
		/// Gets the width of view in world units.
		/// </summary>
		/// <returns>
		/// The width of the view in world units 
		/// (<see cref="BasePresenter{IMapView2D}.View" /> height 
		/// * <see cref="WorldUnitsPerPixel"/>).
		/// </returns>
		double WorldWidth { get; }
		
		/// <summary>
		/// Gets the width of a pixel in world coordinate units.
		/// </summary>
		double WorldUnitsPerPixel { get; }

		/// <summary>
		/// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
		/// </summary>
		void ZoomToExtents();

		/// <summary>
		/// Zooms the map to fit a view bounding box. 
		/// </summary>
		/// <remarks>
		/// Transforms view coordinates into
		/// world coordinates using <see cref="ToWorldTransform"/> to perform zoom. 
		/// This means the heuristic to determine the final value of <see cref="ViewEnvelope"/>
		/// after the zoom is the same as in <see cref="ZoomToWorldBounds"/>.
		/// </remarks>
		/// <param name="viewBounds">
		/// The view bounds, translated into world bounds,
		/// to set the zoom to.
		/// </param>
		void ZoomToViewBounds(Rectangle2D viewBounds);

		/// <summary>
		/// Zooms the map to fit a world bounding box.
		/// </summary>
		/// <remarks>
		/// The <paramref name="zoomBox"/> value is stretched
		/// to fit the current view. For example, if a view has a size of
		/// 100 x 100, which is a 1:1 ratio, and the bounds of zoomBox are 
		/// 200 x 100, which is a 2:1 ratio, the bounds are stretched to be 
		/// 200 x 200 to match the view size ratio. This ensures that at least
		/// all of the area selected in the zoomBox is displayed, and possibly more
		/// area.
		/// </remarks>
		/// <param name="zoomBox">
		/// <see cref="BoundingBox"/> to set zoom to.
		/// </param>
		void ZoomToWorldBounds(BoundingBox zoomBox);

		/// <summary>
		/// Zooms the view to the given width.
		/// </summary>
		/// <remarks>
		/// View modifiers <see cref="MinimumWorldWidth"/>, 
		/// <see cref="MaximumWorldWidth"/> and <see cref="WorldAspectRatio"/>
		/// are taken into account when zooming to this width.
		/// </remarks>
		void ZoomToWorldWidth(double newWorldWidth);
	}
}