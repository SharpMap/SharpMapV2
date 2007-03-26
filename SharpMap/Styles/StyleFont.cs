using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;

namespace SharpMap.Styles
{
    [Serializable]
    public class StyleFont
    {
        private ViewSize2D _size;
        private FontFamily _fontFamily;
        private FontStyle _style = FontStyle.Regular;
        private string _fontName;

        public StyleFont(string fontName, ViewSize2D emSize)
        {
            _fontName = fontName;
            _size = emSize;
        }

        public StyleFont(FontFamily family, ViewSize2D emSize, FontStyle style)
        {
            _fontFamily = family;
            _size = emSize;
            _style = style;
        }

        public ViewSize2D Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        public FontStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }

    }
}
