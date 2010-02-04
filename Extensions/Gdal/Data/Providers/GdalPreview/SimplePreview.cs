#region License

/*
 *  The attached / following is part of GdalForSharpMap.
 *  
 *  GdalForSharpMap is free software © 2009 - 2010 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2009
 *  
 *  Acknolegement:
 *  This code contains sample code from the gdal library by Tamas Serekes
 *  released with this license:
 ******************************************************************************
 * Copyright (c) 2007, Tamas Szekeres
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************
 * 
 */

#endregion
using System;
using System.Drawing;
using System.Drawing.Imaging;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;
using Gdal = OSGeo.GDAL.Gdal;
using GdalDataset = OSGeo.GDAL.Dataset;
using GdalBand = OSGeo.GDAL.Band;
using GdalColorInterp = OSGeo.GDAL.ColorInterp;
using GdalColorTable = OSGeo.GDAL.ColorTable;
using GdalColorEntry = OSGeo.GDAL.ColorEntry;
using GdalDataType = OSGeo.GDAL.DataType;

namespace SharpMap.Data.Providers.GdalPreview
{
    public class SimplePreview : BaseGdalPreview
    {

        #region Implementation of IGdalPreview

        public override Bitmap GetPreview(IExtents viewPort, Matrix2D toViewTransform, out Rectangle2D viewBounds, out Rectangle2D rasterBounds)
        {
            viewBounds = new Rectangle2D();
            rasterBounds = new Rectangle2D();

            GdalReadParameter gdalReadParameter = GetRasterViewPort(viewPort, toViewTransform);
            if (gdalReadParameter == null) return null;

            /*

            //Intersection of viewport with raster extents
            IExtents rasterExtents = viewPort.Intersection(_provider.GetExtents());
            if (rasterExtents.IsEmpty)
                return null;

            IGeometryFactory geoFactory = null;
            if (_provider.CoordinateTransformation != null)
                geoFactory = new GeometryServices()[_provider.OriginalSpatialReference.Wkt];
            else
                geoFactory = _provider.GeometryFactory;

            IExtents shownRasterExtents;
            // put display bounds into current projection
            ICoordinateTransformation coordinateTransformation = _provider.CoordinateTransformation;
            if (coordinateTransformation != null)
                shownRasterExtents = coordinateTransformation.InverseTransform(rasterExtents, geoFactory);
            else
                shownRasterExtents = rasterExtents;

            Double[] geoTrans = new double[6];
            _provider.Dataset.GetGeoTransform(geoTrans);
            GeoTransform geoTransform = new GeoTransform(_provider.GeometryFactory, geoTrans);
            Double x1 = Double.MaxValue;
            Double y1 = Double.MaxValue;
            Double x2 = Double.MinValue;
            Double y2 = Double.MinValue;
            Double count = 0;
            foreach (ICoordinate c in shownRasterExtents.ToGeometry().Coordinates)
            {
                Int32 ctx, cty;
                geoTransform.GroundToImage(c, out ctx, out cty);
                x1 = ctx < x1 ? ctx : x1;
                y1 = cty < y1 ? cty : y1;
                x2 = ctx > x2 ? ctx : x2;
                y2 = cty > y2 ? cty : y2;
                if (++count == 4) break;
            }

            // stay within image
            Size2D imageSize = _provider.Size;
            if (x2 > imageSize.Width) x2 = imageSize.Width;
            if (y2 > imageSize.Height) y2 = imageSize.Height;
            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            Int32 displayImageLength = (int)(x2 - x1);
            Int32 displayImageHeight = (int)(y2 - y1);

            // convert ground coordinates to map coordinates to figure out where to place the bitmap
            Point2D bitmapBR = toViewTransform.TransformVector(rasterExtents.Max[Ordinates.X], rasterExtents.Min[Ordinates.Y]) + Point2D.One;
            //new Point((int)map.WorldToImage(trueImageBbox.BottomRight).X + 1, (int)map.WorldToImage(trueImageBbox.BottomRight).Y + 1);
            Point2D bitmapTL = toViewTransform.TransformVector(rasterExtents.Min[Ordinates.X], rasterExtents.Max[Ordinates.Y]);
            //new Point((int)map.WorldToImage(trueImageBbox.TopLeft).X, (int)map.WorldToImage(trueImageBbox.TopLeft).Y);

            Int32 bitmapLength = (Int32)(bitmapBR.X - bitmapTL.X);
            Int32 bitmapHeight = Math.Abs((Int32)(bitmapBR.Y - bitmapTL.Y));

            // check to see if image is on its side
            if (bitmapLength > bitmapHeight && displayImageLength < displayImageHeight)
            {
                displayImageLength = bitmapHeight;
                displayImageHeight = bitmapLength;
            }
            else
            {
                displayImageLength = bitmapLength;
                displayImageHeight = bitmapHeight;
            }

            // 0 pixels in length or height, nothing to display
            if (bitmapLength < 1 || bitmapHeight < 1)
                return null;

            Size2D size = new Size2D(displayImageLength, displayImageHeight);
            rasterBounds = new Rectangle2D(Point2D.Zero, size);
            viewBounds = new Rectangle2D(bitmapTL, size);
            */
            rasterBounds = new Rectangle2D(0, 0, gdalReadParameter.ResultWidth, gdalReadParameter.ResultHeight);
            viewBounds = gdalReadParameter.ViewPort;

            return BitmapDirect(gdalReadParameter.X1, gdalReadParameter.Y1,
                gdalReadParameter.SourceWidth, gdalReadParameter.SourceHeight,
                gdalReadParameter.ResultWidth, gdalReadParameter.ResultHeight);

        }
        /// <summary>
        /// This code is from GdalRasterIO.cs, a sample file of GDAL source
        /// </summary>
        /// <param name="xOff"></param>
        /// <param name="yOff"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        private Bitmap BitmapDirect(int xOff, int yOff, int width, int height, int imageWidth, int imageHeight)
        {
            GdalDataset ds = _provider.Dataset;

            if (ds.RasterCount == 0)
                return null;

            int[] bandMap = new int[4] { 1, 1, 1, 1 };
            int channelCount = 1;
            bool hasAlpha = false;
            bool isIndexed = false;
            int channelSize = 8;
            GdalColorTable ct = null;
            // Evaluate the bands and find out a proper image transfer format
            for (int i = 0; i < ds.RasterCount; i++)
            {
                GdalBand band = ds.GetRasterBand(i + 1);

                if (Gdal.GetDataTypeSize(band.DataType) > 8)
                    channelSize = 16;
                switch (band.GetRasterColorInterpretation())
                {
                    case GdalColorInterp.GCI_AlphaBand:
                        channelCount = 4;
                        hasAlpha = true;
                        bandMap[3] = i + 1;
                        break;
                    case GdalColorInterp.GCI_BlueBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[0] = i + 1;
                        break;
                    case GdalColorInterp.GCI_RedBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[2] = i + 1;
                        break;
                    case GdalColorInterp.GCI_GreenBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[1] = i + 1;
                        break;
                    case GdalColorInterp.GCI_PaletteIndex:
                        ct = band.GetRasterColorTable();
                        isIndexed = true;
                        bandMap[0] = i + 1;
                        break;
                    case GdalColorInterp.GCI_GrayIndex:
                        isIndexed = true;
                        bandMap[0] = i + 1;
                        break;
                    default:
                        // we create the bandmap using the dataset ordering by default
                        if (i < 4 && bandMap[i] == 0)
                        {
                            if (channelCount < i)
                                channelCount = i;
                            bandMap[i] = i + 1;
                        }
                        break;
                }
            }

            // find out the pixel format based on the gathered information
            PixelFormat pixelFormat;
            GdalDataType dataType;
            int pixelSpace;

            if (isIndexed)
            {
                pixelFormat = PixelFormat.Format8bppIndexed;
                dataType = GdalDataType.GDT_Byte;
                pixelSpace = 1;
            }
            else
            {
                if (channelCount == 1)
                {
                    if (channelSize > 8)
                    {
                        pixelFormat = PixelFormat.Format16bppGrayScale;
                        dataType = GdalDataType.GDT_Int16;
                        pixelSpace = 2;
                    }
                    else
                    {
                        pixelFormat = PixelFormat.Format24bppRgb;
                        channelCount = 3;
                        dataType = GdalDataType.GDT_Byte;
                        pixelSpace = 3;
                    }
                }
                else
                {
                    if (hasAlpha)
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format64bppArgb;
                            dataType = GdalDataType.GDT_UInt16;
                            pixelSpace = 8;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format32bppArgb;
                            dataType = GdalDataType.GDT_Byte;
                            pixelSpace = 4;
                        }
                        channelCount = 4;
                    }
                    else
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format48bppRgb;
                            dataType = GdalDataType.GDT_UInt16;
                            pixelSpace = 6;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format24bppRgb;
                            dataType = GdalDataType.GDT_Byte;
                            pixelSpace = 3;
                        }
                        channelCount = 3;
                    }
                }
            }


            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(imageWidth, imageHeight, pixelFormat);

            if (isIndexed)
            {
                // setting up the color table
                if (ct != null)
                {
                    int iCol = ct.GetCount();
                    ColorPalette pal = bitmap.Palette;
                    for (int i = 0; i < iCol; i++)
                    {
                        GdalColorEntry ce = ct.GetColorEntry(i);
                        pal.Entries[i] = Color.FromArgb(ce.c4, ce.c1, ce.c2, ce.c3);
                    }
                    bitmap.Palette = pal;
                }
                else
                {
                    // grayscale
                    ColorPalette pal = bitmap.Palette;
                    for (int i = 0; i < 256; i++)
                        pal.Entries[i] = Color.FromArgb(255, i, i, i);
                    bitmap.Palette = pal;
                }
            }

            // Use GDAL raster reading methods to read the image data directly into the Bitmap
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, pixelFormat);

            try
            {
                int stride = bitmapData.Stride;
                IntPtr buf = bitmapData.Scan0;

                ds.ReadRaster(xOff, yOff, width, height, buf, imageWidth, imageHeight, dataType,
                    channelCount, bandMap, pixelSpace, stride, 1);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public override string Name
        {
            get { return "SimplePreview"; }
        }

        #endregion
    }
}
