using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles
{
    [Flags]
    public enum FontStyle
    {
        Bold = 1,
        Italic = 2,
        Regular = 0,
        Strikeout = 8,
        Underline = 4
    }
}
