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
    public abstract class VectorRenderer2D<TRenderObject> : Renderer2D, IVectorRenderer2D<TRenderObject>
    {
        #region IVectorRenderer2D Members

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<Path2D> paths,
                                                               StylePen line, StylePen highlightLine,
                                                               StylePen selectLine,
                                                               StylePen outline, StylePen highlightOutline,
                                                               StylePen selectOutline, RenderState renderState);

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                               StylePen highlightOutline,
                                                               StylePen selectOutline, RenderState renderState);

        public abstract IEnumerable<TRenderObject> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                               StyleBrush highlightFill,
                                                               StyleBrush selectFill, StylePen outline,
                                                               StylePen highlightOutline,
                                                               StylePen selectOutline, RenderState renderState);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                 RenderState renderState);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                 ColorMatrix highlight,
                                                                 ColorMatrix select, RenderState renderState);

        public abstract IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                 Symbol2D highlightSymbolData,
                                                                 Symbol2D selectSymbolData, RenderState renderState);

        #endregion

        #region IVectorRenderer2D Explicit Members

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline,
                                                  RenderState renderState)
        {
            return RenderPaths(paths, outline, highlightOutline, selectOutline, renderState);
        }

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                  StylePen highlightLine, StylePen selectLine, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline,
                                                  RenderState renderState)
        {
            return RenderPaths(paths, line, highlightLine, selectLine, 
                outline, highlightOutline, selectOutline, renderState);
        }

        IEnumerable IVectorRenderer2D.RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                  StyleBrush highlightFill, StyleBrush selectFill, StylePen outline,
                                                  StylePen highlightOutline, StylePen selectOutline,
                                                  RenderState renderState)
        {
            return RenderPaths(paths, fill, highlightFill, selectFill, 
                outline, highlightOutline, selectOutline, renderState);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                    RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, renderState);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                    ColorMatrix highlight, ColorMatrix select, RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, highlight, select, renderState);
        }

        IEnumerable IVectorRenderer2D.RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                    Symbol2D highlightSymbolData, Symbol2D selectSymbolData,
                                                    RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, highlightSymbolData, selectSymbolData, renderState);
        }
        #endregion
    }
}