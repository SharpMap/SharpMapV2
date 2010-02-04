using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;
using Gdal = OSGeo.GDAL.Gdal;
using GdalDataset = OSGeo.GDAL.Dataset;
using GdalBand = OSGeo.GDAL.Band;
using GdalResampleAlg = OSGeo.GDAL.ResampleAlg;
using GdalAccess = OSGeo.GDAL.Access;
using GdalCPlErr = OSGeo.GDAL.CPLErr;
using GdalColorInterp = OSGeo.GDAL.ColorInterp;
using GdalColorTable = OSGeo.GDAL.ColorTable;
using GdalColorEntry = OSGeo.GDAL.ColorEntry;

namespace SharpMap.Data.Providers.GdalPreview
{
    public class SharpMapV1RasterLayerPreview : BaseGdalPreview 
    {
        const int PixelSize = 3; //Format24bppRgb = byte[b,g,r]

        private WritePixel _doWritePixel;

        #region Implementation of IGdalPreview

         public override void Initialize(GdalRasterProvider provider)
        {
            _provider = provider;
            IExtents extents = _provider.GetExtents();
            _histoBounds = new Rectangle2D(extents.Min[Ordinates.X], extents.Max[Ordinates.Y], 
                                           extents.Max[Ordinates.X], extents.Min[Ordinates.Y]);
        }

