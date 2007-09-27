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
using System.Data;
using System.Diagnostics;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Presenters;
using SharpMap.Rendering;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;

namespace SharpMap.Presentation.WinForms
{
	internal class MapPresenter : MapPresenter2D
	{
		internal MapPresenter(Map map, MapViewControl mapView)
			: base(map, mapView)
		{

		}

		internal MapViewControl ViewControl
		{
			get { return View as MapViewControl; }
        }

        protected override void RenderFeatureLayer(IFeatureLayer layer)
        {
            IFeatureRenderer<GdiRenderObject> renderer
                = GetRenderer<IFeatureRenderer<GdiRenderObject>>(layer);

            Debug.Assert(renderer != null);

            // TODO: measure the performance of this view creation
            FeatureDataView visibleFeatures = new FeatureDataView(
                layer.Features, ViewEnvelopeInternal.ToGeometry(), "", DataViewRowState.CurrentRows);

            foreach (FeatureDataRow feature in ((IEnumerable<FeatureDataRow>)visibleFeatures))
            {
                Debug.Assert(layer.Style is VectorStyle);
                ViewControl.ShowRenderedObjects(renderer.RenderFeature(feature, layer.Style as VectorStyle));
            }
        }

        protected override void RenderPath(GraphicsPath2D path)
        {
            IVectorRenderer2D<GdiRenderObject> renderer = (VectorRenderer as IVectorRenderer2D<GdiRenderObject>);

            View.ShowRenderedObjects(renderer.RenderPaths(new GraphicsPath2D[] { path }, View.Selection.OutlineStyle, View.Selection.OutlineStyle, View.Selection.OutlineStyle));
        }

        protected override void RenderRasterLayer(IRasterLayer layer)
        {
            throw new NotImplementedException();
        }

		protected override IRenderer CreateRasterRenderer()
        {
            return new GdiRasterRenderer();
		}

		protected override IRenderer CreateVectorRenderer()
		{
			return new GdiVectorRenderer();
		}

        protected override Type GetRenderObjectType()
        {
            return typeof (GdiRenderObject);
        }

		#region MapViewControl accessible members

		internal StyleColor BackgroundColor
		{
			get { return BackgroundColorInternal; }
		}

		internal Point GeoCenter
		{
			get { return GeoCenterInternal; }
		}

		internal double MaximumWorldWidth
		{
			get { return MaximumWorldWidthInternal; }
		}

		internal double MinimumWorldWidth
		{
			get { return MinimumWorldWidthInternal; }
		}

		internal double PixelWorldWidth
		{
			get { return PixelWorldWidthInternal; }
		}

		internal double PixelWorldHeight
		{
			get { return PixelWorldHeightInternal; }
		}

		internal ViewSelection2D Selection
		{
			get { return SelectionInternal; }
		}

		internal Matrix2D ToViewTransform
		{
			get { return ToViewTransformInternal; }
		}

		internal Matrix2D ToWorldTransform
		{
			get { return ToWorldTransformInternal; }
		}

        internal Point2D ToView(Point point)
        {
            return ToViewInternal(point);
        }

        internal Point2D ToView(double x, double y)
        {
            return ToViewInternal(x, y);
        }

        internal Point ToWorld(Point2D point)
        {
            return ToWorldInternal(point);
        }

        internal Point ToWorld(double x, double y)
        {
            return ToWorldInternal(x, y);
        }

		internal BoundingBox ViewEnvelope
		{
			get { return ViewEnvelopeInternal; }
		}

		internal double WorldAspectRatio
		{
			get { return WorldAspectRatioInternal; }
		}

		internal double WorldHeight
		{
			get { return WorldHeightInternal; }
		}

		internal double WorldWidth
		{
			get { return WorldWidthInternal; }
		}

		internal double WorldUnitsPerPixel
		{
			get { return WorldUnitsPerPixelInternal; }
		}

		internal void ZoomToExtents()
		{
			ZoomToExtentsInternal();
		}

		internal void ZoomToViewBounds(Rectangle2D viewBounds)
		{
			ZoomToViewBoundsInternal(viewBounds);
		}

		internal void ZoomToWorldBounds(BoundingBox zoomBox)
		{
			ZoomToWorldBoundsInternal(zoomBox);
		}

		internal void ZoomToWorldWidth(double newWorldWidth)
		{
			ZoomToWorldWidthInternal(newWorldWidth);
		}

		#endregion

        #region View property setters
        protected override void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor)
        {
            ViewControl.BackColor = ViewConverter.Convert(toColor);
        }
        #endregion
    }
}