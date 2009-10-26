// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Collections;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Presentation.Presenters;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Presentation.Views
{
    /// <summary>
    /// Provides the interface definition for a 2 dimensional graphics surface
    /// on which to render features and rasters.
    /// </summary>
    public interface IMapView2D : IView
    {
        event EventHandler<MapActionEventArgs<Point2D>> Hover;
        event EventHandler<MapActionEventArgs<Point2D>> BeginAction;
        event EventHandler<MapActionEventArgs<Point2D>> MoveTo;
        event EventHandler<MapActionEventArgs<Point2D>> EndAction;
        event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>> GeoCenterChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Double>> MaximumWorldWidthChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Double>> MinimumWorldWidthChangeRequested;
        event EventHandler<LocationEventArgs> IdentifyLocationRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
        event EventHandler SizeChanged;
        event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ViewEnvelopeChangeRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Double>> WorldAspectRatioChangeRequested;
        event EventHandler ZoomToExtentsRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> ZoomToWorldBoundsRequested;
        event EventHandler<MapViewPropertyChangeEventArgs<Double>> ZoomToWorldWidthRequested;

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        StyleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets the view resolution in dots per inch.
        /// </summary>
        Double Dpi { get; }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        ICoordinate GeoCenter { get; set; }

        void IdentifyLocation(ICoordinate worldPoint);

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        Double MaximumWorldWidth { get; set; }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        Double MinimumWorldWidth { get; set; }

        /// <summary>
        /// Moves the view by the given vector.
        /// </summary>
        /// <param name="offsetVector">Amount and direction of the offset.</param>
        void Offset(Point2D offsetVector);

        /// <summary>
        /// Gets the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>
        /// The value returned is the same as <see cref="WorldUnitsPerPixel"/> 
        /// unless <see cref="WorldAspectRatio"/> is different from 1.
        /// </remarks>
        Double PixelWorldHeight { get; }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>
        /// The value returned is the same as <see cref="WorldUnitsPerPixel"/>.
        /// </remarks>
        Double PixelWorldWidth { get; }

        /// <summary>
        /// Gets the selection on a view, if one exists.
        /// </summary>
        ViewSelection2D Selection { get; }

        /// <summary>
        /// Draws the rendered object to the view.
        /// </summary>
        /// <param name="renderedObjects">The rendered objects to draw.</param>
        //void ShowRenderedObjects(IEnumerable renderedObjects);

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
        /// Creates a <see cref="Point2D"/> in view space from a
        /// <see cref="IPoint"/> in the map's world space.
        /// </summary>
        /// <param name="point">Point in the map's world space to transform.</param>
        /// <returns>
        /// A <see cref="Point2D"/> in view space coordinates which 
        /// corresponds to the given <paramref name="point"/> in map world space.
        /// </returns>
        Point2D ToView(ICoordinate point);

        /// <summary>
        /// Creates a <see cref="Point2D"/> in view space from X and Y
        /// coordinate values in the map's world space.
        /// </summary>
        /// <param name="x">X coordinate in the map's world space to transform.</param>
        /// <param name="y">Y coordinate in the map's world space to transform.</param>
        /// <returns>
        /// A <see cref="Point2D"/> in view space coordinates which 
        /// corresponds to the given (<paramref name="x"/>, <paramref name="y"/>) coordinate
        /// pair in map world space.
        /// </returns>
        Point2D ToView(Double x, Double y);

        /// <summary>
        /// Creates a <see cref="IPoint"/> in the map's world space from a
        /// <see cref="Point2D"/> in the view space.
        /// </summary>
        /// <param name="point">Point in the view space to transform.</param>
        /// <returns>
        /// A <see cref="IPoint"/> in the map's world space coordinates which 
        /// corresponds to the given <paramref name="point"/> in view space.
        /// </returns>
        ICoordinate ToWorld(Point2D point);

        /// <summary>
        /// Creates a <see cref="IPoint"/> in map world space from X and Y
        /// coordinate values in the view space.
        /// </summary>
        /// <param name="x">X coordinate in the view space to transform.</param>
        /// <param name="y">Y coordinate in the view space to transform.</param>
        /// <returns>
        /// A <see cref="IPoint"/> in view space coordinates which 
        /// corresponds to the given (<paramref name="x"/>, <paramref name="y"/>) coordinate
        /// pair in view space.
        /// </returns>
        ICoordinate ToWorld(Double x, Double y);

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        IExtents2D ViewEnvelope { get; set; }

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
        Double WorldAspectRatio { get; set; }

        /// <summary>
        /// Gets the height of view in world units.
        /// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="WorldAspectRatio"/> 
        /// (<see cref="WorldWidth"/> * <see cref="WorldAspectRatio"/> * 
        /// <see cref="BasePresenter{TView}.View"/> ViewSize height 
        /// / <see cref="BasePresenter{IMapView2D}.View"/> ViewSize width).
        /// </returns>
        Double WorldHeight { get; }

        /// <summary>
        /// Gets the width of view in world units.
        /// </summary>
        /// <returns>
        /// The width of the view in world units 
        /// (<see cref="BasePresenter{IMapView2D}.View" /> height 
        /// * <see cref="WorldUnitsPerPixel"/>).
        /// </returns>
        Double WorldWidth { get; }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        Double WorldUnitsPerPixel { get; }

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
        /// <see cref="IExtents"/> to set zoom to.
        /// </param>
        void ZoomToWorldBounds(IExtents2D zoomBox);

        /// <summary>
        /// Zooms the view to the given width.
        /// </summary>
        /// <remarks>
        /// View modifiers <see cref="MinimumWorldWidth"/>, 
        /// <see cref="MaximumWorldWidth"/> and <see cref="WorldAspectRatio"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        void ZoomToWorldWidth(Double newWorldWidth);


        IRasterizeSurface RasterizeSurface { get; }

        void Display(object viewData);

    }
}