       public override Bitmap GetPreview(IExtents viewPort, Matrix2D toViewTransform, out Rectangle2D viewBounds, out Rectangle2D rasterBounds)
        {
            ChooseWritePixel();

            return GetNonRotatedPreview(viewPort, toViewTransform, out viewBounds, out rasterBounds);

            /*
            viewBounds = new Rectangle2D();
            rasterBounds = new Rectangle2D();

            //Intersection of viewport with raster extents
            IExtents rasterExtents = viewPort.Intersection(_provider.GetExtents());
            if (rasterExtents.IsEmpty)
                return null;

            GdalDataset dataset = _provider.Dataset;

            Double[] geoTrans = new double[6];
            dataset.GetGeoTransform(geoTrans);

            ICoordinateTransformation coordinateTransformation = _provider.CoordinateTransformation;
            // not rotated, use faster display method
            if ((!_useRotation ||
                 (geoTrans[1] == 1 && geoTrans[2] == 0 && geoTrans[4] == 0 && Math.Abs(geoTrans[5]) == 1))
                && !_haveSpot && coordinateTransformation == null)
            {
                return GetNonRotatedPreview(viewPort, toViewTransform, out viewBounds, out rasterBounds);
            }

            // not rotated, but has spot...need default rotation
            if (!_useRotation && !_haveSpot || (geoTrans[0] == 0 && geoTrans[3] == 0))
                geoTrans = new[] {999.5, 1, 0, 1000.5, 0, -1};
            else if ((geoTrans[0] == 0 && geoTrans[3] == 0) && _haveSpot)
                geoTrans = new double[] {999.5, 1, 0, 1000.5, 0, -1};

            IGeometryFactory geoFactory = null;
            if (_provider.CoordinateTransformation != null)
                geoFactory = new GeometryServices()[_provider.OriginalSpatialReference.Wkt];
            else
                geoFactory = _provider.GeometryFactory;

            //Calculate Size of Viewport in device coordinates
            Double deviceWidth = viewPort.GetSize(Ordinates.X) * (Double)toViewTransform[0, 0];
            Double deviceHeight = Math.Abs(viewPort.GetSize(Ordinates.Y) * (Double)toViewTransform[1, 1]);

            //double GndX = 0, GndY = 0, ImgX = 0, ImgY = 0, PixX, PixY;
            Int32 bands = _provider.Bands;
            double[] intVal = new double[bands];

            //Point bitmapTL = new Point(), bitmapBR = new Point();
            //Geometries.Point imageTL = new Geometries.Point(), imageBR = new Geometries.Point();
            //Geometries.BoundingBox shownImageBbox, trueImageBbox;
            //int bitmapLength, bitmapHeight;
            //int displayImageLength, displayImageHeight;

            // init histo
            _histogram = new int[4][];
            for (int i = 0; i < bands + 1; i++)
                _histogram[i] = new int[256];

            IExtents shownRasterExtents;
            // put display bounds into current projection
            if (coordinateTransformation != null)
                shownRasterExtents = coordinateTransformation.InverseTransform(rasterExtents, geoFactory);
            else
                shownRasterExtents = rasterExtents;

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
            if (x2> imageSize.Width) x2 = imageSize.Width;
            if (y2 > imageSize.Height) y2 = imageSize.Height;
            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            Int32 displayImageLength = (int)(x2 - x1);
            Int32 displayImageHeight = (int)(y2 - y1);

            // convert ground coordinates to map coordinates to figure out where to place the bitmap
            Point2D bitmapBR = toViewTransform.TransformVector(rasterExtents.Max[Ordinates.X], rasterExtents.Min[Ordinates.Y]) + Point2D.One;
                //new Point((int)map.WorldToImage(trueImageBbox.BottomRight).X + 1, (int)map.WorldToImage(trueImageBbox.BottomRight).Y + 1);
            Point2D bitmapTL = toViewTransform.TransformVector(rasterExtents.Min[Ordinates.X] , rasterExtents.Max[Ordinates.Y]);
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

            //initialize bitmap
            //lock (_lock)
            //{

            Bitmap bitmap = new Bitmap(bitmapLength, bitmapHeight, PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmapLength, bitmapHeight),
                                                        ImageLockMode.ReadWrite, bitmap.PixelFormat);
                Int32 maxIndex = bitmapLength*PixelSize - 1;
                Boolean success = false;
                try
                {
                    unsafe
                    {
                        // turn everything yellow, so we can make fill transparent
                        for (int y = 0; y < bitmapHeight; y++)
                        {
                            byte* brow = (byte*) bitmapData.Scan0 + (y*bitmapData.Stride);
                            for (int x = 0; x < bitmapLength; x++)
                            {
                                brow[x*3 + 0] = 0;
                                brow[x*3 + 1] = 255;
                                brow[x*3 + 2] = 255;
                            }
                        }

                        // create 3 dimensional buffer [band][x pixel][y pixel]
                        double[][] tempBuffer = new double[bands][];
                        double[][][] buffer = new double[bands][][];
                        for (int i = 0; i < bands; i++)
                        {
                            buffer[i] = new double[displayImageLength][];
                            for (int j = 0; j < displayImageLength; j++)
                                buffer[i][j] = new double[displayImageHeight];
                        }

                        GdalBand[] band = new GdalBand[bands];
                        int[] ch = new int[bands];

                        Boolean palette = false;
                        // get data from image
                        GdalColorTable gdalColorTable = null;
                        OSGeo.GDAL.PaletteInterp gdalPi = OSGeo.GDAL.PaletteInterp.GPI_RGB;
                        Int32 bitDepth = 8;

                        for (int i = 0; i < bands; i++)
                        {
                            tempBuffer[i] = new double[displayImageLength*displayImageHeight];
                            band[i] = dataset.GetRasterBand(i + 1);

                            band[i].ReadRaster((Int32) x1, (Int32) y1, (Int32) (x2 - x1), (Int32) (y2 - y1),
                                               tempBuffer[i], displayImageLength, displayImageHeight, 0, 0);

                            // parse temp buffer into the image x y value buffer
                            long pos = 0;
                            for (int y = 0; y < displayImageHeight; y++)
                            {
                                for (int x = 0; x < displayImageLength; x++, pos++)
                                    buffer[i][x][y] = tempBuffer[i][pos];
                            }

                            if (Gdal.GetDataTypeSize(band[i].DataType) > 8)
                                bitDepth = Gdal.GetDataTypeSize(band[i].DataType);

                            switch (band[i].GetRasterColorInterpretation())
                            {
                                case GdalColorInterp.GCI_BlueBand:
                                    ch[i] = 0;
                                    break;
                                case GdalColorInterp.GCI_GreenBand:
                                    ch[i] = 1;
                                    break;
                                case GdalColorInterp.GCI_RedBand:
                                    ch[i] = 2;
                                    break;
                                case GdalColorInterp.GCI_Undefined:
                                    ch[i] = 3; // infrared
                                    break;
                                case GdalColorInterp.GCI_GrayIndex:
                                    ch[i] = 4;
                                    break;
                                case GdalColorInterp.GCI_PaletteIndex:
                                    ch = new int[] {0, 1, 2};
                                    intVal = new double[3];
                                    gdalColorTable = band[i].GetColorTable();
                                    gdalColorTable.GetPaletteInterpretation();
                                    palette = true;
                                    break;
                                default:
                                    ch[i] = -1;
                                    break;
                            }
                        }

                        // scale
                        Double bitScalar = 1.0;
                        if (bitDepth == 12)
                            bitScalar = 16.0;
                        else if (bitDepth == 16)
                            bitScalar = 256.0;
                        else if (bitDepth == 32)
                            bitScalar = 16777216.0;

                        // store these values to keep from having to make slow method calls
                        int bitmapTLX = (int) bitmapTL.X;
                        int bitmapTLY = (int) bitmapTL.Y;
                        //double imageTop = imageTL.Y;
                        //double imageLeft = imageTL.X;
                        double dblMapPixelWidth = viewPort.GetSize(Ordinates.X)/deviceWidth;
                        //(Double)toViewTransform[0, 0];
                        double dblMapPixelHeight = viewPort.GetSize(Ordinates.Y)/deviceHeight;
                        //(Double)toViewTransform[1, 1];
                        double dblMapMinX = viewPort.Min[Ordinates.X];
                        double dblMapMaxY = viewPort.Max[Ordinates.Y];

                        // get inverse values
                        double geoTop = geoTransform.Inverse[3];
                        double geoLeft = geoTransform.Inverse[0];
                        double geoHorzPixRes = geoTransform.Inverse[1];
                        double geoVertPixRes = geoTransform.Inverse[5];
                        double geoXRot = geoTransform.Inverse[2];
                        double geoYRot = geoTransform.Inverse[4];

                        double dblXScale = (x2 - x1)/(displayImageLength - 1);
                        double dblYScale = (y2 - y1)/(displayImageHeight - 1);
                        double[] dblPoint;

                        // get inverse transform  
                        // NOTE: calling transform.MathTransform.Inverse() once and storing it
                        // is much faster than having to call every time it is needed
                        IMathTransform inverseTransform = null;
                        if (coordinateTransformation != null)
                            inverseTransform = coordinateTransformation.Inverse.MathTransform;

                        Double diffX = bitmapBR.X - bitmapTL.X;
                        Double diffY = bitmapBR.Y - bitmapTL.Y;
                        for (Double PixY = 0; PixY < diffY; PixY++)
                        {
                            byte* row = (byte*) bitmapData.Scan0 + ((int) Math.Round(PixY)*bitmapData.Stride);

                            for (Double PixX = 0; PixX < diffX; PixX++)
                            {
                                // same as Map.ImageToGround(), but much faster using stored values...rather than called each time

                                Double gndX = dblMapMinX + (PixX + (double) bitmapTLX)*dblMapPixelWidth;
                                Double gndY = dblMapMaxY - (PixY + (double) bitmapTLY)*dblMapPixelHeight;

                                // transform ground point if needed
                                if (inverseTransform != null)
                                {
                                    ICoordinate point =
                                        inverseTransform.Transform(geoFactory.CoordinateFactory.Create(gndX, gndY));
                                    gndX = point[Ordinates.X];
                                    gndY = point[Ordinates.Y];
                                }

                                // same as GeoTransform.GroundToImage(), but much faster using stored values...
                                Double ImgX = (geoLeft + geoHorzPixRes*gndX + geoXRot*gndY);
                                Double ImgY = (geoTop + geoYRot*gndX + geoVertPixRes*gndY);

                                if (ImgX < x1 || ImgX > x2 || ImgY < y1 || ImgY > y2)
                                    continue;

                                Int32 lBands = bands;
                                if (palette)
                                {
                                    Double colorIndex = buffer[0]
                                        [(int) ((ImgX - x1)/dblXScale)]
                                        [(int) ((ImgY - y1)/dblYScale)];

                                    GdalColorEntry colorEntry = gdalColorTable.GetColorEntry((int) colorIndex);
                                    intVal[0] = colorEntry.c1;
                                    intVal[1] = colorEntry.c2;
                                    intVal[2] = colorEntry.c3;
                                    lBands = 3;
                                }
                                else
                                {
                                    for (int i = 0; i < lBands; i++)
                                    {
                                        intVal[i] =
                                            buffer[i]
                                                [(int) ((ImgX - x1)/dblXScale)]
                                                [(int) ((ImgY - y1)/dblYScale)];
                                    }
                                }


                                // color correction
                                for (int i = 0; i < lBands; i++)
                                {
                                    //intVal[i] =
                                    //    buffer[i]
                                    //          [(int) ((ImgX - x1)/dblXScale)]
                                    //          [(int) ((ImgY - y1)/dblYScale)];

                                    Double spotVal;
                                    Double imageVal = spotVal = intVal[i] = intVal[i]/bitScalar;

                                    if (_colorCorrect)
                                    {
                                        intVal[i] = ApplyColorCorrection(imageVal, spotVal, ch[i], gndX, gndY);

                                        // if pixel is within ground boundary, add its value to the histogram
                                        if (ch[i] != -1 && intVal[i] > 0 && 
                                            (_histoBounds.Bottom >= (int) gndY) &&
                                             _histoBounds.Top <= (int) gndY &&
                                             _histoBounds.Left <= (int) gndX && 
                                             _histoBounds.Right >= (int) gndX)
                                        {
                                            _histogram[ch[i]][(int) intVal[i]]++;
                                        }
                                    }

                                    if (intVal[i] > 255)
                                        intVal[i] = 255;
                                }

                                // luminosity
                                if (bands >= 3)
                                    _histogram[bands][(int) (intVal[2]*0.2126 + intVal[1]*0.7152 + intVal[0]*0.0722)]++;

                                _doWritePixel(PixX, lBands, intVal, PixelSize, ch, row, maxIndex);
                            }
                        }
                    }
                    Size2D rs = new Size2D(bitmapLength, bitmapHeight);
                    rasterBounds = new Rectangle2D(Point2D.Zero, rs);
                    viewBounds = new Rectangle2D(bitmapTL, rs);
                    success = true;
                }
                finally
                {
                    if (bitmap != null)
                        bitmap.UnlockBits(bitmapData);
                }
                if (!success)
                    return null;
            //}
            bitmap.MakeTransparent(Color.FromArgb(0, 255, 255));

            return bitmap;

             */
        }

