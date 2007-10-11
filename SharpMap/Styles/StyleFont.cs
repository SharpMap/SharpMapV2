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
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
    [Serializable]
    public class StyleFont
    {
        private Size2D _size;
        private readonly StyleFontFamily _fontFamily;
        private StyleFontStyle _style = StyleFontStyle.Regular;

        public StyleFont(StyleFontFamily family, Size2D emSize, StyleFontStyle style)
        {
            if (family == null) throw new ArgumentNullException("family");

            _fontFamily = family;
            _size = emSize;
            _style = style;
        }

        public override string ToString()
        {
            return String.Format("[{0}] Font Family: {1}; Size: {2}; Font Style: {3}",
                                 GetType(), FontFamily, Size, Style);
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode() ^ FontFamily.GetHashCode() ^ Style.GetHashCode();
        }

        public Size2D Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public StyleFontFamily FontFamily
        {
            get { return _fontFamily; }
        }

        public StyleFontStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }
    }
}