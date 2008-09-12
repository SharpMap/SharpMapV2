using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;
using AGGSharp.Utility;

namespace AGGSharp.Drawing
{
    internal static class RasterBufferFactory
    {
        public static IRasterBuffer CreateBuffer(uint pixelWidth, uint pixelHeight, uint stride, uint bytesPerPixel, out byte[] buffer)
        {
            Guard.GreaterThan(pixelWidth, 0u);
            Guard.GreaterThan(pixelHeight, 0u);
            Guard.GreaterThan(stride, 0u);
            Guard.GreaterThan(bytesPerPixel, 0u);

            buffer = new byte[pixelWidth * pixelHeight * stride];
            unsafe
            {
                fixed (byte* bye = &buffer[0])
                    return new RasterBuffer(bye, pixelWidth, pixelHeight, (int)stride, bytesPerPixel);
            }

        }
    }
}