        public override string Name
        {
            get { return "SharpMapV1"; }
        }

        public virtual Bitmap GetNonRotatedPreview(IExtents viewPort, Matrix2D toViewTransform, out Rectangle2D viewBounds, out Rectangle2D rasterBounds)
        {
            viewBounds = new Rectangle2D();
            rasterBounds = new Rectangle2D();

            GdalReadParameter gdalReadParameter = GetRasterViewPort(viewPort, toViewTransform);
            if (gdalReadParameter == null) return null;

            GdalDataset dataset = _provider.Dataset;

            Double[] geoTrans = new double[6];
            dataset.GetGeoTransform(geoTrans);

            //initialize Histogram
            _histogram = new int[4][];
            int bands = _provider.Bands;
            for (int i = 0; i < bands + 1; i++)
                _histogram[i] = new int[256];

            Double dblImginMapW = gdalReadParameter.ResultWidth;
            Double dblImginMapH = gdalReadParameter.ResultHeight;

            Int32 x1 = gdalReadParameter.X1, y1 = gdalReadParameter.Y1;
            Int32 imgPixWidth = gdalReadParameter.SourceWidth;
            Int32 imgPixHeight = gdalReadParameter.SourceHeight;

            Bitmap bitmap = null;
            BitmapData bitmapData = null;
            Boolean success = false;
            try
            {
                Int32 intImginMapW = (int) Math.Round(dblImginMapW);
                Int32 intImginMapH = (int) Math.Round(dblImginMapH);
                Int32 maxIndex = intImginMapW*PixelSize;
                bitmap = new Bitmap(intImginMapW, intImginMapH,PixelFormat.Format24bppRgb);
                bitmapData =
                    bitmap.LockBits(
                        new Rectangle(0, 0, intImginMapW, intImginMapH),
                        ImageLockMode.ReadWrite, bitmap.PixelFormat);

                unsafe
                {
                    byte cr = _provider.NoDataInitColor.R;
                    byte cg = _provider.NoDataInitColor.G;
                    byte cb = _provider.NoDataInitColor.B;

                    Double[][] buffer = new Double[bands][];
                    GdalBand[] band = new GdalBand[bands];
                    int[] ch = new int[bands];

                    // get data from image
                    Boolean palette = false;
                    Double[] intVal = new Double[bands];

                    Double[] noDataValues = new Double[bands];
                    Double[] scales = new Double[bands];
                    GdalColorTable gdalColorTable = null;
                    //OSGeo.GDAL.PaletteInterp gdalPi = OSGeo.GDAL.PaletteInterp.GPI_RGB;

                    // scale
                    //Double bitScalar = 1.0;
                    Int32 bitDepth = 8;

                    Int32 lBands = bands;
                    Int32 displayOption = 0;
                    for (Int32 i = 0; i < bands; i++)
                    {
                        buffer[i] = new double[intImginMapW * intImginMapH];
                        band[i] = dataset.GetRasterBand(i + 1);

                        //get nodata value if present
                        Int32 hasVal = 0;
                        band[i].GetNoDataValue(out noDataValues[i], out hasVal);
                        if (hasVal == 0) noDataValues[i] = Double.NaN;
                        band[i].GetScale(out scales[i], out hasVal);
                        if (hasVal == 0) scales[i] = 1.0;

                        band[i].ReadRaster(x1, y1, imgPixWidth, imgPixHeight,
                                           buffer[i], intImginMapW, intImginMapH,
                                           0, 0);
                        //if (Gdal.GetDataTypeSize(band[i].DataType) > 8)
                        //    bitDepth = Gdal.GetDataTypeSize(band[i].DataType);


                        switch (band[i].GetRasterColorInterpretation())
                        {
                            case GdalColorInterp.GCI_BlueBand:
                                ch[i] = 0;
                                break;
                            case GdalColorInterp.GCI_GreenBand:
                                ch[i] = 1;
                                break;
                            case GdalColorInterp.GCI_RedBand:
                                ch[i] = 2;
                                break;
                            case GdalColorInterp.GCI_Undefined:
                                if (bands > 1)
                                    ch[i] = 3; // infrared
                                else
                                {
                                    lBands = 3;
                                    displayOption = 1;
                                    ch = new int[] {0,1,2};
                                    intVal = new Double[3];
                                    if (_provider.ColorBlend == null)
                                    {
                                        Double dblMin, dblMax;
                                        band[i].GetMinimum(out dblMin, out hasVal);
                                        if (hasVal == 0) dblMin = Double.NaN;
                                        band[i].GetMaximum(out dblMax, out hasVal);
                                        if (hasVal == 0) dblMax = double.NaN;
                                        if (Double.IsNaN(dblMin) || Double.IsNaN(dblMax))
                                        {
                                            double dblMean, dblStdDev;
                                            band[i].GetStatistics(0, 1, out dblMin, out dblMax, out dblMean, out dblStdDev);
                                            //double dblRange = dblMax - dblMin;
                                            //dblMin -= 0.1*dblRange;
                                            //dblMax += 0.1*dblRange;
                                        }
                                        Single[] minmax = new float[] { Convert.ToSingle(dblMin), 0.5f * Convert.ToSingle(dblMin + dblMax), Convert.ToSingle(dblMax) };
                                        StyleColor[] colors = new StyleColor[] { StyleColor.Blue, StyleColor.Green, StyleColor.Red };
                                        _provider.ColorBlend = new StyleColorBlend(colors, minmax);
                                    }
                                }
                                break;
                            case GdalColorInterp.GCI_GrayIndex:
                                ch[i] = 0;
                                break;
                            case GdalColorInterp.GCI_PaletteIndex:
                                lBands = 3;
                                displayOption = 2;
                                ch = new int[] { 0, 1, 2 };
                                intVal = new double[3];
                                gdalColorTable = band[i].GetColorTable();
                                //palette = true;
                                break;
                            default:
                                ch[i] = -1;
                                break;
                        }
                    }

                    /*
                    if (bitDepth == 12)
                        bitScalar = 16.0;
                    else if (bitDepth == 16)
                        bitScalar = 256.0;
                    else if (bitDepth == 32)
                    {
                        bitScalar = 16777216.0;
                        ch = new[] { 0, 1, 2 };
                    }
                    */
                    Int32 p_indx = 0;
                    for (int i = 0; i < lBands + 1; i++)
                        _histogram[i] = new int[256];

                    for (int y = 0; y < intImginMapH; y++)
                    {
                        byte* row = (byte*) bitmapData.Scan0 + (y*bitmapData.Stride);
                        for (int x = 0; x < intImginMapW; x++, p_indx++)
                        {
                            Double imageVal;
                            switch (displayOption)
                            {
                                case 1:
                                    imageVal = buffer[0][p_indx]; // /bitScalar;
                                    if (imageVal != noDataValues[0])
                                    {
                                        StyleColor color = _provider.ColorBlend.GetColor(Convert.ToSingle(imageVal));
                                        intVal[0] = color.B;
                                        intVal[1] = color.G;
                                        intVal[2] = color.R;
                                        //intVal[3] = ce.c4;
                                    }
                                    else
                                    {
                                        intVal[0] = cb;
                                        intVal[1] = cg;
                                        intVal[2] = cr;
                                    }
                                    //lBands = 3;
                                    break;
                                case 2:
                                        imageVal = buffer[0][p_indx]; // / bitScalar;
                                        if (imageVal != noDataValues[0])
                                        {
                                            GdalColorEntry ce = gdalColorTable.GetColorEntry(Convert.ToInt32(imageVal));
                                            intVal[0] = ce.c1;
                                            intVal[1] = ce.c2;
                                            intVal[2] = ce.c3;
                                            //intVal[3] = ce.c4;
                                        }
                                        else
                                        {
                                            intVal[0] = cb;
                                            intVal[1] = cg;
                                            intVal[2] = cr;
                                        }
                                    //lBands = 3;
                                    break;
                                default:
                                    for (int i = 0; i < lBands; i++)
                                        intVal[i] = buffer[i][p_indx]; // / bitScalar;
                                    break;
                            }

                            if (_colorCorrect)
                            {
                                for (int i = 0; i < lBands; i++)
                                {
                                    intVal[i] = Math.Min(ApplyColorCorrection(intVal[i], 0, ch[i], 0, 0), 255);
                                    _histogram[ch[i]][(int)intVal[i]]++;
                                }
                            }

                            if (bands >= 3)
                                _histogram[bands][
                                    (int)(intVal[2] * 0.2126 + intVal[1] * 0.7152 + intVal[0] * 0.0722)]++;

                            _doWritePixel(x, lBands, intVal, PixelSize, ch, row, intImginMapW);

                        }
                    }
                }

                success = true;
            }
            catch
            {
                //return null;
            }
            finally
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
            }

