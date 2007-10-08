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
using SharpMap.Geometries;
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
        private static readonly Dictionary<IMapView2D, Point2D> _actionPositions
            = new Dictionary<IMapView2D, Point2D>();

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

        private static void DoNothing(ActionContext<IMapView2D, Point2D> context) {}

        #region Panning

        private static void QueryPan(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginPan(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }

            _actionPositions[context.MapView] = context.ActionArgs.ActionPoint;
        }

        private static void ContinuePan(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;
            Point2D previousPoint = _actionPositions[view];
            Point2D currentPoint = context.ActionArgs.ActionPoint;
            Point2D difference = previousPoint - currentPoint;

            _actionPositions[view] = currentPoint;

            Rectangle2D viewBounds = new Rectangle2D(difference, view.ViewSize);
            view.ZoomToViewBounds(viewBounds);
        }

        private static void EndPan(ActionContext<IMapView2D, Point2D> context)
        {
            _actionPositions.Remove(context.MapView);
        }

        #endregion

        #region Zoom in

        private static void QueryZoomIn(ActionContext<IMapView2D, Point2D> context) {}

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
                zoomByPercentage(context, true);
            }
        }

        #endregion

        #region Zoom out

        private static void QueryZoomOut(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginZoomOut(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueZoomOut(ActionContext<IMapView2D, Point2D> context) {}

        private static void EndZoomOut(ActionContext<IMapView2D, Point2D> context)
        {
            // Zoom out
            zoomByPercentage(context, false);
        }

        #endregion

        #region Query

        private static void QueryQuery(ActionContext<IMapView2D, Point2D> context)
        {
            Point2D point = context.ActionArgs.ActionPoint;
            Point worldPoint = context.MapView.ToWorld(point);
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
            BoundingBox worldBounds = new BoundingBox(
                view.ToWorld(viewBounds.LowerLeft), view.ToWorld(viewBounds.UpperRight));

            // Apply the GeometryFilter derived from the view's selection
            for (int i = context.Map.Layers.Count - 1; i >= 0; i--)
            {
                GeometryLayer layer = context.Map.Layers[i] as GeometryLayer;

                if (layer == null)
                {
                    continue;
                }

                if (layer.Enabled && layer.IsVisibleWhen(isInView(view.WorldWidth)))
                {
                    layer.SelectedFeatures.GeometryFilter = worldBounds.ToGeometry();
                }
            }
        }

        #endregion

        #region Query add

        private static void QueryQueryAdd(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginQueryAdd(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryAdd(ActionContext<IMapView2D, Point2D> context) {}

        private static void EndQueryAdd(ActionContext<IMapView2D, Point2D> context) {}

        #endregion

        #region Query remove

        private static void QueryQueryRemove(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginQueryRemove(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryRemove(ActionContext<IMapView2D, Point2D> context) {}

        private static void EndQueryRemove(ActionContext<IMapView2D, Point2D> context) {}

        #endregion

        #region Feature add

        private static void QueryFeatureAdd(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginFeatureAdd(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureAdd(ActionContext<IMapView2D, Point2D> context) {}

        private static void EndFeatureAdd(ActionContext<IMapView2D, Point2D> context) {}

        #endregion

        #region Feature remove

        private static void QueryFeatureRemove(ActionContext<IMapView2D, Point2D> context) {}

        private static void BeginFeatureRemove(ActionContext<IMapView2D, Point2D> context)
        {
            if (context.MapView.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureRemove(ActionContext<IMapView2D, Point2D> context) {}

        private static void EndFeatureRemove(ActionContext<IMapView2D, Point2D> context) {}

        #endregion

        private static Predicate<ILayer> isInView(double scale)
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

            if (view.GeoCenter == Point.Empty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }

            view.Selection.Clear();
            view.Selection.AddPoint(context.ActionArgs.ActionPoint);

            _actionPositions[view] = context.ActionArgs.ActionPoint;
        }

        private static void continueSelection(ActionContext<IMapView2D, Point2D> context)
        {
            context.MapView.Selection.AddPoint(context.ActionArgs.ActionPoint);
        }

        private static Rectangle2D endSelection(ActionContext<IMapView2D, Point2D> context)
        {
            IMapView2D view = context.MapView;

            _actionPositions.Remove(view);

            view.Selection.Close();

            return new Rectangle2D(
                view.Selection.Path.Bounds.Location, view.Selection.Path.Bounds.Size);
        }

        private static void zoomByPercentage(ActionContext<IMapView2D, Point2D> context, bool zoomIn)
        {
            IMapView2D view = context.MapView;

            double zoomPercentage;

            if (zoomIn)
            {
                zoomPercentage = .80;
            }
            else
	        {
                zoomPercentage = 1.20;
	        }

            Point2D middleLocation = new Point2D((view.ViewSize.Width / 2), (view.ViewSize.Height / 2));
            Point2D viewDifference = context.ActionArgs.ActionPoint - middleLocation;
            Point2D upperLeft = new Point2D(viewDifference.X * zoomPercentage, viewDifference.Y * zoomPercentage);

            //if (view.Selection.Path.Points.Count > 0)
            //{
            //    // Center the new view over the cursor
            //    upperLeft = new Point2D(viewDifference.X * zoomPercentage, viewDifference.Y * zoomPercentage);
            //}
            //else
            //{
            //    // If there are no points selected then we're zooming via the mouse wheel
            //    upperLeft = new Point2D(viewDifference.X * zoomPercentage, viewDifference.Y * zoomPercentage);
            //}

            Rectangle2D viewBounds =
                new Rectangle2D(upperLeft,
                    new Size2D(view.ViewSize.Width * zoomPercentage, view.ViewSize.Height * zoomPercentage));

            view.ZoomToViewBounds(viewBounds);
        }

        #endregion
    }
}
