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
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer of vector data.
    /// </summary>
    public interface IVectorRenderer<TCoordinate> : IRenderer
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate> 
    {
        void RenderStroke(IScene<TCoordinate> scene, IRenderLayer layer, IPath<TCoordinate> path, IPen pen, Double perpendicularOffset, RenderState renderState);
        void RenderFill(IScene<TCoordinate> scene, IRenderLayer layer, IPath<TCoordinate> path, IBrush fill, TCoordinate displacement, RenderState renderState);
        void RenderPoints(IScene<TCoordinate> scene, IRenderLayer layer, IPath<TCoordinate> path, ISymbol<TCoordinate> graphic, RenderState renderState);
        void RenderPoint(IScene<TCoordinate> scene, IRenderLayer layer, TCoordinate point, ISymbol<TCoordinate> graphic, RenderState renderState);
        void RenderTextOnLine(IScene<TCoordinate> scene, IRenderLayer layer, IPath<TCoordinate> path, String text, IFont font, IHalo halo, IBrush fill, Double perpendicularOffset, Boolean isRepeated, Double initialGap, Double gap, Boolean fitToPath);
        void RenderTextOnPoints(IScene<TCoordinate> scene, IRenderLayer layer, IPath<TCoordinate> path, String text, IFont font, IHalo halo, IBrush fill, TCoordinate anchorPoint, TCoordinate displacement);
    }
}