            if (!success)
                return null;
            rasterBounds = new Rectangle2D(0, 0, gdalReadParameter.ResultWidth, gdalReadParameter.ResultHeight);
            viewBounds = gdalReadParameter.ViewPort;
            return bitmap;

        }

        #endregion

        #region WritePixel

        protected unsafe void ChooseWritePixel()
        {
            Int32 bands = _provider.Bands;
            if (bands == 1)
            {
                Int32 bitDepth = Math.Max(8, Gdal.GetDataTypeSize(_provider.Dataset.GetRasterBand(1).DataType));
                GdalColorInterp ci = _provider.Dataset.GetRasterBand(1).GetRasterColorInterpretation();
                if (bitDepth == 32 || ci == GdalColorInterp.GCI_GrayIndex || ci == GdalColorInterp.GCI_PaletteIndex)
                {
                    if (_showClip)
                        _doWritePixel = WritePixel_Rgb_ShowClip;
                    else
                        _doWritePixel = WritePixel_Rgb;
                    return;
                }
                if (_showClip)
                    _doWritePixel = WritePixel_BW_ShowClip;
                else
                    _doWritePixel = WritePixel_BW;
                return;

            }
            if (DisplayIr && bands == 4)
            {
                if (_showClip)
                    _doWritePixel = WritePixel_Ir_ShowClip;
                else
                    _doWritePixel = WritePixel_Ir;
                return;
            }
            if (DisplayCir && bands == 4)
            {
                if (_showClip)
                    _doWritePixel = WritePixel_Cir_ShowClip;
                else
                    _doWritePixel = WritePixel_Cir;
            }

            if (_showClip)
                _doWritePixel = WritePixel_Rgb_ShowClip;
            else
                _doWritePixel = WritePixel_Rgb;
        }

