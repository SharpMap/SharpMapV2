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
using System.Collections.Generic;
using System.IO;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterRenderer
        : RasterRenderer2D<GeoJsonRenderObject>
    {
        public override IEnumerable<GeoJsonRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                      Rectangle2D rasterBounds)
        {
            yield break;
        }

        public override IEnumerable<GeoJsonRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                      Rectangle2D rasterBounds,
                                                                      IMatrix<DoubleComponent> rasterTransform)
        {
            yield break;
        }
    }
}