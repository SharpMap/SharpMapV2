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
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface for renderers which take computed graphics paths and produce 
    /// rendered objects suitable for display.
    /// </summary>
    public interface IVectorRenderer<TCoordinate> : IRenderer
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        /// <summary>
        /// Renders a set of <see cref="Path{TCoordinate}"/> instances into a set of rendered objects.
        /// </summary>
        /// <param name="paths">The paths to render.</param>
        /// <param name="stroke">Style of the path outline.</param>
        /// <param name="fill">Style of the path outline when highlighted.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        void RenderPaths(IScene scene, IEnumerable<Path<TCoordinate>> paths, IPen stroke, IBrush fill);

        /// <summary>
        /// Renders a set of <typeparamref name="TCoordinate">points</typeparamref> into a set of rendered objects.
        /// </summary>
        /// <param name="locations">The point to render.</param>
        /// <param name="symbolData">The symbol to use for the point.</param>
        /// <returns>A rendered object suitable for direct display.</returns>
        void RenderSymbols(IScene scene, IEnumerable<TCoordinate> locations, ISymbol symbolData);
    }
}