        protected static unsafe void WritePixel_BW_ShowClip
            (double x, Int32 bands, Double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            switch ((int)intVal[0])
            {
                case 0:
                    row[index++] = 255;
                    row[index++] = 0;
                    row[index] = 0;
                    break;
                case 255:
                    row[index++] = 0;
                    row[index++] = 0;
                    row[index] = 255;
                    break;
                default:
                    row[index++] = (byte)intVal[0];
                    row[index++] = (byte)intVal[0];
                    row[index] = (byte)intVal[0];
                    break;
            }
        }

        protected static unsafe void WritePixel_BW
            (double x, Int32 bands, Double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            row[index++] = (byte)intVal[0];
            row[index++] = (byte)intVal[0];
            row[index] = (byte)intVal[0];
        }

        //protected unsafe void WritePixel_PI_ShowClip
        //    (double x, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        //{
        //}

        //protected unsafe void WritePixel_PI
        //    (double x, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        //{
        //}

        protected static unsafe void WritePixel_Ir_ShowClip
            (double x, Int32 bands, Double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            for (int i = 0; i < bands; i++)
            {
                if (ch[i] == 3)
                {
                    switch ((int)intVal[3])
                    {
                        case 0:
                            row[index++] = 255;
                            row[index++] = 0;
                            row[index] = 0;
                            break;
                        case 255:
                            row[index++] = 0;
                            row[index++] = 0;
                            row[index] = 255;
                            break;
                        default:
                            row[index++] = (byte)intVal[i];
                            row[index++] = (byte)intVal[i];
                            row[index] = (byte)intVal[i];
                            break;
                    }
                }
                else
                    continue;
            }
        }

