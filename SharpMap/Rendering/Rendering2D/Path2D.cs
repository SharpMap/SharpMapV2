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
using System.Collections.Generic;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a series of figures of connected points in 2D space.
    /// </summary>
    public class Path2D : Path<TCoordinate, Rectangle<TCoordinate>>
    {
        /// <summary>
        /// Creates a new empty <see cref="Path2D"/>.
        /// </summary>
        public Path2D() { }

        /// <summary>
        /// Creates a new, open <see cref="Path2D"/> with the given points.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        public Path2D(IEnumerable<Point2D> points)
            : base(points) { }

        /// <summary>
        /// Creates a new <see cref="Path2D"/> with the given points, as closed or open.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        /// <param name="closeFigure">True to create a closed path, false for an open path.</param>
        public Path2D(IEnumerable<Point2D> points, Boolean closeFigure)
			: base(points, closeFigure) { }

		/// <summary>
		/// Creates a new <see cref="Path2D"/> with the given 
		/// <see cref="Figure2D"/> instance.
		/// </summary>
		/// <param name="figure">A figure to create the path from.</param>
		public Path2D(Figure2D figure)
			: base(figure) { }

        /// <summary>
        /// Creates a new <see cref="Path2D"/> with the given 
        /// <see cref="Figure2D"/> instances.
        /// </summary>
        /// <param name="figures">An enumeration of figures to create the path from.</param>
        public Path2D(IEnumerable<Figure2D> figures)
            : base(convertToBase(figures)) { }

        /// <summary>
        /// Gets the empty bounds shape: <see cref="Rectangle2D.Empty"/>.
        /// </summary>
        protected override Rectangle2D EmptyBounds
        {
            get { return Rectangle2D.Empty; }
        }

        /// <summary>
        /// Creates a new Path with one figure for each of the 
        /// figures in the give enumeration of <paramref name="figures"/>.
        /// </summary>
        /// <param name="figures">Figures to make the Path from.</param>
        /// <returns>A Path instance with one figure for each of the Figure 
        /// instances in the given enumeration.</returns>
        protected override Path<Point2D, Rectangle2D> CreatePath(IEnumerable<Figure<Point2D, Rectangle2D>> figures)
        {
            return new Path2D(convertFromBase(figures));
        }

        /// <summary>
        /// Creates a new Figure from the given <paramref name="points"/>, 
        /// either open or closed.
        /// </summary>
        /// <param name="points">Points to use in the figure.</param>
        /// <param name="closeFigure">True to close the figure, false to leave it open.</param>
        /// <returns>A Figure instance made from the given points.</returns>
        protected override Figure<Point2D, Rectangle2D> CreateFigure(IEnumerable<Point2D> points, Boolean closeFigure)
        {
            return new Figure2D(points, closeFigure);
        }

        /// <summary>
        /// Computes the <see cref="Figure2D.Bounds"/> for this <see cref="Path2D"/>.
        /// </summary>
        /// <returns>A <see cref="Rectangle2D"/> which describes the minimum bounding rectangle for this path.</returns>
        protected override Rectangle2D ComputeBounds()
        {
            if(Figures.Count == 0)
            {
                return Rectangle2D.Empty;
            }

            Double minX = Double.MaxValue, maxX = Double.MinValue, minY = Double.MaxValue, maxY = Double.MinValue;

            foreach (Figure2D figure in Figures)
            {
                Rectangle2D figureBounds = figure.Bounds;

                if (figureBounds.Left < minX)
                {
                    minX = figureBounds.Left;
                }

                if (figureBounds.Right > maxX)
                {
                    maxX = figureBounds.Right;
                }

                if (figureBounds.Top < minY)
                {
                    minY = figureBounds.Top;
                }

                if (figureBounds.Bottom > maxY)
                {
                    maxY = figureBounds.Bottom;
                }
            }

            return new Rectangle2D(minX, minY, maxX, maxY);
        }

        private static IEnumerable<Figure<Point2D, Rectangle2D>> convertToBase(IEnumerable<Figure2D> figures)
        {
            foreach (Figure2D figure in figures)
            {
                yield return figure;
            }
        }

        private static IEnumerable<Figure2D> convertFromBase(IEnumerable<Figure<Point2D, Rectangle2D>> figures)
        {
            foreach (Figure2D figure in figures)
            {
                yield return figure;
            }
        }
    }
}
