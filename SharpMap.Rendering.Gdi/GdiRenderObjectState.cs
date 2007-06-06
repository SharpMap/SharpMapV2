using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Enumerates the states of the GDI render object
    /// </summary>
    public enum GdiRenderObjectState : byte
    {
        Unknown = 0,
        Normal,
        Hover,
        Selected
    }
}
