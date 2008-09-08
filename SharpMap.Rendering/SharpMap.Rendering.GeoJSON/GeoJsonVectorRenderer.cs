/*
 *	This file is part of SharpMap.Rendering.GeoJson
 *  SharpMap.Rendering.GeoJson is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections.Generic;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonVectorRenderer
        : VectorRenderer2D<GeoJsonRenderObject>
    {
        public override IEnumerable<GeoJsonRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                                     StylePen highlightLine, StylePen selectLine,
                                                                     StylePen outline, StylePen highlightOutline,
                                                                     StylePen selectOutline, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GeoJsonRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                                     StylePen highlightOutline, StylePen selectOutline,
                                                                     RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GeoJsonRenderObject> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                                     StyleBrush highlightFill, StyleBrush selectFill,
                                                                     StylePen outline, StylePen highlightOutline,
                                                                     StylePen selectOutline, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GeoJsonRenderObject> RenderSymbols(IEnumerable<Point2D> locations,
                                                                       Symbol2D symbolData, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GeoJsonRenderObject> RenderSymbols(IEnumerable<Point2D> locations,
                                                                       Symbol2D symbolData, ColorMatrix highlight,
                                                                       ColorMatrix select, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GeoJsonRenderObject> RenderSymbols(IEnumerable<Point2D> locations,
                                                                       Symbol2D symbolData, Symbol2D highlightSymbolData,
                                                                       Symbol2D selectSymbolData,
                                                                       RenderState renderState)
        {
            throw new NotImplementedException();
        }
    }
}