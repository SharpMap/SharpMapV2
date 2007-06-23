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
        #region Type Members
        private static volatile Symbol2D _defaultSymbol;

        static VectorRenderer2D()
        {
        }

        /// <summary>
        /// The default SharpMap symbol for rendering point data.
        /// </summary>
        public static Symbol2D DefaultSymbol
        {
            get 
            {
                if (_defaultSymbol == null)
                {
                    lock (_defaultSymbol)
                    {
                        if (_defaultSymbol == null)
                        {
                            Stream data = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
                            _defaultSymbol = new Symbol2D(data, new ViewSize2D(16, 16));
                        }
                    }
                }

                return _defaultSymbol; 
            }
        }
        #endregion

        private ViewMatrix2D _viewMatrix = new ViewMatrix2D();
        private StyleRenderingMode _renderMode = StyleRenderingMode.Default;
        private bool _disposed = false;

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
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {

        }

        protected bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }
        #endregion
        #endregion

        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members
        public ViewMatrix2D ViewTransform
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
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
        #endregion

        #region Explicit Interface Implementation
        #region IRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members

        IViewMatrix IRenderer.ViewTransform
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
        #endregion
    }
}
