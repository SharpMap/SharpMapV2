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

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a series of figures of connected points in 2D space.
    /// </summary>
    public class GraphicsPath2D : GraphicsPath<Point2D, Rectangle2D>
    {
        /// <summary>
        /// Creates a new empty <see cref="GraphicsPath2D"/>.
        /// </summary>
        public GraphicsPath2D() { }

        /// <summary>
        /// Creates a new, open <see cref="GraphicsPath2D"/> with the given points.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        public GraphicsPath2D(IEnumerable<Point2D> points)
            : base(points) { }

        /// <summary>
        /// Creates a new <see cref="GraphicsPath2D"/> with the given points, as closed or open.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        /// <param name="closeFigure">True to create a closed path, false for an open path.</param>
        public GraphicsPath2D(IEnumerable<Point2D> points, bool closeFigure)
			: base(points, closeFigure) { }

		/// <summary>
		/// Creates a new <see cref="GraphicsPath2D"/> with the given 
		/// <see cref="GraphicsFigure2D"/> instance.
		/// </summary>
		/// <param name="figure">A figure to create the path from.</param>
		public GraphicsPath2D(GraphicsFigure2D figure)
			: base(figure) { }

        /// <summary>
        /// Creates a new <see cref="GraphicsPath2D"/> with the given 
        /// <see cref="GraphicsFigure2D"/> instances.
        /// </summary>
        /// <param name="figures">An enumeration of figures to create the path from.</param>
        public GraphicsPath2D(IEnumerable<GraphicsFigure2D> figures)
            : base(convertToBase(figures)) { }

        /// <summary>
        /// Gets the empty bounds shape: <see cref="Rectangle2D.Empty"/>.
        /// </summary>
        protected override Rectangle2D EmptyBounds
        {
            get { return Rectangle2D.Empty; }
        }

        /// <summary>
        /// Creates a new GraphicsPath with one figure for each of the 
        /// figures in the give enumeration of <paramref name="figures"/>.
        /// </summary>
        /// <param name="figures">Figures to make the GraphicsPath from.</param>
        /// <returns>A GraphicsPath instance with one figure for each of the GraphicsFigure 
        /// instances in the given enumeration.</returns>
        protected override GraphicsPath<Point2D, Rectangle2D> CreatePath(IEnumerable<GraphicsFigure<Point2D, Rectangle2D>> figures)
        {
            return new GraphicsPath2D(convertFromBase(figures));
        }

        /// <summary>
        /// Creates a new GraphicsFigure from the given <paramref name="points"/>, 
        /// either open or closed.
        /// </summary>
        /// <param name="points">Points to use in the figure.</param>
        /// <param name="closeFigure">True to close the figure, false to leave it open.</param>
        /// <returns>A GraphicsFigure instance made from the given points.</returns>
        protected override GraphicsFigure<Point2D, Rectangle2D> CreateFigure(IEnumerable<Point2D> points, bool closeFigure)
        {
            return new GraphicsFigure2D(points, closeFigure);
        }

        /// <summary>
        /// Computes the <see cref="GraphicsFigure2D.Bounds"/> for this <see cref="GraphicsPath2D"/>.
        /// </summary>
        /// <returns>A <see cref="Rectangle2D"/> which describes the minimum bounding rectangle for this path.</returns>
        protected override Rectangle2D ComputeBounds()
        {
            if(Figures.Count == 0)
            {
                return Rectangle2D.Empty;
            }

            double minX = Double.MaxValue, maxX = Double.MinValue, minY = Double.MaxValue, maxY = Double.MinValue;

            foreach (GraphicsFigure2D figure in Figures)
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

        private static IEnumerable<GraphicsFigure<Point2D, Rectangle2D>> convertToBase(IEnumerable<GraphicsFigure2D> figures)
        {
            foreach (GraphicsFigure2D figure in figures)
            {
                yield return figure;
            }
        }

        private static IEnumerable<GraphicsFigure2D> convertFromBase(IEnumerable<GraphicsFigure<Point2D, Rectangle2D>> figures)
        {
            foreach (GraphicsFigure2D figure in figures)
            {
                yield return figure;
            }
        }
    }
}
