using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;

namespace AGGSharp.Drawing.Interface
{
    internal interface IInternalPixMap
    {
        RendererBase Renderer { get; }
        IScanlineCache ScanlineCache { get; }
        IRasterizer Rasterizer { get; }
    }
}
