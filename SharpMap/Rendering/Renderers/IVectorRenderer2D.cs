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

using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public interface IVectorRenderer2D<TRenderObject>
    {
        TRenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        TRenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData);
        TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
    }
}
