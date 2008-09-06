using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGGSharp.Drawing.Interface;
using AGG.PixelFormat;
using AGG;

namespace AGGSharp.Drawing
{
    internal class PixelFormatFactory : IPixelFormatFactory
    {
        #region IPixelFormatFactory Members

        public IPixelFormat CreatePixelFormat(PixelFormat format, uint pixelWidth, uint pixelHeight, out byte[] buffer)
        {
            switch (format)
            {
                case PixelFormat.Gray:
                    {
                        return new FormatGray(
                            RasterBufferFactory.CreateBuffer(pixelWidth, pixelHeight, 3, 3, out buffer),
                            IoC.Instance.Resolve<IBlenderGray>(), 3, 2);
                    }
                case PixelFormat.RGB:
                    {
                        return new FormatRGB(
                           RasterBufferFactory.CreateBuffer(pixelWidth, pixelHeight, 3, 3, out buffer),
                            IoC.Instance.Resolve<IBlender>("rgb"));
                    }
                case PixelFormat.RGBA:
                    {
                        return new FormatRGBA(
                            RasterBufferFactory.CreateBuffer(pixelWidth, pixelHeight, 4, 4, out buffer),
                            IoC.Instance.Resolve<IBlender>("bgra"));
                    }
                default:
                    throw new InvalidOperationException();
            }
        }


        public IPixelFormat CreatePixelFormat(PixelFormat format, IRasterBuffer buffer, uint pixelWidth, uint pixelHeight)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
