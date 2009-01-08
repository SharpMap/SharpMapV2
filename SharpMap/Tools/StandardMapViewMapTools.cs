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
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NPack.Interfaces;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Rendering;

namespace SharpMap.Tools
{
    /// <summary>
    /// Provides a set of standard tools to use on an <see cref="IMapView{TCoordinate}"/>.
    /// </summary>
    public static class StandardMapViewMapTools<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        /// <summary>
        /// No active tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> None;

        /// <summary>
        /// Pan
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> Pan;

        /// <summary>
        /// Zoom in
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> ZoomIn;

        /// <summary>
        /// Zoom out
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> ZoomOut;

        /// <summary>
        /// Query tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> Query;

        /// <summary>
        /// QueryAdd tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> QueryAdd;

        /// <summary>
        /// QueryRemove tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> QueryRemove;

        /// <summary>
        /// Add feature tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> FeatureAdd;

        /// <summary>
        /// Remove feature tool
        /// </summary>
        public static readonly MapTool<IMapView<TCoordinate>, TCoordinate> FeatureRemove;


        static StandardMapViewMapTools()
        {
            None = new MapTool<IMapView<TCoordinate>, TCoordinate>(String.Empty, DoNothing, DoNothing, DoNothing, DoNothing);
            Pan = new MapTool<IMapView<TCoordinate>, TCoordinate>("Pan", QueryPan, BeginPan, ContinuePan, EndPan);
            ZoomIn = new MapTool<IMapView<TCoordinate>, TCoordinate>("ZoomIn", QueryZoomIn, BeginZoomIn, ContinueZoomIn, EndZoomIn);
            ZoomOut = new MapTool<IMapView<TCoordinate>, TCoordinate>("ZoomOut", 
                                                       QueryZoomOut, 
                                                       BeginZoomOut, 
                                                       ContinueZoomOut, 
                                                       EndZoomOut);
            Query = new MapTool<IMapView<TCoordinate>, TCoordinate>("Query", QueryQuery, BeginQuery, ContinueQuery, EndQuery);
            QueryAdd = new MapTool<IMapView<TCoordinate>, TCoordinate>("QueryAdd", 
                                                        QueryQueryAdd, 
                                                        BeginQueryAdd, 
                                                        ContinueQueryAdd, 
                                                        EndQueryAdd);
            QueryRemove = new MapTool<IMapView<TCoordinate>, TCoordinate>("QueryRemove", 
                                                           QueryQueryRemove, 
                                                           BeginQueryRemove, 
                                                           ContinueQueryRemove,
                                                           EndQueryRemove);
            FeatureAdd = new MapTool<IMapView<TCoordinate>, TCoordinate>("FeatureAdd", 
                                                          QueryFeatureAdd, 
                                                          BeginFeatureAdd, 
                                                          ContinueFeatureAdd,
                                                          EndFeatureAdd);
            FeatureRemove = new MapTool<IMapView<TCoordinate>, TCoordinate>("FeatureRemove", 
                                                             QueryFeatureRemove, 
                                                             BeginFeatureRemove,
                                                             ContinueFeatureRemove, 
                                                             EndFeatureRemove);
        }

