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

namespace SharpMap.Rendering
{
    /// <summary>
    /// Represents a series of figures of connected points in an n-dimensional vector space, which is set by
    /// generic parameters.
    /// </summary>
    /// <typeparam name="TViewPoint">Type of point used in the path.</typeparam>
    /// <typeparam name="TViewBounds">Type of rectilinear used to bound the path.</typeparam>
    public abstract class GraphicsPath<TViewPoint, TViewBounds> : ICloneable, IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>>, IEquatable<GraphicsPath<TViewPoint, TViewBounds>>
        where TViewPoint : IViewVector
        where TViewBounds : IViewMatrix
    {
        private readonly List<GraphicsFigure<TViewPoint, TViewBounds>> _figures = new List<GraphicsFigure<TViewPoint, TViewBounds>>();
        private int _currentFigureIndex;
        private TViewBounds _bounds;

        /// <summary>
        /// Creates a new, empty <see cref="GraphicsPath{TViewPoint, TViewBounds}"/>.
        /// </summary>
        protected GraphicsPath() { }

        /// <summary>
        /// Creates a new, open <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with the given points.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        protected GraphicsPath(IEnumerable<TViewPoint> points)
            : this(points, false) { }

        /// <summary>
        /// Creates a new <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with the given points, as closed or open.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        /// <param name="isClosed">True to create a closed path, false for an open path.</param>
        protected GraphicsPath(IEnumerable<TViewPoint> points, bool isClosed)
        {
            GraphicsFigure<TViewPoint, TViewBounds> figure = CreateFigure(points, isClosed);
            _figures.Add(figure);
            _currentFigureIndex = 0;
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with the given 
        /// <see cref="GraphicsFigure{TViewPoint, TViewRectangle}"/> instance.
        /// </summary>
        /// <param name="figure">A figure to create the path from.</param>
        protected GraphicsPath(GraphicsFigure<TViewPoint, TViewBounds> figure)
            : this(new GraphicsFigure<TViewPoint, TViewBounds>[] { figure }) { }

        /// <summary>
        /// Creates a new <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with the given 
        /// <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> instances.
        /// </summary>
        /// <param name="figures">An enumeration of figures to create the path from.</param>
        protected GraphicsPath(IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>> figures)
        {
            _figures.AddRange(figures);

            if (_figures.Count == 0)
            {
                _currentFigureIndex = -1;
            }
            else
            {
                _currentFigureIndex = 0;
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} figure{2} of {3} points; Bounds: {4}", 
				GetType(), Figures.Count, (Figures.Count > 1 ? "s" : ""),  typeof(TViewPoint).Name, ComputeBounds());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19638952;
                foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in _figures)
                {
                    hash ^= figure.GetHashCode();
					hash ^= figure.IsClosed ? 729487 : 9245;
                }

                return hash;
            }
        }

        #region Equality Computation
        /// <summary>
        /// Returns true if two <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> instances are figure-for-figure, 
        /// point-for-point identical.
        /// </summary>
        /// <remarks>
        /// Casts <paramref name="obj"/> to a <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> and calls 
        /// <see cref="Equals(GraphicsPath{TViewPoint, TViewBounds})"/> with the result.
        /// </remarks>
        /// <param name="obj">The value to compare.</param>
        /// <returns>True if the parameter is a path which is figure-for-figure, point-for-point equal.</returns>
        public override bool Equals(object obj)
        {
            GraphicsPath<TViewPoint, TViewBounds> other = obj as GraphicsPath<TViewPoint, TViewBounds>;
            return this.Equals(other);
        }

        #region IEquatable<GraphicsPath<TViewPoint, TViewBounds>> Members
        /// <summary>
        /// Returns true if two <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> instances are figure-for-figure, 
        /// point-for-point identical.
        /// </summary>
        /// <param name="other">The <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> to compare.</param>
        /// <returns>True if the two paths are figure-for-figure, point-for-point equal.</returns>
        public bool Equals(GraphicsPath<TViewPoint, TViewBounds> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Figures.Count != this.Figures.Count)
            {
                return false;
            }

            unchecked
            {
                for (int figureIndex = 0; figureIndex < other.Figures.Count; figureIndex++)
                {
                    if (!this.Figures[figureIndex].Equals(other.Figures[figureIndex]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
        #endregion

        /// <summary>
        /// Gets a list of the <see cref="GraphicsFigure{TViewPoint, TViewBounds}"/> in this path.
        /// </summary>
        public IList<GraphicsFigure<TViewPoint, TViewBounds>> Figures
        {
            get { return _figures.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the points in the <see cref="CurrentFigure"/>.
        /// </summary>
        public IList<TViewPoint> Points
        {
            get 
			{
				if (CurrentFigure == null)
				{
					return new TViewPoint[] { };
				}

				return CurrentFigure.Points; 
			}
        }

        /// <summary>
        /// Gets or sets the current figure in the path.
        /// </summary>
        public GraphicsFigure<TViewPoint, TViewBounds> CurrentFigure
        {
            get 
            { 
                if(_figures.Count == 0)
                {
                    return null;
                }

                return _figures[_currentFigureIndex]; 
            }
            set
            {
                int index = _figures.IndexOf(value);

                if (index < 0)
                {
                    throw new InvalidOperationException("CurrentFigure must be an existing figure in this path.");
                }

                _currentFigureIndex = index;
            }
        }

        /// <summary>
        /// Gets the bounds of the path.
        /// </summary>
        public TViewBounds Bounds
        {
            get 
            {
                if (ReferenceEquals(_bounds, null) || _bounds.Equals(EmptyBounds))
                {
                    _bounds = ComputeBounds();
                }

                return _bounds;
            }
        }

        /// <summary>
        /// Creates a <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with the given figures.
        /// </summary>
        /// <param name="figures">Figures used in creating the path.</param>
        /// <returns>A new path with the given <paramref name="figures"/>.</returns>
        protected abstract GraphicsPath<TViewPoint, TViewBounds> CreatePath(IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>> figures);
        
        /// <summary>
        /// Creates a new <see cref="GraphicsFigure{TViewPoint, TViewBounds}"/> with the given points, either open or closed.
        /// </summary>
        /// <param name="points">Points in the figure.</param>
        /// <param name="isClosed">True if the figure wraps from the last point to the first point, false if it is open.</param>
        /// <returns>A new figure with the given points and open or closed condition.</returns>
        protected abstract GraphicsFigure<TViewPoint, TViewBounds> CreateFigure(IEnumerable<TViewPoint> points, bool isClosed);

        /// <summary>
        /// Computes a minimum bounding box for this path.
        /// </summary>
        /// <returns>A <typeparamref name="TViewBounds"/> instance which is the minimum bounding 
        /// n-dimensional rectilinear shape encompassing this path.</returns>
        protected abstract TViewBounds ComputeBounds();

        /// <summary>
        /// Gets a value indicating an empty bounds shape.
        /// </summary>
        protected abstract TViewBounds EmptyBounds { get; }

        /// <summary>
        /// Creates a new GraphicsFigure from the given <paramref name="points"/>, either closed or open, and adds it to 
        /// this path.
        /// </summary>
        /// <remarks>
        /// Also sets the <see cref="CurrentFigure"/> to the added figure.
        /// </remarks>
        /// <param name="points">Points to make the GraphicsFigure from.</param>
        /// <param name="closeFigure">True to close the figure, false to keep it open.</param>
        public void NewFigure(IEnumerable<TViewPoint> points, bool closeFigure)
        {
            _figures.Add(CreateFigure(points, closeFigure));
            _currentFigureIndex = Figures.Count - 1;
            _bounds = EmptyBounds;
        }

        /// <summary>
        /// Clones this <see cref="GraphicsPath{TViewPoint, TViewBounds}"/>.
        /// </summary>
        /// <returns>A new GraphicsPath with identical figures.</returns>
        public GraphicsPath<TViewPoint, TViewBounds> Clone()
        {
            List<GraphicsFigure<TViewPoint, TViewBounds>> figuresCopy = new List<GraphicsFigure<TViewPoint, TViewBounds>>(_figures.Count);

            foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in _figures)
            {
                figuresCopy.Add(figure.Clone());
            }

            GraphicsPath<TViewPoint, TViewBounds> path = CreatePath(figuresCopy);
            return path;
        }

        #region IEnumerable<GraphicsFigure<TViewPoint,TViewBounds>> Members
        /// <summary>
        /// Enumerates the figures in this path.
        /// </summary>
        /// <returns>An enumerator over this path's <see cref="GraphicsFigure{TViewPoint, TViewBounds}">figures</see>.</returns>
        public IEnumerator<GraphicsFigure<TViewPoint, TViewBounds>> GetEnumerator()
        {
            foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in _figures)
            {
                yield return figure;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
