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
