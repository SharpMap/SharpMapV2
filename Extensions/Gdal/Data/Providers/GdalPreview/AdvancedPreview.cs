using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
//using OSGeo.GDAL;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Utilities;
using GdalBand = OSGeo.GDAL.Band;
using GdalDataset = OSGeo.GDAL.Dataset;
using GdalDataType = OSGeo.GDAL.DataType;
using Gdal = OSGeo.GDAL.Gdal;
using GdalCPLErr = OSGeo.GDAL.CPLErr;
using GdalColorTable = OSGeo.GDAL.ColorTable;
using GdalColorInterp = OSGeo.GDAL.ColorInterp;
using GdalPaletteInterp = OSGeo.GDAL.PaletteInterp;
using GdalColorEntry = OSGeo.GDAL.ColorEntry;

namespace SharpMap.Data.Providers.GdalPreview
{
    
    [Flags]
    public enum GdalDrawMethod
    {
        /// <summary>
        /// a "Gray" or "Undefined" layer drawn as a range of gray colors
        /// </summary>
        SingleBandGray,
        /// <summary>
        /// // a "Gray" or "Undefined" layer drawn using a pseudocolor algorithm
        /// </summary>
        SingleBandPseudoColor,
        /// <summary>
        /// A single band with a color map
        /// </summary>
        SingleBandColorPalette,
        /// <summary>
        /// a "Palette" layer drawn in gray scale (using only one of the color components)
        /// </summary>
        SingleBandGrayPalette,
        /// <summary>
        /// 
        /// </summary>
        MultiBandColor,
        /*
        /// <summary>
        /// a "Palette" layer drawn with pseudocolor algorithm (using only one of the color components)
        /// </summary>
        SingleBandPseudoPalette,
        MultiBandColorPalette,
        MultiBandSingleBandGray,
        MultiBandSingleBandPseudoColor,
         */
    }

    
    public class AdvancedPreview : BaseGdalPreview
    {
        const Int32 DefaultColorValue = 0xffffff;

        private RasterShader _rasterShader = new RasterShader();

        private Int32 _transparancyLevel = 255;
        public Double Transparency
        {
            get { return (255d - _transparancyLevel) / 255d;}
            set
            {
                if (0d <= value && value <= 100d)
                    _transparancyLevel = (Int32) (255*(value/100d));
            }
        }

        private Boolean _invertColor;
        public Boolean InvertColor
        {
            get { return _invertColor; }
            set { _invertColor = value;}
        }

        private RasterTransparency _rasterTransparency = new RasterTransparency();
        public RasterTransparency RasterTransparency
        {
            get { return _rasterTransparency; }
            set { _rasterTransparency = value; }
        }

        private GdalDrawMethod _drawMethod;

        public override void Initialize(GdalRasterProvider provider)
        {
            GdalBand band;
            _provider = provider;
            _provider.ReadFile();

            _rasterShader = new RasterShader();

            //Transparenz
            if (_provider.NoDataValue.HasValue)
                _rasterTransparency.Add(_provider.NoDataValue.Value, 100);

            switch (_provider.RasterType)
            {
                case GdalRasterType.GrayOrUndefined:
                    _drawMethod = GdalDrawMethod.SingleBandGray;
                    band = _provider.Dataset.GetRasterBand(1);
                    if (Gdal.GetDataTypeSize(band.DataType) > 8)
                    {
                        _drawMethod = GdalDrawMethod.SingleBandPseudoColor;
                        _rasterShader.RasterShaderFunction = new FreakOutShaderFunction();
                        Double val;
                        int hasVal;
                        band.GetMinimum(out val, out hasVal);
                        _rasterShader.Minimum = hasVal != 0 ? val : _provider.BandStatistics[0].Minimum;
                        band.GetMaximum(out val, out hasVal);
                        _rasterShader.Maximum = hasVal != 0 ? val : _provider.BandStatistics[0].Maximum;
                    }
                    else
                    {
                        _drawMethod = GdalDrawMethod.SingleBandGray;
                    }
                    break;
                case GdalRasterType.Palette:
                    _drawMethod = GdalDrawMethod.SingleBandColorPalette;
                    band = _provider.Dataset.GetRasterBand(1);
                    GdalColorTable colorTable = band.GetColorTable();
                    _rasterShader.RasterShaderFunction = ColorRampShaderFunction.CreateFromGdalColorTable(colorTable);
                    break;
                case GdalRasterType.MultiBand:
                    _drawMethod = GdalDrawMethod.MultiBandColor;
                    break;
                default:
                    break;

            }

            //switch (_provider.Dataset.RasterCount)
            //{
            //    case 1:
            //        if (band.GetRasterColorInterpretation() == GdalColorInterp.GCI_PaletteIndex)
            //        {
            //            if (band.DataType == GdalDataType.GDT_Byte)
            //                _drawMethod = GdalDrawMethod.SingleBandColorPalette;
            //            else
            //            {
            //                _drawMethod = GdalDrawMethod.SingleBandPseudoColor;
            //                _rasterShader.RasterShaderFunction = new PseudoColorShader();
            //            }
            //        }
            //        else
            //            _drawMethod = GdalDrawMethod.SingleBandGray;
            //        break;
            //    case 3:
            //        _drawMethod = GdalDrawMethod.MultiBandColor;
            //        break;

            //}
        }

