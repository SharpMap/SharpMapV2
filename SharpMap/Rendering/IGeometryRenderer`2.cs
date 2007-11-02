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
using GeoAPI.Geometries;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public interface IGeometryRenderer<TSymbol, TRenderObject>
    {
        IEnumerable<TRenderObject> DrawMultiLineString(MultiLineString lines, StylePen fill,
                                                       StylePen highlightFill, StylePen selectFill, StylePen outline,
                                                       StylePen highlightOutline,
                                                       StylePen selectOutline, RenderState renderState);

        IEnumerable<TRenderObject> DrawLineString(LineString line, StylePen fill, StylePen highlightFill,
                                                  StylePen selectFill, StylePen outline, StylePen highlightOutline,
                                                  StylePen selectOutline, RenderState renderState);

        IEnumerable<TRenderObject> DrawMultiPolygon(MultiPolygon multipolygon, StyleBrush fill,
                                                    StyleBrush highlightFill, StyleBrush selectFill, StylePen outline,
                                                    StylePen highlightOutline,
                                                    StylePen selectOutline, RenderState renderState);

        IEnumerable<TRenderObject> DrawPolygon(Polygon polygon, StyleBrush fill, StyleBrush highlightFill,
                                               StyleBrush selectFill, StylePen outline, StylePen highlightOutline,
                                               StylePen selectOutline, RenderState renderState);

        IEnumerable<TRenderObject> DrawPoint(Point point, TSymbol symbol, TSymbol highlightSymbol, TSymbol selectSymbol,
                                             RenderState renderState);

        IEnumerable<TRenderObject> DrawMultiPoint(MultiPoint points, TSymbol symbol, TSymbol highlightSymbol,
                                                  TSymbol selectSymbol, RenderState renderState);
    }
}