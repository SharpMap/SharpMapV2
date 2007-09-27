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
using System.Collections;
using System.Collections.Generic;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Provides a base class for generating rendered objects from vector shapes.
    /// </summary>
    /// <remarks>
    /// This class is used to create a new IVectorRender2D for various graphics systems.
    /// </remarks>
    /// <typeparam name="TRenderObject">The type of rendered object to produce.</typeparam>
    public abstract class VectorRenderer2D<TRenderObject> : IVectorRenderer2D<TRenderObject>
    {
        #region Instance fields

        private Matrix2D _viewMatrix = new Matrix2D();
        private StyleRenderingMode _renderMode = StyleRenderingMode.Default;
        private bool _isDisposed = false;

        #endregion

        #region Object construction and disposal

        #region Dispose Pattern

        ~VectorRenderer2D()
        {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing) {}

        protected bool IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #endregion

        #endregion

        #region IRenderer<Point2D,ViewSize2D,Rectangle2D,TRenderObject> Members

        /// <summary>
        /// Gets or sets a matrix used to transform 
        /// coordinate values during rendering.
        /// </summary>
        public Matrix2D RenderTransform
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="StyleRenderingMode"/> 
        /// value used to render objects.
        /// </summary>
        public StyleRenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        #endregion

        #region IVectorRenderer2D Members

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> paths,
                                                               StylePen line, StylePen highlightLine,
                                                               StylePen selectLine,
                                                               StylePen outline, StylePen highlightOutline,
                                                               StylePen selectOutline);

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> paths, StylePen outline,
                                                               StylePen highlightOutline,
                                                               StylePen selectOutline);

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> paths, StyleBrush fill,
                                                               StyleBrush highlightFill,
                                                               StyleBrush selectFill, StylePen outline,
                                                               StylePen highlightOutline,
                                                               StylePen selectOutline);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                 ColorMatrix highlight,
                                                                 ColorMatrix select);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                 Symbol2D highlightSymbolData,
                                                                 Symbol2D selectSymbolData);

        #endregion

        #region Explicit Interface Implementation

        #region IRenderer<Point2D,ViewSize2D,Rectangle2D,TRenderObject> Members

        IMatrixD IRenderer.RenderTransform
        {
            get { return RenderTransform; }
            set
            {
                if (!(value is Matrix2D))
                {
                    throw new NotSupportedException("Only a Matrix2D is supported on a FeatureRenderer2D.");
                }

                RenderTransform = value as Matrix2D;
            }
        }

        #endregion

        #endregion

        #region IVectorRenderer2D Explicit Members

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<GraphicsPath2D> paths, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline)
        {
            return RenderPaths(paths, outline, highlightOutline, selectOutline);
        }

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<GraphicsPath2D> paths, StylePen line,
                                                  StylePen highlightLine, StylePen selectLine, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline)
        {
            return RenderPaths(paths, line, highlightLine, selectLine, outline, highlightOutline, selectOutline);
        }

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<GraphicsPath2D> paths, StyleBrush fill,
                                                  StyleBrush highlightFill, StyleBrush selectFill, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline)
        {
            return RenderPaths(paths, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData)
        {
            return RenderSymbols(locations, symbolData);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                    ColorMatrix highlight, ColorMatrix select)
        {
            return RenderSymbols(locations, symbolData, highlight, select);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                    Symbol2D highlightSymbolData, Symbol2D selectSymbolData)
        {
            return RenderSymbols(locations, symbolData, highlightSymbolData, selectSymbolData);
        }

        #endregion
    }
}