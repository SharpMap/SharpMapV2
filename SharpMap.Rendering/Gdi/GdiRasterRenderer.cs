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
using System.Drawing;
using System.IO;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Gdi
{
    public class GdiRasterRenderer : RasterRenderer2D<GdiRenderObject>
    {
        public override IEnumerable<GdiRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds)
        {
            return RenderRaster(rasterData, viewBounds, rasterBounds, Matrix2D.Identity);
        }

        public override IEnumerable<GdiRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds, Rectangle2D rasterBounds, IMatrix<DoubleComponent> rasterTransform)
        {
            GdiRenderObject renderedRaster = new GdiRenderObject();

            try
            {
                if (rasterData != null)
                {
                    if (rasterData.Position != 0 && !rasterData.CanSeek)
                        yield break;

                    rasterData.Seek(0, SeekOrigin.Begin);
                    if (rasterData.CanRead)
                    {
                        Bitmap raster = (Bitmap)Image.FromStream(rasterData);
                        renderedRaster = new GdiRenderObject(raster, 
                            ViewConverter.Convert(viewBounds),
                            ViewConverter.Convert((Matrix2D)rasterTransform), 
                            ViewConverter.Convert(ColorMatrix.Identity));
                        renderedRaster.State = RenderState.Normal;
                    }
                }


            }
            catch (Exception)
            {
                yield break;
            }
            yield return renderedRaster;
        }
    }
}