        protected static unsafe void WritePixel_Ir
            (double x, Int32 bands, Double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            for (int i = 0; i < bands; i++)
            {
                if (ch[i] == 3)
                {
                    row[index++] = (byte)intVal[i];
                    row[index++] = (byte)intVal[i];
                    row[index] = (byte)intVal[i];
                }
                else
                    continue;
            }
        }

        protected static unsafe void WritePixel_Cir_ShowClip
            (double x, Int32 bands, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;

            if (intVal[0] == 0 && intVal[1] == 0 && intVal[3] == 0)
            {
                intVal[3] = intVal[0] = 0;
                intVal[1] = 255;
            }
            else if (intVal[0] == 255 && intVal[1] == 255 && intVal[3] == 255)
                intVal[1] = intVal[0] = 0;

            for (int i = 0; i < bands; i++)
            {
                if (ch[i] != 0 && ch[i] != -1)
                    row[index++ + ch[i] - 1] = (byte)intVal[i];
            }
        }

        protected static unsafe void WritePixel_Cir
            (double x, Int32 bands, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            for (int i = 0; i < bands; i++)
            {
                if (ch[i] != 0 && ch[i] != -1)
                    row[index++ + ch[i] - 1] = (byte)intVal[i];
            }
        }

        protected unsafe void WritePixel_Rgb_ShowClip
            (double x, Int32 bands, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            if (intVal[0] == 0 && intVal[1] == 0 && intVal[2] == 0)
            {
                intVal[0] = intVal[1] = 0;
                intVal[2] = 255;
            }
            else if (intVal[0] == 255 && intVal[1] == 255 && intVal[2] == 255)
                intVal[1] = intVal[2] = 0;

            for (int i = 0; i < bands; i++)
            {
                Int32 byteIndex = ByteIndex(index, i, ch, maxIndex);
                if (byteIndex > -1)
                    row[byteIndex] = (byte)intVal[i];
                //if (ch[i] != 3 && ch[i] != -1 )
                //    row[index + ch[i]] = (byte)intVal[i];
            }

        }

        private static Int32 ByteIndex(Int32 index, Int32 channelIndex, Int32[] channels, Int32 maxIndex)
        {
            if (channels[channelIndex] == -1 || channels[channelIndex] == 3)
                return -1;
            Int32 byteIndex = index + channels[channelIndex];
            return byteIndex < maxIndex ? byteIndex : maxIndex;
        }

        protected unsafe void WritePixel_Rgb
            (double x, Int32 bands, double[] intVal, int iPixelSize, int[] ch, byte* row, int maxIndex)
        {
            Int32 index = (int)Math.Round(x) * iPixelSize;
            for (int i = 0; i < bands; i++)
            {
                Int32 byteIndex = ByteIndex(index, i, ch, maxIndex * iPixelSize);
                if (byteIndex > -1)
                    row[byteIndex] = (byte)intVal[i];
            }
        }

        #endregion

