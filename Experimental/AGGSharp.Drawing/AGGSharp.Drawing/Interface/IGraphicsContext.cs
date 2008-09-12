using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG.Transform;
using AGG;
using GeoAPI.Geometries;

namespace AGGSharp.Drawing.Interface
{
    public interface IGraphicsContext
    {
        ITransform Transform { get; set; }
        IAlphaMask Mask { get; set; }
        IExtents2D ClipBounds { get; set; }
    }
}
