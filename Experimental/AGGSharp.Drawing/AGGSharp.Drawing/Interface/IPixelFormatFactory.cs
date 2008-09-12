using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG.PixelFormat;

namespace AGGSharp.Drawing.Interface
{
    internal interface IPixelFormatFactory
    {
        IPixelFormat CreatePixelFormat(PixelFormat format, uint pixelWidth, uint pixelHeight, out byte[] buffer);
    }
}
