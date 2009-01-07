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
using GeoAPI.Geometries;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    public interface IGeometryRenderer<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        void DrawMultiLineString(IScene scene, IMultiLineString<TCoordinate> multiLineString, IPen stroke, Double perpendicularOffset, RenderState renderState);
        void DrawLineString(IScene scene, ILineString<TCoordinate> line, IPen stroke, Double perpendicularOffset, RenderState renderState);
        void DrawMultiPolygon(IScene scene, IMultiPolygon<TCoordinate> multipolygon, IPen stroke, IBrush fill, Double perpendicularOffset, TCoordinate displacement, RenderState renderState);
        void DrawPolygon(IScene scene, IPolygon<TCoordinate> polygon, IPen stroke, IBrush fill, Double perpendicularOffset, TCoordinate displacement, RenderState renderState);
        void DrawPoint(IScene scene, IPoint<TCoordinate> point, ISymbol<TCoordinate> graphic, RenderState renderState);
        void DrawMultiPoint(IScene scene, IMultiPoint<TCoordinate> multiPoint, ISymbol<TCoordinate> graphic, RenderState renderState);
    }
}