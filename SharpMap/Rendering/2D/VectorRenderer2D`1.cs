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
using System.IO;
using System.Reflection;

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
        #region Fields
        private Matrix2D _viewMatrix = new Matrix2D();
        private StyleRenderingMode _renderMode = StyleRenderingMode.Default;
        private bool _disposed = false;
        #endregion

        #region Object Construction/Destruction
        public VectorRenderer2D()
        {
        }

        ~VectorRenderer2D()
        {
            Dispose(false);
        }

        #region Dispose Pattern
        #region IDisposable Members

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {

        }

        protected bool IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }
        #endregion
        #endregion

        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members
        public Matrix2D RenderTransform
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }

        public StyleRenderingMode StyleRenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }
        #endregion

        #region IVectorLayerRenderer Members
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderSymbol(Point2D location, Symbol2D symbolData);
        public abstract TRenderObject RenderSymbol(Point2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        public abstract TRenderObject RenderSymbol(Point2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
        #endregion

        #region Explicit Interface Implementation
        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members

        IMatrixD IRenderer.RenderTransform
        {
            get
            {
                return RenderTransform;
            }
            set
            {
                if (!(value is Matrix2D))
                {
                    throw new NotSupportedException("Only a ViewMatrix2D is supported on a FeatureRenderer2D.");
                }

                RenderTransform = value as Matrix2D;
            }
        }

        #endregion
        #endregion
    }
}
