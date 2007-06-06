using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Enumerates the type of GDI render object we have
    /// </summary>
    public enum GdiRenderObjectType : byte
    {
        Unknown = 0,
        Path,
        Symbol
    }
}
