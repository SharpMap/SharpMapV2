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
    public class GeoJsonTextRenderer : TextRenderer2D<GeoJsonRenderObject>
    {
        public override IEnumerable<GeoJsonRenderObject> RenderText(string text, StyleFont font,
                                                                    Rectangle2D layoutRectangle, Path2D flowPath,
                                                                    StyleBrush fontBrush, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        public override Size2D MeasureString(string text, StyleFont font)
        {
            return Size2D.Empty; //we will leave it up to the client to measure the string.
        }
    }
}