        // apply any color correction to pixel
        protected Double ApplyColorCorrection(Double imageVal, Double spotVal, int channel, Double gndX, Double gndY)
        {
            Double finalVal = imageVal;

            if (_haveSpot)
            {
                // gamma
                if (_nonSpotGamma != 1)
                    imageVal = 256 * Math.Pow((imageVal / 256), _nonSpotGamma);

                // gain

                switch (channel)
                {
                    case 2:
                        imageVal = imageVal * _nonSpotGain[0];
                        break;
                    case 1:
                        imageVal = imageVal * _nonSpotGain[1];
                        break;
                    case 0:
                        imageVal = imageVal * _nonSpotGain[2];
                        break;
                    case 3:
                        imageVal = imageVal * _nonSpotGain[3];
                        break;
                }

                if (imageVal > 255)
                    imageVal = 255;

                // curve
                if (_nonSpotCurveLut != null)
                    if (_nonSpotCurveLut.Count != 0)
                    {
                        if (channel == 2 || channel == 4)
                            imageVal = _nonSpotCurveLut[0][(int)imageVal];
                        else if (channel == 1)
                            imageVal = _nonSpotCurveLut[1][(int)imageVal];
                        else if (channel == 0)
                            imageVal = _nonSpotCurveLut[2][(int)imageVal];
                        else if (channel == 3)
                            imageVal = _nonSpotCurveLut[3][(int)imageVal];
                    }

                finalVal = imageVal;

                Double distance = Math.Sqrt(Math.Pow(gndX - SpotPoint.X, 2) + Math.Pow(gndY - SpotPoint.Y, 2));

                if (distance <= _innerSpotRadius + _outerSpotRadius)
                {
                    // gamma
                    if (_spotGamma != 1)
                        spotVal = 256 * Math.Pow((spotVal / 256), _spotGamma);

                    // gain
                    if (channel == 2)
                        spotVal = spotVal * _spotGain[0];
                    else if (channel == 1)
                        spotVal = spotVal * _spotGain[1];
                    else if (channel == 0)
                        spotVal = spotVal * _spotGain[2];
                    else if (channel == 3)
                        spotVal = spotVal * _spotGain[3];

                    if (spotVal > 255)
                        spotVal = 255;

                    // curve
                    if (_spotCurveLut != null)
                        if (_spotCurveLut.Count != 0)
                        {
                            if (channel == 2 || channel == 4)
                                spotVal = _spotCurveLut[0][(int)spotVal];
                            else if (channel == 1)
                                spotVal = _spotCurveLut[1][(int)spotVal];
                            else if (channel == 0)
                                spotVal = _spotCurveLut[2][(int)spotVal];
                            else if (channel == 3)
                                spotVal = _spotCurveLut[3][(int)spotVal];
                        }

                    if (distance < _innerSpotRadius)
                        finalVal = spotVal;
                    else
                    {
                        Double imagePct = (distance - _innerSpotRadius) / _outerSpotRadius;
                        Double spotPct = 1 - imagePct;

                        finalVal = (Math.Round((spotVal * spotPct) + (imageVal * imagePct)));
                    }
                }
            }

            // gamma
            if (_gamma != 1)
                finalVal = (256 * Math.Pow((finalVal / 256), _gamma));


            switch (channel)
            {
                case 2:
                    finalVal = finalVal * _gain[0];
                    break;
                case 1:
                    finalVal = finalVal * _gain[1];
                    break;
                case 0:
                    finalVal = finalVal * _gain[2];
                    break;
                case 3:
                    finalVal = finalVal * _gain[3];
                    break;
            }

            if (finalVal > 255)
                finalVal = 255;

            // curve
            if (_curveLut != null)
                if (_curveLut.Count != 0)
                {
                    if (channel == 2 || channel == 4)
                        finalVal = _curveLut[0][(int)finalVal];
                    else if (channel == 1)
                        finalVal = _curveLut[1][(int)finalVal];
                    else if (channel == 0)
                        finalVal = _curveLut[2][(int)finalVal];
                    else if (channel == 3)
                        finalVal = _curveLut[3][(int)finalVal];
                }

            return finalVal;
        }

        #region Private helpers

