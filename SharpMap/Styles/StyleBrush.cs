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

using System;

namespace SharpMap.Styles
{
    /// <summary>
    /// Represents an abstract brush to fill regions with color or patterns.
    /// </summary>
    [Serializable]
    public abstract class StyleBrush
    {
        private StyleColor _color;

        /// <summary>
        /// Creates a new instance of a StyleBrush with a transparent color.
        /// </summary>
        protected StyleBrush() : this(StyleColor.Transparent) { }

        /// <summary>
        /// Creates a new instance of a StyleBrush with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">Base color of the brush.</param>
        public StyleBrush(StyleColor color)
        {
            _color = color;
        }

        /// <summary>
        /// Gets or sets the base color of the brush.
        /// </summary>
        public StyleColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Generates a hash code for use in hashtables and dictionaries.
        /// </summary>
        /// <returns>An integer usable as a hash value in hashtables.</returns>
        public override Int32 GetHashCode()
        {
            return 7602046 ^ _color.GetHashCode();
        }
    }
}