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
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Presentation
{
    /// <summary>
    /// A representation of a selection on a 2D view surface.
    /// </summary>
    public class ViewSelection2D : ViewSelection<Point2D, Size2D, Rectangle2D>
    {
        /// <summary>
        /// Returns a string description of the <see cref="ViewSelection2D"/>.
        /// </summary>
        /// <returns>A string which describes the <see cref="ViewSelection2D"/>.</returns>
        public override string ToString()
        {
            return String.Format("[ViewSelection2D] Bounds: {0}", Path.Bounds);
        }

        /// <summary>
        /// Creates a rectangular selection.
        /// </summary>
        /// <param name="upperLeft">The upper left point of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        /// <returns>A ViewSelection2D rectangular selection with upper left corner at <paramref name="upperLeft"/> and
        /// the given <paramref name="size"/>.</returns>
        public static ViewSelection2D CreateRectangluarSelection(Point2D upperLeft, Size2D size)
        {
            ViewSelection2D selection = new ViewSelection2D();
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);
            selection.AddPoint(upperLeft);

            selection.Expand(size);

            return selection;
        }

        public new Path2D Path
        {
            get { return base.Path as Path2D; }
        }

        protected override Path<Point2D, Rectangle2D> CreatePath()
        {
            return new Path2D(new Point2D[0]);
        }
    }
}