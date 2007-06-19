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
using SharpMap.Layers;
using SharpMap.Geometries;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public abstract class BaseFeatureRenderer2D<TStyle, TRenderObject> : IGeometryRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, PositionedRenderObject2D<TRenderObject>>
        where TStyle : class, IStyle
    {
        private ViewMatrix2D _viewTransform;
        private StyleRenderingMode _renderMode;

        ~BaseFeatureRenderer2D()
        {
            Dispose(false);
        }

        #region Events
        ///// <summary>
        ///// Event fired when the layer has been rendered.
        ///// </summary>
        //public event EventHandler<LayerRenderedEventArgs> LayerRendered;

        /// <summary>
        /// Event fired when a feature has been rendered.
        /// </summary>
        public event EventHandler FeatureRendered;
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        #endregion

        #region IGeometryRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public IEnumerable<PositionedRenderObject2D<TRenderObject>> RenderGeometry(IGeometry geometry, TStyle style)
        {
            IEnumerable<PositionedRenderObject2D<TRenderObject>> renderedObjects = DoRenderGeometry(geometry, style);
            OnFeatureRendered();
            return renderedObjects;
        }

        /// <summary>
        /// Template method to perform the actual geometry rendering.
        /// </summary>
        /// <param name="geometry">Geometry to render.</param>
        /// <param name="style">Style to use in rendering geometry.</param>
        /// <returns></returns>
        protected abstract IEnumerable<PositionedRenderObject2D<TRenderObject>> DoRenderGeometry(IGeometry geometry, TStyle style);

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines 
        /// and curves and the edges of filled areas.
        /// </summary>
        public StyleRenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        /// <summary>
        /// Gets or sets a matrix used to transform world coordinates to graphical display coordinates.
        /// </summary>
        public ViewMatrix2D ViewTransform
        {
            get { return _viewTransform; }
            set { _viewTransform = value; }
        }
        #endregion

        #region Private helper methods
        /// <summary>
        /// Called when a feature is rendered.
        /// </summary>
        private void OnFeatureRendered()
        {
            EventHandler @event = FeatureRendered;
            if (@event != null)
            {
                @event(this, EventArgs.Empty); //Fire event
            }
        }
        #endregion

        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,PositionedRenderObject2D<TRenderObject>> Members

        IViewMatrix IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, PositionedRenderObject2D<TRenderObject>>.ViewTransform
        {
            get
            {
                return ViewTransform;
            }
            set
            {
                if (!(value is ViewMatrix2D))
                {
                    throw new NotSupportedException("Only a ViewMatrix2D is supported on a FeatureRenderer2D.");
                }

                ViewTransform = value as ViewMatrix2D;
            }
        }

        #endregion

        #region IGeometryRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,PositionedRenderObject2D<TRenderObject>> Members

        public IEnumerable<PositionedRenderObject2D<TRenderObject>> RenderGeometry(IGeometry geometry, IStyle style)
        {
            return RenderGeometry(geometry, style as TStyle);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