        // find min and max pixel values of the image
        private void ComputeStretch()
        {
            Double min = Double.MaxValue;
            Double max = Double.MinValue;

            int width, height;
            if (_provider.Dataset.RasterYSize < 4000)
            {
                height = _provider.Dataset.RasterYSize;
                width = _provider.Dataset.RasterXSize;
            }
            else
            {
                height = 4000;
                width = (int)(4000 * (_provider.Dataset.RasterXSize / (double)_provider.Dataset.RasterYSize));
            }

            double[] buffer = new double[width * height];
            Int32 bands = _provider.Bands;
            Int32 bitDepth = 8;
            for (int band = 1; band <= bands; band++)
            {
                GdalBand rBand = _provider.Dataset.GetRasterBand(band);
                bitDepth = Math.Max(bitDepth, Gdal.GetDataTypeSize(rBand.DataType));
                rBand.ReadRaster(0, 0, _provider.Dataset.RasterXSize, _provider.Dataset.RasterYSize, buffer, width, height, 0, 0);

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] < min)
                        min = buffer[i];
                    if (buffer[i] > max)
                        max = buffer[i];
                }
            }

            if (bitDepth == 12)
            {
                min /= 16;
                max /= 16;
            }
            else if (bitDepth == 16)
            {
                min /= 256;
                max /= 256;
            }

            if (max > 255)
                max = 255;

            _stretchPoint = new Point2D(Math.Round(min, 0), Math.Round(max, 0));
        }
        #endregion

        private Boolean _colorCorrect = true;
        public bool ColorCorrect
        {
            get { return _colorCorrect; }
            set { _colorCorrect = value; }
        }

        private Rectangle2D _histoBounds;
        public Rectangle2D HistoBounds
        {
            get { return _histoBounds; }
            set { _histoBounds = value; }
        }

        private Point2D _stretchPoint;
        public Point2D StretchPoint
        {
            get
            {
                if (_stretchPoint.Y == 0)
                    ComputeStretch();

                return _stretchPoint;
            }
            set { _stretchPoint = value; }
        }

        /// <summary>
        /// Gets or sets to display color InfraRed
        /// </summary>
        private Boolean _displayIr;
        public Boolean DisplayIr
        {
            get { return _displayIr && _provider.Bands == 4; }
            set { _displayIr = value; }
        }

        /// <summary>
        /// Gets or sets to display color InfraRed
        /// </summary>
        private Boolean _displayCir;
        public Boolean DisplayCir
        {
            get { return _displayCir && _provider.Bands == 4; }
            set { _displayCir = value; }
        }

        /// <summary>
        /// Gets or sets to display clip
        /// </summary>
        private Boolean _showClip;
        public Boolean ShowClip
        {
            get { return _showClip; }
            set { _showClip = value; }
        }

        /// <summary>
        /// Gets or sets to display gamma
        /// </summary>
        private Double _gamma = 1d;
        public Double Gamma
        {
            get { return _gamma; }
            set { _gamma = value; }
        }

        /// <summary>
        /// Gets or sets to display gamma for Spot spot
        /// </summary>
        private Double _spotGamma = 1d;
        public Double SpotGamma
        {
            get { return _spotGamma; }
            set { _spotGamma = value; }
        }

        /// <summary>
        /// Gets or sets to display gamma for NonSpot
        /// </summary>
        private Double _nonSpotGamma = 1d;
        public Double NonSpotGamma
        {
            get { return _nonSpotGamma; }
            set { _nonSpotGamma = value; }
        }

        /// <summary>
        /// Gets or sets to display red Gain
        /// </summary>
        private Double[] _gain = new Double[] { 1d, 1d, 1d, 1d };
        public Double[] Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        /// <summary>
        /// Gets or sets to display red Gain for Spot spot
        /// </summary>
        private Double[] _spotGain = new Double[] { 1d, 1d, 1d, 1d };
        public double[] SpotGain
        {
            get { return _spotGain; }
            set { _spotGain = value; }
        }

        /// <summary>
        /// Gets or sets to display red Gain for Spot spot
        /// </summary>
        private Double[] _nonSpotGain = new Double[] { 1d, 1d, 1d, 1d };
        public double[] NonSpotGain
        {
            get { return _nonSpotGain; }
            set { _nonSpotGain = value; }
        }

        /// <summary>
        /// Gets or sets to display curve lut
        /// </summary>
        private List<Int32[]> _curveLut;
        public List<int[]> CurveLut
        {
            get { return _curveLut; }
            set { _curveLut = value; }
        }

        ///<summary>
        /// Use rotation information
        /// </summary>
        public Boolean _useRotation = true;
        public bool UseRotation
        {
            get { return _useRotation; }
            set
            {
                _useRotation = value;
                _provider.GetExtents();
            }
        }

        /// <summary>
        /// Correct Spot spot
        /// </summary>
        private Boolean _haveSpot;
        public bool HaveSpot
        {
            get { return _haveSpot; }
            set { _haveSpot = value; }
        }

        /// <summary>
        /// Gets or sets to display curve lut for Spot spot
        /// </summary>
        private List<Int32[]> _spotCurveLut;
        public List<int[]> SpotCurveLut
        {
            get { return _spotCurveLut; }
            set { _spotCurveLut = value; }
        }

        /// <summary>
        /// Gets or sets to display curve lut for NonSpot
        /// </summary>
        private List<Int32[]> _nonSpotCurveLut;
        public List<int[]> NonSpotCurveLut
        {
            get { return _nonSpotCurveLut; }
            set { _nonSpotCurveLut = value; }
        }

        /// <summary>
        /// Gets or sets the center point of the Spot spot
        /// </summary>
        private Point2D _spot = Point2D.Zero;
        public Point2D SpotPoint
        {
            get { return _spot; }
            set { _spot = value; }
        }

        /// <summary>
        /// Gets or sets the inner radius for the spot
        /// </summary>
        private Double _innerSpotRadius;
        public Double InnerSpotRadius
        {
            get { return _innerSpotRadius; }
            set { _innerSpotRadius = value; }
        }

        /// <summary>
        /// Gets or sets the outer radius for the spot (feather zone)
        /// </summary>
        private Double _outerSpotRadius;
        public double OuterSpotRadius
        {
            get { return _outerSpotRadius; }
            set { _outerSpotRadius = value; }
        }

        /// <summary>
        /// Gets the true histogram
        /// </summary>
        private int[][] _histogram;
        public int[][] Histogram
        {
            get { return _histogram; }
            protected set { _histogram = value; }
        }

        /// <summary>
        /// Gets the quick histogram mean
        /// </summary>
        private double[] _histoMean;
        public double[] HistoMean
        {
            get { return _histoMean; }
            protected set { _histoMean = value; }
        }

        /// <summary>
        /// Gets the quick histogram brightness
        /// </summary>
        private double _histoBrightness;
        public double HistoBrightness
        {
            get { return _histoBrightness; }
            protected set { _histoBrightness = value; }
        }

        /// <summary>
        /// Gets the quick histogram contrast
        /// </summary>
        private double _histoContrast;
        public double HistoContrast
        {
            get { return _histoContrast; }
            protected set { _histoContrast = value; }
        }


    }
}
