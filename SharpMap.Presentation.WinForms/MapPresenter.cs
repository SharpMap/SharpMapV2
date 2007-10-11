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
using System.Diagnostics;
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
        private bool _isRenderingSelection = false;
        private bool _isRenderingAll = false;
        private Label2D _viewInfo;

        internal MapPresenter(Map map, MapViewControl mapView)
            : base(map, mapView)
        {
        }

        internal MapViewControl ViewControl
        {
            get { return View as MapViewControl; }
        }

        protected override ITextRenderer2D CreateTextRenderer()
        {
            return new GdiTextRenderer();
        }

        protected override IRasterRenderer2D CreateRasterRenderer()
        {
            return new GdiRasterRenderer();
        }

        protected override IVectorRenderer2D CreateVectorRenderer()
        {
            return new GdiVectorRenderer();
        }

        protected override Type GetRenderObjectType()
        {
            return typeof(GdiRenderObject);
        }

        protected override void OnViewMatrixInitialized()
        {
            initializeInfoLabel();

            base.OnViewMatrixInitialized();
        }

        protected override void OnRenderingAllLayers()
        {
            _isRenderingAll = true;
            base.OnRenderingAllLayers();
        }

        protected override void OnRenderedAllLayers()
        {
            base.OnRenderedAllLayers();

            //GdiLabelRenderer renderer = GetRenderer<GdiLabelRenderer, LabelLayer>();
            
            //Debug.Assert(renderer != null);

            //Size2D labelSize = renderer.MeasureString(_viewInfo.Text, _viewInfo.Font);
            //BoundingBox worldLabelArea = ToWorld(new Rectangle2D(_viewInfo.Location, labelSize));
            //Rectangle2D layoutRectangle =
            //    new Rectangle2D(worldLabelArea.Left, worldLabelArea.Top, worldLabelArea.Right, worldLabelArea.Bottom);
            //StyleFont font = _viewInfo.Font;
            //Point fontSize = ToWorld(font.Size.Width, font.Size.Height);
            //font.Size = new Size2D(fontSize.X, fontSize.Y);

            //IEnumerable<GdiRenderObject> renderedText = renderer.RenderLabel(
            //    _viewInfo.Text, font, layoutRectangle, null, 
            //    _viewInfo.Style.Foreground, _viewInfo.Style.Background, _viewInfo.Style.Halo, 
            //    _viewInfo.Transform);

            //ViewControl.ShowRenderedObjects(renderedText);

            if (!_isRenderingSelection)
            {
                ViewControl.Invalidate();
            }

            _isRenderingAll = false;
        }

        protected override void OnRenderingSelection()
        {
            _isRenderingSelection = true;
            RenderAllLayers();
            _isRenderingSelection = false;
        }

        protected override void OnRenderedSelection()
        {
            ViewControl.Invalidate();
        }

        protected override void RenderFeatureLayer(IFeatureLayer layer)
        {
            if (_isRenderingAll)
            {
                base.RenderFeatureLayer(layer);
            }
            else
            {
                RenderAllLayers();
            }
        }

        protected override void SetViewLocationInformation(string text)
        {
            _viewInfo.Text = text;
            RenderAllLayers();
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

        internal BoundingBox ToWorld(Rectangle2D bounds)
        {
            return ToWorldInternal(bounds);
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

        private void initializeInfoLabel()
        {
            Point2D infoLocation = new Point2D(0, ViewControl.Height - 12);
            LabelStyle style = new LabelStyle();
            style.Font = new StyleFont(new StyleFontFamily("Arial"), new Size2D(12, 12), StyleFontStyle.Regular);
            _viewInfo = new Label2D(String.Empty, infoLocation, 0, 0, Size2D.Empty, style);
        }
    }
}