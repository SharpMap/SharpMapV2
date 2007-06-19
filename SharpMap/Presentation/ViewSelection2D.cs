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

namespace SharpMap.Presentation
{
    /// <summary>
    /// A representation of a selection on a 2D view surface.
    /// </summary>
    public class ViewSelection2D : ViewSelection<ViewPoint2D, ViewSize2D, ViewRectangle2D>
    {
        public override string ToString()
        {
            return String.Format("[ViewSelection2D] Bounds: {0}", this.Path.Bounds);
        }

        /// <summary>
        /// Creates a rectangular selection.
        /// </summary>
        /// <param name="upperLeft">The upper left point of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        /// <returns>A ViewSelection2D rectangular selection with upper left corner at <paramref name="upperLeft"/> and
        /// the given <paramref name="size"/>.</returns>
        public static ViewSelection2D CreateRectangluarSelection(ViewPoint2D upperLeft, ViewSize2D size)
        {
            ViewSelection2D selection = new ViewSelection2D();
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);

            selection.Expand(size);
            
            return selection;
        }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath()
        {
            return new GraphicsPath2D(new ViewPoint2D[0]);
        }
    }
}
