// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Styles
{
    /// <summary>
    /// Specifies information which a type renderer uses to help
    /// determine how to render a font to an output device.
    /// </summary>
    /// <remarks>
    /// For more background on how these hints will influence
    /// rendered text, read 
    /// <a href="http://www.microsoft.com/typography/hinting/tutorial.htm">
    /// Basic hinting philosophies and TrueType instructions.
    /// </a>
    /// </remarks>
    public enum StyleTextRenderingHint
    {
        /// <summary>
        /// The system default for rendering characters to a bitmap grid.
        /// </summary>
        SystemDefault = 0,

        /// <summary>
        /// The characters are to be drawn to a glyph bitmap, with 
        /// typographic hinting to fit curves and stems to the grid.
        /// </summary>
        SingleBitPerPixelGridFit,

        /// <summary>
        /// The characters are to be drawn to a glyph bitmap, without
        /// typographic hinting to help ensure readability of curves and stems. 
        /// This hint usually results in the fastest rendering.
        /// </summary>
        SingleBitPerPixel,

        /// <summary>
        /// The characters are to be drawn to an anti-aliased 
        /// glyph bitmap, with typographic hinting.
        /// </summary>
        AntiAliasGridFit,

        /// <summary>
        /// The characters are to be drawn to an anti-aliased 
        /// glyph bitmap, without typographic hinting.
        /// </summary>
        AntiAlias,

        /// <summary>
        /// Microsoft ClearType is used to render the characters, if available.
        /// </summary>
        ClearTypeGridFit
    }
}
