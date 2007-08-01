// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
    [Serializable]
    public class StyleFont
    {
        private Size2D _size;
        private StyleFontFamily _fontFamily;
        private StyleFontStyle _style = StyleFontStyle.Regular;
        private string _fontName;

        public StyleFont(string fontName, Size2D emSize)
        {
            _fontName = fontName;
            _size = emSize;
        }

        public StyleFont(StyleFontFamily family, Size2D emSize, StyleFontStyle style)
        {
            _fontFamily = family;
            _size = emSize;
            _style = style;
        }

        public Size2D Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public StyleFontFamily FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        public StyleFontStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }

    }
}
