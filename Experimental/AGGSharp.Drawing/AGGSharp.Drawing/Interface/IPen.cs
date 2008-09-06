using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;

namespace AGGSharp.Drawing.Interface
{
    public interface IPen
    {
        IBrush Brush { get; set; }
        IStroke Stroke { get; set; }
    }
}
