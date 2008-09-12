using System;
using AGG.PixelFormat;
namespace AGGSharp.Drawing.Interface
{
    public interface IPixMap
    {
        IGraphics CreateGraphics();
        uint Height { get; }
        uint Stride { get; }
        uint Width { get; }
        IPixelFormat PixelFormat { get; }
    }
}
