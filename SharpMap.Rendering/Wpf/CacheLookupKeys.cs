// Copyright 2007, 2008 - Ariel Yaroshevich (a.k.a blackrussian) (hamlet@inter.net.il)
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
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{
    internal struct BrushLookupKey : IEquatable<BrushLookupKey>
    {
        public readonly RuntimeTypeHandle StyleBrushType;
        public readonly int StyleBrushId;

        public BrushLookupKey(RuntimeTypeHandle type, int styleBrushId)
        {
            StyleBrushType = type;
            StyleBrushId = styleBrushId;
        }

        #region IEquatable<BrushLookupKey> Members

        public Boolean Equals(BrushLookupKey other)
        {
            return other.StyleBrushType.Equals(StyleBrushType)
                   && other.StyleBrushId == StyleBrushId;
        }
        #endregion
        public override Int32 GetHashCode()
        {
            return StyleBrushType.GetHashCode() ^ StyleBrushId;
        }
    }

    internal struct PenLookupKey : IEquatable<PenLookupKey>
    {
        public readonly RuntimeTypeHandle StylePenType;
        public readonly int StylePenId;

        public PenLookupKey(RuntimeTypeHandle type, int stylePenValue)
        {
            StylePenType = type;
            StylePenId = stylePenValue;
        }

        #region IEquatable<PenLookupKey> Members

        public Boolean Equals(PenLookupKey other)
        {
            return other.StylePenType.Equals(StylePenType)
                   && other.StylePenId == StylePenId;
        }
        #endregion
        public override int GetHashCode()
        {
            return StylePenType.GetHashCode() ^ StylePenId;
        }
    }

    internal struct SymbolLookupKey : IEquatable<SymbolLookupKey>
    {
        public readonly int SymbolId;

        public SymbolLookupKey(int symbolId)
        {
            SymbolId = symbolId;
        }

        #region IEquatable<SymbolLookupKey> Members

        public Boolean Equals(SymbolLookupKey other)
        {
            return other.SymbolId == SymbolId;
        }
        #endregion
        public override int GetHashCode()
        {
            return SymbolId;
        }
    }

    internal struct FontLookupKey : IEquatable<FontLookupKey>
    {
        public readonly int FontId;

        public FontLookupKey(int fontId)
        {
            FontId = fontId;
        }

        #region IEquatable<FontLookupKey> Members

        public Boolean Equals(FontLookupKey other)
        {
            return other.FontId == FontId;
        }
        #endregion
        public override int GetHashCode()
        {
            return FontId;
        }
    }

    internal class ShapeColors : IEquatable<ShapeColors>
    {
        StyleBrush Fill;
        StylePen Line;
        StylePen Outline;
        ShapeColors() { }
        public ShapeColors(StyleBrush fill, StylePen line, StylePen outline)
        {
            Fill = fill;
            Line = line;
            Outline = outline;
        }

        #region IEquatable<ShapeColors> Members

        public Boolean Equals(ShapeColors other)
        {
            if ((Fill == null) ^ (other.Fill == null))
            {
                return false;
            }
            if ((Line == null) ^ (other.Line == null))
            {
                return false;
            }
            if ((Outline == null) ^ (other.Outline == null))
            {
                return false;
            }
            if ((Fill != null && Fill != other.Fill))
            {
                return false;
            }
            if ((Outline != null && Outline != other.Outline))
            {
                return false;
            }
            if ((Line != null && Line != other.Line))
            {
                return false;
            }
            return true;
        }

        #endregion

        public override Int32 GetHashCode()
        {
            return Fill.GetHashCode() ^ Line.GetHashCode() ^ Outline.GetHashCode();
        }
    }
}