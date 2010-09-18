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
using GeoAPI.Coordinates;
using SharpMap.Layers;
using SharpMap.Presentation.Presenters;
using SharpMap.Rendering.Wpf;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using wpfMatrix = System.Windows.Media.Matrix;
using GeoAPI.Geometries;

namespace SharpMap.Presentation.Windows
{
    internal class MapPresenter : MapPresenter2D
    {
        private Boolean _isRenderingSelection;
        private Boolean _isRenderingAll;

        internal MapPresenter(Map map, MapViewControl mapView)
            : base(map, mapView) { }

        internal MapViewControl ViewControl
        {
            get { return View as MapViewControl; }
        }

        protected override ITextRenderer2D CreateTextRenderer()
        {
            return new WpfTextRenderer();
        }

        protected override IRasterRenderer2D CreateRasterRenderer()
        {
            return new WpfRasterRenderer();
        }

        protected override IVectorRenderer2D CreateVectorRenderer()
        {
            return new WpfVectorRenderer();
        }

        protected override Type GetRenderObjectType()
        {
            return typeof(WpfRenderObject);
        }

        protected override void OnRenderingAllLayers()
        {
            _isRenderingAll = true;
            base.OnRenderingAllLayers();
        }

        protected override void OnRenderingAllLayersPhase(RenderPhase phase)
        {
            _isRenderingAll = true;
            base.OnRenderingAllLayersPhase(phase);
        }

        protected override void OnRenderedAllLayers()
        {
            base.OnRenderedAllLayers();

            if (!_isRenderingSelection)
            {
                ViewControl.InvalidateVisual();
            }

            _isRenderingAll = false;
        }

        protected override void OnRenderingSelection()
        {
            _isRenderingSelection = true;
            //RenderAllLayers();
        }

        protected override void OnRenderedSelection()
        {
            ViewControl.InvalidateVisual();
            _isRenderingSelection = false;
        }

        protected override void RenderFeatureLayer(IFeatureLayer layer, RenderPhase phase)
        {
            if (_isRenderingAll)
            {
                base.RenderFeatureLayer(layer, phase);
            }
            else
            {
                RenderAllLayers(phase);
            }
        }

        protected override void SetViewLocationInformation(String text)
        {
            ViewControl.Information = text;
        }

        #region MapViewControl accessible members

        internal StyleColor BackgroundColor
        {
            get { return BackgroundColorInternal; }
        }

        internal ICoordinate GeoCenter
        {
            get { return GeoCenterInternal; }
        }

        internal Double MaximumWorldWidth
        {
            get { return MaximumWorldWidthInternal; }
        }

        internal Double MinimumWorldWidth
        {
            get { return MinimumWorldWidthInternal; }
        }

        internal Double PixelWorldWidth
        {
            get { return PixelWorldWidthInternal; }
        }

        internal Double PixelWorldHeight
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

        internal Point2D ToView(ICoordinate point)
        {
            return ToViewInternal(point);
        }

        internal Point2D ToView(Double x, Double y)
        {
            return ToViewInternal(x, y);
        }

        internal ICoordinate ToWorld(Point2D point)
        {
            return ToWorldInternal(point);
        }

        internal ICoordinate ToWorld(Double x, Double y)
        {
            return ToWorldInternal(x, y);
        }

        internal IExtents2D ToWorld(Rectangle2D bounds)
        {
            return ToWorldInternal(bounds);
        }

        internal IExtents2D ViewEnvelope
        {
            get { return ViewEnvelopeInternal; }
        }

        internal Double WorldAspectRatio
        {
            get { return WorldAspectRatioInternal; }
        }

        internal Double WorldHeight
        {
            get { return WorldHeightInternal; }
        }

        internal Double WorldWidth
        {
            get { return WorldWidthInternal; }
        }

        internal Double WorldUnitsPerPixel
        {
            get { return WorldUnitsPerPixelInternal; }
        }

        public Boolean IsRenderingSelection
        {
            get
            {
                return _isRenderingSelection;
            }
        }

        internal void ZoomToExtents()
        {
            ZoomToExtentsInternal();
        }

        internal void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            ZoomToViewBoundsInternal(viewBounds);
        }

        internal void ZoomToWorldBounds(IExtents2D zoomBox)
        {
            ZoomToWorldBoundsInternal(zoomBox);
        }

        internal void ZoomToWorldWidth(Double newWorldWidth)
        {
            ZoomToWorldWidthInternal(newWorldWidth);
        }

        #endregion

        #region View property setters

        protected override void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor)
        {
            ViewControl.Background = ViewConverter.Convert(new SolidStyleBrush(toColor));
        }

        #endregion
    }
}