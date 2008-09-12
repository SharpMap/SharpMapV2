using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;

namespace AGGSharp.Drawing.Interface
{
    public interface IGradientBrush<TGradientProvider>
        : IBrush where TGradientProvider : IGradient
    {

    }
}