        private static void DoNothing(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        #region Panning

        private static void QueryPan(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginPan(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinuePan(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            IMapView<TCoordinate> view = context.MapView;
            TCoordinate previousPoint = context.PreviousPoint;
            TCoordinate currentPoint = context.CurrentPoint;
            TCoordinate difference = previousPoint.Subtract(currentPoint);
            view.Offset(difference);
        }

        private static void EndPan(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
        }

        #endregion

        #region Zoom in

        private static void QueryZoomIn(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginZoomIn(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            beginSelection(context);
        }

        private static void ContinueZoomIn(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            continueSelection(context);
        }

        private static void EndZoomIn(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            if (context.MapView.Selection.Path.Points.Count > 1)
            {
                Rectangle<TCoordinate> viewBounds = endSelection(context);

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

        private static void QueryZoomOut(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginZoomOut(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueZoomOut(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void EndZoomOut(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // Zoom out
            zoomByFactor(context, 0.833333333333333);
        }

        #endregion

        #region Query

        private static void QueryQuery(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            TCoordinate point = context.CurrentPoint;
            ICoordinate worldPoint = context.MapView.ToWorld(point);
            context.MapView.IdentifyLocation(worldPoint);
        }

        /// <summary>
        /// Clear the view's current selection before starting a new one.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView{TCoordinate}, TCoordinate}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void BeginQuery(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            beginSelection(context);
        }

        /// <summary>
        /// Add the current point to the view's selection.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView<TCoordinate>, TCoordinate}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void ContinueQuery(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            continueSelection(context);
        }

        /// <summary>
        /// Close the view's selection and set the map's GeometryFilter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ActionContext{IMapView<TCoordinate>, TCoordinate}"/> which provides 
        /// information about where, and on which view, the action occurred.
        /// </param>
        private static void EndQuery(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            IMapView<TCoordinate> view = context.MapView;

            Rectangle<TCoordinate> viewBounds = endSelection(context);

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

		private static void filterSelected(ILayer layer, IMapView<TCoordinate> view, IExtents worldBounds)
		{
			if (layer == null)
			{
				return;
			}

            FeatureLayer filterLayer = layer as FeatureLayer;

			if (filterLayer != null)
			{
				if (layer.Enabled && 
                    filterLayer.IsInteractive && 
                    layer.IsVisibleWhen(isInView(view.WorldWidth)))
				{
                    SpatialBinaryExpression spatialExpression 
                        = SpatialBinaryExpression.Intersects(new FeaturesCollectionExpression(filterLayer.Features),
				                                             new ExtentsExpression(worldBounds));
					filterLayer.SelectedFilter = filterLayer.SelectedFilter == null 
                        ? new FeatureQueryExpression(new AllAttributesExpression(), spatialExpression)
                        : new FeatureQueryExpression(filterLayer.SelectedFilter, spatialExpression);
				}
			}

		    IEnumerable<ILayer> layers = layer as IEnumerable<ILayer>;

            if (layers != null)
			{
				foreach (ILayer child in layers)
				{
					filterSelected(child, view, worldBounds);
				}
			}
		}

        #endregion

        #region Query add

        private static void QueryQueryAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginQueryAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void EndQueryAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        #endregion

        #region Query remove

        private static void QueryQueryRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginQueryRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueQueryRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void EndQueryRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        #endregion

        #region Feature add

        private static void QueryFeatureAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginFeatureAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void EndFeatureAdd(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        #endregion

        #region Feature remove

        private static void QueryFeatureRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void BeginFeatureRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            // NOTE: changed Point.Empty to null
            if (context.MapView.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }
        }

        private static void ContinueFeatureRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

        private static void EndFeatureRemove(ActionContext<IMapView<TCoordinate>, TCoordinate> context) { }

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
        private static void beginSelection(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            IMapView<TCoordinate> view = context.MapView;

            // NOTE: changed Point.Empty to null
            if (view.GeoCenter == null)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }

            view.Selection.Clear();
            view.Selection.AddPoint(context.CurrentPoint);
        }

        private static void continueSelection(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            context.MapView.Selection.AddPoint(context.CurrentPoint);
        }

        private static Rectangle<TCoordinate> endSelection(ActionContext<IMapView<TCoordinate>, TCoordinate> context)
        {
            IMapView<TCoordinate> view = context.MapView;

            view.Selection.Close();

            return view.Selection.Path.Bounds;
        }

        private static void zoomByFactor(ActionContext<IMapView<TCoordinate>, TCoordinate> context, Double zoomFactor)
        {
            IMapView<TCoordinate> view = context.MapView;
            zoomFactor = 1 / zoomFactor;

            Size<TCoordinate> viewSize = view.ViewSize;
            TCoordinate viewCenter = new TCoordinate((viewSize.Width / 2), (viewSize.Height / 2));
            TCoordinate viewDifference = context.CurrentPoint.Subtract(viewCenter);

            TCoordinate zoomUpperLeft = new TCoordinate(viewDifference[Ordinates.X] * zoomFactor, viewDifference[Ordinates.Y] * zoomFactor);
            Size<TCoordinate> zoomBoundsSize = new Size<TCoordinate>(viewSize.Width * zoomFactor, viewSize.Height * zoomFactor);
            Rectangle<TCoordinate> zoomViewBounds = new Rectangle<TCoordinate>(zoomUpperLeft, zoomBoundsSize);

            view.ZoomToViewBounds(zoomViewBounds);
        }

        #endregion
    }
}
