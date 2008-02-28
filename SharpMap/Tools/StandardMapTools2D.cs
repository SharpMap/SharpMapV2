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
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Tools
{
    /// <summary>
    /// Provides a set of standard tools to use on a map.
    /// </summary>
    public static class StandardMapTools2D
    {
        /// <summary>
        /// No active tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> None;

        /// <summary>
        /// Pan
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> Pan;

        /// <summary>
        /// Zoom in
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> ZoomIn;

        /// <summary>
        /// Zoom out
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> ZoomOut;

        /// <summary>
        /// Query tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> Query;

        /// <summary>
        /// QueryAdd tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> QueryAdd;

        /// <summary>
        /// QueryRemove tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> QueryRemove;

        /// <summary>
        /// Add feature tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> FeatureAdd;

        /// <summary>
        /// Remove feature tool
        /// </summary>
        public static readonly MapTool<IMapView2D, Point2D> FeatureRemove;


        static StandardMapTools2D()
        {
            None = new MapTool<IMapView2D, Point2D>(String.Empty, DoNothing, DoNothing, DoNothing, DoNothing);
            Pan = new MapTool<IMapView2D, Point2D>("Pan", QueryPan, BeginPan, ContinuePan, EndPan);
            ZoomIn = new MapTool<IMapView2D, Point2D>("ZoomIn", QueryZoomIn, BeginZoomIn, ContinueZoomIn, EndZoomIn);
            ZoomOut =
                new MapTool<IMapView2D, Point2D>("ZoomOut", QueryZoomOut, BeginZoomOut, ContinueZoomOut, EndZoomOut);
            Query = new MapTool<IMapView2D, Point2D>("Query", QueryQuery, BeginQuery, ContinueQuery, EndQuery);
            QueryAdd =
                new MapTool<IMapView2D, Point2D>("QueryAdd", QueryQueryAdd, BeginQueryAdd, ContinueQueryAdd, EndQueryAdd);
            QueryRemove =
                new MapTool<IMapView2D, Point2D>("QueryRemove", QueryQueryRemove, BeginQueryRemove, ContinueQueryRemove,
                                                 EndQueryRemove);
            FeatureAdd =
                new MapTool<IMapView2D, Point2D>("FeatureAdd", QueryFeatureAdd, BeginFeatureAdd, ContinueFeatureAdd,
                                                 EndFeatureAdd);
            FeatureRemove =
                new MapTool<IMapView2D, Point2D>("FeatureRemove", QueryFeatureRemove, BeginFeatureRemove,
                                                 ContinueFeatureRemove, EndFeatureRemove);
        }

        private static void DoNothing(ActionContext<IMapView2D, Point2D> context) { }

        #region Panning

        private static void QueryPan(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginPan(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinuePan(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;
            Point2D previousPoint = context.PreviousPoint;
            Point2D currentPoint = context.CurrentPoint;
            Point2D difference = previousPoint - currentPoint;
            view.Offset(difference);
        }

        private static void EndPan(ActionContext<IMapView2D, Point2D> context)
        {
        }

        #endregion

        #region Zoom in

        private static void QueryZoomIn(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            beginSelection(context);
        }

        private static void ContinueZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            continueSelection(context);
        }

        private static void EndZoomIn(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.Selection.Path.Points.Count > 1)
            {
                Rectangle2D viewBounds = endSelection(context);

                context.MapView.ZoomToViewBounds(viewBounds);
            }
            else
            {
                // Zoom in
                zoomByFactor(context, 1.2);
            }
        }

        #endregion

        #region Zoom out

        private static void QueryZoomOut(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginZoomOut(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueZoomOut(ActionContext<IMapView2D, Point2D> context) { }

        private static void EndZoomOut(ActionContext<IMapView2D, Point2D> context)
        {
            // Zoom out
            zoomByFactor(context, 0.833333333333333);
        }

        #endregion

        #region Query

        private static void QueryQuery(ActionContext<IMapView2D, Point2D> context)
        {
            Point2D point = context.CurrentPoint;
            ICoordinate worldPoint = context.MapView.ToWorld(point);
            context.MapView.IdentifyLocation(worldPoint);
        }

        /// <summary>
        /// Clear the view's current selection before starting a new one.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView2D, Point2D}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void BeginQuery(ActionContext<IMapView2D, Point2D> context)
        {
            beginSelection(context);
        }

        /// <summary>
        /// Add the current point to the view's selection.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView2D, Point2D}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void ContinueQuery(ActionContext<IMapView2D, Point2D> context)
        {
            continueSelection(context);
        }

        /// <summary>
        /// Close the view's selection and set the map's GeometryFilter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView2D, Point2D}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void EndQuery(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;

            Rectangle2D viewBounds = endSelection(context);

            // Create a BoundingBox for the view's selection using the map's world space
            IExtents worldBounds = context.Map.GeometryFactory.CreateExtents(
                                        view.ToWorld(viewBounds.LowerLeft), 
                                        view.ToWorld(viewBounds.UpperRight));

            // Apply the GeometryFilter derived from the view's selection
            for (Int32 i = context.Map.Layers.Count - 1; i >= 0; i--)
            {
				filterSelected(context.Map.Layers[i], view, worldBounds);
            }
        }

		private static void filterSelected(ILayer layer, IMapView2D view, IExtents worldBounds)
		{
			if (layer == null)
			{
				return;
			}

			GeometryLayer filterLayer;

			if (layer is GeometryLayer)
			{
				filterLayer = layer as GeometryLayer;

				Debug.Assert(filterLayer.Style != null);

				if (layer.Enabled && (filterLayer.Style.AreFeaturesSelectable) && layer.IsVisibleWhen(isInView(view.WorldWidth)))
				{
					filterLayer.SelectedFeatures.GeometryFilter = worldBounds.ToGeometry();
				}
			}
			else if (layer is LayerGroup)
			{
				foreach (ILayer glayer in (layer as LayerGroup).Layers)
				{
					filterSelected(glayer, view, worldBounds);
				}
			}
		}

        #endregion

        #region Query add

        private static void QueryQueryAdd(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginQueryAdd(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryAdd(ActionContext<IMapView2D, Point2D> context) { }

        private static void EndQueryAdd(ActionContext<IMapView2D, Point2D> context) { }

        #endregion

        #region Query remove

        private static void QueryQueryRemove(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginQueryRemove(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryRemove(ActionContext<IMapView2D, Point2D> context) { }

        private static void EndQueryRemove(ActionContext<IMapView2D, Point2D> context) { }

        #endregion

        #region Feature add

        private static void QueryFeatureAdd(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginFeatureAdd(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureAdd(ActionContext<IMapView2D, Point2D> context) { }

        private static void EndFeatureAdd(ActionContext<IMapView2D, Point2D> context) { }

        #endregion

        #region Feature remove

        private static void QueryFeatureRemove(ActionContext<IMapView2D, Point2D> context) { }

        private static void BeginFeatureRemove(ActionContext<IMapView2D, Point2D> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureRemove(ActionContext<IMapView2D, Point2D> context) { }

        private static void EndFeatureRemove(ActionContext<IMapView2D, Point2D> context) { }

        #endregion

        private static Predicate<ILayer> isInView(Double scale)
        {
            return delegate(ILayer layer)
                   {
                       return layer.Style.MaxVisible >= scale &&
                              layer.Style.MinVisible <= scale;
                   };
        }

        #region Private Helper Methods
        private static void beginSelection(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;

            // NOTE: changed Point.Empty to null
            if (view.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }

            view.Selection.Clear();
            view.Selection.AddPoint(context.CurrentPoint);
        }

        private static void continueSelection(ActionContext<IMapView2D, Point2D> context)
        {
            context.MapView.Selection.AddPoint(context.CurrentPoint);
        }

        private static Rectangle2D endSelection(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;

            view.Selection.Close();

            return view.Selection.Path.Bounds;
        }

        private static void zoomByFactor(ActionContext<IMapView2D, Point2D> context, Double zoomFactor)
        {
            IMapView2D view = context.MapView;
            zoomFactor = 1 / zoomFactor;

            Size2D viewSize = view.ViewSize;
            Point2D viewCenter = new Point2D((viewSize.Width / 2), (viewSize.Height / 2));
            Point2D viewDifference = context.CurrentPoint - viewCenter;

            Point2D zoomUpperLeft = new Point2D(viewDifference.X * zoomFactor, viewDifference.Y * zoomFactor);
            Size2D zoomBoundsSize = new Size2D(viewSize.Width * zoomFactor, viewSize.Height * zoomFactor);
            Rectangle2D zoomViewBounds = new Rectangle2D(zoomUpperLeft, zoomBoundsSize);

            view.ZoomToViewBounds(zoomViewBounds);
        }

        #endregion
    }
}