        /*
        private Double GdalDataTypeMinimum(GdalDataType dataType)
        {
            switch (dataType)
            {
                case GdalDataType.GDT_Byte:
                    return 0d;
                case GdalDataType.GDT_UInt16:
                    return 0d;
                case GdalDataType.GDT_Int16:
                    return -Int16.MaxValue;
                case GdalDataType.GDT_UInt32:
                    return 0;
                case GdalDataType.GDT_Int32:
                    return -Int32.MaxValue;
                case GdalDataType.GDT_CFloat32:
                case GdalDataType.GDT_Float32:
                    return -(Single.MaxValue-1);
                case GdalDataType.GDT_CFloat64:
                case GdalDataType.GDT_Float64:
                default:
                    return -(Double.MaxValue - 1);
            }
        }
         

        private Double GdalDataTypeMaximum(GdalDataType dataType)
        {
            switch (dataType)
            {
                case GdalDataType.GDT_Byte:
                    return 255d;
                case GdalDataType.GDT_UInt16:
                    return UInt16.MaxValue;
                case GdalDataType.GDT_Int16:
                    return Int16.MaxValue;
                case GdalDataType.GDT_UInt32:
                    return UInt32.MaxValue;
                case GdalDataType.GDT_Int32:
                    return Int32.MaxValue;
                case GdalDataType.GDT_CFloat32:
                case GdalDataType.GDT_Float32:
                    return Single.MaxValue;
                case GdalDataType.GDT_CFloat64:
                case GdalDataType.GDT_Float64:
                default:
                    return Double.MaxValue - 1;
            }
        }
        */

        private static IntPtr ReadData(GdalBand rasterBand, int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            Int32 dataTypeSize = Gdal.GetDataTypeSize(rasterBand.DataType) / 8;
            IntPtr ptrToData = Marshal.AllocCoTaskMem(dataTypeSize*width*height);

            GdalCPLErr resultRasterIo = rasterBand.ReadRaster(left, top, clipWidth, clipHeight,
                                                           ptrToData, width, height, 
                                                           rasterBand.DataType, 0, 0);
            switch (resultRasterIo)
            {
                case GdalCPLErr.CE_Failure:
                case GdalCPLErr.CE_Fatal:
                    Marshal.FreeCoTaskMem(ptrToData);
                    ptrToData = IntPtr.Zero;
                    break;

                case GdalCPLErr.CE_Warning:
                case GdalCPLErr.CE_Log:
                    System.Diagnostics.Trace.WriteLine(Gdal.GetLastErrorMsg());
                    break;

                case GdalCPLErr.CE_None:
                    break;
                default:
                    throw new Exception("Should never reach here");
            }
            return ptrToData;
        }

        private unsafe static Double ReadValue(void* buffer, GdalDataType dataType, Int32 index)
        {
            switch (dataType)
            {
                case GdalDataType.GDT_Byte:
                    return ((byte*)buffer)[index];
                case GdalDataType.GDT_UInt16:
                    return ((ushort*)buffer)[index];
                case GdalDataType.GDT_Int16:
                    return ((short*)buffer)[index];
                case GdalDataType.GDT_UInt32:
                    return ((uint*)buffer)[index];
                case GdalDataType.GDT_Int32:
                    return ((int*)buffer)[index];
                case GdalDataType.GDT_Float32:
                    return ((float*)buffer)[index];
                case GdalDataType.GDT_Float64:
                    return ((double*)buffer)[index];
                default:
                    //data type not supported
                    break;
            }
            return 0.0;
        }


        #region Implementation of IGdalPreview

