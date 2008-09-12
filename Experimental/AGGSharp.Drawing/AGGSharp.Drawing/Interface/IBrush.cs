using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;

namespace AGGSharp.Drawing.Interface
{
    public interface IBrush
    {
        RGBA_Bytes this[int v] { get; }
    }
}
