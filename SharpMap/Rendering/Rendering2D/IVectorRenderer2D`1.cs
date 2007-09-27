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

using System.Collections.Generic;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Interface for renderers which take computed graphics paths and produce rendered objects
    /// suitable for display.
    /// </summary>
    /// <typeparam name="TRenderObject">The type of rendered object.</typeparam>
    public interface IVectorRenderer2D<TRenderObject> : IRenderer
    {
        /// <summary>
        /// Renders a set of <see cref="GraphicsPath2D"/> instances into a set of rendered objects.
        /// </summary>
        /// <param name="paths">The paths to render.</param>
        /// <param name="outline">Style of the path outline.</param>
        /// <param name="highlightOutline">Style of the path outline when highlighted.</param>
        /// <param name="selectOutline">Style of the path outline when selected.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> paths, StylePen outline,
                                               StylePen highlightOutline, StylePen selectOutline);

        /// <summary>
        /// Renders a set of <see cref="GraphicsPath2D"/> instances into a set of rendered objects.
        /// </summary>
        /// <param name="paths">The paths to render.</param>
        /// <param name="line">The style of the path line.</param>
        /// <param name="highlightLine">The style of the path line when highlighted.</param>
        /// <param name="selectLine">The style of the path line when selected.</param>
        /// <param name="outline">Style of the path line outline.</param>
        /// <param name="highlightOutline">Style of the path line outline when highlighted.</param>
        /// <param name="selectOutline">Style of the path line outline when selected.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> paths,
                                               StylePen line, StylePen highlightLine,
                                               StylePen selectLine,
                                               StylePen outline, StylePen highlightOutline,
                                               StylePen selectOutline);

        /// <summary>
        /// Renders a set of <see cref="GraphicsPath2D"/> instances into a set of rendered objects.
        /// </summary>
        /// <param name="path">The paths to render.</param>
        /// <param name="fill">The style of the path fill.</param>
        /// <param name="highlightFill">The style of the path fill when highlighted.</param>
        /// <param name="selectFill">The style of the path fill when selected.</param>
        /// <param name="outline">Style of the path outline.</param>
        /// <param name="highlightOutline">Style of the path outline when highlighted.</param>
        /// <param name="selectOutline">Style of the path outline when selected.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderPaths(IEnumerable<GraphicsPath2D> path, StyleBrush fill,
                                               StyleBrush highlightFill,
                                               StyleBrush selectFill, StylePen outline, StylePen highlightOutline,
                                               StylePen selectOutline);

        /// <summary>
        /// Renders a set of <see cref="Point2D">points</see> into a set of rendered objects.
        /// </summary>
        /// <param name="locations">The point to render.</param>
        /// <param name="symbolData">The symbol to use for the point.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData);

        /// <summary>
        /// Renders a set of <see cref="Point2D">point</see> into a set of rendered objects.
        /// </summary>
        /// <param name="locations">The point to render.</param>
        /// <param name="symbolData">The symbol to use for the point.</param>
        /// <param name="highlight">A color matrix used to recolor the symbol during highlight.</param>
        /// <param name="select">A color matrix used to recolor the symbol during selection.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                 ColorMatrix highlight, ColorMatrix select);

        /// <summary>
        /// Renders a set of <see cref="Point2D">point</see> into a set of rendered objects.
        /// </summary>
        /// <param name="locations">The point to render.</param>
        /// <param name="symbolData">The symbol to use for the point.</param>
        /// <param name="highlightSymbolData">The symbol to use for the point when highlighted.</param>
        /// <param name="selectSymbolData">The symbol to use for the point when selected.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        IEnumerable<TRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                 Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
    }
}