        public override Bitmap GetPreview(IExtents viewPort, Matrix2D toViewTransform, out Rectangle2D viewBounds, out Rectangle2D rasterBounds)
        {
            viewBounds = new Rectangle2D();
            rasterBounds = new Rectangle2D();

            GdalReadParameter gdalReadParameter = GetRasterViewPort(viewPort, toViewTransform);
            if (gdalReadParameter == null) return null;
            rasterBounds = new Rectangle2D(0, 0, gdalReadParameter.ResultWidth, gdalReadParameter.ResultHeight);
            viewBounds = gdalReadParameter.ViewPort;

            return BitmapDirect(gdalReadParameter.X1, gdalReadParameter.Y1,
                gdalReadParameter.SourceWidth, gdalReadParameter.SourceHeight,
                gdalReadParameter.ResultWidth, gdalReadParameter.ResultHeight);
        }

        private Bitmap BitmapDirect(int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            switch (_drawMethod)
            {
                case GdalDrawMethod.SingleBandGray:
                    return DrawSingleBandGray(1, left, top, clipWidth, clipHeight, width, height);
                case GdalDrawMethod.SingleBandPseudoColor:
                    return DrawSingleBandPseudoColor(1, left, top, clipWidth, clipHeight, width, height);
                case GdalDrawMethod.SingleBandColorPalette:
                    return DrawSingleBandColorPalette(1, left, top, clipWidth, clipHeight, width, height);
                case GdalDrawMethod.SingleBandGrayPalette:
                    throw new NotSupportedException();
                case GdalDrawMethod.MultiBandColor:
                    return DrawMultiBandColor(left, top, clipWidth, clipHeight, width, height);
                default:
                    return null;
            }
        }

        public override string Name
        {
            get { return "AdvancedPreview"; }
        }
        #endregion

        #region drawing function

        private Bitmap DrawSingleBandColorPalette(int bandNr, int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            GdalBand band = _provider.Dataset.GetRasterBand(bandNr);
            GdalDataType dataType = band.DataType;
            IntPtr pData = ReadData(band, left, top, clipWidth, clipHeight, width, height);

            if (pData == IntPtr.Zero)
                return null;

            GdalColorTable colorTable = band.GetColorTable();

            Bitmap resultBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmData = resultBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb);
            IntPtr scan0 = bmData.Scan0;
            Int32 stride = bmData.Stride;
            Int32 pos = -1;
            unsafe
            {
                void* pDataItem = pData.ToPointer();

                for (Int32 i = 0; i < height; i++)
                {
                    Int32* pRowItem = (Int32*)(new IntPtr(scan0.ToInt64() + i * stride).ToPointer());
                    for (Int32 j = 0; j < width; j++)
                    {
                        pos++;
                        Double val = ReadValue(pDataItem, dataType, pos);
                        if (Double.IsNaN(val))
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        GdalColorEntry ce = new GdalColorEntry();
                        colorTable.GetColorEntryAsRGB((Int32) val, ce);

                        Double red = ce.c1;
                        Double green = ce.c2;
                        Double blue = ce.c3;

                        Int32 alpha = _rasterTransparency.Alpha(red, green, blue, _transparancyLevel);
                        if (alpha == 0)
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        if (_invertColor)
                            pRowItem[j] = Argb(255-(int)red, 255-(int)green, 255-(int)blue, alpha);
                        else
                            pRowItem[j] = Argb((int)val, (int)val, (int)val, alpha);
                    }
                }
            }
            Marshal.FreeCoTaskMem(pData);

            resultBitmap.UnlockBits(bmData);
            return resultBitmap;
        }


        private Bitmap DrawSingleBandGray(int bandNr, int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            GdalBand band = _provider.Dataset.GetRasterBand(bandNr);
            GdalDataType dataType = band.DataType;
            IntPtr pData = ReadData(band, left, top, clipWidth, clipHeight, width, height);

            if ( pData == IntPtr.Zero )
                return null;

            Bitmap resultBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmData = resultBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb);
            IntPtr scan0 = bmData.Scan0;
            Int32 stride = bmData.Stride;
            Int32 pos = -1;
            unsafe
            {
                void* pDataItem = pData.ToPointer();

                for(Int32 i = 0; i <height; i++)
                {
                    Int32* pRowItem = (Int32*)(new IntPtr(scan0.ToInt64() + i*stride).ToPointer());
                    for (Int32 j = 0; j < width; j++)
                    {
                        pos++;
                        Double val = ReadValue(pDataItem, dataType, pos);
                        if (Double.IsNaN(val))
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        Int32 alpha = _rasterTransparency.Alpha(val, _transparancyLevel);
                        if (alpha == 0)
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        if (_invertColor)
                            val = 255 - val;
                        pRowItem[j] = Argb((int)val, (int)val, (int)val, alpha);
                    }
                }
            }
            Marshal.FreeCoTaskMem(pData);

            resultBitmap.UnlockBits(bmData);
            return resultBitmap;
        }

        private Bitmap DrawSingleBandPseudoColor(int bandNr, int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            if (_rasterShader == null) 
                return null;

            GdalBand band = _provider.Dataset.GetRasterBand(bandNr);
            GdalDataType dataType = band.DataType;
            IntPtr pData = ReadData(band, left, top, clipWidth, clipHeight, width, height);

            if (pData == IntPtr.Zero)
                return null;

            Bitmap resultBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmData = resultBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb);
            IntPtr scan0 = bmData.Scan0;
            Int32 stride = bmData.Stride;
            Int32 pos = -1;

            unsafe
            {
                void* pDataItem = pData.ToPointer();

                for (Int32 i = 0; i < height; i++)
                {
                    Int32* pRowItem = (Int32*)(new IntPtr(scan0.ToInt64() + i * stride).ToPointer());
                    for (Int32 j = 0; j < width; j++)
                    {
                        pos++;
                        Double val = ReadValue(pDataItem, dataType, pos);
                        if (Double.IsNaN(val))
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        Int32 alpha = _rasterTransparency.Alpha(val, _transparancyLevel);
                        if (alpha == 0)
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        Int32 r = 0, g = 0, b = 0;
                        _rasterShader.Shade(val, ref r, ref g, ref b);
                        if (!_invertColor)
                            pRowItem[j] = Argb(r, g, b, alpha);
                        else
                            pRowItem[j] = Argb(b, g, r, alpha);
                    }
                }
            }
            Marshal.FreeCoTaskMem(pData);

            resultBitmap.UnlockBits(bmData);
            return resultBitmap;
        }

        private Bitmap DrawMultiBandColor(int left, int top, int clipWidth, int clipHeight, int width, int height)
        {
            GdalBand redBand = _provider.Dataset.GetRasterBand(1);
            GdalBand greenBand = _provider.Dataset.GetRasterBand(2);
            GdalBand blueBand = _provider.Dataset.GetRasterBand(3);

            GdalDataType redDataType = redBand.DataType;
            GdalDataType greenDataType = redBand.DataType;
            GdalDataType blueDataType = redBand.DataType;

            IntPtr pRedData = ReadData(redBand, left, top, clipWidth, clipHeight, width, height);
            IntPtr pGreenData = ReadData(greenBand, left, top, clipWidth, clipHeight, width, height);
            IntPtr pBlueData = ReadData(blueBand, left, top, clipWidth, clipHeight, width, height);
            if (pRedData == IntPtr.Zero || pGreenData == IntPtr.Zero || pBlueData == IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pRedData);
                Marshal.FreeCoTaskMem(pGreenData);
                Marshal.FreeCoTaskMem(pBlueData);
                return null;
            }

            Bitmap resultBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmData = resultBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb);
            IntPtr scan0 = bmData.Scan0;
            Int32 stride = bmData.Stride;
            Int32 pos = -1;
            unsafe
            {
                void* pRedDataItem = pRedData.ToPointer();
                void* pGreenDataItem = pGreenData.ToPointer();
                void* pBlueDataItem = pBlueData.ToPointer();

                for (Int32 i = 0; i < height; i++)
                {
                    Int32* pRowItem = (Int32*)(new IntPtr(scan0.ToInt64() + i * stride).ToPointer());
                    for (Int32 j = 0; j < width; j++)
                    {
                        pos++;
                        Double redVal = ReadValue(pRedDataItem, redDataType, pos);
                        Double greenVal = ReadValue(pGreenDataItem, greenDataType, pos);
                        Double blueVal = ReadValue(pBlueDataItem, blueDataType, pos);

                        if (Double.IsNaN(redVal) || Double.IsNaN(greenVal) || Double.IsNaN(blueVal))
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        Int32 alpha = _rasterTransparency.Alpha(redVal, greenVal, blueVal, _transparancyLevel);
                        if (alpha == 0)
                        {
                            pRowItem[j] = DefaultColorValue;
                            continue;
                        }

                        if (_invertColor)
                        {
                            redVal = 255 - redVal;
                            greenVal = 255 - greenVal;
                            blueVal = 255 - blueVal;
                        }
                        pRowItem[j] = Argb((int)redVal, (int)greenVal, (int)blueVal, alpha);
                    }
                }
            }
            Marshal.FreeCoTaskMem(pRedData);
            Marshal.FreeCoTaskMem(pGreenData);
            Marshal.FreeCoTaskMem(pBlueData);

            resultBitmap.UnlockBits(bmData);
            return resultBitmap;
        }

        private static Int32 Argb(Int32 red, Int32 green, Int32 blue, Int32 alpha)
        {
            return ((((alpha<<8|red)<<8|green)<<8|blue));
        }

        #endregion
    }
}
